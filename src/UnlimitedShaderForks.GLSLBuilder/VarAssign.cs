namespace UnlimitedShaderForks.GLSLBuilder
{
	public class VarAssign<T> : IStatement
	{
		private Var<T> var;
		private Value<T> val;

		public VarAssign(Var<T> var, Value<T> val)
		{
			this.var = var;
			this.val = val;
		}

		public override string ToString()
		{
			return $"{var} = {val}";
		}
	}
}
