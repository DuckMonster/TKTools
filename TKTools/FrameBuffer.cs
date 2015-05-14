using OpenTK;
using OpenTK.Graphics.OpenGL;
using System.Collections.Generic;
using TKTools.Context;

namespace TKTools
{
	public class FrameBuffer
	{
		internal static FrameBuffer activeBuffer;
		internal static void ReleaseActive()
		{
			if (activeBuffer != null)
				activeBuffer.Release();
		}

		internal static void ContextSizeChanged()
		{
			foreach (FrameBuffer b in bufferList)
				b.UpdateSize();
		}

		private static List<FrameBuffer> bufferList = new List<FrameBuffer>();

		int width, height;

		public int Width { get { return width; } }
		public int Height { get { return height; } }

		int bufferHandle;

		Texture texture;
		public Texture Texture
		{
			get { return texture; }
			set
			{
				texture = value;
				if (texture != null)
					texture.BindToFrameBuffer(this);
			}
		}

		RenderBuffer renderBuffer;
		internal RenderBuffer RenderBuffer
		{
			get { return renderBuffer; }
			set
			{
				renderBuffer = value;
				if (renderBuffer != null)
					renderBuffer.BindToFrameBuffer(this);
			}
		}

		public FrameBuffer() : this(null, null) { }
		public FrameBuffer(Texture t) : this(t, null) { }
		public FrameBuffer(Texture t, RenderBuffer rb)
		{
			UpdateSize();

			bufferHandle = GL.GenFramebuffer();

			Texture = t;
			RenderBuffer = rb;

			bufferList.Add(this);
		}

		void UpdateSize()
		{
			this.width = Context.Context.activeContext.ClientSize.Width;
			this.height = Context.Context.activeContext.ClientSize.Height;

			if (Texture != null) Texture.BindToFrameBuffer(this);
			if (RenderBuffer != null) RenderBuffer.BindToFrameBuffer(this);
		}

		public void Dispose()
		{
			GL.DeleteFramebuffer(bufferHandle);
			texture.Dispose();

			bufferList.Remove(this);
		}

		public void Bind()
		{
			GL.BindFramebuffer(FramebufferTarget.FramebufferExt, bufferHandle);
			activeBuffer = this;
		}

		public void Release()
		{
			GL.BindFramebuffer(FramebufferTarget.FramebufferExt, 0);
			activeBuffer = null;
		}

		public void Draw()
		{
			GL.Disable(EnableCap.DepthTest);

			Release();
			Texture.DrawToScreen();
		}
	}
}