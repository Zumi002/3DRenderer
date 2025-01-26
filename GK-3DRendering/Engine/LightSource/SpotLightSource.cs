using GK_3DRendering.Engine.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Mathematics;
using GK_3DRendering.Engine.Objects;
using System.Runtime.CompilerServices;

namespace GK_3DRendering.Engine.LightSource
{
    class SpotLightSource : GameObject
    {
        [InspectorColor]
        public Vector3 Color;

        [InspectorSlider<float>(0,360)]
        public float CutOffAngle;
        [InspectorSlider<float>(0, 360)]
        public float OuterCutOffAngle;


        [InspectorIgnore]
        public float CutOff
        {
            get
            {
                return MathF.Cos(MathHelper.DegreesToRadians(CutOffAngle));
            }
        }
        [InspectorIgnore]
        public float OuterCutOff
        {
            get
            {
                return MathF.Cos(MathHelper.DegreesToRadians(OuterCutOffAngle));
            }
        }

        [InspectorSlider<float>(0, 1f)]
        public float Ambient;
        [InspectorSlider<float>(0, 1f)]
        public float Diffuse;
        [InspectorSlider<float>(0, 1f)]
        public float Specular;


        public float Constatnt;
        public float Linear;
        public float Quadratic;


        public SpotLightSource(Scene scene, Transform transform, Vector3 color, 
                        float cutOffRad, float outerCutOffRad,
                        float ambient = 0.05f, float diffuse = 0.7f, float specular = 1f,
                        float constant = 1.0f, float linear = 0.09f, float quadratic = 0.032f) : base(scene)
        {

            Transform = transform;
            Color = color;

            CutOffAngle = MathHelper.RadiansToDegrees(cutOffRad);
            OuterCutOffAngle = MathHelper.RadiansToDegrees(outerCutOffRad);

            Ambient = ambient;
            Diffuse = diffuse;
            Specular = specular;

            Constatnt = constant;
            Linear = linear;
            Quadratic = quadratic;

            Name = "Spot Light";

            scene.AddLight(this);
        }
        public SpotLightSource(Scene scene, Transform transform, Vector3 color) : this(scene, transform, color, MathHelper.DegreesToRadians(12.5f), MathHelper.DegreesToRadians(17.5f))
        { }
        public SpotLightSource(Scene scene) : this(scene, new Transform(), new Vector3(1f, 1f, 1f), MathHelper.DegreesToRadians(12.5f), MathHelper.DegreesToRadians(17.5f))
        { }
    }
}
