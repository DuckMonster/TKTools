using OpenTK;
using System;
using System.Diagnostics;
using System.Drawing;

namespace TKTools
{
	public class TextBox
	{
		public enum HorizontalAlignment
		{
			Left = 0,
			Center = (1 << 0),
			Right = (1 << 1)
		}
		public enum VerticalAlignment
		{
			Top = 0,
			Center = (1 << 2),
			Bottom = (1 << 3)
		}

		static readonly long updateRate = 700;

		Font font;
		string text;
		string bufferText = null;

		Bitmap bitmap;
		Graphics bitmapGraphics;

		Texture texture;

		float width, height;
		bool newBitmap = true;

		float? setHeight = null;

		Stopwatch updateWatch;

		VerticalAlignment verticalAlign;
		HorizontalAlignment horizontalAlign;
		public VerticalAlignment VerticalAlign
		{
			get { return verticalAlign; }
			set { verticalAlign = value; }
		}
		public HorizontalAlignment HorizontalAlign
		{
			get { return horizontalAlign; }
			set { horizontalAlign = value; }
		}

		StringFormat StringFormat
		{
			get
			{
				StringFormat sf = new StringFormat();
				switch(HorizontalAlign)
				{
					case HorizontalAlignment.Center: sf.Alignment = StringAlignment.Center; break;
					case HorizontalAlignment.Left: sf.Alignment = StringAlignment.Near; break;
					case HorizontalAlignment.Right: sf.Alignment = StringAlignment.Far; break;
				}

				switch (VerticalAlign)
				{
					case VerticalAlignment.Center: sf.LineAlignment = StringAlignment.Center; break;
					case VerticalAlignment.Top: sf.LineAlignment = StringAlignment.Near; break;
					case VerticalAlignment.Bottom: sf.LineAlignment = StringAlignment.Far; break;
				}

				return sf;
			}
		}

		public float? SetHeight
		{
			get { return setHeight; }
			set { setHeight = value; }
		}

		public int Alignment
		{
			set
			{
				HorizontalAlignment hAlign;
				VerticalAlignment vAlign;

				hAlign = (HorizontalAlignment)(value & Convert.ToInt32("0011", 2));
				vAlign = (VerticalAlignment)((value & Convert.ToInt32("1100", 2)) >> 2);

				VerticalAlign = vAlign;
				HorizontalAlign = hAlign;
			}
		}

		Vector2 AlignDrawVector
		{
			get
			{
				float hori = 0f, vert = 0f;
				switch (HorizontalAlign)
				{
					case HorizontalAlignment.Left: hori = 0f; break;
					case HorizontalAlignment.Center: hori = 0.5f; break;
					case HorizontalAlignment.Right: hori = 1f; break;
				}
				switch (VerticalAlign)
				{
					case VerticalAlignment.Top: vert = 0f; break;
					case VerticalAlignment.Center: vert = 0.5f; break;
					case VerticalAlignment.Bottom: vert = 1f; break;
				}

				return new Vector2(hori, vert) * new Vector2(bitmap.Width, bitmap.Height);
			}
		}

		Vector2 AlignVector
		{
			get
			{
				float hori = 0f, vert = 0f;
				switch(HorizontalAlign)
				{
					case HorizontalAlignment.Left: hori = 0f; break;
					case HorizontalAlignment.Center: hori = -0.5f; break;
					case HorizontalAlignment.Right: hori = -1f; break;
				}
				switch (VerticalAlign)
				{
					case VerticalAlignment.Top: vert = -1f; break;
					case VerticalAlignment.Center: vert = -0.5f; break;
					case VerticalAlignment.Bottom: vert = 0f; break;
				}

				return new Vector2(hori, vert) * new Vector2(width, height);
			}
		}

		public Texture Texture
		{
			get
			{
				CheckUpdateAll();
				return texture;
			}
		}

		public string Text
		{
			get { return text; }
			set
			{
				if (value.Equals(text)) return;

				bufferText = value;
			}
		}

		public Vector2[] Vertices
		{
			get
			{
				CheckUpdateAll();

				Vector2 alignVector = AlignVector;
				float scale = 1f;

				if (setHeight != null)
					scale = setHeight.Value / height;

				return new Vector2[] {
					(new Vector2(0, 0) + alignVector) * scale,
					(new Vector2(width, 0) + alignVector) * scale,
					(new Vector2(width, height) + alignVector) * scale,
					(new Vector2(0, height) + alignVector) * scale
				};
			}
		}

		public Vector2[] UV
		{
			get
			{
				CheckUpdateAll();

				return new Vector2[] {
					new Vector2(0, 1),
					new Vector2(1, 1),
					new Vector2(1, 0),
					new Vector2(0, 0)
				};
			}
		}

		public TextBox() : this(new Font("Trebuchet MS", 12f)) { }
		public TextBox(float size) : this(new Font("Trebuchet MS", size)) { }
		public TextBox(Font f)
		{
			bitmap = new Bitmap(1, 1);
			bitmapGraphics = Graphics.FromImage(bitmap);

			font = f;
			texture = new Texture();
		}

		void CreateBitmap(int width, int height)
		{
			if (bitmap != null)
			{
				bitmap.Dispose();
				bitmapGraphics.Dispose();
				bitmap = null;
				bitmapGraphics = null;
			}

			bitmap = new Bitmap(width, height);
			bitmapGraphics = Graphics.FromImage(bitmap);

			this.width = width * 0.01f;
			this.height = height * 0.01f;

			newBitmap = true;
		}

		void CheckUpdateAll()
		{
			if (bufferText != null && (updateWatch == null || updateWatch.ElapsedMilliseconds >= updateRate))
			{
				text = bufferText;

				UpdateBitmap();
				UpdateTexture();

				bufferText = null;

				if (updateWatch == null)
					updateWatch = Stopwatch.StartNew();
				else
					updateWatch.Restart();
			}
		}

		void UpdateBitmap()
		{
			SizeF size = bitmapGraphics.MeasureString(text, font);

			if (size.Width > width || size.Height > height)
				CreateBitmap((int)size.Width, (int)size.Height);

			StringFormat format = StringFormat;
			Vector2 formatVector = AlignDrawVector;

			bitmapGraphics.Clear(System.Drawing.Color.Transparent);
			bitmapGraphics.DrawString(text, font, Brushes.White, formatVector.X, formatVector.Y, format);
		}

		void UpdateTexture()
		{
			if (newBitmap)
			{
				texture.UploadBitmap(bitmap);
				newBitmap = false;
			}
			else
				texture.UpdateBitmap(bitmap);
		}
	}
}
