using OpenTK.Graphics.OpenGL4;
using StbImageSharp;

namespace GrassRendering
{
    public class Texture
    {
        public int handle;
        public string type = "";
        public string path = "";

        public Texture(string path, TextureUnit unit = TextureUnit.Texture0)
        {
            handle = GL.GenTexture();
            this.path = path;

            Use(unit);

            StbImage.stbi_set_flip_vertically_on_load(1); //this will correct that, making the texture display properly.

            ImageResult image = ImageResult.FromStream(File.OpenRead(path), ColorComponents.RedGreenBlueAlpha); //load the image.

            GL.TexImage2D(
                TextureTarget.Texture2D, //type of texture being generated (1D/2D/3D)
                0, //level of detail, if set to sth other than 0 we can set the default mipmap as a level lower than the maximum (?)
                PixelInternalFormat.Rgba, //the format OpenGL will use to store the pixels on the GPU
                image.Width,
                image.Height,
                0, //border of the image, must always be 0
                PixelFormat.Rgba,
                PixelType.UnsignedByte,
                image.Data);

            GL.GenerateMipmap(GenerateMipmapTarget.Texture2D); //generates mipmaps of our texture
        }

        public void Use(TextureUnit unit)
        {
            GL.ActiveTexture(unit);
            GL.BindTexture(TextureTarget.Texture2D, handle);
        }
    }
}
