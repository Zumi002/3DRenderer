using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.Common;
using OpenTK.Mathematics;

using ErrorCode = OpenTK.Graphics.OpenGL.ErrorCode;
using GK_3DRendering.Engine.Buffers;
using GK_3DRendering.Engine.Components;
using GK_3DRendering.Engine.Components.Controllers;
using GK_3DRendering.Engine.Cameras;
using GK_3DRendering.Engine.Objects;
using GK_3DRendering.Engine.SceneRenderer;
using GK_3DRendering.Engine.LightSource;
using ImGuiNET;
using GK_3DRendering.Engine;

namespace GK_3DRendering
{
    internal class Game : GameWindow
    {
        ImGuiController _controller;
        InImGuiWindowRenderer _inImGuiWindowRenderer;
        ImGuiHierarchy _ImGuiHierarchy;
        ImGuiInspector _ImGuiInspector;
        public Game(int width, int height, string title) : base(GameWindowSettings.Default, new NativeWindowSettings() { ClientSize = (width, height), Title = title }) { }

        public Scene Scene { get; set; }
        public ISceneRenderer SceneRenderer { get; set; }
        protected override void OnLoad()
        {
            base.OnLoad();
            VSync = VSyncMode.On;
            _controller = new ImGuiController(ClientSize.X, ClientSize.Y);
            _inImGuiWindowRenderer = new InImGuiWindowRenderer(this);
            _ImGuiHierarchy = new ImGuiHierarchy();
            _ImGuiInspector = new ImGuiInspector();
            ImGui.GetIO().ConfigDebugHighlightIdConflicts = false;

            GL.Enable(EnableCap.DepthTest);
            GL.DepthRange(0.1f, 1.0f);
            GL.ClearColor(0.2f, 0.3f, 0.3f, 1.0f);

            ShaderManager.AddShader(ShaderManager.MyShaders.Default, new Shader(("GK_3DRendering.Shaders.shader.vert", ShaderType.VertexShader),
                                                                                ("GK_3DRendering.Shaders.shader.frag", ShaderType.FragmentShader)));
            ShaderManager.AddShader(ShaderManager.MyShaders.FlagShader, new Shader(("GK_3DRendering.Shaders.flag.vert", ShaderType.VertexShader),
                                                                                   ("GK_3DRendering.Shaders.flag.tesc", ShaderType.TessControlShader),
                                                                                   ("GK_3DRendering.Shaders.flag.tese", ShaderType.TessEvaluationShader),
                                                                                   ("GK_3DRendering.Shaders.shader.frag", ShaderType.FragmentShader)));

            Scene = new Scene();
            SceneRenderer = new OpenGLSceneRenderer();

            ProjectionCamera cam = new ProjectionCamera(Scene, (float)ClientSize.X / ClientSize.Y, MathF.PI / 2, 0.1f, 200f, new Transform());
            cam.Name = "FPP Camera";
            Scene.SetActiveCamera(cam);
            cam.Controller = new FPPController(cam.Transform);
            cam.Transform.SetPosition(new Vector3(0, 0, 10f));

            ProjectionCamera camFollower = new ProjectionCamera(Scene, (float)ClientSize.X / ClientSize.Y, MathF.PI / 2, 0.1f, 200f, new Transform());
            camFollower.Name = "Follower Camera";

            ProjectionCamera noMoveCamera = new ProjectionCamera(Scene, (float)ClientSize.X / ClientSize.Y, MathF.PI / 2, 0.1f, 200f, new Transform(new Vector3(0,16,25), new Vector3(MathHelper.DegreesToRadians(-30),0,0)));
            noMoveCamera.Name = "NoMove Camera";
           

            Cube cube = new Cube(Scene, "Green cube", new Transform(), new Vector3(0.2f, 1f, 0.2f));
            cube.Transform.SetPosition(new Vector3(0, 1.3f, 7f));

            Plane plane = new Plane(Scene);
            plane.Transform.SetPosition(new Vector3(0, -1f, 5f));
            plane.Transform.SetScale(new Vector3(20f, 1f, 20f));

            LoadableObject sofa = new LoadableObject("GK_3DRendering.Resources.Models.sofa.obj", Scene);
            sofa.Transform.SetPosition(new Vector3(1f, -1f, 7f));
            sofa.Transform.SetScale(new Vector3(0.07f, 0.07f, 0.07f));

            LoadableObject toothless = new LoadableObject("GK_3DRendering.Resources.Models.toothless.obj", Scene);
            toothless.Transform.SetPosition(new Vector3(0f, 0f, 17f));
            toothless.Transform.SetScale(new Vector3(0.03f, 0.03f, 0.03f));
            toothless.Transform.Rotate(new Vector3(0, -MathF.PI / 2, 0));

            toothless.AddAction((obj, dt) =>
            {
                obj.Transform.Translate(obj.Transform.Forward * dt * 10f);
                obj.Transform.Rotate(new Vector3(0, dt, 0));

            });

            
            camFollower.Controller = new FollowerController(camFollower.Transform, toothless.Transform);
            

            LoadableObject sphere = new LoadableObject("GK_3DRendering.Resources.Models.sphere.obj", Scene);
            sphere.Transform.SetPosition(new Vector3(0, 1f, 2f));

            Cube flagPole = new Cube(Scene, "Flag pole",
                                     new Transform(new Vector3(-10, 5, -10), Vector3.Zero, new Vector3(1, 10, 1)),
                                     new Vector3(0.7f));
            Flag flag = new Flag(Scene, "Flag white",
                                 new Transform(new Vector3(1f, 0, 0), Vector3.Zero, new Vector3(10, 0.5f, 5)));
            Flag flag2 = new Flag(Scene, "Flag red",
                                  new Transform(new Vector3(1f, 0.5f, 0), Vector3.Zero, new Vector3(10, 0.5f, 5)),
                                  new Vector3(1f, 0, 0));
            flagPole.AddChild(flag);
            flagPole.AddChild(flag2);


            DirectionalLightSource sun = new DirectionalLightSource(Scene);
            PointLightSource bulb1 = new PointLightSource(Scene, new Transform(new Vector3(2f, 1f, 7f)), new Vector3(1f, 1f, 0), 1f, 1f, 1f);

            SpotLightSource refl = new SpotLightSource(Scene, new Transform(new Vector3(0, 100, -30), new Vector3(-MathF.PI / 2, 0, 0)), new Vector3(1f, 0.5f, 1f), MathHelper.DegreesToRadians(30f), MathHelper.DegreesToRadians(35.5f), 0, 1f, 0.5f, 1f, 0.022f, 0.0019f);
            SpotLightSource refl2 = new SpotLightSource(Scene, new Transform(new Vector3(0, 10, 30), new Vector3(0, 0, 0)), new Vector3(1f, 0.5f, 1f), MathHelper.DegreesToRadians(30f), MathHelper.DegreesToRadians(35.5f), 0, 1f, 1f, 1f, 0.0022f, 0.00019f);

            SpotLightSource reflFlag = new SpotLightSource(Scene, new Transform(new Vector3(15, 0.5f, 10), new Vector3(MathHelper.DegreesToRadians(-6), MathHelper.DegreesToRadians(27), 0)), new Vector3(1f, 1f, 1f), MathHelper.DegreesToRadians(30f), MathHelper.DegreesToRadians(35.5f), 0, 1f, 1f, 1f, 0.0022f, 0.00019f);

            reflFlag.SetParent(flagPole);
            refl.SetParent(toothless);
            toothless.AddChild(refl2);
            
            

        }

        protected override void OnRenderFrame(FrameEventArgs args)
        {
            base.OnRenderFrame(args);


            //GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit | ClearBufferMask.StencilBufferBit);

            //SceneRenderer.RenderScene(Scene);

            GL.Clear(ClearBufferMask.DepthBufferBit | ClearBufferMask.StencilBufferBit);

            ImGui.DockSpaceOverViewport();

            _inImGuiWindowRenderer.DrawVieportWindow(SceneRenderer, Scene);

            _ImGuiHierarchy.RenderImGuiHierarchy(Scene);
            RenderImGuiConsoleLog();
            _ImGuiInspector.RenderImGuiInspector(_ImGuiHierarchy.Selected);

            //ImGui.ShowDemoWindow();

            _controller.Render();


            //Console.WriteLine($"{1f / UpdateTime}fps");
            SwapBuffers();
        }

        protected override void OnUpdateFrame(FrameEventArgs args)
        {
            base.OnUpdateFrame(args);
            Scene.Update((float)args.Time);
            Scene.GetActiveCamera().Aspect = _inImGuiWindowRenderer.Aspect;
            MouseState mouseState = MouseState.GetSnapshot();
            _controller.Update(this, (float)args.Time);

            if (_inImGuiWindowRenderer.Focused)
            {
                if (mouseState.IsButtonDown(MouseButton.Left))
                {
                    CursorState = CursorState.Grabbed;
                }
                else
                {
                    CursorState = CursorState.Normal;
                }
                Scene.GetActiveCamera().Controller.HandleInput(KeyboardState.GetSnapshot(), MouseState.GetSnapshot(), (float)args.Time);
            }
            else
            {
                CursorState = CursorState.Normal;
            }
            //Console.WriteLine($"{Scene.GetActiveCamera().Transform.Position}");
        }

        protected override void OnFramebufferResize(FramebufferResizeEventArgs e)
        {
            base.OnFramebufferResize(e);

            GL.Viewport(0, 0, e.Width, e.Height);
            _controller.WindowResized(ClientSize.X, ClientSize.Y);
        }

        private void RenderImGuiConsoleLog()
        {
            ImGui.Begin("Console");
            ImGui.End();
        }



        protected override void OnTextInput(TextInputEventArgs e)
        {
            base.OnTextInput(e);


            _controller.PressChar((char)e.Unicode);
        }

        protected override void OnMouseWheel(MouseWheelEventArgs e)
        {
            base.OnMouseWheel(e);

            _controller.MouseScroll(e.Offset);
        }
    }
}
