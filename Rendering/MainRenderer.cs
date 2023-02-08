using GrassRendering.Controllers;
using GrassRendering.Models;
using GrassRendering.Models.Interfaces;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GrassRendering.Rendering
{
    internal class MainRenderer
    {
        private List<IModel> models;

        private WaterFrameBuffers buffers;

        public MainRenderer()
        {
            buffers = new WaterFrameBuffers();

            models = new List<IModel>()
            {
                new Terrain(buffers),
            };
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
            foreach (IModel model in models)
            {
                model.Draw(camera, scheduler, new Vector4(0, 1, 0, -(Constants.waterHeight + 0.001f)));
            }
            GL.Finish();

            camera.position.Y += distance;
            camera.InvertPitch();
        }

        private void RenderRefractionTexture(Camera camera, DayTimeScheduler scheduler)
        {
            buffers.BindRefractionFrameBuffer();

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            foreach (IModel model in models)
            {
                model.Draw(camera, scheduler, new Vector4(0, -1, 0, Constants.waterHeight));
            }
            GL.Finish();
        }

        private void RenderToScreen(Camera camera, DayTimeScheduler scheduler)
        {
            buffers.UnbindCurrentFrameBuffer();

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            GL.ClearColor((Color4)scheduler.current);
            foreach (IModel model in models)
            {
                model.Draw(camera, scheduler);
            }
        }

        public void Unload()
        {
            foreach (IModel model in models) model.Unload();

            buffers.Unload();
        }
    }
}

