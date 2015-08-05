using OpenTK;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections;
using System.Collections.Generic;

namespace TKTools.Context
{
	public class Vertex
	{
		Mesh mesh;
		Dictionary<string, object> attributes = new Dictionary<string, object>();

		public Mesh Mesh { get { return mesh; } }

		public Vector3 Position
		{
			get { return GetAttribute<Vector3>("position"); }
			set { SetAttribute("position", value); }
		}

		public Vertex(Mesh mesh)
		{
			this.mesh = mesh;
		}

		public void SetAttribute<T>(string name, T value) where T : struct
		{
			if (attributes.ContainsKey(name))
				attributes[name] = value;
			else
				attributes.Add(name, value);
		}

		public T GetAttribute<T>(string name) where T : struct
		{
			if (attributes.ContainsKey(name))
				return (T)attributes[name];
			else
				return default(T);
		}
	}

	public class Mesh : IEnumerable, IDisposable
	{
		#region Attribute & Uniform
		public abstract class AttributeHelper : IDisposable
		{
			protected Mesh mesh;
			protected BufferUsageHint bufferHint = BufferUsageHint.StaticDraw;

			string name;
			public string Name
			{
				get { return name; }
			}

			public BufferUsageHint BufferHint
			{
				get { return bufferHint; }
				set { bufferHint = value; }
			}

			public AttributeHelper(string name, Mesh mesh)
			{
				this.name = name;
				this.mesh = mesh;
			}

			public abstract void Dispose();
			public abstract void UploadData();
		}
		public class AttributeHelper<T> : AttributeHelper where T : struct
		{
			VBO<T> vbo;

			public T[] Data
			{
				set { SetValue(value); }
				get { return GetValue(); }
			}

			public AttributeHelper(string name, Mesh mesh)
				: base(name, mesh)
			{
				vbo = new VBO<T>();
				mesh.vao.BindAttribute(Name, vbo);
			}

			public override void Dispose()
			{
				vbo.Dispose();
			}

			void SetValue(T[] value)
			{
				mesh.SetVertexCount(value.Length);
				for (int i = 0; i < value.Length; i++)
					mesh.Vertices[i].SetAttribute(Name, value[i]);

				UploadData();
			}

			T[] GetValue()
			{
				T[] list = new T[mesh.Vertices.Count];
				for (int i = 0; i < list.Length; i++)
					list[i] = mesh.Vertices[i].GetAttribute<T>(Name);

				return list;
			}

			public override void UploadData()
			{
				vbo.UploadData(GetValue(), BufferHint);
			}
		}
		public class UniformHelper
		{
			protected Mesh mesh;

			string name;
			public string Name
			{
				get { return name; }
			}

			object value;
			public object Value
			{
				get { return value; }
				set { this.value = value; }
			}

			public UniformHelper(Mesh mesh, string name)
			{
				this.mesh = mesh;
				this.name = name;
			}

			public void UploadData()
			{
				Type t = value.GetType();
				ShaderProgram.Uniform u = mesh.Program[name];

				switch (t.Name)
				{
					case "Single": u.SetValue((float)value); break;
					case "Vector2": u.SetValue((Vector2)value); break;
					case "Vector3": u.SetValue((Vector3)value); break;
					case "Vector4": u.SetValue((Vector4)value); break;
					case "Integer": u.SetValue((int)value); break;
					case "Boolean": u.SetValue((bool)value); break;
					case "Color": u.SetValue((Color)value); break;
					case "Matrix4": u.SetValue((Matrix4)value); break;
					default: throw new NotSupportedException();
				}
			}
		}
		#endregion

		#region Enum stuff
		class MeshEnum : IEnumerator
		{
			Mesh mesh;
			int index = -1;

			public MeshEnum(Mesh m)
			{
				this.mesh = m;
			}

			public void Reset() { index = -1; }
			public bool MoveNext()
			{
				index++;
				return mesh.vertices.Count < index;
			}

			public object Current
			{
				get { return mesh.vertices[index]; }
			}
		}
		public IEnumerator GetEnumerator() { return new MeshEnum(this); }
		#endregion

		List<Vertex> vertices = new List<Vertex>();
		Dictionary<string, AttributeHelper> attributes = new Dictionary<string, AttributeHelper>();
		Dictionary<string, UniformHelper> uniforms = new Dictionary<string, UniformHelper>();

		protected VAO vao;
		protected ShaderProgram program;

		public ShaderProgram Program
		{
			get { return program; }
		}

		public List<Vertex> Vertices
		{
			get { return vertices; }
		}

		public Mesh(ShaderProgram program)
		{
			this.program = program;
			vao = new VAO(program);
		}

		public void Dispose()
		{
			foreach (AttributeHelper attr in attributes.Values)
				attr.Dispose();

			attributes.Clear();
			uniforms.Clear();
			vertices.Clear();
		}

		public bool ContainsAttribute(string name) { return attributes.ContainsKey(name); }

		void SetVertexCount(int n)
		{
			if (vertices.Count > n)
				vertices.RemoveRange(n, vertices.Count - n);
			else if (vertices.Count < n)
				while (vertices.Count < n)
					vertices.Add(new Vertex(this));

			UploadAllAttributes();
		}

		public AttributeHelper<T> GetAttribute<T>(string name) where T : struct
		{
			if (attributes.ContainsKey(name))
				return attributes[name] as AttributeHelper<T>;
			else
			{
				AttributeHelper<T> newAttribute = new AttributeHelper<T>(name, this);
				attributes.Add(name, newAttribute);

				return newAttribute;
			}
		}

		void UploadAllAttributes()
		{
			foreach (AttributeHelper attribute in attributes.Values)
				attribute.UploadData();
		}

		public UniformHelper GetUniform(string name)
		{
			if (uniforms.ContainsKey(name))
				return uniforms[name];
			else
			{
				UniformHelper uni = new UniformHelper(this, name);
				uniforms.Add(name, uni);

				return uni;
			}
		}

		public UniformHelper this[string name]
		{
			get { return GetUniform(name); }
		}

		void UploadUniforms()
		{
			foreach (UniformHelper uniform in uniforms.Values)
				uniform.UploadData();
		}

		public virtual void Draw()
		{
			program.Use();
			UploadUniforms();

			vao.Bind();
			GL.DrawArrays(PrimitiveType.Quads, 0, vertices.Count);
			vao.Unbind();
		}
	}
}