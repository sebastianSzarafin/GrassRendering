using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace GrassRendering.Controllers
{
    internal class Camera
    {
        #region Properties

        public const float cameraSpeed = 2.5f;
        public const float sensitivity = 0.2f;

        public Vector3 position;
        public Vector3 up;
        public Vector3 front;
        public float aspectRatio { get; set; }
        private float fov { get; set; }
        public Vector3 right;

        private float pitch { get; set; }
        private float yaw { get; set; }

        private Mouse mouse;
        #endregion

        public Camera(Vector3 position, float aspectRatio)
        {
            this.position = position;
            up = Vector3.UnitY;
            front = -Vector3.UnitZ;
            this.aspectRatio = aspectRatio;
            right = Vector3.Normalize(Vector3.Cross(front, up));

            fov = MathHelper.PiOver4;
            yaw = -MathHelper.PiOver2; // To avoid the case where camera starts rotating 90 degrees right 

            mouse = new Mouse();
        }

        public Matrix4 GetViewMatrix()
        {
            return Matrix4.LookAt(position, position + front, up);
        }

        public Matrix4 GetProjectionMatrix()
        {
            return Matrix4.CreatePerspectiveFieldOfView(fov, aspectRatio, 0.1f, 100.0f);
        }

        #region Camera Movements
        public float Pitch
        {
            get => MathHelper.RadiansToDegrees(pitch);
            set
            {
                var angle = MathHelper.Clamp(value, -89f, 89f);
                pitch = MathHelper.DegreesToRadians(angle);
                UpdateVectors();
            }
        }

        public float Yaw
        {
            get => MathHelper.RadiansToDegrees(yaw);
            set
            {
                yaw = MathHelper.DegreesToRadians(value);
                UpdateVectors();
            }
        }

        public float Fov
        {
            get => MathHelper.RadiansToDegrees(fov);
            set
            {
                var angle = MathHelper.Clamp(value, 1f, 90f);
                fov = MathHelper.DegreesToRadians(angle);
            }
        }

        private void UpdateVectors()
        {
            // Updating Front
            front.X = MathF.Cos(pitch) * MathF.Cos(yaw);
            front.Y = MathF.Sin(pitch);
            front.Z = MathF.Cos(pitch) * MathF.Sin(yaw);

            // It is important to make sure the vectors are all normalised!!!
            front = Vector3.Normalize(front);

            // Recalculate rigth and up vectors
            right = Vector3.Normalize(Vector3.Cross(front, Vector3.UnitY));
            up = Vector3.Normalize(Vector3.Cross(right, front));
        }
        #endregion

        #region Mouse Input
        public void UpdatePos(MouseState mouseState)
        {
            if (mouse.firstMove)
            {
                mouse.lastPos = new Vector2(mouseState.X, mouseState.Y);
                mouse.firstMove = false;
            }
            else
            {
                var deltaX = mouseState.X - mouse.lastPos.X;
                var deltaY = mouseState.Y - mouse.lastPos.Y;
                mouse.lastPos = new Vector2(mouseState.X, mouseState.Y);

                Yaw += deltaX * sensitivity;
                Pitch -= deltaY * sensitivity;
            }
        }
        #endregion
    }
}
