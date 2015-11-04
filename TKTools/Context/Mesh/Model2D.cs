using OpenTK;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using TKTools.Mathematics;

namespace TKTools.Context
{
	public class Model2D : Model
	{
		static Vector3 Vec2ToVec3(Vector2 v) { return new Vector3(v); }
		static Vector2 Vec3ToVec2(Vector3 v) { return v.Xy; }

		public new Vector2[] VertexPosition
		{
			get { return Array.ConvertAll(base.VertexPosition, new Converter<Vector3, Vector2>(Vec3ToVec2)); }
			set { base.VertexPosition = Array.ConvertAll(value, new Converter<Vector2, Vector3>(Vec2ToVec3)); }
		}

		public Model2D()
			: base()
		{
		}
		public Model2D(Vector2[] vertices)
			: this()
		{
			VertexPosition = vertices;
		}
		public Model2D(Vector2[] vertices, Vector2[] uv)
			: this()
		{
			VertexPosition = vertices;
			VertexUV = uv;
		}
	}
}