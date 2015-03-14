using OpenTK;

namespace TKTools.AStar
{
	enum NodeType
	{
		None,
		Start,
		End,
		Solid
	}

	enum NodeStatus
	{
		None,
		Open,
		Closed
	}

	struct NodeIndex
	{
		public int x, y;
		public NodeIndex(int x, int y)
		{
			this.x = x;
			this.y = y;
		}
	}

	class Node
	{
		NodeIndex index;
		Vector2 position;

		NodeType type;
		NodeStatus status;

		public Node(NodeIndex index, Vector2 position, NodeType type)
		{
			this.index = index;
			this.position = position;

			this.type = type;
			status = NodeStatus.None;
		}
	}
}