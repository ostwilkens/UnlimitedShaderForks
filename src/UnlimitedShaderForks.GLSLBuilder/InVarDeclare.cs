namespace UnlimitedShaderForks.GLSLBuilder
{
	public class InVarDeclare<T> : VarDeclare<T>
	{
		public InVarDeclare(Var<T> var) : base(var)
		{
		}

		public override string ToString()
		{
			return $"layout(location = 0) in {base.ToString()}";
		}
	}
}
