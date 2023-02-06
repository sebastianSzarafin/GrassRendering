using GrassRendering.Controllers;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace GrassRendering
{
    internal class Water
    {
        private const float treshhold = 10.0f;
        private const float space = 0.2f;

        private Shader shader;

        private Vertex[] vertices;
        public int[] indices;
        //private List<Texture> grassTexts;
        //private Texture windText;

        private int VAO;
        private int VBO;
        private int EBO;

        public Water(Vertex[] vertices, Shader shader)
        {
            this.shader = shader;
            this.vertices = vertices;

            for (int i = 0; i < vertices.Length; i++) vertices[i].aPosition.Y = -0.15f;

            indices = ProcessIndices();

            Setup();
        }

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
            int colCount = (int)(treshhold * 2 / space), step = 2, quadsInCol = colCount / step, quadsInRow = (int)(0.4 * quadsInCol / step);
            for (int x = 0; x < quadsInRow - 1; x++)
            {
                for (int z = 0; z < quadsInCol; z++)
                {
                    int topLeft = z * step + x * step * colCount;
                    int topRight = z * step + (x + 1) * step * colCount;

                    if (z == quadsInCol - 1)
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

            return indices.ToArray();
        }

        public void Draw(Camera camera, DayTimeScheduler scheduler)
        {
            shader.Use();

            shader.SetFloat("time", (float)scheduler.timer.Elapsed.TotalSeconds);
            shader.SetVector3("cameraPos", camera.position);
            shader.SetVector4("skyColor", scheduler.current);
            shader.SetFloat("fogDensity", scheduler.fogDensity);

            shader.SetMatrix4("view", camera.GetViewMatrix());
            shader.SetMatrix4("projection", camera.GetProjectionMatrix());

            GL.BindVertexArray(VAO);
            GL.DrawElements(BeginMode.Triangles, indices.Length, DrawElementsType.UnsignedInt, 0);
        }

        public void Unload()
        {
            shader.Dispose();
        }
    }
}
