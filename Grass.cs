using GrassRendering.Controllers;
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

namespace GrassRendering
{
    internal class Grass
    {
        private Shader shader;

        private Vertex[] vertices;
        private List<Texture> grassTexts;
        private Texture windText;

        private int VAO;

        public Grass(Vertex[] vertices, Shader shader, int VAO)
        {
            this.vertices = vertices;
            this.shader = shader;
            this.VAO = VAO;

            windText = new Texture("..\\..\\..\\..\\GrassRendering\\assets\\textures\\flowmap.png", TextureUnit.Texture0);
            grassTexts = new List<Texture>
            {
                new Texture($"..\\..\\..\\..\\GrassRendering\\assets\\textures\\grass1_texture.png", TextureUnit.Texture1)
            };
            for (int i = 1; i <= 4; i++)
                grassTexts.Add(new Texture($"..\\..\\..\\..\\GrassRendering\\assets\\textures\\flower{i}_texture.png", TextureUnit.Texture0 + i + 1));
        }

        public void Draw(Camera camera, DayTimeScheduler scheduler)
        {
            int texWindLocation = GL.GetUniformLocation(shader.Handle, "texWind");
            int[] texGrassLocations = new int[grassTexts.Count];
            for(int i = 0; i < grassTexts.Count; i++)
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
            shader.SetFloat("visibility", scheduler.visibility);

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
