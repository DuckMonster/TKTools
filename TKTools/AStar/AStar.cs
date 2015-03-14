using OpenTK;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace TKTools.AStar
{
	public static class AStar
	{
		class NodeList
		{
			Node[] nodeList;
			int GetIndex(int x, int y)
			{
				return x + y * width;
			}

			int width, height;

			public NodeList(int w, int h)
			{
				width = w;
				height = h;

				nodeList = new Node[w * h];
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
					return nodeList[GetIndex(x, y)];
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
					return nodeList[GetIndex(index.x, index.y)];
				}
				set
				{
					nodeList[GetIndex(index.x, index.y)] = value;
				}
			}
		}

		static List<Polygon> solids = new List<Polygon>();
		static NodeList nodeList;

		static int nodeCountW, nodeCountH;

		public static void CreateBuffer(IEnumerable<Polygon> solidList, float resolution)
		{
			solids.AddRange(solidList);

			Polygon solidsCombined = new Polygon();
			foreach (Polygon p in solids)
				solidsCombined.AddPoint(p);

			RectangleF rect = solidsCombined.Bounds;

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

					bool solid = false;
					foreach(Polygon p in solids)
						if (p.Intersects(pos))
						{
							solid = true;
							break;
						}

					nodeList
				}
		}
	}
}
