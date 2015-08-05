using OpenTK.Graphics.OpenGL;

namespace TKTools
{
	public class RenderBuffer
	{
		int bufferHandle;

		public RenderBuffer()
		{
			bufferHandle = GL.GenRenderbuffer();
		}

		internal void Bind()
		{
			GL.BindRenderbuffer(RenderbufferTarget.RenderbufferExt, bufferHandle);
		}

		internal void BindToFrameBuffer(FrameBuffer fb)
		{
			Bind();
			GL.RenderbufferStorage(RenderbufferTarget.RenderbufferExt, RenderbufferStorage.Depth24Stencil8, fb.Width, fb.Height);
			//GL.RenderbufferStorageMultisample(RenderbufferTarget.RenderbufferExt, 16, RenderbufferStorage.Depth24Stencil8, fb.Width, fb.Height);

			fb.Bind();
			GL.FramebufferRenderbuffer(FramebufferTarget.FramebufferExt, FramebufferAttachment.DepthStencilAttachment, RenderbufferTarget.RenderbufferExt, bufferHandle);
			fb.Release();
		}
	}
}