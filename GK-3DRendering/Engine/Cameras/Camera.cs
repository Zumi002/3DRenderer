using GK_3DRendering.Engine.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Mathematics;
using OpenTK.Windowing.Desktop;
using GK_3DRendering.Engine.Components.Controllers;
using GK_3DRendering.Engine.Objects;

namespace GK_3DRendering.Engine.Cameras
{
    abstract class Camera : GameObject
    {

        public IController Controller;
        public virtual void CalculateViewProjectionMatrix() { }
        public virtual void CalculateProjectionMatrix() { }
        public virtual void CalculateViewMatrix() { }
        public Camera(Scene scene, float aspect) : base(scene)
        {
            Aspect = aspect;
            Controller = new NullController();

            UpdateActions.Add((obj, dt) =>
            {
                ((Camera)obj).Controller.Update(dt);
            });
        }

        [InspectorIgnore]
        public float Aspect;

        protected Matrix4 _viewMatrix;
        public Matrix4 ViewMatrix
        {
            get
            {
                CalculateViewMatrix();
                return _viewMatrix;
            }
        }

        protected Matrix4 _projectionMatrix;
        public Matrix4 ProjectionMatrix
        {
            get
            {
                CalculateProjectionMatrix();
                return _projectionMatrix;
            }
        }

        protected Matrix4 _viewProjectionMatrix;
        public Matrix4 ViewProjectionMatrix
        {
            get
            {
                CalculateViewProjectionMatrix();
                return _viewProjectionMatrix;
            }
        }
    }
}
