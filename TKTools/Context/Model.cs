using OpenTK;
using System.Collections.Generic;
using TKTools.Mathematics;

namespace TKTools.Context
{
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

void main() {
	gl_Position = projection * view * model * vec4(vertexPosition, 1.0);
}";

		const string fragmentSource =
					@"
#version 330
uniform vec4 color;
out vec4 fragment;

void main() {
	fragment = color;
}";

		internal static ShaderProgram StandardShader;
		internal static void CompileStandardShader()
		{
			StandardShader = new ShaderProgram(vertexSource, fragmentSource);
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

		public Model()
			: base(StandardShader)
		{
			GetAttribute<Vector3>("vertexPosition");
			//GetAttribute<Vector3>("vertexUV");
		}
		public Model(Vector3[] vertices)
			:this()
		{
			VertexPosition = vertices;
		}
		public Model(Vector3[] vertices, Vector2[] uv)
			:this()
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

		public override void Draw()
		{
			this["projection"].Value = Camera.activeCamera.Projection;
			this["view"].Value = Camera.activeCamera.View;
			this["model"].Value = ModelMatrix;

			base.Draw();
		}
	}
}