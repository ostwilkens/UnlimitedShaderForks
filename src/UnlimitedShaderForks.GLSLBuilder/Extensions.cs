using System;
using System.Numerics;

namespace UnlimitedShaderForks.GLSLBuilder
{
	public static class Extensions
	{
		public static T GetRandomItem<T>(this T[] array, Random rand)
		{
			int i = rand.Next(0, array.Length);
			return array[i];
		}

		public static Type GetInterfaceStartsWith(this Type t, string name)
		{
			foreach(var i in t.GetInterfaces())
			{
				if(i.Name.StartsWith(name))
					return i;
			}

			throw new Exception("Interface not found");
		}

		public static Type GetInterfaceStartsWithOrDefault(this Type t, string name)
		{
			foreach (var i in t.GetInterfaces())
			{
				if (i.Name.StartsWith(name))
					return i;
			}

			return null;
		}

		internal static string Print(this Type type)
		{
			if(type == typeof(Vector2))
				return "vec2";
			else if(type == typeof(Vector3))
				return "vec3";
			else if(type == typeof(Vector4))
				return "vec4";
			else if(type == typeof(float))
				return "float";
			else if(type == typeof(int))
				return "int";
			else if(type == typeof(_Void))
				return "void";

			throw new NotImplementedException();
		}

		internal static string Print<T>(this ActualValue<T> value)
		{
			return Print((T)value);
		}

		private static string Print<T>(T value)
		{
			var type = typeof(T);

            if (type == typeof(float))
                return Print((float)(object)value);
            else if (type == typeof(Vector2))
                return Print((Vector2)(object)value);
            else if (type == typeof(Vector3))
                return Print((Vector3)(object)value);
            else if (type == typeof(Vector4))
                return Print((Vector4)(object)value);
            else if (type == typeof(int))
                return Print((int)(object)value);

			throw new NotImplementedException();
		}

        private static string Print(int value)
        {
            return value.ToString();
        }

		private static string Print(float value)
		{
			return value.ToString("0.0###############################", System.Globalization.CultureInfo.InvariantCulture);
		}

		private static string Print(Vector2 value)
		{
			if(value.X == value.Y)
				return $"vec2({Print(value.X)})";
			else
				return $"vec2({Print(value.X)}, {Print(value.Y)})";
		}

		private static string Print(Vector3 value)
		{
			if(value.X == value.Y && value.X == value.Z)
				return $"vec3({Print(value.X)})";
			else
				return $"vec3({Print(value.X)}, {Print(value.Y)}, {Print(value.Z)})";
		}

		private static string Print(Vector4 value)
		{
			if(value.X == value.Y && value.X == value.Z && value.X == value.W)
				return $"vec4({Print(value.X)})";
			else
				return $"vec4({Print(value.X)}, {Print(value.Y)}, {Print(value.Z)}, {Print(value.W)})";
		}

		internal static string Print(this CompareType compareType)
		{
			switch(compareType)
			{
				case CompareType.Equal:
				return "==";
				case CompareType.EqualOrGreater:
				return ">=";
				case CompareType.EqualOrLess:
				return "<=";
				case CompareType.Greater:
				return ">";
				case CompareType.Less:
				return "<";
				case CompareType.NotEqual:
				return "!=";
				default:
				throw new NotImplementedException();
			}
		}

		internal static string Print(this ComposeType composeType)
		{
			switch(composeType)
			{
				case ComposeType.Add:
				return "+";
				case ComposeType.Divide:
				return "/";
				case ComposeType.Multiply:
				return "*";
				case ComposeType.Subtract:
				return "-";
				default:
				throw new NotImplementedException();
			}
		}

		public static SubVar<Vector2, Vector3> Xyx(this Value<Vector2> value)
		{
			return new SubVar<Vector2, Vector3>(value, "xyx");
        }

        public static SubVar<Vector4, Vector2> Xy(this Value<Vector4> value)
        {
            return new SubVar<Vector4, Vector2>(value, "xy");
        }

        public static SubVar<Vector4, Vector3> Xyz(this Value<Vector4> value)
        {
            return new SubVar<Vector4, Vector3>(value, "xyz");
        }

        public static SubVar<Vector4, float> W(this Value<Vector4> value)
        {
            return new SubVar<Vector4, float>(value, "w");
        }

        public static SubVar<Vector2, Vector2> Yx(this Value<Vector2> value)
		{
			return new SubVar<Vector2, Vector2>(value, "yx");
		}

		public static SubVar<Vector2, Vector2> Xx(this Value<Vector2> value)
		{
			return new SubVar<Vector2, Vector2>(value, "xx");
		}

		public static SubVar<Vector2, Vector2> Yy(this Value<Vector2> value)
		{
			return new SubVar<Vector2, Vector2>(value, "yy");
		}

		public static SubVar<Vector2, float> X(this Value<Vector2> value)
		{
			return new SubVar<Vector2, float>(value, "x");
		}

		public static SubVar<Vector2, float> Y(this Value<Vector2> value)
		{
			return new SubVar<Vector2, float>(value, "y");
		}

		public static SubVar<Vector3, float> X(this Value<Vector3> value)
		{
			return new SubVar<Vector3, float>(value, "x");
		}

		public static SubVar<Vector3, float> Y(this Value<Vector3> value)
		{
			return new SubVar<Vector3, float>(value, "y");
		}

		public static SubVar<Vector3, float> Z(this Value<Vector3> value)
		{
			return new SubVar<Vector3, float>(value, "z");
        }

        public static SubVar<Vector3, Vector2> Xy(this Value<Vector3> value)
        {
            return new SubVar<Vector3, Vector2>(value, "xy");
        }

        public static SubVar<Vector3, Vector2> Xz(this Value<Vector3> value)
        {
            return new SubVar<Vector3, Vector2>(value, "xz");
        }

        public static SubVar<Vector3, Vector2> Yx(this Value<Vector3> value)
		{
			return new SubVar<Vector3, Vector2>(value, "yx");
		}

		public static SubVar<Vector3, Vector2> Zx(this Value<Vector3> value)
		{
			return new SubVar<Vector3, Vector2>(value, "zx");
		}

		public static SubVar<Vector3, Vector2> Zy(this Value<Vector3> value)
		{
			return new SubVar<Vector3, Vector2>(value, "zy");
		}

		public static SubVar<Vector3, Vector2> Yz(this Value<Vector3> value)
		{
			return new SubVar<Vector3, Vector2>(value, "yz");
		}

		public static SubVar<Vector3, Vector3> Xzy(this Value<Vector3> value)
		{
			return new SubVar<Vector3, Vector3>(value, "xzy");
		}

		public static SubVar<Vector3, Vector3> Zyx(this Value<Vector3> value)
		{
			return new SubVar<Vector3, Vector3>(value, "zyx");
		}

		public static SubVar<Vector3, Vector3> Zxy(this Value<Vector3> value)
		{
			return new SubVar<Vector3, Vector3>(value, "zxy");
		}
	}
}
