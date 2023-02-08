using GrassRendering.Controllers;
using GrassRendering.Models.Interfaces;
using GrassRendering.Rendering;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace GrassRendering.Models
{
    internal class Water : IMesh
    {
        private Vertex[] vertices;
        private int[] indices;
        private Texture windText;

        private WaterFrameBuffers buffers;

        private int VAO;
        private int VBO;
        private int EBO;

        public Water(Vertex[] vertices, Texture windText, WaterFrameBuffers buffers)
        {
            this.vertices = vertices;
            this.windText = windText;
            this.buffers = buffers;

            /*this.vertices = new Vertex[4];
            this.vertices[0] = new Vertex(new Vector3(4, Constants.waterHeight, -10), default, new Vector2(0,0));
            this.vertices[1] = new Vertex(new Vector3(4, Constants.waterHeight, 10), default, new Vector2(0, 1));
            this.vertices[2] = new Vertex(new Vector3(8, Constants.waterHeight, -10), default, new Vector2(1, 0));
            this.vertices[3] = new Vertex(new Vector3(8, Constants.waterHeight, 10), default, new Vector2(1, 1));*/

            //for (int i = 0; i < vertices.Length; i++) vertices[i].aPosition.Y = Constants.waterHeight;

            indices = ProcessIndices();

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

            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
        }

        private int[] ProcessIndices()
        {
            List<int> indices = new List<int>();

            float riverWidthFract = (Constants.waterEnd - Constants.waterStart) / Constants.treshhold;

            int colCount = (int)(Constants.treshhold * 2 / Constants.space);
            int step = 2;
            int quadsInCol = colCount / step, quadsInRow = (int)(riverWidthFract * quadsInCol / step);

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
        #endregion

        public void Draw(Shader shader)
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
