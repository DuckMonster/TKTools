using OpenTK;
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
				return new Ray(Position + new Vector3(screenPos.X * 4 * ratio, screenPos.Y * 4, 0), r);
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

			UpdateProjection();

			activeCamera = this;
			ClearBuffer();
			BindBuffer();
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
				projection = Matrix4.CreateOrthographicOffCenter(-4f * ratio, 4f * ratio, -4f, 4f, 1f, 10f);
			else
			{
				float v = (float)Math.Tan(TKMath.ToRadians(45f) / 2);
				float n = 1f;
				float f = 50f;

				projection = new Matrix4(
					1 / (ratio * v), 0f, 0f, 0f,
					0f, 1 / v, 0f, 0f,
					0f, 0f, (-n - f)/-(n - f), (2 * f * n) / (n - f),
					0f, 0f, -1f, 0f
					);

				projection.Transpose();

				//projection = Matrix4.CreatePerspectiveFieldOfView(TKMath.ToRadians(45f), ratio, 1f, 50f);
			}
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

		internal void Draw()
		{
			Texture finalTexture = frameBuffer.Texture;

			foreach(Filter.Filter f in filterList)
			{
				f.Apply(finalTexture);
				finalTexture = f.OutputTexture;
			}

			FrameBuffer.ReleaseActive();
			finalTexture.DrawToScreen();
		}
	}
}