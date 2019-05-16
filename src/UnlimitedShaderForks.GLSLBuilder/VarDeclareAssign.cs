namespace UnlimitedShaderForks.GLSLBuilder
{
	public class VarDeclareAssign<T> : IStatement
	{
		private Var<T> var;
		private Value<T> val;

		public VarDeclareAssign(Var<T> var, Value<T> val)
		{
			this.var = var;
			this.val = val;
		}

		public override string ToString()
		{
			return $"{typeof(T).Print()} {var} = {val}";
		}
	}
}
