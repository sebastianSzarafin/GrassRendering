using GrassRendering.Controllers;
using GrassRendering.Models.Interfaces;
using GrassRendering.Rendering;
using OpenTK.Graphics.ES11;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GrassRendering.Models
{
    internal class Model : IModel
    {
        private Shader shader;
        private List<IMesh> meshes = new List<IMesh>();
        private string directory = "";

        protected Matrix4 model = Matrix4.Identity;

        public Model(string path, Shader shader)
        {
            this.shader = shader;
            LoadModel(path);
        }

        public virtual void Draw(Light light, Camera camera, DayTimeScheduler scheduler, bool _, Vector4? plane = null)
        {
            ConfigureShader(light, camera, scheduler, plane);
            foreach (Mesh mesh in meshes) mesh.Draw(shader);
        }

        private void ConfigureShader(Light light, Camera camera, DayTimeScheduler scheduler, Vector4? plane = null)
        {
            shader.Use();

            for (int i = 0; i < light.Positions.Length; i++)
            {
                shader.SetVector3($"lightPosition[{i}]", light.Positions[i]);
                shader.SetVector3($"lightColor[{i}]", light.Colors[i]);
                shader.SetVector3($"attenuation[{i}]", light.Attenuations[i]);
            }

            shader.SetFloat("time", (float)scheduler.timer.Elapsed.TotalSeconds);
            shader.SetVector3("cameraPos", camera.position);
            shader.SetVector4("skyColor", scheduler.current);
            shader.SetFloat("fogDensity", scheduler.fogDensity);
            if (plane != null) shader.SetVector4("plane", (Vector4)plane);

            shader.SetMatrix4("model", model);
            shader.SetMatrix4("view", camera.GetViewMatrix());
            shader.SetMatrix4("projection", camera.GetProjectionMatrix());
        }

        public virtual void Draw(Vector3 lightColor, Camera camera, DayTimeScheduler scheduler, bool _, Vector4? plane = null)
        {
            ConfigureLightShader(lightColor, camera, scheduler, plane);
            foreach (Mesh mesh in meshes) mesh.Draw(shader);
        }

        private void ConfigureLightShader(Vector3 lightColor, Camera camera, DayTimeScheduler scheduler, Vector4? plane = null)
        {
            shader.Use();

            shader.SetVector3("color", lightColor);

            shader.SetFloat("time", (float)scheduler.timer.Elapsed.TotalSeconds);
            shader.SetVector3("cameraPos", camera.position);
            shader.SetVector4("skyColor", scheduler.current);
            shader.SetFloat("fogDensity", scheduler.fogDensity);
            if (plane != null) shader.SetVector4("plane", (Vector4)plane);

            shader.SetMatrix4("model", model);
            shader.SetMatrix4("view", camera.GetViewMatrix());
            shader.SetMatrix4("projection", camera.GetProjectionMatrix());
        }

        public void Unload()
        {
            shader.Dispose();
            foreach (Mesh mesh in meshes) mesh.Unload();
        }

        void LoadModel(string path)
        {
            Assimp.AssimpContext context = new Assimp.AssimpContext();
            Assimp.Scene scene = context.ImportFile(path, Assimp.PostProcessSteps.Triangulate);

            if (scene == null || scene.SceneFlags.HasFlag(Assimp.SceneFlags.Incomplete) || scene.RootNode == null)
            {
                Console.WriteLine("Assimp error");
                return;
            }
            directory = path.Substring(0, path.LastIndexOf('\\'));

            string objName = directory.Substring(directory.LastIndexOf('\\') + 1);

            ProcessNode(scene.RootNode, scene, objName);
        }

        void ProcessNode(Assimp.Node node, Assimp.Scene scene, string objName)
        {
            Debug.WriteLine(node.Name);
            for (int i = 0; i < node.MeshCount; i++)
            {
                Assimp.Mesh mesh = scene.Meshes[node.MeshIndices[i]];
                meshes.Add(ProcessMesh(mesh, scene, objName));
            }
            for (int i = 0; i < node.ChildCount; i++)
            {
                ProcessNode(node.Children[i], scene, objName);
            }
        }

        Mesh ProcessMesh(Assimp.Mesh mesh, Assimp.Scene scene, string objName)
        {
            List<Vertex> vertices = new List<Vertex>();
            List<int> indices = new List<int>();
            List<Texture> textures = new List<Texture>();
            // process vertices
            for (int i = 0; i < mesh.VertexCount; i++)
            {
                Vertex vertex;
                if (mesh.HasTextureCoords(0))
                {
                    vertex = new Vertex(mesh.Vertices[i], mesh.Normals[i], mesh.TextureCoordinateChannels[0][i]);
                }
                else
                {
                    vertex = new Vertex(mesh.Vertices[i], mesh.Normals[i], new Assimp.Vector3D(0.0f, 0.0f, 0.0f));
                }
                vertices.Add(vertex);
            }
            // process indices
            for (int i = 0; i < mesh.FaceCount; i++)
            {
                Assimp.Face face = mesh.Faces[i];
                for (int j = 0; j < face.IndexCount; j++)
                {
                    indices.Add(face.Indices[j]);
                }
            }
            // process material
            if (mesh.MaterialIndex >= 0)
            {
                Assimp.Material material = scene.Materials[mesh.MaterialIndex];
                List<Texture> diffuseMaps = LoadMaterialTextures(
                    material,
                    Assimp.TextureType.Diffuse, "texture_diffuse",
                    objName);
                textures.AddRange(diffuseMaps);
                List<Texture> specularMaps = LoadMaterialTextures(
                    material,
                    Assimp.TextureType.Specular, "texture_specular",
                    objName);

                textures.AddRange(specularMaps);
            }

            return new Mesh(vertices.ToArray(), indices.ToArray(), textures);
        }

        List<Texture> LoadMaterialTextures(Assimp.Material mat, Assimp.TextureType texType, string typeName, string objName)
        {
            List<Texture> textures = new List<Texture>();
            for (int i = 0; i < mat.GetMaterialTextureCount(texType); i++)
            {
                Assimp.TextureSlot textureSlot;
                mat.GetMaterialTexture(texType, i, out textureSlot);
                Texture texture = new Texture($"..\\..\\..\\assets\\models\\{objName}\\{textureSlot.FilePath}");
                texture.type = typeName;
                textures.Add(texture);
            }
            return textures;
        }
    }
}
