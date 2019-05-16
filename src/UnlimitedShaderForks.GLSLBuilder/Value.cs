namespace UnlimitedShaderForks.GLSLBuilder
{
	public abstract class Value<T> : IStatement
	{
		public static Compose<T> operator +(Value<T> a, Value<T> b) => new Compose<T>(a, b, ComposeType.Add);
		public static ComposeFloat<T> operator +(Value<T> a, Value<float> b) => new ComposeFloat<T>(a, b, ComposeType.Add);

		public static Compose<T> operator -(Value<T> a, Value<T> b) => new Compose<T>(a, b, ComposeType.Subtract);
		public static ComposeFloat<T> operator -(Value<T> a, Value<float> b) => new ComposeFloat<T>(a, b, ComposeType.Subtract);

		public static NegativeValue<T> operator -(Value<T> a) => new NegativeValue<T>(a);

		public static Compose<T> operator /(Value<T> a, Value<T> b) => new Compose<T>(a, b, ComposeType.Divide);
		public static ComposeFloat<T> operator /(Value<T> a, Value<float> b) => new ComposeFloat<T>(a, b, ComposeType.Divide);

		public static Compose<T> operator *(Value<T> a, Value<T> b) => new Compose<T>(a, b, ComposeType.Multiply);
		public static ComposeFloat<T> operator *(Value<T> a, Value<float> b) => new ComposeFloat<T>(a, b, ComposeType.Multiply);

		public static implicit operator Value<T>(T value) => new ActualValue<T>(value);
	}
}
