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
		int width, height;

		public int Width
		{
			get
			{
				return width;
			}
		}
		public int Height
		{
			get
			{
				return height;
			}
		}

		public Texture()
		{
			LoadTexture();
		}
		public Texture(string filename)
		{
			LoadTexture(filename);
		}

		public void Dispose()
		{
			GL.DeleteTexture(textureID);
		}

		void LoadTexture()
		{
			textureID = GL.GenTexture();
			Bind();

			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMinFilter.Nearest);
			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Clamp);
			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Clamp);
		}

		void LoadTexture(string filename)
		{
			using (Bitmap bmp = new Bitmap(filename))
			{
				LoadTexture();
				UploadBitmap(bmp);
			}
		}

		public void BindToFrameBuffer(FrameBuffer buff)
		{
			Bind();
			buff.Bind();

			GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba8, buff.Width, buff.Height, 0, OpenTK.Graphics.OpenGL.PixelFormat.Rgba, PixelType.UnsignedByte, IntPtr.Zero);
			GL.FramebufferTexture2D(FramebufferTarget.FramebufferExt, FramebufferAttachment.ColorAttachment0, TextureTarget.Texture2D, textureID, 0);

			width = buff.Width;
			height = buff.Height;

			buff.Release();
		}

		public void UploadBitmap(Bitmap bmp)
		{
			BitmapData data = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

			Bind();
			GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, data.Width, data.Height, 0, OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, data.Scan0);

			width = bmp.Width;
			height = bmp.Height;

			bmp.UnlockBits(data);
		}

		public void UpdateBitmap(Bitmap bmp)
		{
			BitmapData data = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

			Bind();
			GL.TexSubImage2D(TextureTarget.Texture2D, 0, 0, 0, data.Width, data.Height, OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, data.Scan0);

			bmp.UnlockBits(data);
		}

		public void Bind()
		{
			GL.BindTexture(TextureTarget.Texture2D, textureID);
		}
	}
}