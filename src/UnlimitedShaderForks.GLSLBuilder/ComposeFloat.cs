namespace UnlimitedShaderForks.GLSLBuilder
{
	public class ComposeFloat<T> : Value<T>
	{
		private Value<T> a;
		private Value<float> b;
		private ComposeType composeType;

		public ComposeFloat(Value<T> a, Value<float> b, ComposeType composeType)
		{
			this.a = a;
			this.b = b;
			this.composeType = composeType;
		}

		public override string ToString()
		{
			return $"({a} {composeType.Print()} {b})";
		}
	}
}
