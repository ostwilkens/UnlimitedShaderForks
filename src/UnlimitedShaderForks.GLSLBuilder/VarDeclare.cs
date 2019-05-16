namespace UnlimitedShaderForks.GLSLBuilder
{
	public class VarDeclare<T> : IStatement
	{
		private Var<T> var;

		public VarDeclare(Var<T> var)
		{
			this.var = var;
		}

		public string Prefix { get; set; }

		public override string ToString()
		{
			return string.IsNullOrWhiteSpace(Prefix) ?
				$"{typeof(T).Print()} {var}" :
				$"{Prefix} {typeof(T).Print()} {var}";
		}
	}
}
