using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GrassRendering.Controllers
{
    internal class Mouse
    {
        public bool firstMove { get; set; }
        public Vector2 lastPos { get; set; }

        public Mouse()
        {
            firstMove = true;
            lastPos = Vector2.Zero;
        }
    }
}
