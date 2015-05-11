using OpenTK;
using System.Linq;
using System.Collections.Generic;
using System.Drawing;
using System;
using System.Collections;

namespace TKTools
{
	public class Polygon : IEnumerable
	{
		class PolyEnumerator : IEnumerator
		{
			Vector2[] pointList;
			int position = -1;

			public PolyEnumerator(Vector2[] list)
			{
				pointList = list;
			}

			public bool MoveNext()
			{
				position++;
				return (position < pointList.Length);
			}

			public void Reset()
			{
				position = -1;
			}

			public object Current
			{
				get
				{
					return pointList[position];
				}
			}
		}

		public List<Vector2> pointList = new List<Vector2>(50);

		public RectangleF Bounds
		{
			get
			{
				PolygonProjection vert = new PolygonProjection(this, new Vector2(0, 1)),
						hori = new PolygonProjection(this, new Vector2(1, 0));

				return new RectangleF(hori.Min, vert.Min, hori.Length, vert.Length);
			}
		}

		public Vector2 Center
		{
			get
			{
				RectangleF bounds = Bounds;
				Vector2 center = Vector2.Zero;

				center.X = bounds.X + bounds.Width / 2;
				center.Y = bounds.Y + bounds.Height / 2;

				return center;
			}
		}

		public Polygon()
		{
		}
		public Polygon(IEnumerable<Vector2> points)
		{
			pointList.AddRange(points);
		}

		public void AddPoint(Polygon p)
		{
			foreach (Vector2 point in p.pointList)
				AddPoint(point);
		}
		public void AddPoint(Vector2 point)
		{
			pointList.Add(point);
		}
		public void AddPoint(IEnumerable<Vector2> points)
		{
			pointList.AddRange(points);
		}

		public Vector2 GetEdgeNormal(int n)
		{
			return (pointList[n % pointList.Count] - pointList[(n + 1) % pointList.Count]).PerpendicularRight;
		}

		public bool Intersects(Vector2 point) { return Intersects(new Polygon(new Vector2[] { point })); }
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

		public void Scale(float scale)
		{
			Vector2 center = Center;

			for (int i = 0; i < pointList.Count; i++)
			{
				Vector2 pos = pointList[i] - center;
				pos = pos.Normalized() * pos.Length * scale;
				pointList[i] = pos + center;
			}
		}

		public Vector2 this[int i]
		{
			get
			{
				if (i < 0 || i > pointList.Count) throw new IndexOutOfRangeException();
				return pointList[i];
			}
			set
			{
				if (i < 0 || i > pointList.Count) throw new IndexOutOfRangeException();
				pointList[i] = value;
			}
		}

		//Enumerator stuff
		public IEnumerator GetEnumerator()
		{
			return new PolyEnumerator(pointList.ToArray());
		}
		//

		public static Polygon operator +(Polygon p, Polygon p2)
		{
			Polygon newPoly = new Polygon(p.pointList.ToArray());
			newPoly.AddPoint(p2);

			return newPoly;
		}

		public static Polygon operator +(Polygon p, IEnumerable<Vector2> points)
		{
			Polygon newPoly = new Polygon(p.pointList.ToArray());
			newPoly.AddPoint(points);

			return newPoly;
		}

		public static Polygon operator +(Polygon p, Vector2 point)
		{
			Polygon newPoly = new Polygon(p.pointList.ToArray());
			newPoly.AddPoint(point);

			return newPoly;
		}

		public static Polygon operator *(Polygon p, float scale)
		{
			Polygon newPoly = new Polygon(p.pointList.ToArray());
			newPoly.Scale(scale);

			return newPoly;
		}

		public static implicit operator Vector2[](Polygon p)
		{
			return p.pointList.ToArray();
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