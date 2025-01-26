using GK_3DRendering.Engine.Components;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GK_3DRendering.Engine.Cameras
{
    class ProjectionCamera : Camera
    {

        float Fov;
        float Near, Far;

        public ProjectionCamera(Scene scene, float aspect, float fov, float near, float far, Transform transform) : base(scene, aspect)
        {
            Fov = fov;
            Near = near;
            Far = far;
            Transform = transform;
            Name = "Projection Camera";
        }

        

        public override void CalculateViewProjectionMatrix()
        {
            CalculateViewMatrix();
            CalculateProjectionMatrix();
            _viewProjectionMatrix = Matrix4.Mult(ViewMatrix, ProjectionMatrix);
        }

        public override void CalculateViewMatrix()
        {
            

            _viewMatrix = Matrix4.LookAt(Transform.Position, Transform.Position+Transform.Forward, Transform.Up);

        }

        public override void CalculateProjectionMatrix()
        {
            _projectionMatrix = Matrix4.CreatePerspectiveFieldOfView(Fov, Aspect, Near, Far);
        }
    }
}
