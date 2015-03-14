using OpenTK.Graphics.OpenGL;

namespace TKTools
{
	class Mesh
	{
		int vao;

		public Mesh()
		{

		}

		public void GenerateVAO()
		{
			vao = GL.GenVertexArray();

			GL.BindVertexArray(vao);
		}
	}
}
