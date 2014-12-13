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
			return (float)Math.Atan2(vec.Y, vec.X);
		}

		public static Vector2 GetAngleVector(float a)
		{
			return new Vector2((float)Math.Cos(ToRadians(a)), (float)Math.Sin(ToRadians(a)));
		}

		public static float ToRadians(float deg)
		{
			return (float)(deg / 180 * Math.PI);
		}

		public static float ToDegrees(float rad)
		{
			return (float)(rad * 180 / Math.PI);
		}

		public static float Exp(float v, float max)
		{
			return (float)Math.Exp(-v * max);
		}
	}
}