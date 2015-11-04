using OpenTK;
using OpenTK.Graphics.OpenGL;
using System.Collections.Generic;
using TKTools.Mathematics;

namespace TKTools.Context
{
	public enum MeshPrimitive
	{
		Triangle,
		Quad
	}

	public class Model : Mesh
	{
		const string vertexSource =
			@"
#version 330

in vec3 vertexPosition;
in vec2 vertexUV;

uniform mat4 projection;
uniform mat4 view;
uniform mat4 model;

out vec2 uv;

void main() {
	uv = vertexUV;
	gl_Position = projection * view * model * vec4(vertexPosition, 1.0);
}";

		const string fragmentSource =
					@"
#version 330
uniform vec4 color;

in vec2 uv;

//FILL COLOR
uniform bool fillColor;
//

//TEXTURE STUFF
uniform sampler2D texture;
uniform bool usingTexture;
//

out vec4 fragment;

void main() {
	vec4 finalColor = vec4(1, 1, 1, 1);

	if (usingTexture)
		finalColor = texture(texture, uv);

	if (fillColor)
		finalColor = vec4(color.rgb, finalColor.a * color.a);
	else
		finalColor = finalColor * color;

	if (finalColor.a <= 0.0)
		discard;

	fragment = finalColor;
}";

		public static Model CreateFromPrimitive(MeshPrimitive p)
		{
			Model m = null;

			switch (p)
			{
				#region Triangle
				case MeshPrimitive.Triangle:
					m = new Model2D(
					new Vector2[] {
						new Vector2(-0.5f, -0.5f),
						new Vector2(0f, 0.5f),
						new Vector2(0.5f, -0.5f)
					},

					new Vector2[] {
						new Vector2(0f, 0f),
						new Vector2(0.5f, 1f),
						new Vector2(1f, 0f)
					});
					break;
				#endregion triangle
				#region Quad
				case MeshPrimitive.Quad:
					m = new Model2D(
						new Vector2[] {
							new Vector2(-0.5f, -0.5f),
							new Vector2(-0.5f, 0.5f),
							new Vector2(0.5f, 0.5f),
							new Vector2(0.5f, -0.5f)
						},

						new Vector2[] {
							new Vector2(0f, 0f),
							new Vector2(0f, 1f),
							new Vector2(1f, 1f),
							new Vector2(1f, 0f)
						});
					break;
					#endregion
			}

			return m;
		}
		public static Model CreateFromTexture(Texture t)
		{
			Model m = CreateFromPrimitive(MeshPrimitive.Quad);

			m.Texture = t;
			return m;
		}

		internal static ShaderProgram StandardShader;
		internal static void CompileStandardShader()
		{
			StandardShader = new ShaderProgram(vertexSource, fragmentSource);
		}

		Texture texture;
		bool usingTexture = true;
		public Texture Texture
		{
			get { return texture; }
			set { texture = value; }
		}
		public bool TextureEnabled
		{
			get { return Texture != null ? usingTexture : false; }
			set { usingTexture = value; }
		}

		Matrix4 modelMatrix = Matrix4.Identity;
		public Matrix4 ModelMatrix
		{
			get { return modelMatrix; }
			set { modelMatrix = value; }
		}

		Color color = Color.White;
		public Color Color
		{
			get { return color; }
			set
			{
				color = value;
				this["color"].Value = color;
			}
		}

		bool fillColor = false;
		public bool FillColor
		{
			get { return fillColor; }
			set
			{
				fillColor = value;
				this["fillColor"].Value = fillColor;
			}
		}

		public Vector3[] VertexPosition
		{
			get { return GetAttribute<Vector3>("vertexPosition").Data; }
			set { GetAttribute<Vector3>("vertexPosition").Data = value; }
		}

		public Vector2[] VertexUV
		{
			get { return GetAttribute<Vector2>("vertexUV").Data; }
			set { GetAttribute<Vector2>("vertexUV").Data = value; }
		}

		Polygon polygon = new Polygon();
		public Polygon Polygon
		{
			get
			{
				BakePolygon();
				return polygon;
			}
		}

		void BakePolygon()
		{
			Vector3[] vec = VertexPosition;
			for (int i = 0; i < vec.Length; i++)
				vec[i] = Vector4.Transform(new Vector4(vec[i], 1f), modelMatrix).Xyz;

			polygon.pointList.Clear();
			polygon.pointList.AddRange(vec);
		}

		public Model()
			: base(StandardShader)
		{
			GetAttribute<Vector3>("vertexPosition");
			GetAttribute<Vector2>("vertexUV");

			Color = Color.White;
			FillColor = false;
		}
		public Model(Vector3[] vertices)
			: this()
		{
			VertexPosition = vertices;
		}
		public Model(Vector3[] vertices, Vector2[] uv)
			: this()
		{
			VertexPosition = vertices;
			VertexUV = uv;
		}

		public void Reset()
		{
			modelMatrix = Matrix4.Identity;
		}

		public void Translate(float x, float y) { Translate(new Vector3(x, y, 0f)); }
		public void Translate(float x, float y, float z) { Translate(new Vector3(x, y, z)); }
		public void Translate(Vector2 d) { Translate(new Vector3(d)); }
		public void Translate(Vector3 d)
		{
			modelMatrix = Matrix4.CreateTranslation(d) * modelMatrix;
		}

		public void Scale(float s) { Scale(new Vector3(s)); }
		public void Scale(float x, float y) { Scale(new Vector3(x, y, 1f)); }
		public void Scale(float x, float y, float z) { Scale(new Vector3(x, y, z)); }
		public void Scale(Vector2 s) { Scale(new Vector3(s.X, s.Y, 1f)); }
		public void Scale(Vector3 s)
		{
			modelMatrix = Matrix4.CreateScale(s) * modelMatrix;
		}

		public void RotateX(float d)
		{
			modelMatrix = Matrix4.CreateRotationX(TKMath.ToRadians(d)) * modelMatrix;
		}
		public void RotateY(float d)
		{
			modelMatrix = Matrix4.CreateRotationY(TKMath.ToRadians(d)) * modelMatrix;
		}
		public void RotateZ(float d)
		{
			modelMatrix = Matrix4.CreateRotationZ(TKMath.ToRadians(d)) * modelMatrix;
		}

		public override void Draw(PrimitiveType type)
		{
			this["projection"].Value = Camera.activeCamera.Projection;
			this["view"].Value = Camera.activeCamera.View;
			this["model"].Value = ModelMatrix;

			this["usingTexture"].Value = TextureEnabled;
			if (Texture != null)
				Texture.Bind();

			base.Draw(type);
		}
	}
}