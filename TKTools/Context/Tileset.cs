using OpenTK;

namespace TKTools.Context
{
	public class Tileset
	{
		Texture texture;

		float tileWidth, tileHeight;
		float textureWidth, textureHeight;

		int tileX = 0, tileY = 0;

		public Tileset(Tileset t)
			:this(t.Texture, t.tileWidth, t.tileHeight)
		{
			X = t.X;
			Y = t.Y;
		}
		public Tileset(Texture t, float tileWidth, float tileHeight)
		{
			texture = t;

			this.textureWidth = t.Width;
			this.textureHeight = t.Height;

			this.tileWidth = tileWidth;
			this.tileHeight = tileHeight;
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

		internal float UVWidth
		{
			get { return Width / textureWidth; }
		}
		internal float UVHeight
		{
			get { return Height / textureHeight; }
		}
		internal Vector2 UVSize
		{
			get { return new Vector2(UVWidth, UVHeight); }
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

		public Tileset Copy()
		{
			return new Tileset(this);
		}
	}
}