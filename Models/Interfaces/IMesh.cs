using GrassRendering.Controllers;
using GrassRendering.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GrassRendering.Models.Interfaces
{
    internal interface IMesh
    {
        void Draw(Shader shader);
        void Unload();
    }
}
