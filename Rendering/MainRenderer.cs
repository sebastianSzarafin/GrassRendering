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

        private int watchedObjectIndex = -1;
        private Vector3[] watchedObjectPositions;

        private Terrain terrain;
        private List<IModel> models;

        private Light light;


        public MainRenderer()
        {

            buffers = new WaterFrameBuffers();

            extShaders = new int[]
            {
                Shader.GetShader("..\\..\\..\\shaders\\external\\fog.glsl", ShaderType.FragmentShader),
                Shader.GetShader("..\\..\\..\\shaders\\external\\lightning.glsl", ShaderType.FragmentShader),
            };

            terrain = new Terrain(extShaders, buffers);
            models = new List<IModel>()
            {
                new Duck(
                    "..\\..\\..\\assets\\models\\duck\\duck.dae",
                    out watchedObjectPositions,
                    new Shader(
                        extShaders,
                        Shader.GetShader("..\\..\\..\\shaders\\duck\\duckVS.glsl", ShaderType.VertexShader),
                        Shader.GetShader("..\\..\\..\\shaders\\duck\\duckFS.glsl", ShaderType.FragmentShader))),
            };

            light = new Light(
                "..\\..\\..\\assets\\models\\light\\sphere.obj",
                new Shader(
                        extShaders,
                        Shader.GetShader("..\\..\\..\\shaders\\light\\lightVS.glsl", ShaderType.VertexShader),
                        Shader.GetShader("..\\..\\..\\shaders\\light\\lightFS.glsl", ShaderType.FragmentShader)));
        }

        public void UpdateWatchedObject()
        {
            watchedObjectIndex = (watchedObjectIndex + 1) % watchedObjectPositions.Length;
        }

        public void Draw(Camera camera, DayTimeScheduler scheduler, bool setVibrations)
        {
            if (camera.Type == CameraType.FixedObject)
            {
                camera.position = watchedObjectPositions[watchedObjectIndex] + new Vector3(0f, 0.5f, -0.5f);
            }

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
            Vector4 plane = new Vector4(0, 1, 0, -Constants.waterHeight);
            terrain.Draw(light, camera, scheduler, plane);
            foreach (IModel model in models)
            {
                model.Draw(light, camera, scheduler, setVibrations, plane);
            }
            light.Draw(camera, scheduler, setVibrations, plane);
            GL.Finish();

            camera.position.Y += distance;
            camera.InvertPitch();
        }

        private void RenderRefractionTexture(Camera camera, DayTimeScheduler scheduler, bool setVibrations)
        {
            buffers.BindRefractionFrameBuffer();

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            Vector4 plane = new Vector4(0, -1, 0, Constants.waterHeight);
            terrain.Draw(light, camera, scheduler, plane);
            foreach (IModel model in models)
            {
                model.Draw(light, camera, scheduler, setVibrations, plane);
            }
            light.Draw(camera, scheduler, setVibrations, plane);
            GL.Finish();
        }

        private void RenderToScreen(Camera camera, DayTimeScheduler scheduler, bool setVibrations)
        {
            buffers.UnbindCurrentFrameBuffer();

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            GL.ClearColor((Color4)scheduler.current);
            terrain.Draw(light, camera, scheduler);
            terrain.DrawRiver(light, camera, scheduler);
            foreach (IModel model in models)
            {
                model.Draw(light, camera, scheduler, setVibrations);
            }
            light.Draw(camera, scheduler, setVibrations);
        }

        public void Unload()
        {
            foreach (int shader in extShaders) GL.DeleteShader(shader);

            foreach (IModel model in models) model.Unload();

            light.Unload();

            buffers.Unload();
        }
    }
}

