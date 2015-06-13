﻿using System;
using OpenTK;
using System.Drawing;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;
using GX = OpenTK.Graphics;

namespace TKTools.Context
{
	public delegate void BeginEventHandler();
	public delegate void UpdateEventHandler();
	public delegate void RenderEventHandler();

	public class Context : GameWindow
	{
		internal static Context activeContext;

		public event BeginEventHandler OnBegin;
		public event UpdateEventHandler OnUpdate;
		public event RenderEventHandler OnRender;

		public float AspectRatio
		{
			get { return (float)Size.Width / Size.Height; }
		}

		public Context(int width, int height, GX.GraphicsMode gm)
			:base(width, height, gm)
		{
			activeContext = this;
			Mesh.CompileStandardShader();
			Texture.InitVAO();
		}
		public Context()
			:base()
		{
			activeContext = this;
			Mesh.CompileStandardShader();
			Texture.InitVAO();
		}

		protected override void OnResize(EventArgs e)
		{
			base.OnResize(e);

			GL.Viewport(ClientRectangle);
			if (Camera.activeCamera != null) Camera.activeCamera.UpdateProjection();
			FrameBuffer.ContextSizeChanged();
		}

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);
			GL.ClearColor(0f, 0f, 0f, 1f);
			GL.Enable(EnableCap.Blend);
			GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);

			OnBegin();
		}

		int mouseX, mouseY;
		protected override void OnMouseMove(MouseMoveEventArgs e)
		{
			mouseX = e.X;
			mouseY = e.Y;
		}

		protected override void OnUpdateFrame(FrameEventArgs e)
		{
			base.OnUpdateFrame(e);
			Input.Keyboard.Update();
			Input.Mouse.Update(mouseX, mouseY, this);
			OnUpdate();
		}

		protected override void OnRenderFrame(FrameEventArgs e)
		{
			base.OnRenderFrame(e);

			GL.Clear(ClearBufferMask.ColorBufferBit);

			Camera.activeCamera.ClearBuffer();
			Camera.activeCamera.BindBuffer();

			OnRender();

			Camera.activeCamera.Draw();

			SwapBuffers();
		}
	}
}
