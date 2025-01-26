using OpenTK.Windowing.GraphicsLibraryFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GK_3DRendering.Engine.Components.Controllers
{
    
    class FollowerController : IController
    {
        public float speed = 5f;
        private Transform Parent;
        private Transform Target;

        public FollowerController(Transform parent, Transform target)
        {
            Parent = parent;
            Target = target;
        }
        public void Update(float dt)
        {
            Parent.LookAtTarget(Target.Position);
            Parent.Translate(Parent.Forward * speed * dt);
        }

        public void HandleInput(KeyboardState keyboardState, MouseState mouseState, float deltaTime)
        {
            
        }
    }
}
