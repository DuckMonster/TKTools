using OpenTK.Graphics.OpenGL;

namespace TKTools
{
	public class VAO
	{
		int vaoHandle;

		public VAO()
		{
			InitHandle();
		}

		public void Dispose()
		{
			GL.DeleteVertexArray(vaoHandle);
		}

		void InitHandle()
		{
			vaoHandle = GL.GenVertexArray();
		}

		public void Bind()
		{
			GL.BindVertexArray(vaoHandle);
		}
	}
}