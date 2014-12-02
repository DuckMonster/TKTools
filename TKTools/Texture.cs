using System;
using System.Drawing;
using System.Drawing.Imaging;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace TKTools
{
	public class Texture : IDisposable
	{
		int textureID;

		public Texture(string filename)
		{
			LoadTexture(filename);
		}

		public void Dispose()
		{
			GL.DeleteTexture(textureID);
		}

		public void LoadTexture(string filename)
		{
			using (Bitmap bmp = new Bitmap(filename))
			{
				textureID = GL.GenTexture();
				Bind();

				GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
				GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMinFilter.Nearest);

				BitmapData data = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

				GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, data.Width, data.Height, 0, OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, data.Scan0);

				bmp.UnlockBits(data);
			}
		}

		public void Bind()
		{
			GL.BindTexture(TextureTarget.Texture2D, textureID);
		}
	}
}