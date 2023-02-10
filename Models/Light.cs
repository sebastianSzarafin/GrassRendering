using GrassRendering.Controllers;
using GrassRendering.Rendering;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GrassRendering.Models
{
    internal class Light : Model
    {
        private Vector3[] positions;
        private Vector3[] offColors;
        private Vector3[] onColors;
        private Vector3[] attenuations;

        public Vector3[] Positions { get => positions; }
        public Vector3[] Attenuations { get => attenuations; }


        public Light(string path, Shader shader) : base(path, shader)
        {
            positions = new Vector3[] { -Vector3.UnitZ * 2, -Vector3.UnitZ, Vector3.Zero, Vector3.UnitZ, Vector3.UnitZ * 2 };
            for (int i = 0; i < positions.Length; i++)
            {
                positions[i] = Vector3.UnitX * ((Constants.waterStartPos + Constants.waterEndPos) / 2) + (Vector3.UnitY + positions[i]) * 3/*(Constants.treshhold / positions.Length)*/;
            }
            offColors = new Vector3[] { new Vector3(0.66f, 0.66f, 0.66f), new Vector3(0.66f, 0.66f, 0.66f), new Vector3(0.66f, 0.66f, 0.66f), new Vector3(0.66f, 0.66f, 0.66f), new Vector3(0.66f, 0.66f, 0.66f), };
            onColors = new Vector3[] { Vector3.UnitX, new Vector3(1f, 0.73f, 0.45f), Vector3.UnitY, new Vector3(1f, 0.73f, 0.45f), Vector3.UnitZ };
            attenuations = new Vector3[] { new Vector3(1f, 0.1f, 0.02f), new Vector3(1f, 0.1f, 0.02f), new Vector3(1f, 0.1f, 0.02f), new Vector3(1f, 0.1f, 0.02f), new Vector3(1f, 0.1f, 0.02f) };
        }

        public void Draw(Camera camera, DayTimeScheduler scheduler, bool _, Vector4? plane = null)
        {
            for (int i = 0; i < positions.Length; i++)
            {
                model =
                    Matrix4.CreateScale(0.3f) *
                    Matrix4.CreateTranslation(positions[i]);

                base.Draw(GetColors(scheduler.time)[i], camera, scheduler, _, plane);
            }
        }

        public Vector3[] GetColors(DayTime time)
        {
            return time == DayTime.Evening || time == DayTime.Night
                ? onColors
                : offColors;
        }
    }
}
