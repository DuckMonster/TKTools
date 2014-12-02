using OpenTK;

namespace TKTools
{
	public class Color
	{
		#region Colors
		private static uint white = 0xFFFFFFFF, black = 0xFF000000, gray = 0xFF808080,
			red = 0xFFFF0000, green = 0xFF00FF00, blue = 0xFF0000FF,
			yellow = 0xFFFFFF00, aqua = 0xFF00FFFF, teal = 0xFF008080, pink = 0xFFFF00FF,
			purple = 0xFF800080, orange = 0xFFFFA500, violet = 0xFFEE82EE;
		public static Color White { get { return new Color(white); } }
		public static Color Black { get { return new Color(black); } }
		public static Color Gray { get { return new Color(gray); } }
		public static Color Red { get { return new Color(red); } }
		public static Color Green { get { return new Color(green); } }
		public static Color Blue { get { return new Color(blue); } }
		public static Color Yellow { get { return new Color(yellow); } }
		public static Color Aqua { get { return new Color(aqua); } }
		public static Color Teal { get { return new Color(teal); } }
		public static Color Pink { get { return new Color(pink); } }
		public static Color Purple { get { return new Color(purple); } }
		public static Color Orange { get { return new Color(orange); } }
		public static Color Violet { get { return new Color(violet); } }
		#endregion

		private float r, g, b, a;

		public float R
		{
			set
			{
				r = Clamp(value);
			}
			get
			{
				return r;
			}
		}
		public float G
		{
			set
			{
				g = Clamp(value);
			}
			get
			{
				return g;
			}
		}
		public float B
		{
			set
			{
				b = Clamp(value);
			}
			get
			{
				return b;
			}
		}
		public float A
		{
			set
			{
				a = Clamp(value);
			}
			get
			{
				return a;
			}
		}

		public Vector4 ToVector
		{
			get
			{
				return new Vector4(R, G, B, A);
			}
		}

		public Color(float r, float g, float b, float a = 1f)
		{
			R = r;
			G = g;
			B = b;
			A = a;
		}

		public Color(uint argb)
		{
			uint r = (argb >> 16) & 0xFF;
			uint g = (argb >> 8) & 0xFF;
			uint b = argb & 0xFF;
			uint a = (argb >> 24) & 0xFF;

			R = (float)r / 255;
			G = (float)g / 255;
			B = (float)b / 255;
			A = (float)a / 255;
		}

		public Color(int r, int g, int b, int a = 255)
		{
			R = (float)r / 255;
			G = (float)g / 255;
			B = (float)b / 255;
			A = (float)a / 255;
		}

		public Color(Color c)
		{
			R = c.R;
			G = c.G;
			B = c.B;
			A = c.A;
		}

		public static Color Blend(Color c1, Color c2, float v)
		{
			v = Clamp(v);

			float r = c1.r + (c2.r - c1.r) * v,
					g = c1.g + (c2.g - c1.g) * v,
					b = c1.b + (c2.b - c1.b) * v,
					a = c1.a + (c2.a - c1.a) * v;

			return new Color(r, g, b, a);
		}

		public static Color operator +(Color a, Color b)
		{
			return new Color(a.r + b.r, a.g + b.g, a.b + b.b, a.a + b.a);
		}

		public static Color operator -(Color a, Color b)
		{
			return new Color(a.r - b.r, a.g - b.g, a.b - b.b, a.a - b.a);
		}

		public static Color operator *(Color a, Color b)
		{
			return new Color(a.r * b.r, a.g * b.g, a.b * b.b, a.a * b.a);
		}

		public static Color operator *(Color a, float f)
		{
			return new Color(a.r * f, a.g * f, a.b * f, a.a * f);
		}

		public static float Clamp(float v)
		{
			return (v < 0 ? 0 : (v > 1 ? 1 : v));
		}
	}
}