using GK_3DRendering.Engine.Cameras;
using GK_3DRendering.Engine.Objects;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GK_3DRendering.Engine.Components.Renderers
{
    class FlagRenderer : IRenderable
    {
        [InspectorIgnore]
        public FlagControllMesh Mesh { get; }

        public Material Material { get; }

        [InspectorIgnore]
        public GameObject Parent;

        [InspectorIgnore]
        public Vector2i Size { get; private set; }

        [InspectorIgnore]
        public int Count { get; private set; }
        [InspectorIgnore]
        public float AnimationTime;
        public float AnimationSpeed = 0.2f;

        public bool OneSided;

        public FlagRenderer(FlagControllMesh mesh,  Material material, GameObject parent, bool oneSided = false)
        {
            Mesh = mesh;
            Material = material;
            Parent = parent;
            Size = mesh.Size;
            Count = Size.X*Size.Y;
            AnimationTime = 0;

            OneSided = oneSided;    

            Parent.Scene.AddRenderer(this);
        }
    }
}
