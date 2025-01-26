using GK_3DRendering.Engine.Cameras;
using GK_3DRendering.Engine.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace GK_3DRendering.Engine.Components.Renderers
{
    class MeshRenderer : IRenderable
    {

        [InspectorIgnore]
        public Mesh Mesh { get; }

        [InspectorIgnore]
        public Shader Shader { get; }

        
        public Material Material { get; }

        [InspectorIgnore]
        public GameObject Parent;

        public bool OneSided;

        public MeshRenderer(Mesh mesh, Shader shader, Material material, GameObject parent, bool oneSided = false)
        {
            Mesh = mesh;
            Shader = shader;
            Material = material;
            Parent = parent;

            OneSided = oneSided;

            Parent.Scene.AddRenderer(this);
        }

        public void Render(Camera camera)
        {
            Shader.Use();
            Shader.LoadMat4("model", Parent.Transform.TransformMatrix);
            Shader.LoadMat4("view", camera.ViewMatrix);
            Shader.LoadMat4("projection", camera.ProjectionMatrix);
            Shader.LoadMaterial("material", Material);

            Shader.LoadVec3("lightdirection", new Vector3(-1,-1,-1));
            Shader.LoadVec3("viewPos", camera.Transform.Position);
            Shader.LoadBool("directionalLight", true);

            Mesh.Bind();
            GL.DrawElements(BeginMode.Triangles, Mesh.IndexBuffer.Indices, DrawElementsType.UnsignedInt, 0);
        }
    }
}
