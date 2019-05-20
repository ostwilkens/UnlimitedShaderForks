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

layout(set = 0, binding = 0) uniform _Time { float Time; };

layout(location = 0) in vec2 fsin_Position;
layout(location = 0) out vec4 fsout_Color;

void main()
{
	vec2 uv = fsin_Position;
	vec3 c = vec3(0.);

	c.r += step(0.95, abs(uv.x));
	c.g += step(0.95, abs(uv.y));
	c.b += uv.x + uv.y;
	c += vec3(step(0.0, uv.y + uv.x + sin(Time) * 0.2) * 0.2);

	fsout_Color = vec4(c, 0.);
}";

		public const string PassthroughFragmentCode = @"
#version 450

layout(set = 0, binding = 0) uniform texture2D SourceTexture;
layout(set = 0, binding = 1) uniform sampler SourceSampler;
layout(set = 0, binding = 2) uniform _Time { float Time; };

layout(location = 0) in vec2 fsin_Position;
layout(location = 0) out vec4 fsout_Color;

float rand1d(float n)
{
	return fract(sin(n) * 43758.5453);
}

float noise(vec2 uv, float intensity)
{
    return min(1., (1. / (rand1d(uv.x * 20. + 1.) + rand1d(uv.y * 40.))) * intensity);
}

vec2 pR(vec2 p, float a) {
    // thx to hg (http://mercury.sexy/hg_sdf)
	return cos(a)*p + sin(a)*vec2(p.y, -p.x);
}

float smoothStairs(float x)
{
	return (sin((fract(x)-0.5) / 0.32) * 0.5 + floor(x));
}

void main()
{
	vec2 uv = fsin_Position/2. + 0.5;

	float dx = abs(0.5-uv.x);
	float dy = abs(0.5-uv.y);
	dx *= dx;
	dy *= dy;
	uv -= 0.5;
	uv *= 1.02;
	uv.x *= 1.0 + (dy * 0.15);
	uv.y *= 1.0 + (dx * 0.1);
	uv += 0.5;

	// distortions
	vec2 tuv = uv;
	tuv.y += sin(4. * 3.14 * Time / 3) * sin(abs(4. * 3.14 * Time) / 7) * 0.01;

	vec3 c = texture(sampler2D(SourceTexture, SourceSampler), tuv).rgb;
	vec3 overbleed = max(vec3(0.), c - vec3(1.));
	vec3 underbleed = max(vec3(0.), -c);
	c = clamp(c, 0., 1.);

	//c /= 1. + overbleed;
	//c += (underbleed * 3.).grb;

	// grading
	c -= 0.02;
	c *= 1.1;
	c = sqrt(c);
	c = c * c * (2.5 - 1.5 * c * c); // contrast
	c = pow(c, vec3(1.0, 0.96, 1.0)); // soft green
	c *= vec3(1.08, 0.99, 0.99); // tint red
	c.z = (c.z + 0.05) / 1.05; // bias blue
	c = mix(c, c.yyy, 0.12); // desaturate
	c += noise(pR(floor(uv * 400.) / 400., Time), 0.5f) * 0.15f;

	// noise
	c = clamp(c, 0.04, 1.);
	c += noise(pR(floor(uv * 200.) / 200., Time), 0.5) * 0.05;

	// scanlines
	uv.y -= 0.001;
	c *= sin(uv.y * (720. / 4.) * 3.14 * 2.) * 0.35 + 0.6;
	c *= sin(uv.x * (1280. / 4.) * 3.14 * 2. * 2.) * 0.07 + 0.9;
	c += noise(pR(uv, Time), 0.3f) * 0.03f;
	//c += 0.07;
	//c *= 1.05;

	c += smoothstep(overbleed, vec3(0.), vec3(2.)) * 0.3;

	// fx
	c -= smoothstep(0.15, 0.7, abs(uv.x - 0.5)) * 0.02; // vignette x
	c *= 1. - smoothstep(0.2, 0.7, abs(uv.x - 0.5)) * 0.2; // vignette x
	c -= smoothstep(0.17, 0.7, abs(uv.y - 0.5)) * 0.02; // vignette y
	c *= 1. - smoothstep(0.17, 0.7, abs(uv.y - 0.5)) * 0.2; // vignette y
	c *= 1.3;
	//c += sin(Time * 0.4) * 0.02; // ambience
	c *= 1. - step(0.35, abs(uv.y - 0.5)); // letterbox

	// clip edges
	if(uv.y > 1.0 || uv.x < 0.0 || uv.x > 1.0 || uv.y < 0.0)
		c = vec3(0.);

	fsout_Color = vec4(c, 0.);
}
";
	}
}
