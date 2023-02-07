using GrassRendering.Controllers;
using GrassRendering.shaders;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace GrassRendering.Renderers
{
    internal class GrassRenderer
    {
        private Shader shader;
        private int VAO;

        private List<Texture> grassTexts;
        private Texture windText;

        public GrassRenderer(Shader shader, int VAO)
        {
            this.shader = shader;
            this.VAO = VAO;

            windText = new Texture("..\\..\\..\\assets\\textures\\flowmap.png", TextureUnit.Texture0);
            grassTexts = new List<Texture>
            {
                new Texture($"..\\..\\..\\assets\\textures\\grass1_texture.png", TextureUnit.Texture1)
            };
            for (int i = 1; i <= 4; i++)
                grassTexts.Add(new Texture($"..\\..\\..\\assets\\textures\\flower{i}_texture.png", TextureUnit.Texture0 + i + 1));
        }

        public void ConfigureShader(Camera camera, DayTimeScheduler scheduler, Vector4? plane = null)
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
        }
    }
}
