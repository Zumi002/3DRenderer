using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GK_3DRendering.Engine.Objects;
using OpenTK.Mathematics;
namespace GK_3DRendering.Engine.LightSource
{
    class DirectionalLightSource : GameObject
    {
        [InspectorColor]
        public Vector3 Color;
        
        public Vector3 Direction;

        [InspectorSlider<float>(0,1f)]
        public float Ambient;
        [InspectorSlider<float>(0, 1f)]
        public float Diffuse;
        [InspectorSlider<float>(0, 1f)]
        public float Specular;

        private bool _on;
        public float animSpeed = 1f;
        private float animTime = 0;
        public bool animate = false;

        public DirectionalLightSource(Scene scene, Vector3 color, Vector3 direction,
                                      float ambient = 0.05f, float diffuse = 0.7f, float specular = 1f ) : base(scene)
        {
            Color = color;
            Direction = direction;

            Ambient = ambient;
            Diffuse = diffuse;
            Specular = specular;

            Name = "Directional Light";

            scene.AddLight(this);
            _on = true;

            UpdateActions.Add((obj, dt) =>
            {
                
                DirectionalLightSource dirL = (DirectionalLightSource)obj;
                if (dirL.animate)
                {
                    dirL.animTime += dt* dirL.animSpeed*0.05f;
                    float phase = dirL.animTime * MathF.PI * 2;
                    ((DirectionalLightSource)obj).Direction = new Vector3(-MathF.Cos(phase), -MathF.Sin(phase), 0);
                    if (dirL.animTime > 0.5f)
                    {
                        dirL.Off();
                    }
                    else
                    {
                        dirL.On();
                    }
                    dirL.animTime %= 1;
                }
            });
        }

        public DirectionalLightSource(Scene scene) : this(scene, new Vector3(1f), new Vector3(0,-1f,0)) 
        {

        }

        public void Off()
        {
            if (_on)
            {
                Scene.RemoveLight(this);
                _on = false;
            }
        }

        public void On()
        {
            if (!_on)
            {
                Scene.AddLight(this);
                _on = true;
            }
        }

        public bool IsOn()
        {
            return _on;
        }

        public void FlipAnimate()
        {
            animate = !animate; 
        }
    }
}
