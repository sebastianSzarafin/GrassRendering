using GrassRendering.Controllers;
using GrassRendering.shaders;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace GrassRendering.Objects
{
    internal class Water
    {
        private Shader shader;

        private Vertex[] vertices;
        private int[] indices;
        private WaterFrameBuffers buffers;
        //private List<Texture> grassTexts;
        private Texture windText;

        private int VAO;
        private int VBO;
        private int EBO;

        public Water(Vertex[] vertices, Shader shader, Texture windText, WaterFrameBuffers buffers)
        {
            this.shader = shader;
            this.vertices = vertices;
            this.buffers = buffers;
            this.windText = windText;

            /*this.vertices = new Vertex[4];
            this.vertices[0] = new Vertex(new Vector3(4, Constants.waterHeight, -10));
            this.vertices[1] = new Vertex(new Vector3(4, Constants.waterHeight, 10));
            this.vertices[2] = new Vertex(new Vector3(8, Constants.waterHeight, -10));
            this.vertices[3] = new Vertex(new Vector3(8, Constants.waterHeight, 10));*/

            for (int i = 0; i < vertices.Length; i++) vertices[i].aPosition.Y = Constants.waterHeight;

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
            int colCount = (int)(Constants.treshhold * 2 / Constants.space), step = 2, quadsInCol = colCount / step, quadsInRow = (int)(0.4 * quadsInCol / step);
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

            /*List<int> indices = new List<int>()
            {
                0,1,2,2,1,3
            };*/
            return indices.ToArray();
        }

        public void Draw(Camera camera, DayTimeScheduler scheduler, Vector4? plane)
        {
            int texReflectLocation = GL.GetUniformLocation(shader.Handle, "texReflect");
            int texRefractLocation = GL.GetUniformLocation(shader.Handle, "texRefract");
            int texWindLocation = GL.GetUniformLocation(shader.Handle, "texWind");

            shader.Use();

            GL.Uniform1(texReflectLocation, 0);
            buffers.ReflectionTexture.Use(TextureUnit.Texture0);
            GL.Uniform1(texRefractLocation, 1);
            buffers.RefractionTexture.Use(TextureUnit.Texture1);
            GL.Uniform1(texWindLocation, 2);
            windText.Use(TextureUnit.Texture2);

            shader.SetFloat("time", (float)scheduler.timer.Elapsed.TotalSeconds);
            shader.SetVector3("cameraPos", camera.position);
            shader.SetVector4("skyColor", scheduler.current);
            shader.SetFloat("fogDensity", scheduler.fogDensity);
            if (plane != null) shader.SetVector4("plane", (Vector4)plane);

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
