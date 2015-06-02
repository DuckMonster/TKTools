using OpenTK;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;

namespace TKTools.Context
{
	public class Mesh
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

uniform sampler2D texture;
uniform bool usingTexture;
uniform vec4 color;
uniform bool fillColor;

uniform vec2 tileSize;
uniform vec2 tilePosition;
uniform bool tiledTexture;

in vec2 uv;
out vec4 fragment;


void main() {
	vec4 finalColor;

	if (tiledTexture)
		uv = tileSize * tilePosition + tileSize * uv;

	if (usingTexture) {
		finalColor = texture2D(texture, uv) * color;
	}
	else {
		finalColor = color;
	}

	if (fillColor)
	{
		finalColor.rgb = color.rgb;
	}

	if (finalColor.a <= 0) discard;
	else fragment = finalColor;
}";

		public const int VERTEX_POSITION_ID = 0;
		public const int VERTEX_UV_ID = 1;

		internal static ShaderProgram StandardShader;
		internal static void CompileStandardShader()
		{
			StandardShader = new ShaderProgram(vertexSource, fragmentSource);
			StandardShader.SetAttribute("vertexPosition", VERTEX_POSITION_ID);
			StandardShader.SetAttribute("vertexUV", VERTEX_UV_ID);
		}

		public static Mesh CreateFromTexture(Texture t)
		{
			Mesh m = new Mesh(
				new Vector3[] {
					new Vector3(-0.5f, -0.5f, 0f),
					new Vector3(-0.5f, 0.5f, 0f),
					new Vector3(0.5f, 0.5f, 0f),
					new Vector3(0.5f, -0.5f, 0f)
				},

				new Vector2[] {
					new Vector2(0f, 0f),
					new Vector2(0f, 1f),
					new Vector2(1f, 1f),
					new Vector2(1f, 0f)
				});

			m.Texture = t;
			return m;
		}

		List<Vector3> vertices = new List<Vector3>();
		List<Vector2> uv = new List<Vector2>();

		VAO vao;
		VBO<Vector3> vertexPosition;
		VBO<Vector2> vertexUV;

		Color color = Color.White;
		public Color Color
		{
			get { return color; }
			set { color = value; }
		}

		bool textureEnabled = true;
		public bool TextureEnabled
		{
			get { return textureEnabled; }
			set { textureEnabled = value; }
		}

		Texture texture = null;
		public Texture Texture
		{
			get { return texture; }
			set
			{
				texture = value;
				tileset = null;
			}
		}

		Tileset tileset = null;
		internal Tileset Tileset
		{
			get { return tileset; }
			set
			{
				tileset = value;
				texture = tileset.Texture;
			}
		}

		bool fillColor = false;
		public bool FillColor
		{
			get { return fillColor; }
			set { fillColor = value; }
		}

		Matrix4 modelMatrix = Matrix4.Identity;
		public Matrix4 ModelMatrix
		{
			get { return modelMatrix; }
			set { modelMatrix = value; }
		}

		public Vector3[] Vertices
		{
			get { return vertices.ToArray(); }
			set
			{
				vertices.Clear();
				vertices.AddRange(value);
			}
		}

		public Vector2[] UV
		{
			get { return uv.ToArray(); }
			set
			{
				uv.Clear();
				uv.AddRange(value);
			}
		}

		ShaderProgram Program
		{
			get { return Mesh.StandardShader; }
		}

		public Mesh()
		{
			GenerateBuffers();
		}
		public Mesh(IEnumerable<Vector3> points)
			:this()
		{
			AddVertices(points);
		}
		public Mesh(IEnumerable<Vector3> points, IEnumerable<Vector2> uvs)
			:this()
		{
			AddVertices(points, uvs);
		}

		public void Dispose()
		{
			vertexPosition.Dispose();
			vertexUV.Dispose();
		}

		void GenerateBuffers()
		{
			vao = new VAO();
			vertexPosition = new VBO<Vector3>();
			vertexUV = new VBO<Vector2>();

			vertexPosition.BindToVAO(vao);
			vertexUV.BindToVAO(vao);
		}

		void AddVertices(IEnumerable<Vector3> vert)
		{
			vertices.AddRange(vert);
			UploadVertices();
		}
		void AddVertices(IEnumerable<Vector3> vert, IEnumerable<Vector2> uvs)
		{
			vertices.AddRange(vert);
			uv.AddRange(uvs);
			UploadVertices();
		}

		void UploadVertices()
		{
			vao.Bind();
			if (uv.Count == vertices.Count)
			{
				vertexPosition.UploadData(vertices.ToArray());
				vertexUV.UploadData(uv.ToArray());
				vertexPosition.BindToAttribute(Mesh.VERTEX_POSITION_ID);
				vertexUV.BindToAttribute(Mesh.VERTEX_UV_ID);
			} else
			{
				vertexPosition.UploadData(vertices.ToArray());
				vertexPosition.BindToAttribute(Mesh.VERTEX_POSITION_ID);
				vertexPosition.BindToAttribute(Mesh.VERTEX_UV_ID);
			}
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
			modelMatrix = Matrix4.CreateRotationX(d) * modelMatrix;
		}
		public void RotateY(float d)
		{
			modelMatrix = Matrix4.CreateRotationY(d) * modelMatrix;
		}
		public void RotateZ(float d)
		{
			modelMatrix = Matrix4.CreateRotationZ(d) * modelMatrix;
		}

		public void Draw()
		{
			vao.Bind();

			Program["projection"].SetValue(Camera.activeCamera.Projection);
			Program["view"].SetValue(Camera.activeCamera.View);
			Program["model"].SetValue(modelMatrix);

			if (textureEnabled && texture != null)
			{
				Program["usingTexture"].SetValue(true);
				texture.Bind();

				if (tileset != null)
				{
					Program["tiledTexture"].SetValue(true);
					Program["tileSize"].SetValue(tileset.Size);
					Program["tilePosition"].SetValue(tileset.Position);
				}
				else Program["tiledTexture"].SetValue(false);
			}
			else Program["usingTexture"].SetValue(false);

			Program["color"].SetValue(Color);
			Program["fillColor"].SetValue(FillColor);

			GL.DrawArrays(PrimitiveType.Quads, 0, 4);
		}
	}
}