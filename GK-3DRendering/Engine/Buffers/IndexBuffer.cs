using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using ImGuiNET;
using OpenTK.Graphics.OpenGL4;

namespace GK_3DRendering.Engine.Buffers
{
    class IndexBuffer : Buffer
    {
        public override BufferTarget BufferTarget => BufferTarget.ElementArrayBuffer;
        public DrawElementsType ElementsType;
        public int Indices;

        public IndexBuffer(DrawElementsType elementsType, int indices, BufferUsageHint bufferUsageHint) : base(bufferUsageHint)
        {
            ElementsType = elementsType;
            Indices = indices;
        }
    }
}
