﻿using OpenTK;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using TKTools.Mathematics;

namespace TKTools.Context
{
	public enum MeshPrimitive
	{
		Triangle,
		Quad
	}

	public class Mesh : IDisposable
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

	vec2 finalUV = uv;

	if (tiledTexture)
		finalUV = tileSize * tilePosition + tileSize * finalUV;

	if (usingTexture) {
		finalColor = texture2D(texture, finalUV) * color;
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
			//StandardShader.SetAttribute("vertexPosition", VERTEX_POSITION_ID);
			//StandardShader.SetAttribute("vertexUV", VERTEX_UV_ID);
		}

		public static Mesh CreateFromPrimitive(MeshPrimitive p)
		{
			Mesh m = null;

			switch (p)
			{
				#region Triangle
				case MeshPrimitive.Triangle:
					m = new Mesh(
					new Vector3[] {
						new Vector3(-0.5f, -0.5f, 0f),
						new Vector3(0f, 0.5f, 0f),
						new Vector3(0.5f, -0.5f, 0f)
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
					m = new Mesh(
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
					break;
				#endregion
			}

			return m;
		}

		public static Mesh CreateFromTexture(Texture t)
		{
			Mesh m = CreateFromPrimitive(MeshPrimitive.Quad);

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
		public Tileset Tileset
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

		PrimitiveType primitiveType = PrimitiveType.Polygon;
		public PrimitiveType PrimitiveType
		{
			get { return primitiveType; }
			set { primitiveType = value; }
		}

		Matrix4 modelMatrix = Matrix4.Identity;
		public Matrix4 ModelMatrix
		{
			get { return modelMatrix; }
			set { modelMatrix = value; }
		}

		static Vector3 Vec2ToVec3(Vector2 v) { return new Vector3(v); }
		static Vector2 Vec3ToVec2(Vector3 v) { return v.Xy; }

        public Vector2[] Vertices2
		{
			get { return Array.ConvertAll(Vertices, new Converter<Vector3, Vector2>(Vec3ToVec2)); }
			set { Vertices = Array.ConvertAll(value, new Converter<Vector2, Vector3>(Vec2ToVec3)); }
		}

		public Vector3[] Vertices
		{
			get { return vertices.ToArray(); }
			set
			{
				vertices.Clear();
				vertices.AddRange(value);

				UploadVertices();
			}
		}

		public Vector2[] UV
		{
			get { return uv.ToArray(); }
			set
			{
				uv.Clear();
				uv.AddRange(value);

				UploadVertices();
			}
		}

		ShaderProgram Program
		{
			get { return Mesh.StandardShader; }
		}

		public Polygon Polygon
		{
			get
			{
				Vector3[] p = new Vector3[vertices.Count];
				for (int i = 0; i < p.Length; i++)
					p[i] = Vector3.Transform(vertices[i], ModelMatrix);

				return new Polygon(p);
			}
		}

		public Mesh()
		{
			GenerateBuffers();
		}
		public Mesh(IEnumerable<Vector2> points)
			:this()
		{
			AddVertices(points);
		}
		public Mesh(IEnumerable<Vector2> points, IEnumerable<Vector2> uvs)
			:this()
		{
			AddVertices(points, uvs);
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

		void AddVertices(IEnumerable<Vector2> vert)
		{
			Vector3[] v = Array.ConvertAll(vert.ToArray(), Vec2ToVec3);
			AddVertices(v);
		}
		void AddVertices(IEnumerable<Vector2> vert, IEnumerable<Vector2> uv)
		{
			Vector3[] v = Array.ConvertAll(vert.ToArray(), Vec2ToVec3);
			AddVertices(v, uv);
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
				vertexPosition.BindToAttribute(Program, "vertexPosition");
				vertexUV.BindToAttribute(Program, "vertexUV");
			} else
			{
				vertexPosition.UploadData(vertices.ToArray());
				vertexPosition.BindToAttribute(Program, "vertexPosition");
				vertexPosition.BindToAttribute(Program, "vertexUV");
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

		public void Draw() { Draw(PrimitiveType); }
        public void Draw(PrimitiveType pt)
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
					Program["tileSize"].SetValue(tileset.UVSize);
					Program["tilePosition"].SetValue(tileset.Position);
				}
				else Program["tiledTexture"].SetValue(false);
			}
			else Program["usingTexture"].SetValue(false);

			Program["color"].SetValue(Color);
			Program["fillColor"].SetValue(FillColor);

			GL.DrawArrays(pt, 0, vertices.Count);
		}
	}
}