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
			var time = body.DeclareUniform<float>("Time", 0);
			var offset = body.DeclareUniform<Vector2>("Offset", 1);
			var zoom = body.DeclareUniform<float>("Zoom", 2);
			var position = body.DeclareIn<Vector2>("fsin_Position");
			var colorOut = body.DeclareOut<Vector4>("fsout_Color");

			var beatLen = body.Declare<float>("beat", (7.5f / 8.0f));
			var beats = body.Declare<float>("beats", time / beatLen);

			var pR = body.DeclareFunction<Vector2, float, Vector2>("pR", "p", "a");
			pR.Set(pR.A1, (pR.A1 * fn.cos_f.Call(pR.A2)) + (fn.vec2_ff.Call(pR.A1.Y(), -pR.A1.X()) * fn.sin_f.Call(pR.A2)));
			pR.Return(pR.A1);

			var rand1d = body.DeclareFunction<float, float>("rand1d", "n");
			rand1d.Return(fn.fract_f.Call(fn.sin_f.Call(rand1d.A1) * 43758.5453f));

			var noise = body.DeclareFunction<Vector2, float, float>("noise", "uv", "intensity");
			noise.Return(fn.min_ff.Call(1f, (1f / (rand1d.Call((noise.A1.X() * 20f) + 1f) + rand1d.Call(noise.A1.Y() * 40f))) * noise.A2));

			var spikeFunc = body.DeclareFunction<float, float>("spikeFunc", "x");
			spikeFunc.Append("return max(min(min(fract(x / -2.) * 2. -1., sin((x + 1.) / 0.31831 ) + 1.), sin((x - 1.278) / 0.31831) + 0.645), 0.)");

			//float superclamp(float val, float start, float end)
			//{
			//	float dur = end - start;
			//	float halfdur = dur / 2.;
			//	float prog = clamp(val, start, end) - start;
			//	return (halfdur - abs(prog - halfdur)) / halfdur;
			//}

			var superclamp = body.DeclareFunction<float, float, float, float>("superclamp", "val", "start", "end");
			{
				var val = superclamp.A1;
				var start = superclamp.A2;
				var end = superclamp.A3;
				var dur = superclamp.Declare("dur", end - start);
				var halfdur = superclamp.Declare("halfdur", dur / 2f);
				var prog = superclamp.Declare("prog", fn.Clamp(val, start, end) - start);
				superclamp.Return((halfdur - fn.Abs(prog - halfdur)) / halfdur); 
			}

			var main = body.DeclareFunction<_Void>("main");
			var uv = main.Declare<Vector2>("uv", position);
			main.Set(uv, (uv / 2f) - offset);
			main.Set(uv, uv * (1f + zoom * zoom));
			var c = main.Declare<Vector3>("c", new Vector3(0f));

			//main.Set(uv, fn.Vec2(uv.X() + 0.1f * (spikeFunc.Call(timeD) * fn.Sin(timeD * 60f)), uv.Y()));

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
				fn.Length(uv),
				fn.Length(uv),
				CompositeFn.From((a1, a2) => fn.Length(fn.Vec2(a1, a2))),
				fn.Sin(2 * beats * 1.57f),
				fn.Tan(2 * beats) / 2f,
				beats,
				CompositeFn.From((a1, a2) => pR.Call(fn.Vec2(a1, a2), beats).X()),
				CompositeFn.From((a1, a2) => pR.Call(fn.Vec2(a1, a2), beats).Y()),
				CompositeFn.From((a1, a2) => pR.Call(fn.Vec2(uv.X(), a1), a2).X()), // 4
				CompositeFn.From((a1) => pR.Call(uv, a1).X()), // 4
				CompositeFn.From((a1) => pR.Call(uv, a1).Y()), // 4
				rand1d,
				uv.X(),
				uv.Y(),
				fn.Step(beats, 0.5f),
			});

			var r = main.Declare<Vector3>("r", new Vector3(0.68f, 0.18f, 0.14f));
			var g = main.Declare<Vector3>("g", new Vector3(0.87f, 0.89f, 0.78f));
			var b = main.Declare<Vector3>("b", new Vector3(0f, 0.6f, 0.96f));

			var l = main.Declare<float>("l", fn.Clamp(_vc.Complicate(times: 1000), 0f, 0.95f));
			main.Set(c, fn.Vec3(1f / _vc.Complicate(100), 1f / _vc.Complicate(100), 1f / _vc.Complicate(100)));
			main.Set(c, c * 0.9f);

			var actualColor = main.Declare<Vector3>("actualColor", new Vector3(1f));
			main.Set(actualColor, actualColor - fn.Vec3(c.X()) * r);
			main.Set(actualColor, actualColor - fn.Vec3(c.Y()) * g);
			main.Set(actualColor, actualColor - fn.Vec3(c.Z()) * b);
			main.Set(c, fn.Vec3(l) * actualColor);
			main.Set(c, c + noise.Call(pR.Call(uv, time), 0.5f) * 0.021f);

			main.Set(c, fn.Max(c, fn.Vec3(0f)));
			main.Set(c, c * fn.Smoothstep(0.1f, 0.3f, time)); // fade in

			//main.Set(c, fn.Vec3(0f + fn.Step(0f, uv.X() + uv.Y())));
			//main.Set(c, fn.Clamp(c, 0f, 1f));

			//main.Set(c, c + spikeFunc.Call(timeD));
			var testa = main.Declare("testa", superclamp.Call(fn.Fract(beats * 4f), 0f, 0.1f));
			//var testb = main.Declare("testb", superclamp.Call(fn.Fract(((beats + 0.1f) * 4f)), 0f, 0.2f));

			var t = fn.Step(0.2f, fn.Length(uv));

			main.Set(c, fn.Vec3(t) * fn.Vec3(testa * 0.2f, 0f, 0f));

			main.Set(colorOut, fn.Vec4(c, 1f));
			return body.ToString();
		}
	}
}
