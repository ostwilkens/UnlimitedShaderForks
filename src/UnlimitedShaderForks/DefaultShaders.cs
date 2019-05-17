namespace UnlimitedShaderForks
{
	public static class DefaultShaders
	{
		public const string VertexCode = @"
#version 450

layout(location = 0) in vec2 Position;
layout(location = 0) out vec2 fsin_Position;

void main()
{
	gl_Position = vec4(Position, 0, 1);
	fsin_Position = Position;
}";

		public const string FragmentCode = @"
#version 450

layout(set = 0, binding = 0) uniform _ { float _Time; };

layout(location = 0) in vec2 fsin_Position;
layout(location = 0) out vec4 fsout_Color;

void main()
{
	vec2 uv = fsin_Position;
	vec3 c = vec3(0.);

	c.r += step(0.95, abs(uv.x));
	c.g += step(0.95, abs(uv.y));
	c.b += uv.x + uv.y;
	c += vec3(step(0.0, uv.y + uv.x + sin(_Time) * 0.2) * 0.2);

	fsout_Color = vec4(c, 0.);
}";

		public const string PassthroughFragmentCode = @"
#version 450

layout(set = 0, binding = 0) uniform texture2D SourceTexture;
layout(set = 0, binding = 1) uniform sampler SourceSampler;

layout(location = 0) in vec2 fsin_Position;
layout(location = 0) out vec4 fsout_Color;

void main()
{
	vec2 uv = fsin_Position/2. + 0.5;
	fsout_Color = texture(sampler2D(SourceTexture, SourceSampler), uv);
}
";
	}
}
