using GrassRendering.Controllers;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GrassRendering.Models
{
    internal class Sun
    {
        public float ambient = 1f;
        public Vector3 color = Vector3.One;
        public Vector3 direction = new Vector3(-0.2f, -1.0f, -0.3f);

        public void UpdateStrength(double dayPart)
        {
            dayPart += DayTimeScheduler.day / DayTimeScheduler.dayTimes;
            if (dayPart > DayTimeScheduler.day) dayPart = 0;
            dayPart /= DayTimeScheduler.day;
            dayPart -= 0.5;
            dayPart *= 2 * 90;

            ambient = (float)Math.Cos(MathHelper.DegreesToRadians(dayPart));
            ambient = (float)Math.Min(Math.Max(ambient, 0.2), 0.8);
        }
    }
}
