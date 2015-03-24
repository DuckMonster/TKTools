using OpenTK;
using System;
using System.Collections;
using System.Collections.Generic;

namespace TKTools.AStar
{
	public enum NodeType
	{
		None,
		Start,
		End,
		Solid
	}

	public enum NodeStatus
	{
		None,
		Open,
		Closed
	}

	public struct NodeIndex
	{
		public int x, y;
		public NodeIndex(int x, int y)
		{
			this.x = x;
			this.y = y;
		}
	}

	public class Node
	{
		NodeList list;

		NodeIndex index;
		Vector2 position;

		NodeType type;
		NodeStatus status;

		Node parent;

		int g, h;

		public int F
		{
			get
			{
				return g + h;
			}
		}

		public Node Parent
		{
			get
			{
				return parent;
			}
			set
			{
				parent = value;
				Status = NodeStatus.Open;

				g = parent.g + GetStepValue(this, parent);
			}
		}

		public NodeIndex Index
		{
			get
			{
				return index;
			}
		}

		public Vector2 Position
		{
			get
			{
				return position;
			}
		}

		public NodeStatus Status
		{
			get
			{
				return status;
			}
			set
			{
				if (value == NodeStatus.Open && status == NodeStatus.Closed) return;

				status = value;

				if (value == NodeStatus.Closed)
					parent = null;
				if (value == NodeStatus.Open)
					list.Open(this);
				if (value == NodeStatus.Closed)
					list.Close(this);
			}
		}

		public bool Solid
		{
			get
			{
				return type == NodeType.Solid;
			}
		}

		IEnumerable<Node> AdjacentNodes
		{
			get
			{
				for(int xx = -1; xx<=1; xx++)
					for(int yy= -1; yy<=1; yy++)
					{
						if (xx == 0 && yy == 0) continue;

						Node n = list[index.x + xx, index.y + yy];
						if (n != null && !n.Solid) yield return n;
					}
			}
		}

		public Node(NodeIndex index, Vector2 position, NodeType type, NodeList list)
		{
			this.list = list;

			this.index = index;
			this.position = position;

			this.type = type;
			status = NodeStatus.None;
		}

		public void SetStart()
		{
			type = NodeType.Start;
		}

		public void SetEnd()
		{
			type = NodeType.End;
		}

		public void CalculateValues(Node endNode)
		{
			h = Math.Abs(index.x - endNode.index.x) + Math.Abs(index.y - endNode.index.y);
		}

		public void DoPathFind()
		{
			Status = NodeStatus.Closed;

			foreach(Node n in AdjacentNodes)
				if (n.Status == NodeStatus.None || (n.Status == NodeStatus.Open && n.g > g + GetStepValue(this, n)))
					n.Parent = this;
		}

		static int GetStepValue(Node a, Node b)
		{
			if (a.index.x != b.index.x &&
				a.index.y != b.index.y)
				return 14;
			else
				return 10;
		}
	}
}