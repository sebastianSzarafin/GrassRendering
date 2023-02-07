using GrassRendering.Controllers;
using GrassRendering.shaders;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace GrassRendering.Objects
{
    internal class Grass
    {
        private Shader shader;

        private Vertex[] vertices;
        private List<Texture> grassTexts;
        private Texture windText;

        private int VAO;
        private int VBO;

        public Grass(Vertex[] vertices, Shader shader)
        {
            this.vertices = vertices;
            this.shader = shader;

            Setup();

            windText = new Texture("..\\..\\..\\assets\\textures\\flowmap.png", TextureUnit.Texture0);
            grassTexts = new List<Texture>
            {
                new Texture($"..\\..\\..\\assets\\textures\\grass1_texture.png", TextureUnit.Texture1)
            };
            for (int i = 1; i <= 4; i++)
                grassTexts.Add(new Texture($"..\\..\\..\\assets\\textures\\flower{i}_texture.png", TextureUnit.Texture0 + i + 1));
        }

        private void Setup()
        {
            VAO = GL.GenVertexArray();
            VBO = GL.GenBuffer();

            GL.BindVertexArray(VAO);

            GL.BindBuffer(BufferTarget.ArrayBuffer, VBO);
            GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * Marshal.SizeOf(typeof(Vertex)), vertices, BufferUsageHint.StaticDraw);

            // aPosition
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, Marshal.SizeOf(typeof(Vertex)), 0);
            GL.EnableVertexAttribArray(0);
            // aTexIndex
            GL.VertexAttribPointer(1, 1, VertexAttribPointerType.Float, false, Marshal.SizeOf(typeof(Vertex)), Marshal.OffsetOf(typeof(Vertex), "aTexIndex"));
            GL.EnableVertexAttribArray(1);

            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
        }

        public void Draw(Camera camera, DayTimeScheduler scheduler, Vector4? plane = null)
        {
            int texWindLocation = GL.GetUniformLocation(shader.Handle, "texWind");
            int[] texGrassLocations = new int[grassTexts.Count];
            for (int i = 0; i < grassTexts.Count; i++)
            {
                texGrassLocations[i] = GL.GetUniformLocation(shader.Handle, $"texGrass{i + 1}");
            }

            shader.Use();

            GL.Uniform1(texWindLocation, 0);
            windText.Use(TextureUnit.Texture0);
            for (int i = 0; i < grassTexts.Count; i++)
            {
                GL.Uniform1(texGrassLocations[i], i + 1);
                grassTexts[i].Use(TextureUnit.Texture0 + i + 1);
            }

            shader.SetFloat("time", (float)scheduler.timer.Elapsed.TotalSeconds);
            shader.SetVector3("cameraPos", camera.position);
            shader.SetVector4("skyColor", scheduler.current);
            shader.SetFloat("fogDensity", scheduler.fogDensity);
            if (plane != null) shader.SetVector4("plane", (Vector4)plane);

            shader.SetMatrix4("view", camera.GetViewMatrix());
            shader.SetMatrix4("projection", camera.GetProjectionMatrix());

            GL.BindVertexArray(VAO);
            GL.DrawArrays(PrimitiveType.Points, 0, vertices.Length);
        }

        public void Unload()
        {
            shader.Dispose();
        }
    }
}
