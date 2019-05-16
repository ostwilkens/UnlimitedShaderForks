namespace UnlimitedShaderForks.GLSLBuilder
{
	public class DummyStatement : IStatement
	{
		private string statement;

		public DummyStatement(string statement)
		{
			this.statement = statement;
		}

		public override string ToString()
		{
			return statement;
		}
	}
}
