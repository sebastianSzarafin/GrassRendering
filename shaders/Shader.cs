using OpenTK.Compute.OpenCL;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GrassRendering.shaders
{
    public class Shader : IDisposable
    {
        public int Handle { get; private set; } //location of the final shader program
        private bool disposedValue = false;

        #region Constructors
        public Shader(params int[] shaders)
        {
            Handle = GL.CreateProgram();

            foreach (int shader in shaders) GL.AttachShader(Handle, shader);
            GL.LinkProgram(Handle);
            {
                GL.GetProgram(Handle, GetProgramParameterName.LinkStatus, out int success);
                if (success == 0)
                {
                    string infoLog = GL.GetProgramInfoLog(Handle);
                    Console.WriteLine(infoLog);
                }
            }

            //cleanup
            foreach (int shader in shaders)
            {
                GL.DetachShader(Handle, shader);
                GL.DeleteShader(shader);
            };
        }

        public static int GetShader(string path, ShaderType type)
        {
            int handle;

            //load source code from file
            string source = File.ReadAllText(path);
            //generate + bind code 
            handle = GL.CreateShader(type);
            GL.ShaderSource(handle, source);
            //compile and check for errors
            GL.CompileShader(handle);
            {
                GL.GetShader(handle, ShaderParameter.CompileStatus, out int success);
                if (success == 0)
                {
                    string infoLog = GL.GetShaderInfoLog(handle);
                    Console.WriteLine(infoLog);
                }
            }

            return handle;
        }
        #endregion

        public void Use()
        {
            GL.UseProgram(Handle);
        }

        public void SetFloat(string name, float value)
        {
            int location = GL.GetUniformLocation(Handle, name);

            GL.Uniform1(location, value);
        }

        public void SetVector3(string name, Vector3 vector)
        {
            int location = GL.GetUniformLocation(Handle, name);

            GL.Uniform3(location, vector);
        }

        public void SetVector4(string name, Vector4 vector)
        {
            int location = GL.GetUniformLocation(Handle, name);

            GL.Uniform4(location, vector);
        }

        public void SetMatrix4(string name, Matrix4 matrix)
        {
            int location = GL.GetUniformLocation(Handle, name);

            GL.UniformMatrix4(location, true, ref matrix);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                GL.DeleteProgram(Handle);

                disposedValue = true;
            }
        }

        ~Shader()
        {
            GL.DeleteProgram(Handle);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
