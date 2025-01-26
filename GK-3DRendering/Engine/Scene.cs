using GK_3DRendering.Engine.Cameras;
using GK_3DRendering.Engine.Components.Renderers;
using GK_3DRendering.Engine.LightSource;
using GK_3DRendering.Engine.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GK_3DRendering.Engine
{
    class Scene
    {
        List<GameObject> Objects = new ();

        public Camera? ActiveCamera { get; private set; }

        List<MeshRenderer> Meshes = new();
        List<FlagRenderer> FlagRenderers = new ();

        SceneLights SceneLights = new();

        public void Update(float deltaTime)
        {
            foreach (var obj in Objects)
            {
                obj.Update(deltaTime);
            }
        }

        public void AddGameObject(GameObject obj)
        {
            Objects.Add(obj);
        }


        public void SetActiveCamera(Camera camera)
        {
            ActiveCamera = camera; 
        }


        public Camera? GetActiveCamera()
        {
            return ActiveCamera;
        }

        public void AddRenderer(IRenderable renderer)
        {
            if(renderer as MeshRenderer != null)
            {
                Meshes.Add((MeshRenderer)renderer);
            }
            if (renderer as FlagRenderer != null)
            {
                FlagRenderers.Add((FlagRenderer)renderer);
            }
        }

        public List<MeshRenderer> GetMeshRenderers()
        {
            return Meshes;  
        }

        public List<FlagRenderer> GetFlagRenderers()
        {
            return FlagRenderers;
        }

        public SceneLights GetSceneLights()
        {
            return SceneLights;
        }

        public List<GameObject> GetGameObjects()
        {
            return Objects;
        }
        public void AddLight(DirectionalLightSource light)
        {
            SceneLights.directionalLightSources.Add(light);
        }
        public void AddLight(PointLightSource light)
        {
            SceneLights.pointLightSources.Add(light);
        }
        public void AddLight(SpotLightSource light)
        {
            SceneLights.spotLightSources.Add(light);
        }

        public void RemoveLight(DirectionalLightSource light)
        {
            SceneLights.directionalLightSources.Remove(light);
        }
    }

    class SceneLights
    {
        public List<DirectionalLightSource> directionalLightSources;
        public List<PointLightSource> pointLightSources;
        public List<SpotLightSource> spotLightSources;

        public SceneLights()
        {
            directionalLightSources = new();
            pointLightSources = new();
            spotLightSources = new();
        }
    }
}
