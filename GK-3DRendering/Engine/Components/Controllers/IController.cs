using OpenTK.Windowing.GraphicsLibraryFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GK_3DRendering.Engine.Components.Controllers
{
    internal interface IController
    {
        void Update(float dt);

        void HandleInput(KeyboardState keyboardState, MouseState mouseState, float deltaTime);
    }
}
