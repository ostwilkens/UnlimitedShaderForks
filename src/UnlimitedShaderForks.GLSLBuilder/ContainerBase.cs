using System.Collections.Generic;
using System.Text;

namespace UnlimitedShaderForks.GLSLBuilder
{
	public abstract class ContainerBase : IContainer, IStatement
	{
		private List<IStatement> statements = new List<IStatement>();

		public void Append(IStatement statement)
		{
			statements.Add(statement);
		}

		public void Append(string statement)
		{
			statements.Add(new DummyStatement(statement));
		}

		public Var<T> Declare<T>(string name)
		{
			var var = new Var<T>(name);
			var decl = new VarDeclare<T>(var);
			Append(decl);
			return var;
		}

		public Var<T> Declare<T>(string name, Value<T> val)
		{
			var var = new Var<T>(name);
			var declAss = new VarDeclareAssign<T>(var, val);
			Append(declAss);
			return var;
		}

		public void Set<T>(Var<T> var, Value<T> val)
		{
			var ass = new VarAssign<T>(var, val);
			Append(ass);
		}

		public For<T> For<T>(Value<T> init, Value<T> max, Value<T> incrementBy)
		{
			var index = new Var<T>("index");
			var condition = new Compare<T>(index, max, CompareType.Less);
			var incr = new VarAssign<T>(index, index + incrementBy);

			var forLoop = new For<T>(index, init, condition, incr);
			Append(forLoop);

			return forLoop;
		}

		public override string ToString()
		{
			var sb = new StringBuilder();
			foreach(var statement in statements)
			{
				sb.AppendLine($"{statement};");
			}

			return sb.ToString();
		}
	}
}
