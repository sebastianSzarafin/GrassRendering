using GrassRendering;
using GrassRendering.Controllers;
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

namespace GrassRendering
{
    internal class Terrain
    {
        private const float treshhold = 30.0f;
        private const float space = 0.2f;

        private Shader shader;

        public Vertex[] vertices { get; private set; }
        public int[] indices;
        private Texture texTerrain;

        private int VAO;
        private int VBO;
        private int EBO;

        public Terrain(Shader shader, out Grass grass)
        {
            this.shader = shader;

            texTerrain = new Texture("..\\..\\..\\..\\GrassRendering\\assets\\textures\\terrain.png", TextureUnit.Texture0);

            vertices = ProcessVertices();
            indices = ProcessIndices();

            Setup();

            grass = new Grass(
                vertices,
                new Shader(
                    Shader.GetShader("..\\..\\..\\..\\GrassRendering\\assets\\shaders\\grass\\grassVS.glsl", ShaderType.VertexShader),
                    Shader.GetShader("..\\..\\..\\..\\GrassRendering\\assets\\shaders\\grass\\grassGS.glsl", ShaderType.GeometryShader),
                    Shader.GetShader("..\\..\\..\\..\\GrassRendering\\assets\\shaders\\fog\\fog.glsl", ShaderType.FragmentShader),
                    Shader.GetShader("..\\..\\..\\..\\GrassRendering\\assets\\shaders\\grass\\grassFS.glsl", ShaderType.FragmentShader)),
                VAO);
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

        public void Draw(Camera camera, Vector4 skyColor)
        {
            int texTerrainLocation = GL.GetUniformLocation(shader.Handle, "texTerrain");

            shader.Use();

            GL.Uniform1(texTerrainLocation, 0);
            texTerrain.Use(TextureUnit.Texture0);

            shader.SetVector3("cameraPos", camera.position);
            shader.SetVector4("skyColor", skyColor);
            shader.SetMatrix4("view", camera.GetViewMatrix());
            shader.SetMatrix4("projection", camera.GetProjectionMatrix());

            GL.BindVertexArray(VAO);
            GL.DrawElements(BeginMode.Triangles, vertices.Length, DrawElementsType.UnsignedInt, 0);
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
            for (float x = -treshhold; x < treshhold; x += space)
            {
                for (float z = -treshhold; z < treshhold; z += space)
                {
                    vertices.Add(
                        new Vertex(
                            new Vector3(x, 0, z),
                            Vector3.UnitY,
                            new Vector2(Math.Abs(x) / treshhold, Math.Abs(z) / treshhold),
                            WithdrawTexture()));
                }
            }
            this.vertices = vertices.ToArray();
            return vertices.ToArray();
        }

        private int[] ProcessIndices()
        {
            List<int> indices = new List<int>();
            int colCount = (int)(treshhold * 2 / space), step = colCount / 10, size = colCount / step, x = 0;
            
            for (x = 0; x < size - 1; x++)
            {
                for (int z = 0; z < size; z++)
                {
                    int topLeft = z * step + x * step * colCount;
                    int topRight = z * step + (x + 1) * step * colCount;

                    if (z != 0)
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

            x = size - 1;
            for (int z = 0; z < size; z++)
            {
                int topLeft = z * step + x * step * colCount;
                int topRight = vertices.Length - (colCount - z * step);

                if (z != 0)
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
    }
}