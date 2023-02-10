using GrassRendering.Rendering;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GrassRendering.Controllers
{
    internal static class Keyboard
    {
        public static bool ProcessInput(KeyboardState input, Camera camera, MainRenderer renderer, float time, ref bool setVibrations)
        {
            if (input.IsKeyDown(Keys.Escape)) return true;

            if (input.IsKeyPressed(Keys.V))
            {
                setVibrations = !setVibrations;
            }       
            
            if (input.IsKeyDown(Keys.C) && input.IsKeyDown(Keys.Up))
            {
                camera.Type = CameraType.FixedScene;
            }
            if (input.IsKeyDown(Keys.C) && input.IsKeyDown(Keys.Down))
            {
                camera.Type = CameraType.FPP;
            }
            if (input.IsKeyDown(Keys.C) && input.IsKeyDown(Keys.Right))
            {
                camera.Type = CameraType.FixedObject;
                renderer.UpdateWatchedObject();
                Thread.Sleep(100);
            }

            if (camera.Type != CameraType.FPP) return false;

            if (input.IsKeyDown(Keys.W))
            {
                camera.position += camera.front * Camera.cameraSpeed * time; // Forward
            }
            if (input.IsKeyDown(Keys.S))
            {
                camera.position -= camera.front * Camera.cameraSpeed * time; // Backwards
            }
            if (input.IsKeyDown(Keys.A))
            {
                camera.position -= camera.right * Camera.cameraSpeed * time; // Left
            }
            if (input.IsKeyDown(Keys.D))
            {
                camera.position += camera.right * Camera.cameraSpeed * time; // Right
            }
            if (input.IsKeyDown(Keys.Space))
            {
                camera.position += camera.up * Camera.cameraSpeed * time; // Up
            }
            if (input.IsKeyDown(Keys.LeftShift))
            {
                camera.position -= camera.up * Camera.cameraSpeed * time; // Down
            }            

            if (camera.position.Y <= 1) camera.position.Y = 1;

            return false;
        }
    }
}
