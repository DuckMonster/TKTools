﻿using OpenTK;
using OpenTK.Graphics.OpenGL;
using System;
using TKTools.Mathematics;
using TKTools.Context.Filter;
using System.Collections.Generic;

namespace TKTools.Context
{
	public class Camera
	{
		internal static Camera activeCamera;

		Vector3 position;
		Vector3 target;

		bool orthogonal;

		FrameBuffer frameBuffer;
		List<Filter.Filter> filterList = new List<Filter.Filter>();

		float ratio;
		float fieldOfView = 45f;
		float farPlane = 1000f;

		public Vector3 Position
		{
			get { return position; }
			set { position = value; viewDirty = true; }
		}

		public Vector3 Target
		{
			get { return target; }
			set { target = value; viewDirty = true; }
		}

		public bool Orthogonal
		{
			get { return orthogonal; }
			set
			{
				orthogonal = value;
				UpdateProjection();
			}
		}

		internal float Ratio
		{
			get { return ratio; }
		}

		public float FarPlane
		{
			get { return farPlane; }
			set { farPlane = value; }
		}

		public Camera() : this(new Vector3(0, 0, 2f), Vector3.Zero) { }
		public Camera(Vector3 pos, Vector3 tar)
		{
			position = pos;
			target = tar;
			projection = Matrix4.Identity;
			view = Matrix4.Identity;

			UpdateViewMatrix();
			UpdateProjection();

			frameBuffer = new FrameBuffer(new Texture(), new RenderBuffer());
		}

		bool viewDirty = true;
		Matrix4 projection, view;

		public Ray GetRayFromScreen(Vector2 screenPos)
		{
			if (screenPos.X < -1 || screenPos.X > 1
				|| screenPos.Y < -1 || screenPos.Y > 1)
				return new Ray(Vector3.Zero, Vector3.Zero);

			if (orthogonal)
			{
				Vector3 r;

				r = new Vector3(0, 0, -1);
				return new Ray(Position + new Vector3(screenPos.X * 5 * ratio, screenPos.Y * 5, 0), r);
			}
			else
			{
				Vector3 r;

				float zPlane = 1f / (float)Math.Tan(TKMath.ToRadians(fieldOfView) / 2);
				r = new Vector3(screenPos.X * Ratio, screenPos.Y, -zPlane);

				return new Ray(Position + r, r.Normalized());
			}
		}

		public void AddFilter(Filter.Filter filter)
		{
			filterList.Add(filter);
		}

		public void RemoveFilter(Filter.Filter filter)
		{
			filterList.Remove(filter);
		}

		public T GetFilter<T>() where T : Filter.Filter
		{
			foreach (Filter.Filter f in filterList)
				if (f is T) return (f as T);

			return null;
		}

		public void Use()
		{
			if (activeCamera == this) return;

			Camera a = activeCamera;

			UpdateProjection();

			activeCamera = this;
			ClearBuffer();
			BindBuffer();

			if (a != null) a.Draw(frameBuffer);
		}

		internal void ClearBuffer()
		{
			frameBuffer.Bind();
			GL.Clear(ClearBufferMask.DepthBufferBit | ClearBufferMask.ColorBufferBit);
			frameBuffer.Release();
		}

		internal void BindBuffer()
		{
			frameBuffer.Bind();
		}

		void UpdateViewMatrix()
		{
			view = Matrix4.LookAt(Position, Target, Vector3.UnitY);
		}
		internal void UpdateProjection()
		{
			ratio = Context.activeContext.AspectRatio;

			if (Orthogonal)
				projection = Matrix4.CreateOrthographicOffCenter(-5f * ratio, 5f * ratio, -5f, 5f, 1f, farPlane);
			else
				projection = Matrix4.CreatePerspectiveFieldOfView(TKMath.ToRadians(45f), ratio, 1f, farPlane);
		}

		public Matrix4 View
		{
			get
			{
				if (viewDirty) UpdateViewMatrix();
				return view;
			}
		}
		public Matrix4 Projection
		{
			get { return projection; }
		}

		public Matrix4 ViewProjection
		{
			get { return Projection * View; }
		}

		internal void Draw(FrameBuffer fb = null)
		{
			Texture finalTexture = frameBuffer.Texture;

			foreach (Filter.Filter f in filterList)
			{
				f.Apply(finalTexture);
				finalTexture = f.OutputTexture;
			}

			if (fb == null)
				FrameBuffer.ReleaseActive();
			else
				fb.Bind();

			finalTexture.DrawToScreen();
		}
	}
}