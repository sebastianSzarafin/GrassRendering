using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GrassRendering
{
    public static class Constants
    {
        public const float treshhold = 10.0f;
        public const float space = 0.2f;

        public const float waterHeight = -0.05f;
        public const float waterStartFract = 0.4f;
        public const float waterEndFract =  0.8f;
        public const float waterWidthFract = waterEndFract - waterStartFract;
        public const float waterStartPos = waterStartFract * treshhold;
        public const float waterEndPos = waterEndFract * treshhold;

    }
}
