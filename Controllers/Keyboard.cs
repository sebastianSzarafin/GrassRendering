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
        public static bool ProcessInput(KeyboardState input, Camera camera, float time)
        {
            bool shouldClose = false;
            //args.Time = deltaTime
            //Explanation:
            //Graphics applications and games usually keep track of a deltatime variable that stores the time
            //it takes to render the last frame. We then multiply all velocities with this deltaTime value.
            //The result is that when we have a large deltaTime in a frame, meaning that the last frame took
            //longer than average, the velocity for that frame will also be a bit higher to balance it all out.
            //When using this approach it does not matter if you have a very fast or slow pc, the velocity of
            //the camera will be balanced out accordingly so each user will have the same experience.
            //keyboard
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
            if (input.IsKeyDown(Keys.Escape))
            {
                shouldClose = true;
            }

            if (camera.position.Y <= 1) camera.position.Y = 1;

            return shouldClose;
        }
    }
}
