using System;
using OpenTK;
using System.Drawing;
using OpenTK.Graphics.OpenGL;

namespace TKTools.Context
{
	public delegate void UpdateEventHandler();
	public delegate void RenderEventHandler();

	public class Context : GameWindow
	{
		internal static Context activeContext;

		public event UpdateEventHandler OnUpdate;
		public event RenderEventHandler OnRender;

		public Context()
			:base()
		{
			activeContext = this;
			Mesh.CompileStandardShader();
		}

		protected override void OnResize(EventArgs e)
		{
			base.OnResize(e);

			GL.Viewport(ClientRectangle);
			if (Camera.activeCamera != null) Camera.activeCamera.UpdateProjection();
		}

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);
			GL.ClearColor(0f, 0f, 0f, 1f);
		}

		protected override void OnUpdateFrame(FrameEventArgs e)
		{
			base.OnUpdateFrame(e);
			OnUpdate();
		}

		protected override void OnRenderFrame(FrameEventArgs e)
		{
			base.OnRenderFrame(e);

			GL.Clear(ClearBufferMask.ColorBufferBit);
			OnRender();

			SwapBuffers();
		}
	}
}