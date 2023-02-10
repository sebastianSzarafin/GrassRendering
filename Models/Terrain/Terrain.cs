using GrassRendering.Controllers;
using GrassRendering.Models.Interfaces;
using GrassRendering.Rendering;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace GrassRendering.Models.Terrain
{
    internal class Terrain
    {
        private List<Shader> shaders;
        private Shader waterShader;

        private List<IMesh> meshes;
        private Water water;

        public Terrain(int[] extShaders, WaterFrameBuffers buffers)
        {
            shaders = new List<Shader>()
            {
                //Ground shader
                new Shader(
                    extShaders,
                    Shader.GetShader("..\\..\\..\\shaders\\ground\\groundVS.glsl", ShaderType.VertexShader),
                    Shader.GetShader("..\\..\\..\\shaders\\ground\\groundFS.glsl", ShaderType.FragmentShader)),
                //Grass shader
                new Shader(
                    extShaders,
                    Shader.GetShader("..\\..\\..\\shaders\\grass\\grassVS.glsl", ShaderType.VertexShader),
                    Shader.GetShader("..\\..\\..\\shaders\\grass\\grassGS.glsl", ShaderType.GeometryShader),
                    Shader.GetShader("..\\..\\..\\shaders\\grass\\grassFS.glsl", ShaderType.FragmentShader)),
            };
            waterShader =
                new Shader(
                    extShaders,
                    Shader.GetShader("..\\..\\..\\shaders\\water\\waterVS.glsl", ShaderType.VertexShader),
                    Shader.GetShader("..\\..\\..\\shaders\\water\\waterFS.glsl", ShaderType.FragmentShader));

            List<Vertex> vertices = ProcessVertices();

            Texture windText = new Texture("..\\..\\..\\assets\\textures\\flowmap.png");

            meshes = new List<IMesh>()
            {
                new Ground(vertices.ToArray()),
                new Grass(
                    vertices.Where(v => v.aPosition.Y >= 0).ToArray(),
                    windText),
            };
            water = new Water(
                    vertices.Where(v => v.aPosition.Y < 0).Select(v => { v.aPosition.Y = Constants.waterHeight; return v; }).ToArray(),
                    windText,
                    buffers);
        }

        #region Preprocessing
        private List<Vertex> ProcessVertices()
        {
            List<Vertex> vertices = new List<Vertex>();
            for (float x = -Constants.treshhold; x < Constants.treshhold; x = (float)Math.Round(x + Constants.space, 2))
            {
                float y = getHeight(x, Constants.waterStartPos, Constants.waterEndPos);
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
            return vertices;

            float getHeight(float x, float begin, float end)
            {
                if (x < begin || x > end) return 0.0f;

                float a = (float)(-Math.PI / 2), b = -a;
                float height = (float)-Math.Cos((b - a) * (x - begin) / (end - begin) + a);
                return Math.Min(height, 0);
            }
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
        #endregion

        public void Draw(Light light, Camera camera, DayTimeScheduler scheduler, Vector4? plane = null)
        {
            for (int i = 0; i < meshes.Count; i++)
            {
                ConfigureShader(shaders[i], light, camera, scheduler, plane);
                meshes[i].Draw(shaders[i]);
            }
        }

        public void DrawRiver(Light light, Camera camera, DayTimeScheduler scheduler, Vector4? plane = null)
        {
            ConfigureShader(waterShader, light, camera, scheduler, plane);
            water.Draw(waterShader);
        }

        private void ConfigureShader(Shader shader, Light light, Camera camera, DayTimeScheduler scheduler, Vector4? plane = null)
        {
            shader.Use();

            shader.SetFloat("sun.ambient", scheduler.Sun.ambient);
            shader.SetVector3("sun.color", scheduler.Sun.color);
            shader.SetVector3("sun.direction", scheduler.Sun.direction);

            for (int i = 0; i < light.Positions.Length; i++)
            {
                shader.SetVector3($"lightPosition[{i}]", light.Positions[i]);
                shader.SetVector3($"lightColor[{i}]", light.GetColors(scheduler.time)[i]);
                shader.SetVector3($"attenuation[{i}]", light.Attenuations[i]);
            }

            shader.SetFloat("dayTime", (float)scheduler.time);
            shader.SetFloat("time", (float)scheduler.timer.Elapsed.TotalSeconds);
            shader.SetVector3("cameraPos", camera.position);
            shader.SetVector4("skyColor", scheduler.current);
            shader.SetFloat("fogDensity", scheduler.fogDensity);
            if (plane != null) shader.SetVector4("plane", (Vector4)plane);

            shader.SetMatrix4("view", camera.GetViewMatrix());
            shader.SetMatrix4("projection", camera.GetProjectionMatrix());
        }

        public void Unload()
        {
            for (int i = 0; i < shaders.Count; i++) shaders[i].Dispose();
            for (int i = 0; i < meshes.Count; i++) meshes[i].Unload();
        }
    }
}