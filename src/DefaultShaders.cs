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

layout(location = 0) in vec2 fsin_Position;
layout(location = 0) out vec4 fsout_Color;

void main()
{
	fsout_Color = vec4(fsin_Position.y);
}";
	}
}
