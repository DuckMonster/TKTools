namespace TKTools.Context.Filter
{
	public class FilterCrazy : Filter
	{
		private const string fragment = @"
#version 330
uniform sampler2D texture;
uniform float magnitude;
in vec2 uv;

out vec4 fragment;

void main() {
	float red = texture2D(texture, vec2(uv.x-magnitude, uv.y)).r;
	float green = texture2D(texture, uv).g;
	float blue = texture2D(texture, vec2(uv.x+magnitude, uv.y)).b;

	fragment = vec4(red, green, blue, 1.0);
}
";

		float magnitude = 0.01f;
		public float Magnitude
		{
			get { return magnitude; }
			set
			{
				magnitude = value;
				program["magnitude"].SetValue(magnitude);
			}
		}

		public FilterCrazy() : base(fragment)
		{
			Magnitude = 0.01f;
		}
	}
}