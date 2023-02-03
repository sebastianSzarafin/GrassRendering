using OpenTK.Graphics.ES20;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GrassRendering
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
        private const float day = 60; 
        private const int dayTimes = 4;

        DayTime time;
        Vector4[] colors;
        public Vector4 current 
        {
            get
            {
                /*Vector4 c1 = colors[(int)time];
                Vector4 c2 = colors[(int)(time + 1) % dayTimes]; 
                return c1 * (1 - mix) + c2 * mix;*/
                return colors[0];
            }
        }
        public float mix;
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
            double totalSeconds = timer.Elapsed.TotalSeconds;

            switch(totalSeconds)
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

            if (totalSeconds >= 60) timer.Restart();

            mix = (float)(totalSeconds % (period) / period);
        }
    }
}
