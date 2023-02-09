using GrassRendering.Controllers;
using GrassRendering.Models.Terrain;
using GrassRendering.Models.Interfaces;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using GrassRendering.Models;

namespace GrassRendering.Rendering
{
    internal class MainRenderer
    {
        private WaterFrameBuffers buffers;

        private int[] extShaders;

        private List<IModel> models;


        public MainRenderer()
        {
            buffers = new WaterFrameBuffers();

            extShaders = new int[]
            {
                Shader.GetShader("..\\..\\..\\shaders\\fog\\fog.glsl", ShaderType.FragmentShader),
            };
            models = new List<IModel>()
            {
                new Terrain(extShaders, buffers),
                new Duck(
                    "..\\..\\..\\assets\\models\\duck\\duck.dae",
                    new Shader(
                        extShaders,
                        Shader.GetShader("..\\..\\..\\shaders\\duck\\duckVS.glsl", ShaderType.VertexShader),
                        Shader.GetShader("..\\..\\..\\shaders\\duck\\duckFS.glsl", ShaderType.FragmentShader))),
        };
        }

        public void Draw(Camera camera, DayTimeScheduler scheduler, bool setVibrations)
        {
            GL.Enable(EnableCap.ClipDistance0);

            RenderReflectionTexture(camera, scheduler, setVibrations);

            RenderRefractionTexture(camera, scheduler, setVibrations);

            GL.Disable(EnableCap.ClipDistance0);

            RenderToScreen(camera, scheduler, setVibrations);
        }

        private void RenderReflectionTexture(Camera camera, DayTimeScheduler scheduler, bool setVibrations)
        {
            buffers.BindReflectionFrameBuffer();

            float distance = 2 * (camera.position.Y - Constants.waterHeight);
            camera.position.Y -= distance;
            camera.InvertPitch();

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            foreach (IModel model in models)
            {
                model.Draw(camera, scheduler, setVibrations, new Vector4(0, 1, 0, -(Constants.waterHeight + 0.001f)));
            }
            GL.Finish();

            camera.position.Y += distance;
            camera.InvertPitch();
        }

        private void RenderRefractionTexture(Camera camera, DayTimeScheduler scheduler, bool setVibrations)
        {
            buffers.BindRefractionFrameBuffer();

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            foreach (IModel model in models)
            {
                model.Draw(camera, scheduler, setVibrations, new Vector4(0, -1, 0, Constants.waterHeight));
            }
            GL.Finish();
        }

        private void RenderToScreen(Camera camera, DayTimeScheduler scheduler, bool setVibrations)
        {
            buffers.UnbindCurrentFrameBuffer();

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            GL.ClearColor((Color4)scheduler.current);
            foreach (IModel model in models)
            {
                model.Draw(camera, scheduler, setVibrations);
            }
        }

        public void Unload()
        {
            foreach (int shader in extShaders) GL.DeleteShader(shader);

            foreach (IModel model in models) model.Unload();

            buffers.Unload();
        }
    }
}

