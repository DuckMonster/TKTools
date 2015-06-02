using OpenTK;
using System;

namespace TKTools.Context
{
	public class Sprite
	{
		Mesh mesh;

		bool dirtyMatrix = true;

		Vector3 position = Vector3.Zero;
		float rotation = 0f;
		float scale = 1f;

		public Sprite(Texture t)
		{
			mesh = Mesh.CreateFromTexture(t);
		}

		public Sprite(Tileset t)
		{
			mesh = Mesh.CreateFromTexture(t.Texture);
			mesh.Tileset = t;
		}

		public Texture Texture
		{
			get { return mesh.Texture; }
			set { mesh.Texture = value; }
		}

		public Tileset Tileset
		{
			get { return mesh.Tileset; }
			set { mesh.Tileset = value; }
		}

		public Vector3 Position
		{
			get { return position; }
			set
			{
				position = value;
				dirtyMatrix = true;
			}
		}

		public float Scale
		{
			get { return scale; }
			set
			{
				scale = value;
				dirtyMatrix = true;
			}
		}

		public float Rotation
		{
			get { return rotation; }
			set
			{
				rotation = value;
				dirtyMatrix = true;
			}
		}

		void UpdateMatrix()
		{
			float cos = (float)Math.Cos(rotation);
			float sin = (float)Math.Sin(rotation);

			mesh.ModelMatrix = new Matrix4(
				scale * cos, scale * sin, 0, 0,
				scale * -sin, scale * cos, 0, 0,
				0, 0, 1, 0,
				position.X, position.Y, position.Z, 1
				);
		}

		public void Draw()
		{
			if (dirtyMatrix) UpdateMatrix();
			mesh.Draw();
		}

		public void Draw(Vector3 position, float scale, float rotation)
		{
			Position = position;
			Scale = scale;
			Rotation = rotation;

			Draw();
		}
	}
}