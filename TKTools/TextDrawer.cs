using System;
using System.Drawing;
using IMG = System.Drawing.Imaging;

using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace TKTools
{
	public class TextDrawer : IDisposable
	{
		Font font = new Font("Trebuchet MS", 12f);

		Bitmap textBitmap;
		Graphics textGraphics;

		Texture texture;
		Color color = Color.White;

		public Color Color
		{
			get
			{
				return color;
			}
			set
			{
				color = value;
			}
		}

		public System.Drawing.Color SystemColor
		{
			get
			{
				return System.Drawing.Color.FromArgb((int)(255 * color.R), (int)(255 * color.G), (int)(255 * color.B));
			}
		}

		public Vector2 CanvasSize
		{
			get
			{
				return new Vector2(textBitmap.Width, textBitmap.Height);
			}
			set
			{
				textGraphics.Dispose();
				textBitmap.Dispose();

				textBitmap = new Bitmap((int)value.X, (int)value.Y);
				textGraphics = Graphics.FromImage(textBitmap);
			}
		}

		public TextDrawer(int width, int height)
		{
			textBitmap = new Bitmap(width, height);
			textGraphics = Graphics.FromImage(textBitmap);

			texture = new Texture();
			texture.UploadBitmap(textBitmap);
		}

		public void Dispose()
		{
			texture.Dispose();
			textGraphics.Dispose();
			textBitmap.Dispose();

			texture = null;
			textGraphics = null;
			textBitmap = null;
		}

		public void Clear()
		{
			textGraphics.Clear(System.Drawing.Color.Transparent);
			UpdateTexture();
		}

		public void Write(string text, float x, float y, float size, StringAlignment hori = StringAlignment.Center, StringAlignment vert = StringAlignment.Center)
		{
			x = x * textBitmap.Width;
			y = y * textBitmap.Height;

			Font f = new Font(font.FontFamily, (CanvasSize * size * 0.4f).Length);
			f = GetTargetWidthFont(text, (int)CanvasSize.X, f, textGraphics);

			StringFormat format = new StringFormat();
			format.Alignment = hori;
			format.LineAlignment = vert;

			textGraphics.DrawString(text, f, new SolidBrush(SystemColor), x, y, format);

			UpdateTexture();
		}

		public Vector2 MeasureString(string text, float size)
		{
			Font f = new Font(font.FontFamily, (CanvasSize * size * 0.4f).Length);
			f = GetTargetWidthFont(text, (int)CanvasSize.X, f, textGraphics);

			SizeF finalSize = textGraphics.MeasureString(text, f);

			Vector2 finalSizeVector = new Vector2(finalSize.Width, finalSize.Height);
			finalSizeVector.X -= f.Size * 0.4f;
			finalSizeVector.Y -= f.Size * 0.8f;

			return new Vector2(finalSizeVector.X / textBitmap.Width, finalSizeVector.Y / textBitmap.Height);
		}

		public void UpdateTexture()
		{
			texture.UpdateBitmap(textBitmap);
		}

		public static implicit operator Texture(TextDrawer t)
		{
			return t.texture;
		}

		public static Font GetTargetWidthFont(string text, int maxWidth, Font f, Graphics g)
		{
			for (int i = (int)f.Size; i > 0; i--)
			{
				Font testFont = new Font(f.FontFamily, i);
				if (g.MeasureString(text, testFont).Width <= maxWidth) return testFont;
			}

			return f;
		}
	}
}