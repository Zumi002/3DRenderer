using GK_3DRendering.Engine.Components;
using GK_3DRendering.Engine.Objects;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GK_3DRendering.Engine.LightSource
{
    class PointLightSource : GameObject
    {
        [InspectorColor]
        public Vector3 Color;

        [InspectorSlider<float>(0,1f)]
        public float Ambient;
        [InspectorSlider<float>(0, 1f)]
        public float Diffuse;
        [InspectorSlider<float>(0, 1f)]
        public float Specular;

        public float Constatnt;
        public float Linear;
        public float Quadratic;

        

        public PointLightSource(Scene scene, Transform transform, Vector3 color,
                        float ambient = 0.05f, float diffuse = 0.7f, float specular = 1f,
                        float constant = 1.0f, float linear = 0.09f, float quadratic = 0.032f) : base(scene)
        {
            Transform = transform;
            Color = color;

            Ambient = ambient;
            Diffuse = diffuse;
            Specular = specular;

            Constatnt = constant;
            Linear = linear;
            Quadratic = quadratic;

            Name = "Point Light";
            
            scene.AddLight(this);
        }
        public PointLightSource(Scene scene) : this(scene, new Transform(), new Vector3(1f, 1f, 1f))
        { }
    }
}
