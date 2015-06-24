using System;
using System.Collections.Generic;
using System.IO;

using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace TKTools
{
	public class ShaderProgram : IDisposable
	{
		public struct Argument
		{
			ShaderProgram program;
			int ID;

			public Argument(int ID, ShaderProgram prog)
			{
				program = prog;
				this.ID = ID;
			}

			public void SetValue<T>(VBO<T> vbo) where T : struct
			{
				GL.UseProgram(program.programID);
				vbo.Bind();
				GL.EnableVertexAttribArray(ID);
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
			public void SetValue(float[] val)
			{
				GL.UseProgram(program.programID);
				GL.Uniform1(ID, val.Length, val);
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

		string vertexLog, fragmentLog, programLog;

		public string Log
		{
			get
			{
				return vertexLog + "\n" + fragmentLog + "\n" + programLog;
			}
		}

		Dictionary<string, int> attributeList = new Dictionary<string, int>();
		Dictionary<string, int> uniformList = new Dictionary<string, int>();
		public int programID;

		public ShaderProgram(string vertexSource, string fragmentSource)
		{
			int vertexID = CreateShader(ShaderType.VertexShader, vertexSource, ref vertexLog);
			int fragmentID = CreateShader(ShaderType.FragmentShader, fragmentSource, ref fragmentLog);
			programID = CreateProgram(vertexID, fragmentID, ref programLog);
		}

		public ShaderProgram(string source)
		{
			string vertexSource = "", fragmentSource = "";
			ReadFileCombined(source, ref vertexSource, ref fragmentSource);

			int vertexID = CreateShader(ShaderType.VertexShader, vertexSource, ref vertexLog);
			int fragmentID = CreateShader(ShaderType.FragmentShader, fragmentSource, ref fragmentLog);
			programID = CreateProgram(vertexID, fragmentID, ref programLog);
		}

		public void Dispose()
		{
			GL.DeleteProgram(programID);
		}

		static int CreateShader(ShaderType type, string src, ref string log)
		{
			int id = GL.CreateShader(type);
			GL.ShaderSource(id, src);
			GL.CompileShader(id);

			log = GL.GetShaderInfoLog(id);

			return id;
		}

		static int CreateProgram(int vertex, int fragment, ref string log)
		{
			int id = GL.CreateProgram();
			GL.AttachShader(id, vertex);
			GL.AttachShader(id, fragment);
			GL.LinkProgram(id);

			log = GL.GetShaderInfoLog(id);

			GL.DetachShader(id, vertex);
			GL.DetachShader(id, fragment);
			GL.DeleteShader(vertex);
			GL.DeleteShader(fragment);

			return id;
		}

		public void SetAttribute(string name, int ID)
		{
			GL.BindAttribLocation(programID, ID, name);
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

				if (uni != -1) return new Argument(uni, this);
				else if (attr != -1) return new Argument(attr, this);
				else throw new NullReferenceException("Uniform/Attribute \"" + name + "\" does not exist in this program!");
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