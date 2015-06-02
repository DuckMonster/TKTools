using OpenTK;

namespace TKTools.Context
{
	public class Tileset
	{
		Texture texture;
		float tileWidth, tileHeight;
		int tileX = 0, tileY = 0;

		public Tileset(Texture t, float tileWidth, float tileHeight)
		{
			texture = t;
			this.tileWidth = tileWidth / t.Width;
			this.tileHeight = tileHeight / t.Height;
		}

		public Texture Texture
		{
			get { return texture; }
		}

		public float Width
		{
			get { return tileWidth; }
			set { tileWidth = value; }
		}

		public float Height
		{
			get { return tileHeight; }
			set { tileHeight = value; }
		}

		public int X
		{
			get { return tileX; }
			set { tileX = value; }
		}

		public int Y
		{
			get { return tileY; }
			set { tileY = value; }
		}

		public Vector2 Position
		{
			get { return new Vector2(X, Y); }
			set
			{
				X = (int)value.X;
				Y = (int)value.Y;
			}
		}

		public Vector2 Size
		{
			get { return new Vector2(Width, Height); }
			set
			{
				Width = value.X;
				Height = value.Y;
			}
		}
	}
}