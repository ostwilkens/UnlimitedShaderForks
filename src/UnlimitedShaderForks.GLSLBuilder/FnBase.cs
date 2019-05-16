using System.Text;

namespace UnlimitedShaderForks.GLSLBuilder
{
	public abstract class FnBase : ContainerBase
	{
		public string Name { get; private set; }

		public FnBase(string name)
		{
			Name = name;
		}

		internal virtual string Head { get; }

		public override string ToString()
		{
			StringBuilder sb = new StringBuilder();

			sb.AppendLine(Head);
			sb.AppendLine("{");
			sb.AppendLine(base.ToString());
			sb.AppendLine("}");

			return sb.ToString();
		}

		public void Return<T>(Value<T> value)
		{
			Append(new ReturnStatement<T>(value));
		}
	}
}
