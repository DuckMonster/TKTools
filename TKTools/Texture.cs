using System;
using System.Drawing;
using System.Drawing.Imaging;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using TKTools.Context;
using System.Collections.Generic;

namespace TKTools
{
	public class Texture : IDisposable
	{
		private const string screenVertexSource = @"
#version 330
in vec2 vertexPosition;
in vec2 vertexUV;

out vec2 uv;

void main() {
	uv = vertexUV;
	gl_Position = vec4(vertexPosition, 0.0, 1.0);
}
";
		private const string screenFragmentSource = @"
#version 330
in vec2 uv;
uniform sampler2D texture;

out vec4 fragment;

void main() {
	fragment = texture2D(texture, uv);
	//fragment = vec4(1.0, 0.0, 0.0, 1.0);
}
";

		private static ShaderProgram screenProgram;
		private static VAO screenVAO;
		private static VBO<Vector2> vboVertexPosition, vboVertexUV;

		internal static void InitVAO()
		{
			screenProgram = new ShaderProgram(screenVertexSource, screenFragmentSource);

			screenVAO = new VAO(screenProgram);

			vboVertexPosition = new VBO<Vector2>();
			vboVertexPosition.UploadData(new Vector2[] {
				new Vector2(-1, -1),
				new Vector2(1, -1),
				new Vector2(1, 1),
				new Vector2(-1, 1)
			});

			vboVertexUV = new VBO<Vector2>();
			vboVertexUV.UploadData(new Vector2[] {
				new Vector2(0, 0),
				new Vector2(1, 0),
				new Vector2(1, 1),
				new Vector2(0, 1)
			});

			screenVAO.BindAttribute("vertexPosition", vboVertexPosition);
			screenVAO.BindAttribute("vertexUV", vboVertexUV);
		}

		internal static void DrawTextureToScreen(Texture t) { DrawTextureToScreen(new Texture[] { t }, screenProgram); }
		internal static void DrawTextureToScreen(Texture t, ShaderProgram program) { DrawTextureToScreen(new Texture[] { t }, program); }
		internal static void DrawTextureToScreen(Texture[] textures) { DrawTextureToScreen(textures, screenProgram); }
		internal static void DrawTextureToScreen(Texture[] textures, ShaderProgram program)
		{
			GL.Disable(EnableCap.DepthTest);

			screenVAO.Bind();
			program.Use();
			textures[0].Bind();

			GL.DrawArrays(PrimitiveType.Quads, 0, 4);
		}

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
			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);
		}

		void LoadTexture(string filename)
		{
			using (Bitmap bmp = new Bitmap(filename))
			{
				bmp.RotateFlip(RotateFlipType.RotateNoneFlipY);

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

		public void UploadBitmap(Bitmap b)
		{
			Bitmap temp = new Bitmap(b);
			temp.RotateFlip(RotateFlipType.RotateNoneFlipY);

			BitmapData data = temp.LockBits(new Rectangle(0, 0, temp.Width, temp.Height), ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

			Bind();
			GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, data.Width, data.Height, 0, OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, data.Scan0);

			width = temp.Width;
			height = temp.Height;

			temp.UnlockBits(data);

			temp.Dispose();
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

		public void DrawToScreen() { DrawTextureToScreen(this); }
		public void DrawToScreen(ShaderProgram p) { DrawTextureToScreen(this, p); }
	}
}