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

			public void Append(params IStatement[] values)
			{
				_candidates1 = _candidates1.Union(values
					.Select(x => x as Value<float>)
					.Where(x => x != null)
					).ToArray();

				_candidates2 = _candidates2.Union(values
					.Select(x => x as IFn<float, float>)
					.Where(x => x != null)
					).ToArray();

				_candidates3 = _candidates3.Union(values
					.Select(x => x as IFn<float, float, float>)
					.Where(x => x != null)
					).ToArray();
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

		private static Random _rand;
		private ValueCollection _vc;

		public GLSLGenerator(int seed)
		{
			_rand = new Random(seed);
		}

		public string Generate3(int it)
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

			var sphereFunc = body.DeclareFunction<Vector3, float, float>("sphere", "p", "radius");
			sphereFunc.Append("return length(p) - radius");

			var scene = body.DeclareFunction<Vector3, Vector4>("scene", "p");
			{
				var p = scene.A1;

				_vc = new ValueCollection(new IStatement[] {
					fn.sin_f,
					CompositeFn.From((a1, a2) => a1 + a2),
					fn.Length(p) * 10f,
					p.X() / 1f,
					p.Y() / 1f,
					p.Z() / 1f,
					new ActualValue<float>(3.1416f),
					CompositeFn.From(a1 => fn.Floor(a1 * 7f) / 7f),
					CompositeFn.From(a1 => fn.Round(a1 * 7f) / 7f),
					CompositeFn.From(a1 => -a1),
					fn.Sin(beats * 1.57f * 0.25f),
					CompositeFn.From((a1) => pR.Call(p.Xy() * 2f, a1).X()),
					CompositeFn.From((a1) => pR.Call(p.Yz() * 2f, a1).Y()),
					CompositeFn.From((a1) => pR.Call(p.Zx() * 2f, a1).X()),
					CompositeFn.From((a1, a2) => (a1 / (1.3f / a2))),
					CompositeFn.From((a1, a2) => pR.Call(fn.Vec2(a1, a2), beats * 0.25f).X()),
					CompositeFn.From((a1, a2) => pR.Call(fn.Vec2(a1, a2), beats * 0.25f).Y()),
					CompositeFn.From((a1, a2) => pR.Call(fn.Vec2(p.X() * 1f, a1), a2).X()),
					CompositeFn.From((a1, a2) => pR.Call(fn.Vec2(p.Y() * 1f, a1), a2).Y()),
					CompositeFn.From((a1, a2) => pR.Call(fn.Vec2(p.Z() * 1f, a1), a2).Y()),
					CompositeFn.From((a1, a2) => fn.Length(fn.Vec2(a1, a2))),
					CompositeFn.From((a1) => pR.Call(p.Xy() * 1.5f, a1).Y()),
					CompositeFn.From(a1 => fn.Smoothstep(a1, 0f, 0.1f)),
					fn.Tan(beats * 3.14f * 0.5f) / 2f,
					fn.max_ff,
					CompositeFn.From((a1, a2) => a1 - a2),
					CompositeFn.From(a1 => fn.Tan(a1) / 2f),
					fn.Sin(beats * 1.57f * 0.5f) * 4f,
					fn.cos_f,
					CompositeFn.From((a1) => pR.Call(p.Zy() * 1.5f, a1).X()),
					//audio1,
					//audio2,
					//audio3,
					fn.Sin(beats * 1.57f * 0.25f),
					CompositeFn.From(a1 => fn.Round(a1 * 7f) / 7f),
					CompositeFn.From(a1 => -a1),
					CompositeFn.From(a1 => fn.Floor(a1 * 7f) / 7f),
				});

				//var l = scene.Declare<float>("l", _vc.Complicate(times: 10));
				//var sphere = scene.Declare<float>("sphere", sphereFunc.Call(p, 0.1f));
				//scene.Return(fn.Min(0.001f, sphere - l));

				//var l = scene.Declare<float>("l", _vc.Complicate(times: 10));
				//var sphere = scene.Declare<float>("sphere", sphereFunc.Call(fn.Vec3(p.X(), p.Y(), p.Z() / 10f), 1f));
				//scene.Return(fn.Min(0.001f, 1f + l - sphere * 1f));

				var l = scene.Declare<float>("l", _vc.Complicate(times: 10));
				var sphere = scene.Declare<float>("sphere", sphereFunc.Call(p, 1f));

				//var r = scene.Declare<Vector3>("r", fn.Vec3(_vc.Complicate(times: 10)) * new Vector3(0.68f, 0.18f, 0.14f));
				//var g = scene.Declare<Vector3>("g", fn.Vec3(_vc.Complicate(times: 10)) * new Vector3(0.87f, 0.89f, 0.78f));
				//var b = scene.Declare<Vector3>("b", fn.Vec3(_vc.Complicate(times: 10)) * new Vector3(0f, 0.6f, 0.96f));
				var r = scene.Declare<Vector3>("r", fn.Vec3(_vc.Complicate(times: 10)) * new Vector3(1f, 0f, 0f));
				var g = scene.Declare<Vector3>("g", fn.Vec3(_vc.Complicate(times: 10)) * new Vector3(0f, 1f, 0f));
				var b = scene.Declare<Vector3>("b", fn.Vec3(_vc.Complicate(times: 10)) * new Vector3(0f, 0f, 1f));

				var c = scene.Declare<Vector3>("c", fn.Clamp(r+g+b, 0.1f, 1f)) / l;
				scene.Return(fn.Vec4(c, fn.Min(0.01f, l + sphere)));
			}

			var march = body.DeclareFunction<Vector2, Vector3>("march", "uv");
			{
				var uv = march.A1;

				//var beaty = march.Declare<float>("beaty", (fn.Sin(beats * 3.14f * 1f) - 0.5f) * 2.7f);
				var beaty = march.Declare<float>("beaty", (fn.Sin(beats * 3.14f * 0.1f) - 0.5f) * 1f);
				//var cameraOrigin = march.Declare<Vector3>("cameraOrigin", fn.Vec3(0f + offset.X() + beaty, 0f + offset.Y(), 5f + time * 5f) * (1f + zoom * zoom));
				var cameraOrigin = march.Declare<Vector3>("cameraOrigin", fn.Vec3(0f + offset.X() + beaty, 0f + offset.Y(), 4f) * (1f + zoom * zoom));

				//march.Set(uv, uv * (1f + zoom * zoom));
				//march.Append("cameraOrigin += vec3(sin(Time) * 3., sin(Time * 0.4) * 0.5, sin(Time) * 1.)");
				var cameraTarget = march.Declare<Vector3>("cameraTarget", fn.Vec3(0f, 0f, 0f));
				//var cameraTarget = march.Declare<Vector3>("cameraTarget", cameraOrigin + fn.Vec3(0f, 0f, 1f));
				var upDirection = march.Declare<Vector3>("upDirection", fn.Vec3(0f, 1f, 0f));
				var cameraDir = march.Declare<Vector3>("cameraDir", fn.Normalize(cameraTarget - cameraOrigin));
				var cameraRight = march.Declare<Vector3>("cameraRight", fn.Normalize(fn.Cross(upDirection, cameraOrigin)));
				var cameraUp = march.Declare<Vector3>("cameraUp", fn.Cross(cameraDir, cameraRight));

				var rayDir = march.Declare<Vector3>("rayDir", fn.Normalize(cameraRight * uv.X() + cameraUp * uv.Y() + cameraDir));

				var maxDist = march.Declare<float>("MAX_DIST", 1000f);
				var epsilon = march.Declare<float>("EPSILON", 0.001f);

				var totalDist = march.Declare<float>("totalDist", 0f);
				var p = march.Declare<Vector3>("p", cameraOrigin);

				var dist = march.Declare<float>("dist", epsilon);
				var resultColor = march.Declare<Vector3>("resultColor", fn.Vec3(0f));

				var marchFor = march.For<int>(0, 1000, 1);
				{
					marchFor.Append("if (dist < EPSILON || totalDist > MAX_DIST) break");

					var result = marchFor.Declare<Vector4>("result", scene.Call(p));
					marchFor.Set(dist, result.W());
					marchFor.Set(resultColor, result.Xyz());
					marchFor.Set(totalDist, totalDist + dist);
					marchFor.Set(p, p + fn.Vec3(dist) * rayDir);
				}

				var c = march.Declare("c", fn.Vec3(0f));

				//march.Append("if(dist < EPSILON) {");
				march.Append("if(totalDist < MAX_DIST) {");

				var eps = march.Declare<Vector2>("eps", fn.Vec2(0f, 0.4f));
				var normal = march.Declare<Vector3>("normal", fn.Normalize(fn.Vec3(
					scene.Call(p + eps.Yxx()).W() - scene.Call(p - eps.Yxx()).W(),
					scene.Call(p + eps.Xyx()).W() - scene.Call(p - eps.Xyx()).W(),
					scene.Call(p + eps.Xxy()).W() - scene.Call(p - eps.Xxy()).W()
					)));
				var diffuse = march.Declare("diffuse", fn.Max(0f, fn.Dot(-rayDir, normal)));
				var specular = march.Declare("specular", fn.Pow(diffuse, 10f));
				march.Set(c, c + fn.Smoothstep(0f, 1.2f, diffuse + 0.05f) * 0.85f);
				march.Set(c, c + fn.Smoothstep(0f, 1f, specular) * 0.1f);
				march.Set(c, fn.Sqrt(c - fn.Vec3(0.1f)) * 1.05f);
				march.Set(c, c * resultColor);

				march.Append("} else {");



				march.Append("}");

				march.Return(c);
			}


			var main = body.DeclareFunction<_Void>("main");
			{
				var uv = main.Declare<Vector2>("uv", position);
				main.Set(uv, fn.Vec2(uv.X() * (16f / 9f), uv.Y()));

				// audio glitching
				var audio1 = main.Declare("audio1", fn.Max(0f, 0.2f - fn.Fract(beats * 2f + 0.0f)) * 0.6f);
				//var audio2 = main.Declare("audio2", fn.Max(0f, 0.1f - fn.Fract(beats * 4f + 0.05f - 0.4f)));
				//var audio3 = main.Declare("audio3", fn.Step(6.0f, (fn.Fract(beats / 2f) * 2f) * 4f) - fn.Step(8f, (fn.Fract(beats / 2f) * 2f) * 4f));
				main.Set(uv, fn.Vec2(uv.X() + (audio1 * (noise.Call(pR.Call(fn.Vec2(uv.Y()), time), 0.2f) - 0.2f)), uv.Y()));
				//main.Set(uv, fn.Vec2(uv.X() + (audio2 * (noise.Call(pR.Call(fn.Vec2(uv.Y()), time), 0.2f) - 0.2f)), uv.Y()));
				//main.Set(uv, fn.Vec2(uv.X() + (audio3 * (noise.Call(pR.Call(fn.Vec2(uv.Y()), time), 0.2f) - 0.12f)), uv.Y()));

				//var audio4 = main.Declare("audio4", fn.Max(0f, 0.1f - fn.Fract(beats * 0.5f - 1f)));
				//var audio5 = main.Declare("audio5", fn.Max(0f, 0.1f - fn.Fract(beats * 0.5f + 0.5f)));
				//main.Set(uv, fn.Vec2(uv.X() + (audio4 * 2f), uv.Y()));
				//main.Set(uv, fn.Vec2(uv.X() - (audio5 * 2f), uv.Y()));

				var c = main.Declare<Vector3>("c", new Vector3(0f));

				main.Set(c, march.Call(uv));

				//_vc = new ValueCollection(new IStatement[] {
				//	fn.sin_f,
				//	CompositeFn.From((a1, a2) => a1 + a2),
				//	fn.Length(uv) * 10f,
				//	uv.X() * 2f,
				//	uv.Y() * 1.5f,
				//	new ActualValue<float>(3.1416f),
				//	CompositeFn.From(a1 => fn.Floor(a1 * 7f) / 7f),
				//	CompositeFn.From(a1 => fn.Round(a1 * 7f) / 7f),
				//	CompositeFn.From(a1 => -a1),
				//	fn.Sin(beats * 1.57f * 0.25f),
				//	uv.X() * 10f,
				//	uv.Y() * 8f,
				//});

				//var l = main.Declare<float>("l", fn.Clamp(_vc.Complicate(times: 700), 0f, 0.95f));

				//main.Set(c, fn.Vec3(l));

				//main.Append("float colorFactor = max(0.01, 1.0 - (mod(beats, 0.5)) * 5.0)");
				//main.Append("c = round(c / colorFactor) * colorFactor");
				//main.Append("c *= 1. + colorFactor * 0.5");


				//main.Set(c, c + noise.Call(pR.Call(uv, time), 0.5f) * 0.021f); // noise
				//main.Set(c, fn.Max(c, fn.Vec3(0f))); // min 0
				//main.Set(c, c * fn.Smoothstep(0.1f, 0.3f, time)); // fade in

				main.Set(colorOut, fn.Vec4(c, 1f));
			}

			Console.WriteLine(body);

			return body.ToString();
		}



		public string Generate1(int it)
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

			var audio1 = main.Declare("audio1", fn.Max(0f, 0.1f - fn.Fract(beats * 4f + 0.04f)));
			var audio2 = main.Declare("audio2", fn.Max(0f, 0.1f - fn.Fract(beats * 4f + 0.05f - 0.4f)));
			var audio3 = main.Declare("audio3", fn.Step(6.35f, (fn.Fract(beats / 2f) * 2f) * 4f) - fn.Step(8f, (fn.Fract(beats / 2f) * 2f) * 4f));

			var uv = main.Declare<Vector2>("uv", position);

			main.Set(uv, fn.Vec2(uv.X() + (audio1 * (noise.Call(pR.Call(fn.Vec2(uv.Y()), time), 0.1f) - 0.1f)), uv.Y()));
			main.Set(uv, fn.Vec2(uv.X() + (audio2 * (noise.Call(pR.Call(fn.Vec2(uv.Y()), time), 0.1f) - 0.1f)), uv.Y()));
			main.Set(uv, fn.Vec2(uv.X() + (audio3 * (noise.Call(pR.Call(fn.Vec2(uv.Y()), time), 0.1f) - 0.1f)), uv.Y()));

			main.Set(uv, (uv / 2f) - offset);
			main.Set(uv, uv * (1f + zoom * zoom));

			var c = main.Declare<Vector3>("c", new Vector3(0f));

			//main.Set(uv, fn.Vec2(uv.X() + 0.1f * (spikeFunc.Call(timeD) * fn.Sin(timeD * 60f)), uv.Y()));

			_vc = new ValueCollection(new IStatement[] {
				fn.sin_f,
				CompositeFn.From((a1, a2) => a1 + a2),
				fn.Length(uv) * 10f,
			});

			if (it > 1)
				_vc.Append(new IStatement[] {
					uv.X() * 2f,
					uv.Y() * 1.5f,
				});

			if (it > 2)
				_vc.Append(new IStatement[] {
				new ActualValue<float>(3.1416f),
				CompositeFn.From(a1 => fn.Floor(a1 * 7f) / 7f),
			});

			if (it > 3)
				_vc.Append(new IStatement[] {
				CompositeFn.From(a1 => fn.Round(a1 * 7f) / 7f),
				CompositeFn.From(a1 => -a1),
			});

			if (it > 4)
				_vc.Append(new IStatement[] {
				fn.Sin(beats * 1.57f * 0.25f),
			});

			if (it > 5)
				_vc.Append(new IStatement[] {
				uv.X() * 10f,
				uv.Y() * 8f,
			});

			if (it > 6)
				_vc.Append(new IStatement[] {
				audio3,
			});

			if (it > 7)
				_vc.Append(new IStatement[] {
				audio1,
				audio2,
			});

			if (it > 8)
				_vc.Append(new IStatement[] {
				fn.cos_f,
			});

			if (it > 9)
				_vc.Append(new IStatement[] {
				fn.Sin(beats * 1.57f * 0.5f) * 4f,
			});

			if (it > 10)
				_vc.Append(new IStatement[] {
				CompositeFn.From(a1 => fn.Tan(a1) / 2f),
			});

			if (it > 11)
				_vc.Append(new IStatement[] {
				CompositeFn.From((a1, a2) => a1 - a2),
			});

			if (it > 12)
				_vc.Append(new IStatement[] {
				fn.max_ff,
			});

			if (it > 13)
				_vc.Append(new IStatement[] {
				fn.Tan(beats * 3.14f * 0.5f) / 2f,
			});

			if (it > 14)
				_vc.Append(new IStatement[] {
				CompositeFn.From(a1 => fn.Smoothstep(a1, 0f, 0.1f)),
			});

			if (it > 15)
				_vc.Append(new IStatement[] {
				CompositeFn.From((a1) => pR.Call(uv * 1.5f, a1).Y()),
			});

			if (it > 16)
				_vc.Append(new IStatement[] {
				CompositeFn.From((a1) => pR.Call(uv * 1.5f, a1).X()),
			});

			if (it > 17)
				_vc.Append(new IStatement[] {
				new ActualValue<float>(4f),
			});

			if (it > 18)
				_vc.Append(new IStatement[] {
				CompositeFn.From((a1, a2) => fn.Length(fn.Vec2(a1, a2))),
			});

			if (it > 19)
				_vc.Append(new IStatement[] {
				CompositeFn.From((a1, a2) => pR.Call(fn.Vec2(uv.X() * 10f, a1), a2).X()),
			});

			if (it > 20)
				_vc.Append(new IStatement[] {
				CompositeFn.From((a1, a2) => pR.Call(fn.Vec2(uv.Y() * 10f, a1), a2).Y()),
			});

			if (it > 21)
				_vc.Append(new IStatement[] {
				CompositeFn.From((a1, a2) => pR.Call(fn.Vec2(a1, a2), beats * 0.25f).X()),
				CompositeFn.From((a1, a2) => pR.Call(fn.Vec2(a1, a2), beats * 0.25f).Y()),
			});

			if (it > 22)
				_vc.Append(new IStatement[] {
				CompositeFn.From((a1) => pR.Call(uv * 5f, a1).X()),
				CompositeFn.From((a1) => pR.Call(uv * 5f, a1).Y()),
			});

			if (it > 23)
				_vc.Append(new IStatement[] {
				CompositeFn.From((a1, a2) => (a1 / (1.3f / a2))),
			});

			var r = main.Declare<Vector3>("r", new Vector3(0.68f, 0.18f, 0.14f));
			var g = main.Declare<Vector3>("g", new Vector3(0.87f, 0.89f, 0.78f));
			var b = main.Declare<Vector3>("b", new Vector3(0f, 0.6f, 0.96f));

			var l = main.Declare<float>("l", fn.Clamp(_vc.Complicate(times: 700), 0f, 0.95f));
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
			//var testa = main.Declare("testa", superclamp.Call(fn.Fract(beats * 4f), 0f, 0.4f));
			//var testb = main.Declare("testb", superclamp.Call(fn.Fract(((beats + 0.1f) * 4f)), 0f, 0.2f));
			//var testa = main.Declare("testa", .Call((beats * 4f + 0.05f)));
			//var testb = main.Declare("testb", spikeFunc.Call((beats * 4f + 0.05f - 0.1f)));
			//var testb = main.Declare("testb", spikeFunc.Call((beats + 0.4f) * 4f));

			//var t = fn.Step(0.1f, fn.Length(uv) - testa * 0.1f + testb * 0.1f - testc * 0.1f);
			//main.Set(c, fn.Vec3(t));

			main.Set(colorOut, fn.Vec4(c, 1f));
			return body.ToString();
		}
	}
}
