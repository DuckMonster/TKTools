using OpenTK;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;

namespace TKTools.Context
{
	class Mesh
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

in vec2 uv;
out vec4 fragment;

void main() {
	vec4 finalColor;

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
	}
	public class Mesh<T> where T : struct
	{
		List<T> vertices = new List<T>();

		int vao;
		VBO<T> vertexPosition, vertexUV;

		ShaderProgram Program
		{
			get { return Mesh.StandardShader; }
		}

		public Mesh()
		{
			GenerateBuffers();
		}
		public Mesh(IEnumerable<T> points)
			:this()
		{
			AddVertices(points);
		}

		void GenerateBuffers()
		{
			vao = GL.GenVertexArray();
			vertexPosition = new VBO<T>();
			vertexUV = new VBO<T>();
		}

		void AddVertices(IEnumerable<T> vert)
		{
			vertices.AddRange(vert);
			UploadVertices();
		}

		void BindVAO()
		{
			GL.BindVertexArray(vao);
		}

		void UploadVertices()
		{
			BindVAO();
			vertexPosition.UploadData(vertices.ToArray());
			vertexPosition.BindToAttribute(Mesh.VERTEX_POSITION_ID);
			vertexPosition.BindToAttribute(Mesh.VERTEX_UV_ID);
		}

		public void Draw()
		{
			BindVAO();

			Program["projection"].SetValue(Camera.activeCamera.Projection);
			Program["view"].SetValue(Camera.activeCamera.View);
			Program["model"].SetValue(Matrix4.Identity);

			Program["usingTexture"].SetValue(false);
			Program["color"].SetValue(Color.White);
			Program["fillColor"].SetValue(true);

			GL.DrawArrays(PrimitiveType.Polygon, 0, 4);
		}
	}
}