namespace UnlimitedShaderForks.GLSLBuilder
{
	public class Compare<T> : Value<bool>
	{
		private Value<T> a, b;
		private CompareType compareType;

		public Compare(Value<T> a, Value<T> b, CompareType compareType)
		{
			this.a = a;
			this.b = b;
			this.compareType = compareType;
		}

		public override string ToString()
		{
			return $"({a} {compareType.Print()} {b})";
		}
	}
}
