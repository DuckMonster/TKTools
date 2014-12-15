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
	}

	public class SHITBALLS
	{

	}
}