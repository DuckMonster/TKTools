using OpenTK.Graphics.OpenGL;

namespace TKTools
{
	public class FrameBuffer
	{
		int width, height;
		Texture texture;


		public int Width { get { return width; } }
		public int Height { get { return height; } }

		int frameBufferID;
		int stencilBufferID;

		public Texture Texture
		{
			get { return texture; }
		}

		public FrameBuffer(int width, int height)
		{
			this.width = width;
			this.height = height;

			frameBufferID = GL.GenFramebuffer();
			texture = new Texture();
			texture.BindToFrameBuffer(this);
			GenStencil();
		}

		void GenStencil()
		{
			stencilBufferID = GL.GenRenderbuffer();

			GL.BindRenderbuffer(RenderbufferTarget.RenderbufferExt, stencilBufferID);
			GL.RenderbufferStorage(RenderbufferTarget.RenderbufferExt, RenderbufferStorage.Depth24Stencil8, width, height);

			GL.BindFramebuffer(FramebufferTarget.FramebufferExt, frameBufferID);
			GL.FramebufferRenderbuffer(FramebufferTarget.FramebufferExt, FramebufferAttachment.DepthStencilAttachment, RenderbufferTarget.RenderbufferExt, stencilBufferID);
			GL.BindFramebuffer(FramebufferTarget.FramebufferExt, 0);
		}

		public void Dispose()
		{
			GL.DeleteFramebuffer(frameBufferID);
			texture.Dispose();
		}

		public void Bind()
		{
			GL.BindRenderbuffer(RenderbufferTarget.RenderbufferExt, stencilBufferID);
			GL.BindFramebuffer(FramebufferTarget.FramebufferExt, frameBufferID);
		}

		public void Release()
		{
			GL.BindFramebuffer(FramebufferTarget.FramebufferExt, 0);
		}
	}
}