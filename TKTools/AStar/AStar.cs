using OpenTK;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;

namespace TKTools.AStar
{
	public class NodeList : IEnumerable
	{
		Node[] nodeList;
		List<Node> openList = new List<Node>();
		List<Node> closedList = new List<Node>();

		public Node NextPathNode
		{
			get
			{
				Node best = null;

				foreach(Node n in openList)
				{
					if (best == null || n.F <= best.F)
						best = n;
				}

				return best;
			}
		}

		int GetIndex(int x, int y)
		{
			if (x < 0 || x >= width || y < 0 || y >= height) return -1;
			return x + y * width;
		}

		int width, height;

		public NodeList(int w, int h)
		{
			width = w;
			height = h;

			nodeList = new Node[w * h];
		}

		public void Reset()
		{
			foreach (Node n in openList)
				n.Status = NodeStatus.None;
			foreach (Node n in closedList)
				n.Status = NodeStatus.None;

			openList.Clear();
			closedList.Clear();
		}

		public void Open(Node n)
		{
			if (!openList.Contains(n)) openList.Add(n);
		}

		public void Close(Node n)
		{
			if (!closedList.Contains(n))
			{
				closedList.Add(n);

				if (openList.Contains(n))
					openList.Remove(n);
			}
		}

		public IEnumerator GetEnumerator()
		{
			foreach (Node n in nodeList)
				if (n != null) yield return n;
		}

		public Node this[int id]
		{
			get
			{
				return nodeList[id];
			}
			set
			{
				nodeList[id] = value;
			}
		}

		public Node this[int x, int y]
		{
			get
			{
				int i = GetIndex(x, y);

				if (i == -1) return null;
				return nodeList[i];
			}
			set
			{
				nodeList[GetIndex(x, y)] = value;
			}
		}

		public Node this[NodeIndex index]
		{
			get
			{
				int i = GetIndex(index.x, index.y);

				if (i == -1) return null;
				return nodeList[i];
			}
			set
			{
				nodeList[GetIndex(index.x, index.y)] = value;
			}
		}

		public Node[] ToArray()
		{
			return nodeList;
		}
	}

	public static class AStar
	{
		static List<Polygon> solids = new List<Polygon>();
		public static NodeList nodeList;

		static int nodeCountW, nodeCountH;

		public static void CreateBuffer(IEnumerable<Polygon> solidList, float resolution, RectangleF? bounds = null)
		{
			solids.AddRange(solidList);

			Polygon solidsCombined = new Polygon();
			foreach (Polygon p in solids)
				solidsCombined.AddPoint(p);

			RectangleF rect;

			if (bounds == null)
				rect = solidsCombined.Bounds;
			else
				rect = bounds.Value;

			nodeCountW = (int)(Math.Round(rect.Width * resolution));
			nodeCountH = (int)(Math.Round(rect.Height * resolution));

			nodeList = new NodeList(nodeCountW, nodeCountH);

			for(int x=0; x<nodeCountW; x++)
				for(int y=0; y<nodeCountH; y++)
				{
					Vector2 pos = new Vector2(
						rect.X + (rect.Width * ((float)x / nodeCountW)),
						rect.Y + (rect.Height * ((float)y / nodeCountH))
						);

					Polygon colPoly = new Polygon( new Vector2[] {
						pos + new Vector2(0.5f / resolution, 0.5f / resolution),
						pos + new Vector2(0.5f / resolution, -0.5f / resolution),
						pos + new Vector2(-0.5f / resolution, -0.5f / resolution),
						pos + new Vector2(-0.5f / resolution, 0.5f / resolution)
					});

					bool solid = false;
					foreach(Polygon p in solids)
						if (p.Intersects(colPoly))
						{
							solid = true;
							break;
						}

					nodeList[x, y] = new Node(new NodeIndex(x, y), pos, solid ? NodeType.Solid : NodeType.None, nodeList);
				}
		}

		public static List<Vector2> FindPath(Vector2 start, Vector2 end)
		{
			nodeList.Reset();

			Node startNode = FindClosestNode(start);
			Node endNode = FindClosestNode(end);

			startNode.SetStart();
			endNode.SetStart();

			foreach (Node n in nodeList)
				n.CalculateValues(endNode);

			startNode.Status = NodeStatus.Open;

			while(true)
			{
				Node n = nodeList.NextPathNode;

				if (n == null) return null;
				if (n == endNode)
				{
					List<Vector2> path = new List<Vector2>();

					while (n != null)
					{
						if (path.Contains(n.Position))
						{
							Console.WriteLine("Parent loop!");
							break;
						}

						path.Add(n.Position);
						n = n.Parent;
					}

					return path;
				}

				n.DoPathFind();
			}
		}

		static Node FindClosestNode(Vector2 pos)
		{
			Node closest = null;
			float closestDist = 0;

			foreach(Node n in nodeList)
			{
				if (n.Solid) continue;

				float dist = (pos - n.Position).LengthFast;

				if (closest == null || dist < closestDist)
				{
					closest = n;
					closestDist = dist;
				}
			}

			return closest;
		}
	}
}
