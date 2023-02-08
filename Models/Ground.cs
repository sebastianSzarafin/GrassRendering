using GrassRendering.Controllers;
using GrassRendering.Models.Interfaces;
using GrassRendering.Rendering;
using OpenTK.Graphics.OpenGL4;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace GrassRendering.Models
{
    internal class Ground : IMesh
    {
        private Vertex[] vertices;
        public int[] indices;
        private Texture texTerrain;

        private int VAO;
        private int VBO;
        private int EBO;

        public Ground(Vertex[] vertices)
        {
            this.vertices = vertices;

            indices = ProcessIndices();

            texTerrain = new Texture("..\\..\\..\\assets\\textures\\terrain.png", TextureUnit.Texture0);

            Setup();
        }

        #region Preprocessing
        private void Setup()
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
            // aTexIndex
            GL.VertexAttribPointer(3, 1, VertexAttribPointerType.Float, false, Marshal.SizeOf(typeof(Vertex)), Marshal.OffsetOf(typeof(Vertex), "aTexIndex"));
            GL.EnableVertexAttribArray(3);

            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
        }

        private int[] ProcessIndices()
        {
            List<int> indices = new List<int>();
            int colCount = (int)(Constants.treshhold * 2 / Constants.space), step = colCount / 10, quads = colCount / step, x = 0;
            //left side of the river
            for (x = 0; x < (int)(0.7 * quads); x++)
            {
                for (int z = 0; z < quads; z++)
                {
                    int topLeft = z * step + x * step * colCount;
                    int topRight = z * step + (x + 1) * step * colCount;

                    if (z == quads - 1)
                    {
                        topLeft--;
                        topRight--;
                    }

                    int bottomLeft = topLeft + step;
                    int bottomRight = topRight + step;

                    indices.Add(topLeft);
                    indices.Add(bottomLeft);
                    indices.Add(topRight);
                    indices.Add(topRight);
                    indices.Add(bottomLeft);
                    indices.Add(bottomRight);
                }
            }
            //river
            step = 2; quads = colCount / step;
            for (x = (int)(0.7 * quads); x < (int)(0.9 * quads); x++)
            {
                for (int z = 0; z < quads; z++)
                {
                    int topLeft = z * step + x * step * colCount;
                    int topRight = z * step + (x + 1) * step * colCount;

                    if (z == quads - 1)
                    {
                        topLeft--;
                        topRight--;
                    }

                    int bottomLeft = topLeft + step;
                    int bottomRight = topRight + step;

                    indices.Add(topLeft);
                    indices.Add(bottomLeft);
                    indices.Add(topRight);
                    indices.Add(topRight);
                    indices.Add(bottomLeft);
                    indices.Add(bottomRight);
                }
            }
            //right side of the river
            step = colCount / 10; quads = colCount / step; x = (int)(0.9 * quads);
            for (int z = 0; z < quads; z++)
            {
                int topLeft = z * step + x * step * colCount;
                int topRight = vertices.Length - (colCount - z * step);

                if (z == quads - 1)
                {
                    topLeft--;
                    topRight--;
                }

                int bottomLeft = topLeft + step;
                int bottomRight = topRight + step;

                indices.Add(topLeft);
                indices.Add(bottomLeft);
                indices.Add(topRight);
                indices.Add(topRight);
                indices.Add(bottomLeft);
                indices.Add(bottomRight);
            }

            return indices.ToArray();
        }
        #endregion

        public void Draw(Shader shader)
        {
            int texTerrainLocation = GL.GetUniformLocation(shader.Handle, "texTerrain");

            shader.Use();

            GL.Uniform1(texTerrainLocation, 0);
            texTerrain.Use(TextureUnit.Texture0);

            GL.BindVertexArray(VAO);
            GL.DrawElements(BeginMode.Triangles, indices.Length, DrawElementsType.UnsignedInt, 0);
        }

        public void Unload()
        {
            GL.BindVertexArray(0);
            GL.DeleteVertexArray(VAO); //unbind VAO buffer

            GL.BindBuffer(BufferTarget.ArrayBuffer, 0); //unbind VBO buffer
            GL.DeleteBuffer(VBO);

            GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0); //unbind EBO buffer
            GL.DeleteBuffer(EBO);
        }
    }
}
