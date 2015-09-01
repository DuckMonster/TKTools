using OpenTK;
using System;
using TKTools.Mathematics;

namespace TKTools.Context
{
	public class Sprite : IDisposable
	{
		Mesh mesh;

		bool dirtyMatrix = true;

		Vector3 position = Vector3.Zero;
		float rotation = 0f;
		Vector2 scale = Vector2.One;

		public Sprite()
		{
			mesh = Mesh.CreateFromPrimitive(MeshPrimitive.Quad);
		}

		public Sprite(Texture t)
		{
			mesh = Mesh.CreateFromTexture(t);
		}

		public Sprite(Tileset t)
		{
			mesh = Mesh.CreateFromTexture(t.Texture);
			mesh.Tileset = t;
		}

		public void Dispose()
		{
			mesh.Dispose();
		}

		public Mesh Mesh
		{
			get { return mesh; }
			set
			{
				if (value != mesh)
					mesh.Dispose();

				mesh = value;
			}
		}

		public bool FillColor
		{
			get { return mesh.FillColor; }
			set { mesh.FillColor = value; }
		}

		public Color Color
		{
			get { return mesh.Color; }
			set { mesh.Color = value; }
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

		public float ScaleF
		{
			set
			{
				Scale = new Vector2(value);
			}
		}
		public Vector2 Scale
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
				rotation = TKMath.ToRadians(value);
				dirtyMatrix = true;
			}
		}

		void UpdateMatrix()
		{
			float sin = 0f, cos = 1f;

			if (rotation != 0f)
			{
				cos = (float)Math.Cos(rotation);
				sin = (float)Math.Sin(rotation);
			}

			mesh.ModelMatrix = new Matrix4(
				scale.X * cos, scale.X * sin, 0, 0,
				scale.Y * -sin, scale.Y * cos, 0, 0,
				0, 0, 1, 0,
				position.X, position.Y, position.Z, 1
				);
		}

		public void SetTransform(Vector2 position, float scale, float rotation) { SetTransform(new Vector3(position), scale, rotation); }
		public void SetTransform(Vector3 position, float scale, float rotation)
		{
			Position = position;
			ScaleF = scale;
			Rotation = rotation;
		}
		public void SetTransform(Vector2 position, Vector2 scale, float rotation) { SetTransform(new Vector3(position), scale, rotation); }
		public void SetTransform(Vector3 position, Vector2 scale, float rotation)
		{
			Position = position;
			Scale = scale;
			Rotation = rotation;
		}

		public void Draw()
		{
			if (dirtyMatrix) UpdateMatrix();
			mesh.Draw();
		}

		public void Draw(Vector2 position, float scale, float rotation) { Draw(new Vector3(position), scale, rotation); }
		public void Draw(Vector3 position, float scale, float rotation)
		{
			SetTransform(position, scale, rotation);

			Draw();
		}
		public void Draw(Vector2 position, Vector2 scale, float rotation) { Draw(new Vector3(position), scale, rotation); }
		public void Draw(Vector3 position, Vector2 scale, float rotation)
		{
			SetTransform(position, scale, rotation);

			Draw();
		}
	}
}