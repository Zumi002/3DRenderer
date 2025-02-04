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
            _controller = new ImGuiController(ClientSize.X, ClientSize.Y, defaultIni);
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

            ProjectionCamera camFollower = new ProjectionCamera(Scene, (float)ClientSize.X / ClientSize.Y, MathF.PI / 2, 0.1f, 200f, new Transform(new Vector3(0, 16, 10)));
            camFollower.Name = "Follower Camera";

            ProjectionCamera noMoveCamera = new ProjectionCamera(Scene, (float)ClientSize.X / ClientSize.Y, MathF.PI / 2, 0.1f, 200f, new Transform(new Vector3(0,16,25), new Vector3(MathHelper.DegreesToRadians(-30),0,0)));
            noMoveCamera.Name = "NoMove Camera";
           

            Cube cube = new Cube(Scene, "Green cube", new Transform(), new Vector3(0.2f, 1f, 0.2f));
            cube.Transform.SetPosition(new Vector3(0, 1.3f, 7f));

            Plane plane = new Plane(Scene);
            plane.Transform.SetPosition(new Vector3(0, -1f, 5f));
            plane.Transform.SetScale(new Vector3(20f, 1f, 20f));

            LoadableObject sofa = new LoadableObject("GK_3DRendering.Resources.Models.sofa.obj", Scene);
            sofa.Name = "Sofa";
            sofa.Transform.SetPosition(new Vector3(1f, -1f, 7f));
            sofa.Transform.SetScale(new Vector3(0.07f, 0.07f, 0.07f));

            LoadableObject toothless = new LoadableObject("GK_3DRendering.Resources.Models.toothless.obj", Scene);
            toothless.Name = "Toothless";
            toothless.Transform.SetPosition(new Vector3(0f, 0f, 17f));
            toothless.Transform.SetScale(new Vector3(0.03f, 0.03f, 0.03f));
            toothless.Transform.Rotate(new Vector3(0, -MathF.PI / 2, 0));

            toothless.AddAction((obj, dt) =>
            {
                obj.Transform.Translate(obj.Transform.Forward * dt * 10f);
                obj.Transform.Rotate(new Vector3(0, dt, 0));

            });

            
            camFollower.Controller = new FollowerController(camFollower.Transform, toothless.Transform);
            ((FollowerController)camFollower.Controller).speed = 0;
            

            LoadableObject sphere = new LoadableObject("GK_3DRendering.Resources.Models.sphere.obj", Scene);
            sphere.Transform.SetPosition(new Vector3(0, 1f, 2f));
            sphere.Name = "Sphere";

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
            
            ProjectionCamera onToothlessCamera = new ProjectionCamera(Scene, (float)ClientSize.X / ClientSize.Y, MathF.PI / 2, 0.1f, 200f, new Transform(new Vector3(0,100,-20)));
            toothless.AddChild(onToothlessCamera);

        }

        protected override void OnRenderFrame(FrameEventArgs args)
        {
            base.OnRenderFrame(args);

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

        private static readonly string defaultIni =
    "[Window][WindowOverViewport_11111111]\r\nPos=0,0\r\nSize=1500,800\r\nCollapsed=0\r\n\r\n[Window][Debug##Default]\r\nPos=60,60\r\nSize=400,400\r\nCollapsed=0\r\n\r\n[Window][Dear ImGui Demo]\r\nPos=590,108\r\nSize=516,306\r\nCollapsed=0\r\n\r\n[Window][uff]\r\nSize=347,1017\r\nCollapsed=0\r\nDockId=0x00000001,0\r\n\r\n[Window][GameWindow]\r\nPos=218,0\r\nSize=963,647\r\nCollapsed=0\r\nDockId=0x00000002,0\r\n\r\n[Window][Hierarchy]\r\nPos=0,0\r\nSize=216,800\r\nCollapsed=0\r\nDockId=0x00000007,0\r\n\r\n[Window][Console]\r\nPos=218,649\r\nSize=1282,151\r\nCollapsed=0\r\nDockId=0x00000009,0\r\n\r\n[Window][Dear ImGui Debug Log]\r\nPos=407,560\r\nSize=589,408\r\nCollapsed=0\r\n\r\n[Window][Inspector]\r\nPos=1183,0\r\nSize=317,647\r\nCollapsed=0\r\nDockId=0x00000004,0\r\n\r\n[Window][Dear ImGui Demo/##Basket_87771727]\r\nIsChild=1\r\nSize=444,260\r\n\r\n[Window][Dear ImGui Demo/ResizableChild_478B81A3]\r\nIsChild=1\r\nSize=465,136\r\n\r\n[Window][Dear ImGui Demo/Red_BEEF922B]\r\nIsChild=1\r\nSize=200,100\r\n\r\n[Window][Dear ImGui Style Editor]\r\nPos=172,46\r\nSize=353,794\r\nCollapsed=0\r\n\r\n[Window][Dear ImGui ID Stack Tool]\r\nPos=480,183\r\nSize=354,287\r\nCollapsed=0\r\n\r\n[Window][Example: Assets Browser]\r\nPos=60,60\r\nSize=800,480\r\nCollapsed=0\r\n\r\n[Window][Example: Property editor]\r\nPos=60,60\r\nSize=679,450\r\nCollapsed=0\r\n\r\n[Window][Example: Property editor/##tree_37EC733C]\r\nIsChild=1\r\nSize=300,425\r\n\r\n[Window][Example: Custom rendering]\r\nPos=60,60\r\nSize=623,414\r\nCollapsed=0\r\n\r\n[Table][0x51D6F5EA,3]\r\nColumn 0  Weight=1.0000\r\nColumn 1  Weight=1.0000\r\nColumn 2  Weight=1.0000\r\n\r\n[Table][0xB6880529,2]\r\nRefScale=13\r\nColumn 0  Sort=0v\r\n\r\n[Table][0x2048C668,2]\r\nRefScale=13\r\nColumn 0  Width=54\r\nColumn 1  Weight=2.0000\r\n\r\n[Docking][Data]\r\nDockSpace         ID=0x08BD597D Window=0x1BBC0F80 Pos=0,0 Size=1500,800 Split=X Selected=0x834F1B15\r\n  DockNode        ID=0x00000007 Parent=0x08BD597D SizeRef=216,800 Selected=0xBABDAE5E\r\n  DockNode        ID=0x00000008 Parent=0x08BD597D SizeRef=1282,800 Split=Y\r\n    DockNode      ID=0x00000006 Parent=0x00000008 SizeRef=1500,647 Split=X\r\n      DockNode    ID=0x00000003 Parent=0x00000006 SizeRef=963,800 Split=X\r\n        DockNode  ID=0x00000001 Parent=0x00000003 SizeRef=347,600 Selected=0x23966E15\r\n        DockNode  ID=0x00000002 Parent=0x00000003 SizeRef=1571,600 CentralNode=1 Selected=0x834F1B15\r\n      DockNode    ID=0x00000004 Parent=0x00000006 SizeRef=317,800 Selected=0x36DC96AB\r\n    DockNode      ID=0x00000009 Parent=0x00000008 SizeRef=1500,151 Selected=0xEA83D666\r\n";

    }
}
