using GrassRendering.Controllers;
using GrassRendering.Models.Interfaces;
using GrassRendering.Rendering;
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

namespace GrassRendering.Models.Terrain
{
    internal class Grass : IMesh
    {
        private Vertex[] vertices;
        private List<Texture> grassTexts;
        private Texture windText;

        private int VAO;
        private int VBO;

        public Grass(Vertex[] vertices, Texture windText)
        {
            this.vertices = vertices;
            this.windText = windText;

            grassTexts = new List<Texture>
            {
                new Texture($"..\\..\\..\\assets\\textures\\grass1_texture.png", TextureUnit.Texture1)
            };
            for (int i = 1; i <= 4; i++)
                grassTexts.Add(new Texture($"..\\..\\..\\assets\\textures\\flower{i}_texture.png", TextureUnit.Texture0 + i + 1));

            Setup();
        }

        #region Preprocessing
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
            // aNormal
            GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, Marshal.SizeOf(typeof(Vertex)), Marshal.OffsetOf(typeof(Vertex), "aNormal"));
            GL.EnableVertexAttribArray(1);
            // aTexIndex
            GL.VertexAttribPointer(2, 1, VertexAttribPointerType.Float, false, Marshal.SizeOf(typeof(Vertex)), Marshal.OffsetOf(typeof(Vertex), "aTexIndex"));
            GL.EnableVertexAttribArray(2);

            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
        }
        #endregion

        public void Draw(Shader shader)
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

            GL.BindVertexArray(VAO);
            GL.DrawArrays(PrimitiveType.Points, 0, vertices.Length);
        }

        public void Unload()
        {
            GL.BindVertexArray(0);
            GL.DeleteVertexArray(VAO); //unbind VAO buffer

            GL.BindBuffer(BufferTarget.ArrayBuffer, 0); //unbind VBO buffer
            GL.DeleteBuffer(VBO);
        }
    }
}
