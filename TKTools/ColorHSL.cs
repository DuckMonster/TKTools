using OpenTK;
using System;
using TKTools.Mathematics;
using DRAW = System.Drawing;

namespace TKTools
{
	public struct ColorHSL
	{
		#region Colors
		public static ColorHSL White { get { return new ColorHSL(0f, 0f, 1f); } }
		public static ColorHSL Black { get { return new ColorHSL(0f, 0f, 0f); } }
		public static ColorHSL Gray { get { return new ColorHSL(0f, 0f, 0.5f); } }
		public static ColorHSL Red { get { return new ColorHSL(0f, 1f, 0.5f); } }
		public static ColorHSL Green { get { return new ColorHSL(120f, 1f, 0.5f); } }
		public static ColorHSL Blue { get { return new ColorHSL(240f, 1f, 0.5f); } }
		public static ColorHSL Yellow { get { return new ColorHSL(60f, 1f, 0.5f); } }
		public static ColorHSL Aqua { get { return new ColorHSL(210f, 1f, 0.5f); } }
		public static ColorHSL Teal { get { return new ColorHSL(160f, 1f, 0.5f); } }
		public static ColorHSL Pink { get { return new ColorHSL(300f, 1f, 0.5f); } }
		public static ColorHSL Purple { get { return new ColorHSL(270f, 1f, 0.5f); } }
		public static ColorHSL Orange { get { return new ColorHSL(30f, 1f, 0.5f); } }
		#endregion

		private float h, s, l;

		public float H
		{
			set
			{
				h = Clamp(value);
			}
			get
			{
				return h;
			}
		}
		public float S
		{
			set
			{
				s = Clamp(value);
			}
			get
			{
				return s;
			}
		}
		public float L
		{
			set
			{
				l = Clamp(value);
			}
			get
			{
				return l;
			}
		}

		public Color ToRGB
		{
			get
			{
				float C = s * (1 - Math.Abs(l * 2 - 1));
				float H = h / 60;

				float N = (1 - Math.Abs(TKMath.Mod(H, 0, 2) - 1)) * C;

				Color rgb = new Color(0, 0, 0);

				if (0 <= H && H < 1)
					rgb = new Color(C, N, 0);
				else if (1 <= H && H < 2)
					rgb = new Color(N, C, 0);
				else if (2 <= H && H < 3)
					rgb = new Color(0, C, N);
				else if (3 <= H && H < 4)
					rgb = new Color(0, N, C);
				else if (4 <= H && H < 5)
					rgb = new Color(N, 0, C);
				else if (5 <= H && H < 6)
					rgb = new Color(C, 0, N);

				float m = l - C * 0.5f;
				return new Color(rgb.R + m, rgb.G + m, rgb.B + m);
			}
		}

		public Vector3 ToVector
		{
			get
			{
				return new Vector3(H, S, L);
			}
		}

		public ColorHSL(float _h, float _s, float _l)
		{
			h = _h;
			s = _s;
			l = _l;
		}

		public static ColorHSL Blend(ColorHSL c1, ColorHSL c2, float v)
		{
			v = Clamp(v);

			float h = c1.h + (c2.h - c1.h) * v,
					s = c1.s + (c2.s - c1.s) * v,
					l = c1.l + (c2.l - c1.l) * v;

			return new ColorHSL(h, s, l);
		}

		public static float Clamp(float v)
		{
			return (v < 0 ? 0 : (v > 1 ? 1 : v));
		}
	}
}