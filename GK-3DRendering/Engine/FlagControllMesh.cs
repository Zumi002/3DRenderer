using GK_3DRendering.Engine.Buffers;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL4;
using System.Runtime.InteropServices;
using System.Drawing;


namespace GK_3DRendering.Engine
{
    class FlagControllMesh 
    {
        public Mesh controllMesh {  get; private set; }
        public Vector2i Size { get; private set; }
        public FlagControllMesh(IEnumerable<Vertex> vertices, Vector2i size)
        {
            Size = size;

            controllMesh = new Mesh(vertices, CreateFlagUVVBO());
        }

        private VertexBuffer CreateFlagUVVBO()
        {
            VertexBufferAttrib[] vertexBufferAttribs = { new VertexBufferAttrib(3, 2, VertexAttribType.Float, false, 0)}; //flagUV
            VertexBuffer vbo = new VertexBuffer(vertexBufferAttribs, BufferUsageHint.DynamicDraw);

            List<Vector2> vectorUV = new();
            for (int j = 0; j < Size.Y; j++)
            {
                for (int i = 0; i < Size.X; i++)
                {
                    vectorUV.Add(new Vector2((float)i / (Size.X - 1), (float)j / (Size.Y - 1)));
                }
            }
            vbo.Load(vectorUV.ToArray(), vectorUV.Count * Marshal.SizeOf<Vertex>());

            return vbo;
        }
    }
}
