using OpenTK.Graphics.ES20;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GrassRendering.Controllers
{
    public enum DayTime
    {
        Morning,
        Afternoon,
        Evening,
        Night
    }
    internal class DayTimeScheduler
    {
        public const float day = 60;
        public const int dayTimes = 4;

        private Vector4[] colors;

        public float mix;
        public DayTime time;
        public Vector4 current
        {
            get
            {
                /*Vector4 c1 = colors[(int)time];
                Vector4 c2 = colors[(int)(time + 1) % dayTimes];
                return c1 * (1 - mix) + c2 * mix;*/

                //Fog only
                return colors[0];
            }
        }
        public float fogDensity
        {
            get
            {
                /*if (time == DayTime.Night && mix >= 0.5) return (mix - 0.5f) / 5;
                else if (time == DayTime.Morning && mix <= 0.5) return (0.5f - mix) / 5;
                return 0.0f;*/

                /*//Fog only
                return 0.1f;*/

                //No fog
                return 0.0f;
            }
        }
        public Stopwatch timer;

        public DayTimeScheduler(DayTime time)
        {
            this.time = time;

            colors = new Vector4[dayTimes];
            colors[0] = new Vector4(0.94f, 0.97f, 0.96f, 1.0f); //Morning
            colors[1] = new Vector4(0.52f, 0.81f, 0.92f, 1.0f); //Afternoon
            colors[2] = new Vector4(0.99f, 0.38f, 0.32f, 1.0f); //Evening
            colors[3] = new Vector4(0.05f, 0.08f, 0.27f, 1.0f); //Night

            timer = new Stopwatch();
        }

        public void UpdateTime()
        {
            const double period = day / dayTimes;
            double dayPart = timer.Elapsed.TotalSeconds % day;

            switch (dayPart)
            {
                case <= period:
                    time = DayTime.Morning;
                    break;
                case <= period * 2:
                    time = DayTime.Afternoon;
                    break;
                case <= period * 3:
                    time = DayTime.Evening;
                    break;
                default:
                    time = DayTime.Night;
                    break;
            }

            mix = (float)(dayPart % period / period);
        }
    }
}
