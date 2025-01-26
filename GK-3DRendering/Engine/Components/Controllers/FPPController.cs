using OpenTK.Windowing.GraphicsLibraryFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Mathematics;
using GK_3DRendering.Engine.Components;

namespace GK_3DRendering.Engine.Components.Controllers
{
    class FPPController : IController
    {
        public float speed = 5f;
        public float sensitivity= 0.002f;
        private Transform Parent;
        public FPPController(Transform parent)
        {
            Parent = parent;    
        }
        public void Update(float dt)
        {
        }

        public void HandleInput(KeyboardState keyboardState, MouseState mouseState, float deltaTime)
        {
            Vector3 movement = Vector3.Zero;
            if(keyboardState.IsKeyDown(Keys.W))
            {
                movement += Parent.LocalForward * speed * deltaTime;
            }
            if (keyboardState.IsKeyDown(Keys.A))
            {
                movement += -Parent.LocalRight * speed * deltaTime;
            }
            if (keyboardState.IsKeyDown(Keys.D))
            {
                movement += Parent.LocalRight * speed * deltaTime;
            }
            if (keyboardState.IsKeyDown(Keys.S))
            {
                movement += -Parent.LocalForward * speed * deltaTime;
            }
            if(keyboardState.IsKeyDown(Keys.Space))
            {
                movement += Parent.LocalUp * speed * deltaTime;
            }
            if (keyboardState.IsKeyDown(Keys.LeftShift))
            {
                movement += -Parent.LocalUp * speed * deltaTime;
            }

            Vector3 rotation = Parent.LocalRotation;

            if(mouseState.IsButtonDown(MouseButton.Left))
            {

                rotation += new Vector3(-mouseState.Delta.Y, -mouseState.Delta.X, 0f)*sensitivity;
            }

            rotation.X = Math.Clamp(rotation.X,-MathF.PI/2, MathF.PI/2);
            Parent.SetRotation(rotation);
            Parent.Translate(movement);
        }
    }
}
