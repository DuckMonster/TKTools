using OpenTK;
using System.Linq;
using System.Collections.Generic;

namespace TKTools
{
	public class Polygon
	{
		public List<Vector2> pointList = new List<Vector2>();

		public Polygon(params Vector2[] points)
		{
			pointList.AddRange(points);
		}

		public Vector2 GetEdgeNormal(int n)
		{
			return (pointList[n % pointList.Count] - pointList[(n + 1) % pointList.Count]).PerpendicularRight;
		}

		public bool Intersects(Polygon p)
		{
			if (pointList.Count > 1)
			{
				for (int i = 0; i < pointList.Count; i++)
				{
					Vector2 normal = GetEdgeNormal(i);

					PolygonProjection
						a = new PolygonProjection(this, normal),
						b = new PolygonProjection(p, normal);

					if (!(a & b)) return false;
				}
			}

			if (p.pointList.Count > 1)
			{
				for (int i = 0; i < p.pointList.Count; i++)
				{
					Vector2 normal = p.GetEdgeNormal(i);

					PolygonProjection
						a = new PolygonProjection(this, normal),
						b = new PolygonProjection(p, normal);

					if (!(a & b)) return false;
				}
			}

			return true;
		}
	}

	public class PolygonProjection
	{
		public List<float> hitPointList = new List<float>();

		public float Max
		{
			get
			{
				return hitPointList.Max();
			}
		}

		public float Min
		{
			get
			{
				return hitPointList.Min();
			}
		}

		public float Length
		{
			get
			{
				return Max - Min;
			}
		}

		public PolygonProjection(Polygon p, Vector2 axis)
		{
			axis.Normalize();

			foreach (Vector2 point in p.pointList)
			{
				hitPointList.Add(Vector2.Dot(point, axis));
			}
		}

		public static bool operator &(PolygonProjection a, PolygonProjection b)
		{
			return (a.Min < b.Max && b.Min < a.Max);
		}
	}
}