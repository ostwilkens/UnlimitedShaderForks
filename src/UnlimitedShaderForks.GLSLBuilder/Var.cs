namespace UnlimitedShaderForks.GLSLBuilder
{
	public class Var<T> : Value<T>
	{
		public string Name { get; private set; }

		public Var(string name)
		{
			Name = name;
		}

		public override string ToString()
		{
			return Name;
		}
	}
}
