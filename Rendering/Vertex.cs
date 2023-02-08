using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GrassRendering.Rendering
{
    internal struct Vertex
    {
        public Vector3 aPosition;
        public Vector3 aNormal;
        public Vector2 aTexCoords;
        public float aTexIndex;

        public Vertex(Vector3 position, Vector3 normal = default, Vector2 texCoords = default, float texIndex = 0)
        {
            aPosition = position;
            aNormal = normal;
            aTexCoords = texCoords;
            aTexCoords = texCoords;
            aTexIndex = texIndex;
        }
    }
}
