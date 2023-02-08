using GrassRendering.Models.Interfaces;
using GrassRendering.Rendering;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace GrassRendering.Models
{
    internal class Mesh :IMesh
    {
        public Vertex[] vertices;
        public int[] indices;
        public List<Texture> textures;

        private int VAO;
        private int VBO;
        private int EBO;

        public Mesh(Vertex[] _vertices, int[] _indices, List<Texture> _textures)
        {
            vertices = _vertices;
            indices = _indices;
            textures = _textures;

            Vector3 first = vertices[0].aPosition;
            float minX = first.X, maxX = first.X, minY = first.Y, maxY = first.Y, minZ = first.Z, maxZ = first.Z;

            foreach (Vertex v in vertices)
            {
                if (v.aPosition.X < minX) minX = v.aPosition.X;
                if (v.aPosition.X > maxX) maxX = v.aPosition.X;
                if (v.aPosition.Y < minY) minY = v.aPosition.Y;
                if (v.aPosition.Y > maxY) maxY = v.aPosition.Y;
                if (v.aPosition.Z < minZ) minZ = v.aPosition.Z;
                if (v.aPosition.Z > maxZ) maxZ = v.aPosition.Z;
            }

            Vector3 center = new Vector3((maxX + minX) / 2.0f, (maxY + minY) / 2.0f, (maxZ + minZ) / 2.0f);

            for (int i = 0; i < vertices.Length; i++)
            {
                vertices[i].aPosition -= center;

                vertices[i].aPosition.X /= Math.Abs(maxX - minX);
                vertices[i].aPosition.Y /= Math.Abs(maxY - minY);
                vertices[i].aPosition.Z /= Math.Abs(maxZ - minZ);
            }

            Setup();
        }

        void Setup()
        {
            VAO = GL.GenVertexArray();
            VBO = GL.GenBuffer();
            EBO = GL.GenBuffer();

            GL.BindVertexArray(VAO);

            GL.BindBuffer(BufferTarget.ArrayBuffer, VBO);
            GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * Marshal.SizeOf(typeof(Vertex)), vertices, BufferUsageHint.StaticDraw);

            GL.BindBuffer(BufferTarget.ElementArrayBuffer, EBO);
            GL.BufferData(BufferTarget.ElementArrayBuffer, indices.Length * sizeof(int), indices, BufferUsageHint.StaticDraw);

            // aPosition
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, Marshal.SizeOf(typeof(Vertex)), 0);
            GL.EnableVertexAttribArray(0);
            // aNormal
            GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, Marshal.SizeOf(typeof(Vertex)), Marshal.OffsetOf(typeof(Vertex), "aNormal"));
            GL.EnableVertexAttribArray(1);
            // aTexCoords
            GL.VertexAttribPointer(2, 2, VertexAttribPointerType.Float, false, Marshal.SizeOf(typeof(Vertex)), Marshal.OffsetOf(typeof(Vertex), "aTexCoords"));
            GL.EnableVertexAttribArray(2);


            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
        }

        public void Draw(Shader shader)
        {
            int diffuseNr = 1;
            int specularNr = 1;

            int[] texLocations = new int[textures.Count];
            for (int i = 0; i < textures.Count; i++)
            {
                string number = "";
                string name = textures[i].type;
                if (name.Equals("texture_diffuse"))
                {
                    number = $"{diffuseNr++}";
                }
                else if (name.Equals("texture_specular"))
                {
                    number = $"{specularNr++}";
                }

                texLocations[i] = GL.GetUniformLocation(shader.Handle, $"material.{name}{number}");
            }

            shader.Use();

            for (int i = 0; i < textures.Count; i++)
            {
                GL.Uniform1(texLocations[i], i);
                textures[i].Use(TextureUnit.Texture0 + i);
            }

            // draw mesh
            GL.BindVertexArray(VAO);
            GL.DrawElements(BeginMode.Triangles, indices.Length, DrawElementsType.UnsignedInt, 0);
            //GL.BindVertexArray(0);
        }

        public void Unload()
        {
            GL.BindVertexArray(0);
            GL.DeleteVertexArray(VAO);

            GL.BindBuffer(BufferTarget.ArrayBuffer, 0); //unbind VBO buffer
            GL.DeleteBuffer(VBO);

            GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0); //unbind EBO buffer
            GL.DeleteBuffer(EBO);
        }
    }
}
