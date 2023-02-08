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

        public Vertex(Assimp.Vector3D posistion, Assimp.Vector3D normal, Assimp.Vector3D texCoords, float texIndex = 0)
        {
            aPosition = new Vector3(posistion.X, posistion.Y, posistion.Z);
            aNormal = new Vector3(normal.X, normal.Y, normal.Z);
            aTexCoords = new Vector2(texCoords.X, texCoords.Y);
            aTexIndex = texIndex;
        }
    }
}
