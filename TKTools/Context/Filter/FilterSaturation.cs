namespace TKTools.Context.Filter
{
	public class FilterSaturation : Filter
	{
		private const string fragment = @"
#version 330
uniform sampler2D texture;
in vec2 uv;

out vec4 fragment;

void main() {
	vec4 clr = texture2D(texture, uv);
	float total = (clr.r + clr.g + clr.b) / 3.0;
	fragment = vec4(total, total, total, clr.a);
}
";

		public FilterSaturation() : base(fragment) { }
	}
}