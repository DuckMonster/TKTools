using OpenTK;
using System;
using TKTools.Mathematics;

namespace TKTools.Context
{
	public class Camera
	{
		internal static Camera activeCamera;

		Vector3 position;
		Vector3 target;

		bool orthogonal;

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

		public Camera(Vector3 pos, Vector3 tar)
		{
			position = pos;
			target = tar;
			projection = Matrix4.Identity;
			view = Matrix4.Identity;

			UpdateViewMatrix();
			UpdateProjection();
		}

		bool viewDirty = true;
		Matrix4 projection, view;

		public void Use()
		{
			activeCamera = this;
		}

		void UpdateViewMatrix()
		{
			view = Matrix4.LookAt(Position, Target, Vector3.UnitY);
		}
		internal void UpdateProjection()
		{
			float ratio = (float)Context.activeContext.Size.Height / Context.activeContext.Size.Width;

			if (Orthogonal)
				projection = Matrix4.CreateOrthographicOffCenter(-10f, 10f, -10f * ratio, 10f * ratio, 1f, 10f);
			else
				projection = Matrix4.CreatePerspectiveFieldOfView(TKMath.ToRadians(45f), 1f / ratio, 1f, 50f);
		}

		internal Matrix4 View
		{
			get
			{
				if (viewDirty) UpdateViewMatrix();
				return view;
			}
		}
		internal Matrix4 Projection
		{
			get { return projection; }
		}
	}
}