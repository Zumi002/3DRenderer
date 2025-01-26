using ImGuiNET;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.Desktop;
using GK_3DRendering.Engine.SceneRenderer;
using GK_3DRendering.Engine;

namespace GK_3DRendering
{
    class InImGuiWindowRenderer : IDisposable
    {
        int FboHandle;
        int RboHandle;
        int TexColor;

        public bool Focused = false;

        public float Aspect = 16f / 9f;

        Vector2i fboSize = default;
        private readonly GameWindow window;

        public InImGuiWindowRenderer(GameWindow window)
        {
            FboHandle = 0;
            RboHandle = 0;
            TexColor = 0;

            this.window = window;
        }

        public void DrawVieportWindow(ISceneRenderer sceneRenderer, Scene scene)
        {
            ImGui.Begin("GameWindow");
            {
                // Using a Child allow to fill all the space of the window.
                // It also alows customization
                ImGui.BeginChild("GameRender");

                // Get the size of the child (i.e. the whole draw size of the windows).
                System.Numerics.Vector2 wsize = ImGui.GetWindowSize();
                Aspect = wsize.X / wsize.Y;
                // make sure the buffers are the currect size
                Vector2i wsizei = new((int)wsize.X, (int)wsize.Y);

                if (fboSize != wsizei)
                {
                    fboSize = wsizei;

                    // create our frame buffer if needed
                    if (FboHandle == 0)
                    {
                        FboHandle = GL.GenFramebuffer();
                        // bind our frame buffer
                        GL.BindFramebuffer(FramebufferTarget.Framebuffer, FboHandle);
                        GL.ObjectLabel(ObjectLabelIdentifier.Framebuffer, FboHandle, 10, "GameWindow");
                    }

                    // bind our frame buffer
                    GL.BindFramebuffer(FramebufferTarget.Framebuffer, FboHandle);

                    if (TexColor > 0)
                        GL.DeleteTexture(TexColor);

                    TexColor = GL.GenTexture();
                    GL.BindTexture(TextureTarget.Texture2D, TexColor);
                    GL.ObjectLabel(ObjectLabelIdentifier.Texture, TexColor, 16, "GameWindow:Color");
                    GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgb, wsizei.X, wsizei.Y, 0, PixelFormat.Rgb, PixelType.UnsignedByte, nint.Zero);
                    GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
                    GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
                    GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0, TextureTarget.Texture2D, TexColor, 0);

                    if (RboHandle > 0)
                        GL.DeleteRenderbuffer(RboHandle);

                    RboHandle = GL.GenRenderbuffer();
                    GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, RboHandle);
                    GL.ObjectLabel(ObjectLabelIdentifier.Renderbuffer, RboHandle, 16, "GameWindow:Depth");
                    GL.RenderbufferStorage(RenderbufferTarget.Renderbuffer, RenderbufferStorage.DepthComponent32f, wsizei.X, wsizei.Y);
                    GL.FramebufferRenderbuffer(FramebufferTarget.Framebuffer, FramebufferAttachment.DepthAttachment, RenderbufferTarget.Renderbuffer, RboHandle);
                    //GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, 0);

                    //texDepth = GL.GenTexture();
                    //GL.BindTexture(TextureTarget.Texture2D, texDepth);
                    //GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.DepthComponent32f, 800, 600, 0, PixelFormat.DepthComponent, PixelType.Float, IntPtr.Zero);
                    //GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.DepthAttachment, TextureTarget.Texture2D, texDepth, 0);

                    // make sure the frame buffer is complete
                    FramebufferErrorCode errorCode = GL.CheckFramebufferStatus(FramebufferTarget.Framebuffer);
                    if (errorCode != FramebufferErrorCode.FramebufferComplete)
                        throw new Exception();
                }
                else
                {
                    // bind our frame and depth buffer
                    GL.BindFramebuffer(FramebufferTarget.Framebuffer, FboHandle);
                    GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, FboHandle);
                }

                GL.Viewport(0, 0, wsizei.X, wsizei.Y); // change the viewport to window

                // actually draw the scene
                {
                    GL.ClearColor(Color4.DimGray);
                    GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

                    //Draw triangle
                    sceneRenderer.RenderScene(scene);
                }

                // unbind our bo so nothing else uses it
                GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);

                GL.Viewport(0, 0, window.ClientSize.X, window.ClientSize.Y); // back to full screen size

                // Because I use the texture from OpenGL, I need to invert the V from the UV.
                GL.ActiveTexture(TextureUnit.Texture0);
                GL.BindTexture(TextureTarget.Texture2D, TexColor);
                ImGui.Image(new nint(TexColor), wsize, System.Numerics.Vector2.UnitY, System.Numerics.Vector2.UnitX);
                //ImGui.Image(new IntPtr(TexColor), wsize);

                Focused = ImGui.IsWindowFocused();

                ImGui.EndChild();
            }
            ImGui.End();
        }

        public void Dispose()
        {
            GL.DeleteFramebuffer(FboHandle);
        }
    }

}
