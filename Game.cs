using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using System.Diagnostics;
using GrassRendering.Controllers;
using System.Reflection.Metadata;
using GrassRendering.Models;
using GrassRendering.Rendering;

namespace GrassRendering
{
    public class Game : GameWindow
    {
        private DayTimeScheduler scheduler;

        private Camera camera;

        private MainRenderer renderer;

        private bool setVibrations = false;

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

            renderer = new MainRenderer();
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

            renderer.Unload();

            base.OnUnload();
        }

        protected override void OnUpdateFrame(FrameEventArgs e)        
        {
            if (!IsFocused) // check to see if the window is focused
            {
                return;
            }

            base.OnUpdateFrame(e);

            if(Keyboard.ProcessInput(KeyboardState, camera, renderer, (float)e.Time, ref setVibrations))
            {
                Close();
            }

            camera.UpdatePos(MouseState);

            scheduler.UpdateTime();
        }

        protected override void OnRenderFrame(FrameEventArgs args)
        {
            renderer.Draw(camera, scheduler, setVibrations);

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
