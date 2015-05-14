namespace TKTools.Context.Filter
{
	public class FilterCrazy2 : Filter
	{
		private const string fragment = @"
#version 330
uniform sampler2D texture;
uniform float magnitude;
in vec2 uv;

out vec4 fragment;

void main() {
	vec2 offset = uv - vec2(0.5, 0.5);

	float red = texture2D(texture, uv + offset * magnitude).r;
	float green = texture2D(texture, uv).g;
	float blue = texture2D(texture, uv - offset * magnitude).b;

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

		public FilterCrazy2() : base(fragment)
		{
			Magnitude = 0.01f;
		}
	}
}