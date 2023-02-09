using GrassRendering.Controllers;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GrassRendering.Models.Interfaces
{
    interface IModel
    {
        void Draw(Camera camera, DayTimeScheduler scheduler, bool setVibrations, Vector4? plane = null);
        void Unload();
    }
}
