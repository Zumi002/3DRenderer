using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using GK_3DRendering.Engine.Buffers;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace GK_3DRendering.Engine
{
    class Mesh
    {
        public int vaoHandle;

        public List<Vertex> Vertices = new();
        public List<int> Indices = new();
        private List<VertexBuffer> VertexBuffers = new();
        public IndexBuffer IndexBuffer;
        
        public Mesh(IEnumerable<Vertex> vertices)
        {
            Vertices.AddRange(vertices);

            CreateVertexBuffer();
            SetupBuffers();
        }

        public Mesh(IEnumerable<Vertex> vertices, params VertexBuffer[] additionalVertexBuffers)
        {
            Vertices.AddRange(vertices);

            CreateVertexBuffer();
            VertexBuffers.AddRange(additionalVertexBuffers);
            SetupBuffers();
        }

        public Mesh(IEnumerable<Vertex> vertices, IEnumerable<int> indices)
        {
            Vertices.AddRange(vertices);
            Indices.AddRange(indices);

            CreateBuffers();
            SetupBuffersIndexed();
        }

        public Mesh(IndexBuffer indexBuffer, params VertexBuffer[] vertexBuffers)
        {
            VertexBuffers.AddRange(vertexBuffers);
            IndexBuffer = indexBuffer;

            SetupBuffersIndexed();
        }

        private void CreateBuffers()
        {
            CreateIndexBuffer();

            CreateVertexBuffer();
        }

        private void CreateIndexBuffer()
        {
            IndexBuffer = new IndexBuffer(DrawElementsType.UnsignedInt, Indices.Count, BufferUsageHint.DynamicDraw);
            IndexBuffer.Load(Indices.ToArray(), Indices.Count * sizeof(int));
        }

        private void CreateVertexBuffer()
        {
            VertexBufferAttrib[] vertexBufferAttribs = { new VertexBufferAttrib(0, 3, VertexAttribType.Float, false, 0), //position
                                                         new VertexBufferAttrib(1, 3, VertexAttribType.Float, true, 3*sizeof(float)), //normal vector
                                                         new VertexBufferAttrib(2, 2, VertexAttribType.Float, false, 6*sizeof(float))}; //tex cords

            VertexBuffer vbo = new VertexBuffer(vertexBufferAttribs, BufferUsageHint.DynamicDraw);
            vbo.Load(Vertices.ToArray(), Vertices.Count * Marshal.SizeOf<Vertex>());
            VertexBuffers.Add(vbo);
        }

        public void SetupBuffers()
        {
            //Create VAO
            GL.CreateVertexArrays(1, out vaoHandle);

            //Bind VBOs to VAO
            VertexBufferBindAttribs();
        }

        private void SetupBuffersIndexed()
        {
            SetupBuffers();
            //Bind indexBuffer to VAO
            GL.VertexArrayElementBuffer(vaoHandle, IndexBuffer.Handle);
        }

        private void VertexBufferBindAttribs()
        {
            //Bind VBOs to VAO
            for (int i = 0; i < VertexBuffers.Count; i++)
            {
                VertexBuffers[i].BindAttribs(vaoHandle, i);
            }
        }



        public void Bind()
        {
            GL.BindVertexArray(vaoHandle);
        }

        public void Unbind()
        {
            GL.BindVertexArray(0);
        }

        public void Dispose()
        {
            GL.DeleteVertexArray(vaoHandle);
            foreach (var vertexBuffer in VertexBuffers)
            {
                vertexBuffer.Dispose();
            }
            IndexBuffer.Dispose();
            GC.SuppressFinalize(this);
        }
    }

    struct Vertex
    {
        public Vector3 Position;
        public Vector3 Normal;
        public Vector2 Texture;


        public Vertex(float x, float y, float z) : this(new Vector3(x, y, z), new Vector3(0, 1, 0), new Vector2(0, 0))
        {  }
        public Vertex(float x, float y, float z, float tx, float ty) : this(new Vector3(x, y, z), new Vector3(0, 1, 0), new Vector2(tx, ty))
        { }
        public Vertex(float x, float y, float z, float nx, float ny, float nz) : this(new Vector3(x, y, z), new Vector3(nx, ny, nz), new Vector2(0, 0))
        { }
        public Vertex(float x, float y, float z, float nx, float ny, float nz, float tx, float ty) : this(new Vector3(x, y, z), new Vector3(nx, ny, nz), new Vector2(tx, ty))
        { }
        public Vertex(Vector3 position) : this(position, new Vector3(0, 1, 0), new Vector2(0, 0))
        { }
        public Vertex(Vector3 position, Vector2 texture) : this(position, new Vector3(0, 1, 0), texture)
        { }
        public Vertex(Vector3 position, Vector3 normals) : this(position, normals, new Vector2(0, 0))
        { }
        public Vertex(Vector3 position, Vector3 normal, Vector2 texture)
        {
            Position = position;
            Normal = normal;
            Texture = texture;
        }
    }
}
