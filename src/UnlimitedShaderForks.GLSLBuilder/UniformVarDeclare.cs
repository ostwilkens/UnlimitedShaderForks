namespace UnlimitedShaderForks.GLSLBuilder
{
	public class UniformVarDeclare<T> : VarDeclare<T>
	{
		public UniformVarDeclare(Var<T> var) : base(var)
		{
		}

		public override string ToString()
		{
			return $"layout(set = 0, binding = 0) uniform _{var} {{ {base.ToString()}; }}";
		}
	}
}
