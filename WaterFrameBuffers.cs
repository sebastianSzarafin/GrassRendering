using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.GraphicsLibraryFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GrassRendering
{
    internal class WaterFrameBuffers
    {
        private const int REFLECTION_WIDTH = 1280;
        private const int REFLECTION_HEIGHT = 720;

        private const int REFRACTION_WIDTH = 1280;
        private const int REFRACTION_HEIGHT = 720;

        private int reflectionFrameBuffer;
        private Texture reflectionTexture;
        private int reflectionDepthBuffer;

        private int refractionFrameBuffer;
        private Texture refractionTexture;
        private Texture refractionDepthTexture;

        public Texture ReflectionTexture { get => reflectionTexture; }
        public Texture RefractionTexture { get => refractionTexture; }
        public Texture RefractionDepthTexture { get => refractionDepthTexture; }

        public WaterFrameBuffers()
        {
            reflectionFrameBuffer = CreateFrameBuffer();
            reflectionTexture = new Texture(TextureUnit.Texture0, PixelInternalFormat.Rgb, REFLECTION_WIDTH, REFLECTION_HEIGHT, PixelFormat.Rgb, PixelType.UnsignedByte, FramebufferAttachment.ColorAttachment0);
            reflectionDepthBuffer = CreateDepthBufferAttachment(REFLECTION_WIDTH, REFLECTION_HEIGHT);
            UnbindCurrentFrameBuffer();

            refractionFrameBuffer = CreateFrameBuffer();
            refractionTexture = new Texture(TextureUnit.Texture1, PixelInternalFormat.Rgb, REFRACTION_WIDTH, REFRACTION_HEIGHT, PixelFormat.Rgb, PixelType.UnsignedByte, FramebufferAttachment.ColorAttachment0);
            refractionDepthTexture = new Texture(TextureUnit.Texture2, PixelInternalFormat.DepthComponent, REFRACTION_WIDTH, REFRACTION_HEIGHT, PixelFormat.DepthComponent, PixelType.Float, FramebufferAttachment.DepthAttachment);
            UnbindCurrentFrameBuffer();
        }

        private int CreateFrameBuffer()
        {
            int frameBuffer = GL.GenFramebuffer();
            //generate name for frame buffer
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, frameBuffer);
            //create the framebuffer
            GL.DrawBuffer(DrawBufferMode.ColorAttachment0);
            //indicate that we will always render to color attachment 0
            return frameBuffer;
        }

        private int CreateTextureAttachment(int width, int height)
        {
            int texture = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2D, texture);
            GL.TexImage2D(
                TextureTarget.Texture2D,
                0,
                PixelInternalFormat.Rgb,
                width,
                height,
                0,
                PixelFormat.Rgb,
                PixelType.UnsignedByte,
                (IntPtr)null);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMinFilter.LinearMipmapLinear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.LinearMipmapLinear);
            GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);
            GL.FramebufferTexture(
                FramebufferTarget.Framebuffer,
                FramebufferAttachment.ColorAttachment0,
                texture,
                0);
            return texture;
        }

        private int CreateDepthTextureAttachment(int width, int height)
        {
            int texture = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2D, texture);
            GL.TexImage2D(
                TextureTarget.Texture2D,
                0,
                PixelInternalFormat.DepthComponent32,
                width,
                height,
                0,
                PixelFormat.DepthComponent,
                PixelType.Float,
                (IntPtr)null);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMinFilter.LinearMipmapLinear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.LinearMipmapLinear);
            GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);
            GL.FramebufferTexture(
                FramebufferTarget.Framebuffer,
                FramebufferAttachment.DepthAttachment,
                texture,
                0);
            return texture;
        }

        private int CreateDepthBufferAttachment(int width, int height)
        {
            int depthBuffer = GL.GenRenderbuffer();
            GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, depthBuffer);
            GL.RenderbufferStorage(RenderbufferTarget.Renderbuffer, RenderbufferStorage.DepthComponent, width, height);
            GL.DrawBuffer(DrawBufferMode.ColorAttachment0);
            GL.FramebufferRenderbuffer(FramebufferTarget.Framebuffer, FramebufferAttachment.DepthAttachment, RenderbufferTarget.Renderbuffer, depthBuffer);
            return depthBuffer;
        }

        public void BindReflectionFrameBuffer()
        {//call before rendering to this FBO
            BindFrameBuffer(reflectionFrameBuffer, REFLECTION_WIDTH, REFLECTION_HEIGHT);
        }

        public void BindRefractionFrameBuffer()
        {//call before rendering to this FBO
            BindFrameBuffer(refractionFrameBuffer, REFRACTION_WIDTH, REFRACTION_HEIGHT);
        }

        private void BindFrameBuffer(int frameBuffer, int width, int height)
        {
            GL.BindTexture(TextureTarget.Texture2D, 0);//To make sure the texture isn't bound
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, frameBuffer);
            GL.Viewport(0, 0, width, height);
        }

        public void UnbindCurrentFrameBuffer()
        {//call to switch to default frame buffer
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
            GL.Viewport(0, 0, 1280, 720);
        }

        public void Unload()
        {
            GL.DeleteFramebuffer(reflectionFrameBuffer);
            reflectionTexture.Unload();
            GL.DeleteRenderbuffer(reflectionDepthBuffer);
            GL.DeleteFramebuffer(refractionFrameBuffer);
            refractionTexture.Unload();
            refractionDepthTexture.Unload();
        }
    }
}
