using GK_3DRendering.Engine.Components;
using GK_3DRendering.Engine.Components.Renderers;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GK_3DRendering.Engine.Objects
{
    class Flag : GameObject
    {

       

        public Flag(Scene scene, string name, Transform transform, Vector3 color) : base(scene, name, transform) 
        {
            List<Vertex> vertices = new List<Vertex>();

            for (int j = 0; j < 4; j++)
            {
                for (int i = 0; i<4; i++)
                {
                    vertices.Add(new Vertex((float)i / 3, (float)j / 3, 0));
                }
            }

            Renderer = new FlagRenderer(new FlagControllMesh(vertices, new Vector2i(4, 4)), new Material(color), this, true);

            UpdateActions.Add((obj, dt) => 
            {
                ((FlagRenderer)obj.Renderer).AnimationTime+= dt * ((FlagRenderer)obj.Renderer).AnimationSpeed;
                if (((FlagRenderer)obj.Renderer).AnimationTime > 1)
                {
                    ((FlagRenderer)obj.Renderer).AnimationTime %= 1;
                }
            });
        }
        public Flag(Scene scene, string name, Transform transform) : this(scene, name, transform, new Vector3(1))
        { }
        public Flag(Scene scene, string name) : this (scene, name, new Transform())
        { }
        public Flag(Scene scene) : this(scene, "Flag")
        { }

        public Flag(Scene scene, IEnumerable<Vertex> vertices, Vector2i size, float animationTime) : base(scene)
        {

            Name = "Flag";
            Renderer = new FlagRenderer(new FlagControllMesh(vertices, size), new Material(), this, true);

            UpdateActions.Add((obj, dt) => { ((FlagRenderer)obj.Renderer).AnimationTime += dt * ((FlagRenderer)obj.Renderer).AnimationSpeed; });
        }
    }
}
