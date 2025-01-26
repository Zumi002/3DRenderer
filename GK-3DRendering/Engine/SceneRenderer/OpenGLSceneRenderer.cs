using GK_3DRendering.Engine.Cameras;
using GK_3DRendering.Engine.Components.Renderers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using GK_3DRendering.Engine.LightSource;

namespace GK_3DRendering.Engine.SceneRenderer
{
    class OpenGLSceneRenderer : ISceneRenderer
    {
        public void RenderScene(Scene scene)
        {
            RenderMeshes(scene);
            RenderFlags(scene);
        }

        private void RenderMeshes(Scene scene)
        {
            Shader meshShader = ShaderManager.GetShader(ShaderManager.MyShaders.Default);
            Camera? camera = scene.GetActiveCamera();
            if (camera == null)
                return;
            meshShader.Use();

            SetupLights(meshShader, scene, camera);
            
            meshShader.LoadMat4("view", camera.ViewMatrix);
            meshShader.LoadMat4("projection", camera.ProjectionMatrix);
            foreach (var meshRenderer in scene.GetMeshRenderers())
            {
                meshShader.LoadMat4("model", meshRenderer.Parent.Transform.TransformMatrix);
                meshShader.LoadMaterial("material", meshRenderer.Material);
                meshShader.LoadBool("oneSided", meshRenderer.OneSided);
                //rendinrng 
                meshRenderer.Mesh.Bind();
                GL.DrawElements(BeginMode.Triangles, meshRenderer.Mesh.IndexBuffer.Indices, DrawElementsType.UnsignedInt, 0);
            }
        }

        private void SetupLights(Shader shader, Scene scene, Camera camera)
        {
            SceneLights sceneLights = scene.GetSceneLights();


            shader.LoadInt("directionalLightsCount", sceneLights.directionalLightSources.Count);
            shader.LoadInt("spotLightsCount", sceneLights.spotLightSources.Count);
            shader.LoadInt("pointLightsCount", sceneLights.pointLightSources.Count);

            //at time of setting up buffers, also converting lights to view space
 
            Matrix3 invView = new Matrix3(Matrix4.Transpose(Matrix4.Invert(camera.ViewMatrix)));

            for(int i = 0; i < sceneLights.directionalLightSources.Count; i++)
            {
                DirectionalLightSource light = sceneLights.directionalLightSources[i];
                shader.LoadVec3($"directionalLights[{i}].color", light.Color);
                shader.LoadVec3($"directionalLights[{i}].direction", Vector3.TransformRow(light.Direction, invView));

                shader.LoadFloat($"directionalLights[{i}].ambient", light.Ambient);
                shader.LoadFloat($"directionalLights[{i}].diffuse", light.Diffuse);
                shader.LoadFloat($"directionalLights[{i}].specular", light.Specular);
            }

            for (int i = 0; i < sceneLights.pointLightSources.Count; i++)
            {
                PointLightSource light = sceneLights.pointLightSources[i];
                shader.LoadVec3($"pointLights[{i}].color", light.Color);
                shader.LoadVec3($"pointLights[{i}].position", new Vector3(Vector4.TransformRow(new Vector4(light.Transform.Position, 1f),camera.ViewMatrix)));

                shader.LoadFloat($"pointLights[{i}].ambient", light.Ambient);
                shader.LoadFloat($"pointLights[{i}].diffuse", light.Diffuse);
                shader.LoadFloat($"pointLights[{i}].specular", light.Specular);

                shader.LoadFloat($"pointLights[{i}].constant", light.Constatnt);
                shader.LoadFloat($"pointLights[{i}].linear", light.Linear);
                shader.LoadFloat($"pointLights[{i}].quadratic", light.Quadratic);
            }

            for (int i = 0; i < sceneLights.spotLightSources.Count; i++)
            {
                SpotLightSource light = sceneLights.spotLightSources[i];

                shader.LoadVec3($"spotLights[{i}].color", light.Color);
                shader.LoadVec3($"spotLights[{i}].direction", Vector3.TransformRow(light.Transform.Forward, invView));
                shader.LoadVec3($"spotLights[{i}].position", new Vector3(Vector4.TransformRow(new Vector4(light.Transform.Position, 1f), camera.ViewMatrix)));

                shader.LoadFloat($"spotLights[{i}].cutOff", light.CutOff);
                shader.LoadFloat($"spotLights[{i}].outerCutOff", light.OuterCutOff);

                shader.LoadFloat($"spotLights[{i}].ambient", light.Ambient);
                shader.LoadFloat($"spotLights[{i}].diffuse", light.Diffuse);
                shader.LoadFloat($"spotLights[{i}].specular", light.Specular);

                shader.LoadFloat($"spotLights[{i}].constant", light.Constatnt);
                shader.LoadFloat($"spotLights[{i}].linear", light.Linear);
                shader.LoadFloat($"spotLights[{i}].quadratic", light.Quadratic);
            }

        }
        private void RenderFlags(Scene scene)
        {
            Shader flagShader = ShaderManager.GetShader(ShaderManager.MyShaders.FlagShader);
            Camera? camera = scene.GetActiveCamera();
            if (camera == null)
                return;

            flagShader.Use();

            SetupLights(flagShader, scene, camera);

            flagShader.LoadMat4("view", camera.ViewMatrix);
            flagShader.LoadMat4("projection", camera.ProjectionMatrix);
            foreach (var flagRenderer in scene.GetFlagRenderers())
            {
                flagShader.LoadMat4("model", flagRenderer.Parent.Transform.TransformMatrix);
                flagShader.LoadMaterial("material", flagRenderer.Material);
                flagShader.LoadFloat("animTime", flagRenderer.AnimationTime);
                flagShader.LoadVec2i("patchSize", flagRenderer.Size);
                flagShader.LoadBool("oneSided", flagRenderer.OneSided);
                //rendinrng 
                flagRenderer.Mesh.controllMesh.Bind();
                GL.PatchParameter(PatchParameterInt.PatchVertices, flagRenderer.Count);
                //GL.PolygonMode(TriangleFace.FrontAndBack, PolygonMode.Line);
                GL.DrawArrays(PrimitiveType.Patches, 0, flagRenderer.Count);
                //GL.PolygonMode(TriangleFace.FrontAndBack, PolygonMode.Fill);
                //Console.WriteLine(GL.GetError());
            }
        }
    }
}
