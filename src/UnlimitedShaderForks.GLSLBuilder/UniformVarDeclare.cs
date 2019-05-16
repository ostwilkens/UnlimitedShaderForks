namespace UnlimitedShaderForks.GLSLBuilder
{
	public class UniformVarDeclare<T> : VarDeclare<T>
	{
		public UniformVarDeclare(Var<T> var) : base(var)
		{
		}

		public override string ToString()
		{
			return $"uniform {base.ToString()}";
		}
	}
}
