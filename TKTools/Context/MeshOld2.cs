using OpenTK;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;

namespace TKTools.Context
{
	public abstract class Attribute
	{
		protected string name;

		public string Name
		{
			get { return name; }
		}

		public abstract Type Type
		{
			get;
		}

		public Attribute(string name)
		{
			this.name = name;
		}

		public abstract void Dispose();
	}
	public class Attribute<T> : Attribute where T : struct
	{
		VBO<T> vbo;
		List<T> data = new List<T>();

		internal VBO<T> VBO
		{
			get { return vbo; }
		}

		public override Type Type
		{
			get
			{
				return typeof(T);
			}
		}

		public T[] Data
		{
			get { return data.ToArray(); }
			set
			{
				data.Clear();
				data.AddRange(value);
				vbo.UploadData(value);
			}
		}

		public Attribute(string name, VAO vao, ShaderProgram program)
			: base(name)
		{
			vbo = new VBO<T>();
			vao.BindAttribute(name, vbo);
		}

		public override void Dispose()
		{
			vbo.Dispose();
		}

		void UpdateVBO()
		{
			vbo.UploadData(data.ToArray());
		}

		public void AddVertex(T data)
		{
			this.data.Add(data);
			UpdateVBO();
		}

		public void AddVertex(IList<T> data)
		{
			this.data.AddRange(data);
			UpdateVBO();
		}
	}

	public class Vertex
	{
		Mesh mesh;
		Dictionary<string, object> attributeData = new Dictionary<string, object>();

		public Vertex(Mesh mesh)
		{
			this.mesh = mesh;
		}

		public void SetAttribute<T>(string name, T data) where T : struct
		{
			if (!attributeData.ContainsKey(name))
				attributeData.Add(name, data);
			else
				attributeData[name] = data;
		}

		public T GetAttribute<T>(string name) where T : struct
		{
			if (!attributeData.ContainsKey(name))
				return default(T);
			else
				return (T)attributeData[name];
		}
    }

	public class Mesh
	{
		protected List<Vertex> vertices = new List<Vertex>();
		internal Dictionary<string, Attribute> attributes = new Dictionary<string, Attribute>();

		VAO vao;
		ShaderProgram shaderProgram;
		Matrix4 modelMatrix = Matrix4.Identity;

		public Vertex[] Vertices
		{
			get { return vertices.ToArray(); }
		}

		public ShaderProgram ShaderProgram
		{
			get { return shaderProgram; }
			set
			{
				shaderProgram = value;
				vao.Program = value;
			}
		}

		public Matrix4 ModelMatrix
		{
			get { return modelMatrix; }
			set { modelMatrix = value; }
		}

		public Mesh(ShaderProgram program)
		{
			vao = new VAO(program);
			shaderProgram = program;
		}

		public Attribute<T> AddAttribute<T>(string name) where T : struct
		{
			Attribute<T> attr = new Attribute<T>(name, vao, ShaderProgram);
			attributes.Add(name, attr);

			return attr;
		}
		public Attribute<T> AddAttribute<T>(string name, T[] data) where T : struct
		{
			Attribute<T> attr = AddAttribute<T>(name);
			attr.Data = data;

			return attr;
		}

		public Attribute<T> GetAttribute<T>(string name) where T : struct
		{
			return attributes[name] as Attribute<T>;
		}

		public void RemoveAttribute<T>(string name) where T : struct
		{
			if (!attributes.ContainsKey(name)) return;

			attributes[name].Dispose();
			attributes.Remove(name);
		}

		protected T[] GetAttributeArray<T>(string name) where T : struct
		{
			T[] arr = new T[vertices.Count];

			for (int i = 0; i < vertices.Count; i++)
				arr[i] = vertices[i].GetAttribute<T>(name);

			return arr;
		}

		protected void UploadAttributes()
		{
			foreach(Attribute a in attributes.Values)
			{
				Type t = a.Type;
				switch(t.Name)
				{
					case "Vector2": (a as Attribute<Vector2>).Data = GetAttributeArray<Vector2>(a.Name); break;
					case "Vector3": (a as Attribute<Vector3>).Data = GetAttributeArray<Vector3>(a.Name); break;
					case "Vector4": (a as Attribute<Vector4>).Data = GetAttributeArray<Vector4>(a.Name); break;
					default: throw new NotSupportedException();
				}
			}
		}

		public virtual void UploadUniforms()
		{
		}

		public void Draw()
		{
			ShaderProgram.Use();

			vao.Bind();

			UploadUniforms();

			GL.DrawArrays(PrimitiveType.Polygon, 0, 4);

			vao.Unbind();
		}
	}
}