using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL4;

namespace GK_3DRendering.Engine.Buffers
{
    class VertexBuffer : Buffer
    {
        public override BufferTarget BufferTarget => BufferTarget.ArrayBuffer;
        public List<VertexBufferAttrib> Attribs = new();
        private int Stride = 0;
        public VertexBuffer(VertexBufferAttrib[] attribs, BufferUsageHint usageHint) : base(usageHint)
        {
            Attribs.AddRange(attribs);
            Stride = Attribs.Select(a => a.getSize()).Sum();
        }

        public void BindAttribs(int vaoHandle, int index)
        {
            GL.VertexArrayVertexBuffer(vaoHandle, index, Handle, 0, Stride);
            foreach (var attrib in Attribs)
            {
                GL.VertexArrayAttribBinding(vaoHandle, attrib.Index, index);
                GL.VertexArrayAttribFormat(vaoHandle, attrib.Index, attrib.Count, attrib.Type, attrib.Normalize, attrib.Offset);
                GL.EnableVertexArrayAttrib(vaoHandle, attrib.Index);
            }
        }
    }

    class VertexBufferAttrib
    {
        private static readonly Dictionary<VertexAttribType, int> TypeSizes = new()
        {
            { VertexAttribType.Byte, sizeof(sbyte) },
            { VertexAttribType.UnsignedByte, sizeof(byte) },
            { VertexAttribType.Short, sizeof(short) },
            { VertexAttribType.UnsignedShort, sizeof(ushort) },
            { VertexAttribType.Int, sizeof(int) },
            { VertexAttribType.UnsignedInt, sizeof(uint) },
            { VertexAttribType.Float, sizeof(float) },
            { VertexAttribType.Double, sizeof(double) },
            { VertexAttribType.HalfFloat, 2 }
        };
        public int Index;
        public int Count;
        public VertexAttribType Type;
        public bool Normalize;
        public int Offset;

        public VertexBufferAttrib(int index, int count, VertexAttribType type, bool normalize, int offset)
        {
            Index = index;
            Count = count;
            Type = type;
            Normalize = normalize;
            Offset = offset;
        }

        public int getSize()
        {
            return Count * TypeSizes[Type];
        }
    }
}
