namespace UnlimitedShaderForks.GLSLBuilder
{
	public class NegativeValue<T> : Value<T>
	{
		private Value<T> value;

		public NegativeValue(Value<T> value)
		{
			this.value = value;
		}

		public override string ToString()
		{
			return $"(-{value})";
		}
	}
}
