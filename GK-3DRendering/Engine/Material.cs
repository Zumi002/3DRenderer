using OpenTK.Graphics.ES20;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Mathematics;

namespace GK_3DRendering.Engine
{
    class Material
    {
        [InspectorColor]
        public Vector3 Color;
        [InspectorSlider<float>(0,1f)]
        public float ka;
        [InspectorSlider<float>(0, 1f)]
        public float kd;
        [InspectorSlider<float>(0, 1f)]
        public float ks;
        [InspectorSlider<float>(1f, 100f)]
        public float shinniness;
        public Material() : this(Vector3.One, 0.2f,0.7f,0.4f,30)
        {
            Color = Vector3.One;
            ka = 0.2f;
            kd = 0.7f;
            ks = 0.7f;
            shinniness = 30;
        }
        public Material(Vector3 color, float ka = 0.2f, float kd = 0.7f, float ks = 0.4f, float shinniness = 30)
        {
            Color = color;
            this.ka = ka;
            this.kd = kd;
            this.ks = ks;
            this.shinniness = shinniness;
        }
    }
}
