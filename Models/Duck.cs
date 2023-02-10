using GrassRendering.Controllers;
using GrassRendering.Rendering;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GrassRendering.Models
{
    internal class Duck : Model
    {
        private const int ducksCount = 10;
        private const float speed = 0.0025f;
        private const float offsetY = 0.1f;

        private float vibrationAngle = MathHelper.DegreesToRadians(2.5f);

        private Vector3[] positions;

        private Stopwatch timer;

        public Duck(string path, out Vector3[] positions, Shader shader) : base(path, shader)
        {
            positions = new Vector3[ducksCount];
            for(int i = 0; i < positions.Length; i++) positions[i] = ResetPosition();

            this.positions = positions;

            timer = new Stopwatch();
            timer.Start();
        }

        public override void Draw(Light light, Camera camera, DayTimeScheduler scheduler, bool setVibrations,  Vector4? plane = null)
        {
            for (int i = 0; i < positions.Length; i++)
            {
                if (positions[i].Z + speed < Constants.treshhold) positions[i].Z += speed;
                else positions[i] = ResetPosition();

                positions[i].Y = offsetY + (float)Math.Cos(positions[i].Z * 5) / 20;

                model = Move(positions[i], setVibrations, vibrationAngle);

                base.Draw(light, camera, scheduler, setVibrations, plane);

                if (!ShouldDrawMore(i + 1)) break;
            }

            vibrationAngle *= -1;
        }


        private Vector3 ResetPosition()
        {
            return new Vector3(WithdrawStartPos(), offsetY, -Constants.treshhold);
        }

        private Matrix4 Move(Vector3 position, bool setVibrations, float angle)
        {
            if (!setVibrations) angle = 0;

            return 
                Matrix4.CreateScale(0.5f) *
                Matrix4.CreateRotationX(angle) *
                Matrix4.CreateRotationY(MathHelper.DegreesToRadians(-90f)) *
                Matrix4.CreateTranslation(position);
        }

        private float WithdrawStartPos()
        {
            Random r = new Random();
            return (Constants.waterStartFract + 0.1f + (float)r.NextDouble() / (1f / Constants.waterWidthFract * 2)) * Constants.treshhold;
        }

        private bool ShouldDrawMore(int i)
        {
            if (!timer.IsRunning) return true;

            if (timer.Elapsed.TotalSeconds >= ducksCount * 2)
            {
                timer.Stop();
                return true;
            }

            if (timer.Elapsed.TotalSeconds >= i * 2) return true;
            return false;
        }
    }
}
