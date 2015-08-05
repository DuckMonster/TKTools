using System;
using System.Collections.Generic;

using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace TKTools
{
	public abstract class VBO : IDisposable
	{
		int vboHandle;

		public abstract int Dimensions{ get; }

		public VBO()
		{
			GenerateBuffer();
		}

		public void Dispose()
		{
			GL.DeleteBuffer(vboHandle);
		}

		void GenerateBuffer()
		{
			vboHandle = GL.GenBuffer();
		}

		public void Bind()
		{
			GL.BindBuffer(BufferTarget.ArrayBuffer, vboHandle);
		}

		public void Unbind()
		{
			GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
		}
	}

	public class VBO<T> : VBO where T : struct
	{
		//protected int vboHandle;
		List<T> vertexList = new List<T>();
		int dimensions = 0;

		public T[] Array
		{
			get
			{
				return vertexList.ToArray();
			}
		}
		public override int Dimensions
		{
			get
			{
				return dimensions;
			}
		}
		int ByteSize
		{
			get
			{
				return dimensions * sizeof(float);
			}
		}

		public VBO()
			:base()
		{
			switch (typeof(T).Name)
			{
				case "Single": dimensions = 1; break;
				case "Vector2": dimensions = 2; break;
				case "Vector3": dimensions = 3; break;
				case "Vector4": dimensions = 4; break;
				default: throw new NotSupportedException();
			}
		}

		public void UploadData(T[] data, BufferUsageHint hint = BufferUsageHint.StaticDraw)
		{
			vertexList.Clear();
			vertexList.AddRange(data);

			Bind();
			GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(ByteSize * data.Length), data, hint);
			Unbind();
		}
	}
}