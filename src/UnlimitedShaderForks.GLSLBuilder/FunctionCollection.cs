using System.Numerics;

namespace UnlimitedShaderForks.GLSLBuilder
{
	public class FunctionCollection
	{
		public readonly Fn<float, float> round_f = new Fn<float, float>("round");
		public Value<float> Round(Value<float> a) => round_f.Call(a);

		public readonly Fn<float, float> floor_f = new Fn<float, float>("floor");
		public Value<float> Floor(Value<float> a) => floor_f.Call(a);

		public readonly Fn<float, float> abs_f = new Fn<float, float>("abs");
		public Value<float> Abs(Value<float> a) => abs_f.Call(a);

		public readonly Fn<Vector3, Vector3> abs_v3 = new Fn<Vector3, Vector3>("abs");
		public Value<Vector3> Abs(Value<Vector3> a) => abs_v3.Call(a);

		public readonly Fn<float, float> cos_f = new Fn<float, float>("cos");
		public Value<float> Cos(Value<float> a) => cos_f.Call(a);

		public readonly Fn<Vector3, Vector3> cos_v3 = new Fn<Vector3, Vector3>("cos");
		public Value<Vector3> Cos(Value<Vector3> a) => cos_v3.Call(a);

		public readonly Fn<float, float> sin_f = new Fn<float, float>("sin");
		public Value<float> Sin(Value<float> a) => sin_f.Call(a);

		public readonly Fn<float, float> tan_f = new Fn<float, float>("tan");
		public Value<float> Tan(Value<float> a) => tan_f.Call(a);

		public readonly Fn<float, float> fract_f = new Fn<float, float>("fract");
		public Value<float> Fract(Value<float> a) => fract_f.Call(a);

		public readonly Fn<Vector2, Vector2> fract_v2 = new Fn<Vector2, Vector2>("fract");
		public Value<Vector2> Fract(Value<Vector2> a) => fract_v2.Call(a);

		public readonly Fn<Vector3, Vector3> fract_v3 = new Fn<Vector3, Vector3>("fract");
		public Value<Vector3> Fract(Value<Vector3> a) => fract_v3.Call(a);

		public readonly Fn<float, float, float, float> clamp_fff = new Fn<float, float, float, float>("clamp");
		public Value<float> Clamp(Value<float> a, Value<float> b, Value<float> c) => clamp_fff.Call(a, b, c);

		public readonly Fn<Vector2, float, float, Vector2> clamp_v2ff = new Fn<Vector2, float, float, Vector2>("clamp");
		public Value<Vector2> Clamp(Value<Vector2> a, Value<float> b, Value<float> c) => clamp_v2ff.Call(a, b, c);

		public readonly Fn<Vector3, float, float, Vector3> clamp_v3ff = new Fn<Vector3, float, float, Vector3>("clamp");
		public Value<Vector3> Clamp(Value<Vector3> a, Value<float> b, Value<float> c) => clamp_v3ff.Call(a, b, c);

		public readonly Fn<Vector2, float> length_v2 = new Fn<Vector2, float>("length");
		public Value<float> Length(Value<Vector2> a) => length_v2.Call(a);

		public readonly Fn<Vector3, float> length_v3 = new Fn<Vector3, float>("length");
		public Value<float> Length(Value<Vector3> a) => length_v3.Call(a);

		public readonly Fn<float, float, float> min_ff = new Fn<float, float, float>("min");
		public Value<float> Min(Value<float> a, Value<float> b) => min_ff.Call(a, b);

		public readonly Fn<Vector3, Vector3, Vector3> min_v3v3 = new Fn<Vector3, Vector3, Vector3>("min");
		public Value<Vector3> Min(Value<Vector3> a, Value<Vector3> b) => min_v3v3.Call(a, b);

		public readonly Fn<float, float, float> max_ff = new Fn<float, float, float>("max");
		public Value<float> Max(Value<float> a, Value<float> b) => max_ff.Call(a, b);

		public readonly Fn<Vector3, Vector3, Vector3> max_v3v3 = new Fn<Vector3, Vector3, Vector3>("max");
		public Value<Vector3> Max(Value<Vector3> a, Value<Vector3> b) => max_v3v3.Call(a, b);

		public readonly Fn<float, Vector2> vec2_f = new Fn<float, Vector2>("vec2");
		public Value<Vector2> Vec2(Value<float> a) => vec2_f.Call(a);

		public readonly Fn<float, float, Vector2> vec2_ff = new Fn<float, float, Vector2>("vec2");
		public Value<Vector2> Vec2(Value<float> a, Value<float> b) => vec2_ff.Call(a, b);

		public readonly Fn<float, Vector3> vec3_f = new Fn<float, Vector3>("vec3");
		public Value<Vector3> Vec3(Value<float> a) => vec3_f.Call(a);

		public readonly Fn<Vector2, float, Vector3> vec3_v2f = new Fn<Vector2, float, Vector3>("vec3");
		public Value<Vector3> Vec3(Value<Vector2> a, Value<float> b) => vec3_v2f.Call(a, b);

		public readonly Fn<float, Vector2, Vector3> vec3_fv2 = new Fn<float, Vector2, Vector3>("vec3");
		public Value<Vector3> Vec3(Value<float> a, Value<Vector2> b) => vec3_fv2.Call(a, b);

		public readonly Fn<float, float, float, Vector3> vec3_fff = new Fn<float, float, float, Vector3>("vec3");
		public Value<Vector3> Vec3(Value<float> a, Value<float> b, Value<float> c) => vec3_fff.Call(a, b, c);

		public readonly Fn<Vector3, float, Vector4> vec4_v3f = new Fn<Vector3, float, Vector4>("vec4");
		public Value<Vector4> Vec4(Value<Vector3> a, Value<float> b) => vec4_v3f.Call(a, b);

		public readonly Fn<Vector3, Vector3> normalize_v3 = new Fn<Vector3, Vector3>("normalize");
		public Value<Vector3> Normalize(Value<Vector3> a) => normalize_v3.Call(a);

		public readonly Fn<float, float, float> step_ff = new Fn<float, float, float>("step");
		public Value<float> Step(Value<float> a, Value<float> b) => step_ff.Call(a, b);

		public readonly Fn<float, float, float, float> smoothstep_fff = new Fn<float, float, float, float>("smoothstep");
		public Value<float> Smoothstep(Value<float> a, Value<float> b, Value<float> c) => smoothstep_fff.Call(a, b, c);

		public readonly Fn<Vector3, Vector3, Vector3> cross_v3v3 = new Fn<Vector3, Vector3, Vector3>("cross");
		public Value<Vector3> Cross(Value<Vector3> a, Value<Vector3> b) => cross_v3v3.Call(a, b);

		public readonly Fn<Vector3, Vector3, float> dot_v3v3 = new Fn<Vector3, Vector3, float>("dot");
		public Value<float> Dot(Value<Vector3> a, Value<Vector3> b) => dot_v3v3.Call(a, b);

		public readonly Fn<float, float, float> pow_ff = new Fn<float, float, float>("pow");
		public Value<float> Pow(Value<float> a, Value<float> b) => pow_ff.Call(a, b);

		public readonly Fn<float, float, float> mod_ff = new Fn<float, float, float>("mod");
		public Value<float> Mod(Value<float> a, Value<float> b) => mod_ff.Call(a, b);

		public readonly Fn<float, float> sqrt_f = new Fn<float, float>("sqrt");
		public Value<float> Sqrt(Value<float> a) => sqrt_f.Call(a);

		public readonly Fn<Vector3, Vector3> sqrt_v3 = new Fn<Vector3, Vector3>("sqrt");
		public Value<Vector3> Sqrt(Value<Vector3> a) => sqrt_v3.Call(a);
	}
}
