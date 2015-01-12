using System;
using System.Collections.Generic;

using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace TKTools
{
	public class VBO<T> : IDisposable where T : struct
	{
		protected int vbo_ID;
		List<T> vertexList = new List<T>();
		int dimensions = 0;

		public T[] Array
		{
			get
			{
				return vertexList.ToArray();
			}
		}
		public int Count
		{
			get
			{
				return vertexList.Count;
			}
		}
		public int Dimensions
		{
			get
			{
				return dimensions;
			}
		}
		public int ByteSize
		{
			get
			{
				return dimensions * 4;
			}
		}

		public VBO()
		{
			vbo_ID = GL.GenBuffer();
		}

		public void Dispose()
		{
			GL.DeleteBuffer(vbo_ID);
		}

		public void Bind()
		{
			GL.BindBuffer(BufferTarget.ArrayBuffer, vbo_ID);
		}

		public void UploadData(T[] data)
		{
			vertexList.Clear();
			vertexList.AddRange(data);

			switch (typeof(T).Name)
			{
				case "Vector2": dimensions = 2; break;
				case "Vector3": dimensions = 3; break;
				case "Vector4": dimensions = 4; break;
			}

			Bind();
			GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(ByteSize * data.Length), data, BufferUsageHint.StaticDraw);
		}
	}
}