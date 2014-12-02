using System;
using System.Collections.Generic;
using System.IO;

using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace TKTools
{
	public class ShaderProgram : IDisposable
	{
		public enum ArgumentType
		{
			Uniform,
			Attribute
		}

		public struct Argument
		{
			ShaderProgram program;
			ArgumentType type;
			int ID;

			public Argument(ArgumentType type, int ID, ShaderProgram prog)
			{
				program = prog;
				this.type = type;
				this.ID = ID;
			}

			public void SetValue<T>(VBO<T> vbo) where T : struct
			{
				GL.UseProgram(program.programID);
				vbo.Bind();
				GL.VertexAttribPointer(ID, vbo.Dimensions, VertexAttribPointerType.Float, false, 0, 0);
			}

			public void SetValue(bool val)
			{
				GL.UseProgram(program.programID);
				GL.Uniform1(ID, val ? 1 : 0);
			}
			public void SetValue(float val)
			{
				GL.UseProgram(program.programID);
				GL.Uniform1(ID, val);
			}
			public void SetValue(double val)
			{
				GL.UseProgram(program.programID);
				GL.Uniform1(ID, val);
			}
			public void SetValue(int val)
			{
				GL.UseProgram(program.programID);
				GL.Uniform1(ID, val);
			}

			public void SetValue(Vector2 vec)
			{
				GL.UseProgram(program.programID);
				GL.Uniform2(ID, ref vec);
			}

			public void SetValue(Vector3 vec)
			{
				GL.UseProgram(program.programID);
				GL.Uniform3(ID, ref vec);
			}

			public void SetValue(Vector4 vec)
			{
				GL.UseProgram(program.programID);
				GL.Uniform4(ID, ref vec);
			}

			public void SetValue(Color c)
			{
				SetValue(c.ToVector);
			}

			public void SetValue(Matrix4 mat)
			{
				GL.UseProgram(program.programID);
				GL.UniformMatrix4(ID, false, ref mat);
			}
		}

		Dictionary<string, int> attributeList = new Dictionary<string, int>();
		Dictionary<string, int> uniformList = new Dictionary<string, int>();
		public int programID;

		public ShaderProgram(string source)
		{
			string vertexSrc = "", fragmentSrc = "";
			ReadFileCombined(source, ref vertexSrc, ref fragmentSrc);

			int vertexID = CreateShader(ShaderType.VertexShader, vertexSrc);
			int fragmentID = CreateShader(ShaderType.FragmentShader, fragmentSrc);
			programID = CreateProgram(vertexID, fragmentID);
		}

		public void Dispose()
		{
			GL.DeleteProgram(programID);
		}

		static int CreateShader(ShaderType type, string src)
		{
			int id = GL.CreateShader(type);
			GL.ShaderSource(id, src);
			GL.CompileShader(id);

			Console.WriteLine("Shader log:");
			Console.WriteLine(GL.GetShaderInfoLog(id));

			return id;
		}

		static int CreateProgram(int vertex, int fragment)
		{
			int id = GL.CreateProgram();
			GL.AttachShader(id, vertex);
			GL.AttachShader(id, fragment);
			GL.LinkProgram(id);

			Console.WriteLine("Program log:");
			Console.WriteLine(GL.GetProgramInfoLog(id));

			GL.DetachShader(id, vertex);
			GL.DetachShader(id, fragment);
			GL.DeleteShader(vertex);
			GL.DeleteShader(fragment);

			return id;
		}

		public int GetAttribute(string name)
		{
			if (attributeList.ContainsKey(name)) return attributeList[name];

			int attrib = GL.GetAttribLocation(programID, name);
			if (attrib != -1)
			{
				attributeList.Add(name, attrib);
			}

			return attrib;
		}

		public int GetUniform(string name)
		{
			if (uniformList.ContainsKey(name)) return uniformList[name];

			int uni = GL.GetUniformLocation(programID, name);
			if (uni != -1)
			{
				uniformList.Add(name, uni);
			}

			return uni;
		}

		public void BindVBO(int vbo, string attr)
		{
			int att = GetAttribute(attr);
			GL.VertexAttribPointer(att, 3, VertexAttribPointerType.Float, false, 0, 0);
		}

		public void Use()
		{
			GL.UseProgram(programID);
			foreach (KeyValuePair<string, int> entry in attributeList)
			{
				GL.EnableVertexAttribArray(entry.Value);
			}
		}

		public void Clean()
		{
			foreach (KeyValuePair<string, int> entry in attributeList)
			{
				GL.DisableVertexAttribArray(entry.Value);
			}
		}

		public Argument this[string name]
		{
			get
			{
				int uni = GetUniform(name), attr = GetAttribute(name);

				if (uni != -1) return new Argument(ArgumentType.Uniform, uni, this);
				else if (attr != -1) return new Argument(ArgumentType.Attribute, attr, this);
				else throw new NullReferenceException();
			}
		}

		public static void ReadFileCombined(string filename, ref string vertexSrc, ref string fragmentSrc)
		{
			using (StreamReader reader = new StreamReader(filename))
			{
				string head = "";
				string src = "";
				string line = "";

				while (!reader.EndOfStream)
				{
					line = reader.ReadLine();

					if (head == "")
					{
						if (line.StartsWith("@")) head = line;
					}
					else
					{
						if (line.StartsWith("@"))
						{
							switch (head[1])
							{
								case 'v':
									vertexSrc = src;
									break;

								case 'f':
									fragmentSrc = src;
									break;
							}

							head = "";
							src = "";
						}
						else src += line + "\n";
					}
				}
			}
		}
	}
}