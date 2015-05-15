using OpenTK;
using System;

namespace TKTools.Context.Filter
{
	public class FilterBlur : Filter
	{
		class BlurDir : Filter
		{
			private const string fragment = @"
#version 330
uniform sampler2D texture;
uniform int textureWidth;
uniform int textureHeight;

uniform vec2 direction;
uniform int kernel;
uniform float opacity;
uniform float weights[50];

in vec2 uv;

out vec4 fragment;

void main() {
	vec2 pixelUV = vec2(1.0 / textureWidth, 1.0 / textureHeight) * direction;
	vec4 total = vec4(0, 0, 0, 0);

	for(int i=-kernel; i<=kernel; i++) {
		total += texture2D(texture, uv + pixelUV * i) * weights[int(abs(float(i)))];
	}

	//total = texture2D(texture, vec2(uv + pixelUV * kernel));

	//fragment = mix(texture2D(texture, uv), total, opacity);
	fragment = total;

	/*
	int k = int((uv.x * 2 - 1) * kernel);
	//if (k < 0) k *= -1;
	k = int(abs(float(k)));
	fragment = vec4(weights[k], 0.0, 0.0, 1.0);
	*/
}
";

			Vector2 direction;
			public Vector2 Direction
			{
				get { return direction; }
				set
				{
					direction = value;
					program["direction"].SetValue(direction);
				}
			}

			int kernel;
			public int Kernel
			{
				get { return kernel; }
				set
				{
					kernel = value;
					program["kernel"].SetValue(kernel);
				}
			}

			float opacity;
			public float Opacity
			{
				get { return opacity; }
				set
				{
					opacity = value;
					program["opacity"].SetValue(opacity);
				}
			}

			float[] weights;
			float exponent;
			public float Exponent
			{
				get { return exponent; }
				set
				{
					exponent = value;
					CalculateWeights();
				}
			}

			public BlurDir(Vector2 dir)
				: base(fragment)
			{
				Direction = dir;
				Opacity = 1f;
				Kernel = 15;
				Exponent = 0.2f;
			}

			void CalculateWeights()
			{
				weights = new float[kernel + 1];
				float total = 0f;

				for(int i=0; i< weights.Length; i++)
				{
					float w = (float)Math.Exp(-exponent * i);

					weights[i] = w;
					total += i == 0 ? w : w*2;
				}

				for (int i = 0; i < weights.Length; i++)
				{
					weights[i] /= total;
                }

				program["weights"].SetValue(weights);
			}

			public override void Apply(Texture t)
			{
				program["textureWidth"].SetValue(t.Width);
				program["textureHeight"].SetValue(t.Height);
				base.Apply(t);
			}
		}

		BlurDir blurHori, blurVert;
		int kernel;
		public int Kernel
		{
			get { return kernel; }
			set
			{
				kernel = value;
				blurHori.Kernel = value;
				blurVert.Kernel = value;
			}
		}

		float opactity;
		public float Opacity
		{
			get { return opactity; }
			set
			{
				opactity = MathHelper.Clamp(value, 0f, 1f);
				blurHori.Opacity = opactity;
				blurVert.Opacity = opactity;
			}
		}

		float exponent;
		public float Exponent
		{
			get { return exponent; }
			set
			{
				exponent = value;
				blurHori.Exponent = exponent;
				blurVert.Exponent = exponent;
			}
		}

		int repeat = 1;
		public int Repeat
		{
			get { return repeat; }
			set { repeat = value; }
		}

		public FilterBlur() : this(0.15f, 15, 1) { }
		public FilterBlur(float exponent) : this(exponent, 15, 1) { }
		public FilterBlur(float exponent, int kernel) :this(exponent, kernel, 1) { }
		public FilterBlur(float exponent, int kernel, int repeat) : base()
		{
			blurHori = new BlurDir(new Vector2(1, 0));
			blurVert = new BlurDir(new Vector2(0, 1));

			Exponent = exponent;
			Kernel = kernel;
			Repeat = repeat;
		}

		public override void Apply(Texture t)
		{
			Texture finalText = t;

			for (int i = 0; i < repeat; i++)
			{
				blurVert.Apply(finalText);
				finalText = blurVert.OutputTexture;
				blurHori.Apply(finalText);
				finalText = blurHori.OutputTexture;
			}

			base.Apply(finalText);
		}
	}
}