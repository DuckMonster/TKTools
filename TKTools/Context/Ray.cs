using OpenTK;

namespace TKTools.Context
{
	public struct Ray
	{
		public static Ray FromTarget(Vector3 start, Vector3 target)
		{
			return new Ray(start, (target - start).Normalized());
		}

		Vector3 start;
		Vector3 direction;

		public Vector3 Start
		{
			get { return start; }
		}
		public Vector3 Direction
		{
			get { return direction; }
		}

		public Ray(Vector3 start, Vector3 direction)
		{
			this.start = start;
			this.direction = direction.Normalized();
		}

		public override string ToString()
		{
			return string.Format("{0} => {1}", start, direction);
		}
	}
}