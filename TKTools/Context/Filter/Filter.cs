using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace TKTools.Context.Filter
{
	public class Filter
	{
		private const string tempVertex = @"
#version 330

in vec2 vertexPosition;
in vec2 vertexUV;

out vec2 uv;

void main() {
	uv = vertexUV;
	gl_Position = vec4(vertexPosition, 0.0, 1.0);
}
";
		private const string tempFragment = @"
#version 330

uniform sampler2D texture;

in vec2 uv;
out vec4 fragment;

void main() {
	fragment = texture2D(texture, uv);
}
";

		protected ShaderProgram program;
		protected FrameBuffer frameBuffer;

		Texture outTexture;

		public Texture OutputTexture
		{
			get { return outTexture; }
		}

		public Filter()
		{
			program = new ShaderProgram(tempVertex, tempFragment);
			outTexture = new Texture();
			frameBuffer = new FrameBuffer(outTexture);
		}
		public Filter(string fragment)
		{
			program = new ShaderProgram(tempVertex, fragment);
			outTexture = new Texture();
			frameBuffer = new FrameBuffer(outTexture);
		}

		public virtual void Apply(Texture t)
		{
			FrameBuffer activeBuffer = FrameBuffer.activeBuffer;
			frameBuffer.Bind();

			t.DrawToScreen(program);

			if (activeBuffer == null)
				frameBuffer.Release();
			else
				activeBuffer.Bind();
		}
	}
}