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
        public static bool ProcessInput(KeyboardState input, Camera camera, float time, ref bool setVibrations)
        {
            bool shouldClose = false;
            
            if (input.IsKeyDown(Keys.Escape))
            {
                shouldClose = true;
            }

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
            
            if (input.IsKeyPressed(Keys.V))
            {
                setVibrations = !setVibrations;
            }
            
            /*if (input.IsKeyPressed(Keys.C) *//*&& input.IsKeyPressed(Keys.KeyPad1)*//*)
            {
                camera.position = new Vector3(Constants.treshhold, 2, Constants.treshhold);
            }*/

            if (camera.position.Y <= 1) camera.position.Y = 1;

            return shouldClose;
        }
    }
}
