using System.Text;

namespace UnlimitedShaderForks.GLSLBuilder
{
	public class For<TIndex> : ContainerBase
	{
		private VarDeclareAssign<TIndex> indexDeclAss;
		private Value<bool> condition;
		private VarAssign<TIndex> incr;
		private Var<TIndex> index;
		public Var<TIndex> Index => index;

		public For(Var<TIndex> index, Value<TIndex> init, Value<bool> condition, VarAssign<TIndex> incr)
		{
			this.index = index;
			indexDeclAss = new VarDeclareAssign<TIndex>(index, init);
			this.condition = condition;
			this.incr = incr;
		}

		public override string ToString()
		{
			var sb = new StringBuilder();

			sb.AppendLine($"for({indexDeclAss};{condition};{incr})");
			sb.AppendLine("{");
			sb.AppendLine(base.ToString());
			sb.AppendLine("}");

			return sb.ToString();
		}
	}
}
