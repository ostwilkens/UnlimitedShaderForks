using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace UnlimitedShaderForks.GLSLBuilder
{
	public class GLSLGenerator
	{
		public GLSLGenerator()
		{
		}

		public string Test()
		{
			var fn = new FunctionCollection();
			var body = new ShaderBody();
			var time = body.DeclareUniform<float>("_Time");
			var position = body.DeclareIn<Vector2>("fsin_Position");
			var color = body.DeclareOut<Vector4>("fsout_Color");

			var main = body.DeclareFunction<_Void>("main");
			var uv = main.Declare<Vector2>("uv", position);
			var c = main.Declare<Vector3>("c", fn.Vec3(time));
			main.Set(color, fn.Vec4(c, 1f));

			return body.ToString();
		}
	}
}
