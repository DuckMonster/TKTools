using System;
using OpenTK.Graphics.OpenGL;

namespace TKTools
{
	public class VAO
	{
		public static VAO boundVBO;

		int vaoHandle;
		ShaderProgram program;

		public ShaderProgram Program
		{
			get { return program; }
			set { program = value; }
		}

		public VAO(ShaderProgram program)
		{
			InitHandle();
			this.program = program;
		}

		public void Dispose()
		{
			GL.DeleteVertexArray(vaoHandle);
		}

		void InitHandle()
		{
			vaoHandle = GL.GenVertexArray();
		}

		public void BindAttribute<T>(string attribute, VBO<T> vbo) where T : struct
		{
			Bind();

			int attr = program.GetAttribute(attribute);
			if (attr == -1) throw new NullReferenceException("Attribute \"" + attribute + "\" does not exist in the program!");

			vbo.Bind();
			GL.EnableVertexAttribArray(attr);
			GL.VertexAttribPointer(attr, vbo.Dimensions, VertexAttribPointerType.Float, false, 0, 0);

			Unbind();
			vbo.Unbind();
		}

		public void Bind()
		{
			GL.BindVertexArray(vaoHandle);
			boundVBO = this;
		}

		public void Unbind()
		{
			GL.BindVertexArray(0);
			boundVBO = null;
		}
	}
}