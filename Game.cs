using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using System.Diagnostics;
using GrassRendering.Controllers;

namespace GrassRendering
{
    public class Game : GameWindow
    {
        private DayTimeScheduler scheduler;

        private Camera camera;

        private Terrain terrain;
        private Grass grass;

        public Game(int width, int height, string title)
            : base(
                  GameWindowSettings.Default,
                  new NativeWindowSettings()
                  {
                      Size = (width, height),
                      Title = title,
                      WindowBorder = WindowBorder.Fixed,
                      StartVisible = false, //won't display the small initial window while setting things up
                      StartFocused = true,
                      API = ContextAPI.OpenGL,
                      Profile = ContextProfile.Core,
                      APIVersion = new Version(3, 3),
                  }) 
        {
            CenterWindow();

            scheduler = new DayTimeScheduler(DayTime.Morning);

            camera = new Camera(new Vector3(0.0f, 1.0f, 3.0f), Size.X / (float)Size.Y);            

            terrain = new Terrain(
                new Shader(
                    Shader.GetShader("..\\..\\..\\..\\GrassRendering\\assets\\shaders\\terrain\\terrainVS.glsl", ShaderType.VertexShader),
                    Shader.GetShader("..\\..\\..\\..\\GrassRendering\\assets\\shaders\\fog\\fog.glsl", ShaderType.FragmentShader),
                    Shader.GetShader("..\\..\\..\\..\\GrassRendering\\assets\\shaders\\terrain\\terrainFS.glsl", ShaderType.FragmentShader)),
                out grass);
        }

        protected override void OnResize(ResizeEventArgs e)
        {
            GL.Viewport(0, 0, e.Width, e.Height); //tells that every time the view port is the whole window (while resizing)
            base.OnResize(e);
        }

        protected override void OnLoad()
        {
            scheduler.timer.Start();
            CursorState = CursorState.Grabbed;
            IsVisible = true;

            GL.Enable(EnableCap.DepthTest); //enable z buffer

            base.OnLoad();
        }

        protected override void OnUnload()
        {
            scheduler.timer.Stop();

            terrain.Unload();
            grass.Unload();
         
            base.OnUnload();
        }

        protected override void OnUpdateFrame(FrameEventArgs e)        
        {
            if (!IsFocused) // check to see if the window is focused
            {
                return;
            }

            base.OnUpdateFrame(e);

            if(Keyboard.ProcessInput(KeyboardState, camera, (float)e.Time))
            {
                Close();
            }

            camera.UpdatePos(MouseState);

            scheduler.UpdateTime();
        }

        protected override void OnRenderFrame(FrameEventArgs args)
        {
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            GL.ClearColor((Color4)scheduler.current);



            terrain.Draw(camera, scheduler);
            grass.Draw(camera, scheduler);

            Context.SwapBuffers();
            base.OnRenderFrame(args);
        }

        protected override void OnMouseWheel(MouseWheelEventArgs e)
        {
            base.OnMouseWheel(e);

            camera.Fov -= e.OffsetY;
        }
    }
}
