namespace UnlimitedShaderForks.GLSLBuilder
{
	public class Compose<T> : Value<T>
	{
		private Value<T> a, b;
		private ComposeType composeType;

		public Compose(Value<T> a, Value<T> b, ComposeType composeType)
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
