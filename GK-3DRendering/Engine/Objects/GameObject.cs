using GK_3DRendering.Engine.Components;
using GK_3DRendering.Engine.Components.Renderers;
using OpenTK.Graphics.ES11;
using OpenTK.Windowing.GraphicsLibraryFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GK_3DRendering.Engine.Objects
{
    abstract class GameObject
    {
        public string Name { get; set; }
        public Transform Transform { get; set; }
        public IRenderable Renderer;
        public List<Action<GameObject, float>> UpdateActions {  get; set; }
        public Scene Scene { get; private set; }

        public GameObject? Parent;
        public List<GameObject> Childern {  get; private set; }


        public GameObject(Scene scene, string name, Transform transform)
        {
            Scene = scene;
            UpdateActions = new List<Action<GameObject, float>>();
            Transform = transform;
            Renderer = new NullRenderer();
            Scene.AddGameObject(this);
            Childern = new List<GameObject>();

            Name = name;
        }
        public GameObject(Scene scene, Transform transform) : this(scene, "Object", transform)
        { }
        public GameObject(Scene scene, string name) : this(scene, name, new Transform())
        { }
        public GameObject(Scene scene) : this(scene, "Object")
        { }
        

        public virtual void Update(float deltaTime)
        {
            foreach (var action in UpdateActions)
            {
                action.Invoke(this, deltaTime);
            }
        }

        public void AddAction(Action<GameObject, float> action)
        {
            UpdateActions.Add(action);
        }

        public void AddChild(GameObject child)
        {
            child.SetParent(this);
        }
        public void SetParent(GameObject parent)
        {
            if (Parent != null)
            {
                Parent.Childern.Remove(this);
            }
            Parent = parent;
            Parent.Childern.Add(this);
            Transform.SetParent(Parent.Transform);
        }
    }
}
