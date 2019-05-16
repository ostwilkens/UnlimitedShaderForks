namespace UnlimitedShaderForks.GLSLBuilder
{
	public class ReturnStatement<T> : IStatement
	{
		private Value<T> value;

		public ReturnStatement(Value<T> value)
		{
			this.value = value;
		}

		public override string ToString()
		{
			return $"return {value}";
		}
	}
}
