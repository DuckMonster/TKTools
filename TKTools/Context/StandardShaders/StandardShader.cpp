@v------------------

#version 330

in vec3 vertexPosition;
in vec2 vertexUV;

uniform mat4 projection;
uniform mat4 view;
uniform mat4 model;

out vec2 uv;

void main() {
	uv = vertexUV;
	gl_Position = projection * view * model * vec4(vertexPosition, 1.0);
}

@------------------

@f------------------

#version 330

uniform sampler2D texture;
uniform bool usingTexture;
uniform vec4 color;
uniform bool fillColor;

in vec2 uv;
out vec4 fragment;

void main() {
	vec4 finalColor;

	if (usingTexture) {
		finalColor = texture2D(texture, uv) * color;
	}
	else {
		finalColor = color;
	}

	if (fillColor)
	{
		finalColor.rgb = color.rgb;
	}

	if (finalColor.a <= 0) discard;
	else fragment = finalColor;
}

@------------------ -