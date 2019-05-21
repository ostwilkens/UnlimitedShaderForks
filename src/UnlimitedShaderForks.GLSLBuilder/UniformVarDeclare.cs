namespace UnlimitedShaderForks.GLSLBuilder
{
	public class UniformVarDeclare<T> : VarDeclare<T>
	{
		private int binding;

		public UniformVarDeclare(Var<T> var, int binding) : base(var)
		{
			this.binding = binding;
		}

		public override string ToString()
		{
			return $"layout(set = 0, binding = {binding}) uniform _{var} {{ {base.ToString()}; }}";
		}
	}
}
