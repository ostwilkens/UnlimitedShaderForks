using System;
using System.Linq;
using System.Numerics;

namespace UnlimitedShaderForks.GLSLBuilder
{
	public class GLSLGenerator
	{
		private class ValueCollection
		{
			private IStatement[] _values;
			private Value<float>[] _candidates1;
			private IFn<float, float>[] _candidates2;
			private IFn<float, float, float>[] _candidates3;

			public ValueCollection(params IStatement[] values)
			{
				_values = values;

				_candidates1 = _values
					.Select(x => x as Value<float>)
					.Where(x => x != null)
					.ToArray();

				_candidates2 = _values
					.Select(x => x as IFn<float, float>)
					.Where(x => x != null)
					.ToArray();

				_candidates3 = _values
					.Select(x => x as IFn<float, float, float>)
					.Where(x => x != null)
					.ToArray();
			}

			private Value<float> PickValue()
			{
				return _candidates1.GetRandomItem(_rand);
			}

			private Value<float> PickValue(Value<float> a)
			{
				return _candidates2.GetRandomItem(_rand).Call(a);
			}

			private Value<float> PickValue(Value<float> a, Value<float> b)
			{
				return _candidates3.GetRandomItem(_rand).Call(a, b);
			}

			public Value<float> Complicate(int times = 1)
			{
				return Complicate(PickValue(), times);
			}

			public Value<float> Complicate(Value<float> a, int times = 1)
			{
				while (times > 0)
				{
					times--;
					int r = _rand.Next(1, 4);

					switch (r)
					{
						case 1: break;
						case 2: a = PickValue(a); break;
						case 3: a = PickValue(a, PickValue()); break;
						default: throw new InvalidOperationException();
					}
				}

				return a;
			}
		}

		private static Random _rand = new Random();
		private ValueCollection _vc;

		public GLSLGenerator()
		{
		}

		public string Generate()
		{
			var fn = new FunctionCollection();
			var body = new ShaderBody();
			var time = body.DeclareUniform<float>("Time");
			var position = body.DeclareIn<Vector2>("fsin_Position");
			var colorOut = body.DeclareOut<Vector4>("fsout_Color");

			var pR = body.DeclareFunction<Vector2, float, Vector2>("pR", "p", "a");
			pR.Set(pR.A1, (pR.A1 * fn.cos_f.Call(pR.A2)) + (fn.vec2_ff.Call(pR.A1.Y(), -pR.A1.X()) * fn.sin_f.Call(pR.A2)));
			pR.Return(pR.A1);

			var rand1d = body.DeclareFunction<float, float>("rand1d", "n");
			rand1d.Return(fn.fract_f.Call(fn.sin_f.Call(rand1d.A1) * 43758.5453f));

			var noise = body.DeclareFunction<Vector2, float, float>("noise", "uv", "intensity");
			noise.Return(fn.min_ff.Call(1f, (1f / (rand1d.Call((noise.A1.X() * 20f) + 1f) + rand1d.Call(noise.A1.Y() * 40f))) * noise.A2));

			var main = body.DeclareFunction<_Void>("main");
			var uv = main.Declare<Vector2>("uv", position);
			var c = main.Declare<Vector3>("c", new Vector3(0f));

			//main.Set(uv, uv + noise.Call(fn.Vec2(time), 0.5f) * 0.02f);

			_vc = new ValueCollection(new IStatement[] {
				fn.sin_f,
				fn.cos_f,
				fn.tan_f, // 3
				//CompositeFn.From((a1, a2) => a1 * a2), // 1
				CompositeFn.From((a1, a2) => a1 / a2), // 2
				CompositeFn.From((a1, a2) => a1 + a2), // 5
				CompositeFn.From((a1, a2) => a1 - a2), // 3
				CompositeFn.From(a1 => fn.Round(a1 * 10f) / 10f), // 2
				CompositeFn.From(a1 => fn.Floor(a1 * 10f) / 10f), // 2
				CompositeFn.From(a1 => -a1), // 2
				CompositeFn.From(a1 => fn.Tan(a1) / 2f),
				fn.max_ff, // 3
				//fn.min_ff, // 1
				CompositeFn.From((a1, a2) => fn.Length(fn.Vec2(a1, a2))),
				CompositeFn.From(a1 => fn.Smoothstep(a1, 0.97f, 1f)),
				new ActualValue<float>(3.1416f),
				uv.X() * 20f,
				uv.Y() * 20f,
				fn.Length(uv),
				CompositeFn.From((a1, a2) => fn.Length(fn.Vec2(a1, a2))),
				fn.Sin(2 * time * 1.57f),
				fn.Tan(2 * time) / 2f,
				time * 2f,
				CompositeFn.From((a1, a2) => pR.Call(fn.Vec2(a1, a2), time).X()),
				CompositeFn.From((a1, a2) => pR.Call(fn.Vec2(a1, a2), time).Y()),
				CompositeFn.From((a1, a2) => pR.Call(fn.Vec2(uv.X(), a1), a2).X()), // 4
				CompositeFn.From((a1) => pR.Call(uv, a1).X()), // 4
				CompositeFn.From((a1) => pR.Call(uv, a1).Y()), // 4
				rand1d,
				uv.X(),
				uv.Y(),
			});

			var r = main.Declare<Vector3>("r", new Vector3(0.68f, 0.18f, 0.14f));
			var g = main.Declare<Vector3>("g", new Vector3(0.87f, 0.89f, 0.78f));
			var b = main.Declare<Vector3>("b", new Vector3(0f, 0.6f, 0.96f));

			var l = main.Declare<float>("l", fn.Clamp(_vc.Complicate(times: 100), 0f, 0.95f));
			main.Set(c, fn.Vec3(1f / _vc.Complicate(100), 1f / _vc.Complicate(100), 1f / _vc.Complicate(100)));
			main.Set(c, c * 0.9f);

			var actualColor = main.Declare<Vector3>("actualColor", new Vector3(1f));
			main.Set(actualColor, actualColor - fn.Vec3(c.X()) * r);
			main.Set(actualColor, actualColor - fn.Vec3(c.Y()) * g);
			main.Set(actualColor, actualColor - fn.Vec3(c.Z()) * b);
			main.Set(c, fn.Vec3(l) * actualColor);
			//main.Set(c, c + noise.Call(pR.Call(uv, time), 0.5f) * 0.021f);

			main.Set(c, fn.Max(c, fn.Vec3(0f)));
			main.Set(c, c * fn.Smoothstep(0.1f, 0.3f, time)); // fade in
			//main.Set(c, c - (fn.Smoothstep(0.55f, 1.3f, fn.Abs(uv.X())) * 0.2f));
			//main.Set(c, c - (fn.Smoothstep(0.17f, 0.7f, fn.Abs(uv.Y())) * 0.2f));
			//main.Set(c, c + fn.Sin(time * 0.4f) * 0.02f);
			//main.Set(c, fn.Max(c, new Vector3(0.04f)));
			//main.Set(c, c - fn.Step(0.65f, fn.Abs(uv.Y())));

			//main.Append(@"
			//	// grading
			//	c -= 0.02;
			//	c *= 1.1;
			//	c = sqrt(c);
			//	c = c * c * (2.5 - 1.5 * c * c); // contrast
			//	c = pow(c, vec3(1.0, 0.96, 1.0)); // soft green
			//	c *= vec3(1.08, 0.99, 0.99); // tint red
			//	c.z = (c.z + 0.05) / 1.05; // bias blue
			//	c = mix(c, c.yyy, 0.12); // desaturate");

			//main.Set(c, fn.Vec3(0f + fn.Step(0f, uv.X() + uv.Y())));

			main.Set(colorOut, fn.Vec4(c, 1f));
			return body.ToString();
		}
	}
}
