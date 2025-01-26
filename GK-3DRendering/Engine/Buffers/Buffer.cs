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
    abstract class Buffer : IDisposable
    {
        public abstract BufferTarget BufferTarget { get; }
        public int Handle;
        public BufferUsageHint UsageHint;
        public Buffer(BufferUsageHint usageHint)
        {
            GL.CreateBuffers(1, out Handle);
            UsageHint = usageHint;
        }

        public void Load(Array data, int size)
        {
            GCHandle gchandle = GCHandle.Alloc(data, GCHandleType.Pinned);
            GL.NamedBufferData(Handle, size, gchandle.AddrOfPinnedObject(), UsageHint);
            gchandle.Free();
        }

        public void Bind()
        {
            GL.BindBuffer(BufferTarget, Handle);
        }

        public void Unbind()
        {
            GL.BindBuffer(BufferTarget, 0);
        }

        public void Dispose()
        {
            GL.DeleteBuffer(Handle);
            GC.SuppressFinalize(this);
        }
    }
}
