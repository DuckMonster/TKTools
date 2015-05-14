using OpenTK;

namespace TKTools.Context.Filter
{
	public class FilterBlur : Filter
	{
		class BlurHori : Filter
		{
			private const string fragment = @"
#version 330
uniform sampler2D texture;
uniform int textureWidth;
uniform int textureHeight;

uniform int magnitude;
uniform float weigth;

in vec2 uv;

out vec4 fragment;

void main() {
	vec2 pixelUV = vec2(1.0 / textureWidth, 1.0 / textureHeight);
	
	vec4 total = vec4(0.0, 0.0, 0.0, 0.0);
	for(int i=-magnitude; i<=magnitude; i++) {
		total += texture2D(texture, uv + vec2(pixelUV.x, 0.0) * i);
	}

	total /= (magnitude * 2) + 1;

	fragment = total;
}
";
			int magnitude;
			public int Magnitude
			{
				get { return magnitude; }
				set
				{
					magnitude = value;
					program["magnitude"].SetValue(magnitude);
				}
			}

			float weigth;
			public float Weigth
			{
				get { return weigth; }
				set
				{
					weigth = value;
					program["weigth"].SetValue(weigth);
				}
			}

			public BlurHori() : base(fragment)
			{
			}

			public override void Apply(Texture t)
			{
				program["textureWidth"].SetValue(t.Width);
				program["textureHeight"].SetValue(t.Height);
				base.Apply(t);
			}
		}

		class BlurVert : Filter
		{
			private const string fragment = @"
#version 330
uniform sampler2D texture;
uniform int textureWidth;
uniform int textureHeight;

uniform int magnitude;
uniform float weigth;

in vec2 uv;

out vec4 fragment;

void main() {
	vec2 pixelUV = vec2(1.0 / textureWidth, 1.0 / textureHeight);
	
	vec4 total = vec4(0.0, 0.0, 0.0, 0.0);
	for(int i=-5; i<=5; i++) {
		total += texture2D(texture, uv + vec2(0.0, pixelUV.y) * i);
	}

	total /= 11.0;

	fragment = total;
}
";
			int magnitude;
			public int Magnitude
			{
				get { return magnitude; }
				set
				{
					magnitude = value;
					program["magnitude"].SetValue(magnitude);
				}
			}

			float weigth;
			public float Weight
			{
				get { return weigth; }
				set
				{
					weigth = value;
					program["weigth"].SetValue(weigth);
				}
			}

			public BlurVert() : base(fragment)
			{
			}

			public override void Apply(Texture t)
			{
				program["textureWidth"].SetValue(t.Width);
				program["textureHeight"].SetValue(t.Height);
				base.Apply(t);
			}
		}

		BlurHori blurHori;
		BlurVert blurVert;
		int magnitude;
		public int Magnitude
		{
			get { return magnitude; }
			set
			{
				magnitude = value;
				blurHori.Magnitude = value;
				blurVert.Magnitude = value;
			}
		}

		float weigth;
		public float Weigth
		{
			get { return weigth; }
			set
			{
				weigth = MathHelper.Clamp(value, 0f, 1f);
				blurHori.Weigth = weigth;
				blurVert.Weight = weigth;
			}
		}

		public FilterBlur() : base()
		{
			blurHori = new BlurHori();
			blurVert = new BlurVert();
		}

		public override void Apply(Texture t)
		{
			Texture finalText = t;

			blurHori.Apply(finalText);
			finalText = blurHori.OutputTexture;
			blurVert.Apply(finalText);
			finalText = blurVert.OutputTexture;

			base.Apply(finalText);
		}
	}
}