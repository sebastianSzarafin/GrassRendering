using GrassRendering.Controllers;
using GrassRendering.shaders;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace GrassRendering.Objects
{
    internal class Terrain
    {
        private Shader shader;

        private Vertex[] vertices;
        public int[] indices;
        private Texture texTerrain;

        private Grass grass;
        private Water water;

        private int VAO;
        private int VBO;
        private int EBO;

        public Terrain(Shader shader, WaterFrameBuffers buffers)
        {
            this.shader = shader;

            texTerrain = new Texture("..\\..\\..\\assets\\textures\\terrain.png", TextureUnit.Texture0);

            vertices = ProcessVertices();
            indices = ProcessIndices();

            Setup();

            Texture windText = new Texture("..\\..\\..\\assets\\textures\\flowmap.png");

            grass = new Grass(
                vertices.Where(v => v.aPosition.Y >= 0).ToArray(),
                new Shader(
                    Shader.GetShader("..\\..\\..\\shaders\\grass\\grassVS.glsl", ShaderType.VertexShader),
                    Shader.GetShader("..\\..\\..\\shaders\\grass\\grassGS.glsl", ShaderType.GeometryShader),
                    Shader.GetShader("..\\..\\..\\shaders\\fog\\fog.glsl", ShaderType.FragmentShader),
                    Shader.GetShader("..\\..\\..\\shaders\\grass\\grassFS.glsl", ShaderType.FragmentShader)),
                windText);

            water = new Water(
                vertices.Where(v => v.aPosition.Y < 0).ToArray(),
                new Shader(
                    Shader.GetShader("..\\..\\..\\shaders\\water\\waterVS.glsl", ShaderType.VertexShader),
                    Shader.GetShader("..\\..\\..\\shaders\\fog\\fog.glsl", ShaderType.FragmentShader),
                    Shader.GetShader("..\\..\\..\\shaders\\water\\waterFS.glsl", ShaderType.FragmentShader)),
                windText,
                buffers);
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
        private int WithdrawTexture()
        {
            int range = 32; // 28 for grass and 1 per each flower

            Random r = new Random();
            int d = r.Next(1, range + 1);

            switch (d)
            {
                case <= 28:
                    return 1;
                case 29:
                    return 2;
                case 30:
                    return 3;
                case 31:
                    return 4;
                default:
                    return 5;
            }
        }

        private Vertex[] ProcessVertices()
        {
            List<Vertex> vertices = new List<Vertex>();
            for (float x = -Constants.treshhold; x < Constants.treshhold; x = (float)Math.Round(x + Constants.space, 2))
            {
                float y = getHeight(x, 4f / 10f * Constants.treshhold, 8f / 10f * Constants.treshhold);
                for (float z = -Constants.treshhold; z < Constants.treshhold; z = (float)Math.Round(z + Constants.space, 2))
                {
                    vertices.Add(
                        new Vertex(
                            new Vector3(x, y, z),
                            Vector3.UnitY,
                            new Vector2(Math.Abs(x) / Constants.treshhold, Math.Abs(z) / Constants.treshhold),
                            WithdrawTexture()));
                }
            }
            this.vertices = vertices.ToArray();
            return vertices.ToArray();

            float getHeight(float x, float begin, float end)
            {
                if (x < begin || x > end) return 0.0f;

                float a = (float)(-Math.PI / 2), b = -a;
                float height = (float)-Math.Cos((b - a) * (x - begin) / (end - begin) + a) * Constants.treshhold / 10.0f;
                return Math.Min(height, 0);
            }
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
            int lastGrass = indices.Count;
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

        public void Draw(Camera camera, DayTimeScheduler scheduler, Vector4? plane = null)
        {
            int texTerrainLocation = GL.GetUniformLocation(shader.Handle, "texTerrain");

            shader.Use();

            GL.Uniform1(texTerrainLocation, 0);
            texTerrain.Use(TextureUnit.Texture0);

            shader.SetVector3("cameraPos", camera.position);
            shader.SetVector4("skyColor", scheduler.current);
            shader.SetFloat("fogDensity", scheduler.fogDensity);
            shader.SetMatrix4("view", camera.GetViewMatrix());
            shader.SetMatrix4("projection", camera.GetProjectionMatrix());
            if (plane != null) shader.SetVector4("plane", (Vector4)plane);

            GL.BindVertexArray(VAO);
            GL.DrawElements(BeginMode.Triangles, indices.Length, DrawElementsType.UnsignedInt, 0);

            grass.Draw(camera, scheduler, plane);
            water.Draw(camera, scheduler, plane);
        }

        public void Unload()
        {
            GL.BindVertexArray(0);
            GL.DeleteVertexArray(VAO); //unbind VAO buffer

            GL.BindBuffer(BufferTarget.ArrayBuffer, 0); //unbind VBO buffer
            GL.DeleteBuffer(VBO);

            GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0); //unbind EBO buffer
            GL.DeleteBuffer(EBO);

            shader.Dispose();

            grass.Unload();
            water.Unload();
        }
    }
}