using System;
using OpenTK;

namespace TKTools
{
	public static class TKMath
	{
		public static float GetAngle(Vector2 a, Vector2 b)
		{
			return GetAngle(b - a);
		}
		public static float GetAngle(Vector2 vec)
		{
			return ToDegrees((float)Math.Atan2(vec.Y, vec.X));
		}

		public static int Mod(int v, int max)
		{
			while (v < 0) v += max;
			return v % max;
		}

		public static int Mod(int v, int min, int max)
		{
			while (v < min) v += (max - min);
			while (v > max) v -= (max - min);

			return v;
		}

		public static float Mod(float v, float max)
		{
			while (v < 0) v += max;
			return v % max;
		}

		public static float Mod(float v, float min, float max)
		{
			while (v < min) v += (max - min);
			while (v > max) v -= (max - min);

			return v;
		}

		public static double Mod(double v, double max)
		{
			while (v < 0) v += max;
			return v % max;
		}

		public static double Mod(double v, double min, double max)
		{
			while (v < min) v += (max - min);
			while (v > max) v -= (max - min);

			return v;
		}

		public static Vector2 GetAngleVector(float a)
		{
			return new Vector2((float)Math.Cos(ToRadians(a)), (float)Math.Sin(ToRadians(a)));
		}

		public static float ToRadians(float deg)
		{
			return MathHelper.DegreesToRadians(deg);
		}

		public static float ToDegrees(float rad)
		{
			return MathHelper.RadiansToDegrees(rad);
		}

		public static float Exp(float v, float max)
		{
			return Exp(v, max, (float)Math.E);
		}

		public static float Exp(float v, float max, float exp)
		{
			return (float)Math.Pow(exp, -v * max);
		}

		public static Vector2 PolarPointVector(float a, int n)
		{
			a = ToRadians(a);
			return new Vector2(PolarPoint(a, n) * (float)Math.Cos(a), PolarPoint(a, n) * (float)Math.Sin(a));
		}

		static float PolarPoint(float a, int n)
		{
			return Sec(2f / n * (float)Math.Asin(Math.Sin(n / 2f * a)));
		}

		public static float Sec(float a)
		{
			return (float)(1f / Math.Cos(a));
		}
	}
}