using GrassRendering.Controllers;
using GrassRendering.Objects;
using GrassRendering.shaders;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GrassRendering.Renderers
{
    internal class MainRenderer
    {
        private Terrain terrain;

        private WaterFrameBuffers buffers;

        public MainRenderer()
        {
            buffers = new WaterFrameBuffers();

            terrain = new Terrain(
                new Shader(
                    Shader.GetShader("..\\..\\..\\shaders\\terrain\\terrainVS.glsl", ShaderType.VertexShader),
                    Shader.GetShader("..\\..\\..\\shaders\\fog\\fog.glsl", ShaderType.FragmentShader),
                    Shader.GetShader("..\\..\\..\\shaders\\terrain\\terrainFS.glsl", ShaderType.FragmentShader)),
                buffers);
        }

        public void Draw(Camera camera, DayTimeScheduler scheduler)
        {
            GL.Enable(EnableCap.ClipDistance0);

            RenderReflectionTexture(camera, scheduler);

            RenderRefractionTexture(camera, scheduler);

            GL.Disable(EnableCap.ClipDistance0);

            RenderToScreen(camera, scheduler);
        }

        private void RenderReflectionTexture(Camera camera, DayTimeScheduler scheduler)
        {
            buffers.BindReflectionFrameBuffer();

            float distance = 2 * (camera.position.Y - Constants.waterHeight);
            camera.position.Y -= distance;
            camera.InvertPitch();

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            terrain.Draw(camera, scheduler, new Vector4(0, 1, 0, -(Constants.waterHeight + 0.001f)));
            GL.Finish();

            camera.position.Y += distance;
            camera.InvertPitch();
        }

        private void RenderRefractionTexture(Camera camera, DayTimeScheduler scheduler)
        {
            buffers.BindRefractionFrameBuffer();

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            terrain.Draw(camera, scheduler, new Vector4(0, -1, 0, Constants.waterHeight));
            GL.Finish();
        }

        private void RenderToScreen(Camera camera, DayTimeScheduler scheduler)
        {
            buffers.UnbindCurrentFrameBuffer();

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            GL.ClearColor((Color4)scheduler.current);
            terrain.Draw(camera, scheduler);
        }

        public void Unload()
        {
            terrain.Unload();

            buffers.Unload();
        }
    }
}
