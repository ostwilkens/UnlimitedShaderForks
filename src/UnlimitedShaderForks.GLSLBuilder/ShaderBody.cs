using System.Text;

namespace UnlimitedShaderForks.GLSLBuilder
{
	public class ShaderBody : ContainerBase
	{
		public Var<T> DeclareUniform<T>(string name, int binding)
		{
			var var = new Var<T>(name);
			var decl = new UniformVarDeclare<T>(var, binding);
			Append(decl);
			return var;
		}

		public Var<T> DeclareOut<T>(string name)
		{
			var var = new Var<T>(name);
			var decl = new OutVarDeclare<T>(var);
			Append(decl);
			return var;
		}

		public Var<T> DeclareIn<T>(string name)
		{
			var var = new Var<T>(name);
			var decl = new InVarDeclare<T>(var);
			Append(decl);
			return var;
		}

		public Fn<TResult> DeclareFunction<TResult>(string name)
		{
			var fn = new Fn<TResult>(name);
			Append(fn);
			return fn;
		}

		public Fn<T1, TResult> DeclareFunction<T1, TResult>(string name, string argname1 = "a1")
		{
			var fn = new Fn<T1, TResult>(name, argname1);
			Append(fn);
			return fn;
		}

		public Fn<T1, T2, TResult> DeclareFunction<T1, T2, TResult>(string name, string argname1 = "a1", string argname2 = "a2")
		{
			var fn = new Fn<T1, T2, TResult>(name, argname1, argname2);
			Append(fn);
			return fn;
		}

		public Fn<T1, T2, T3, TResult> DeclareFunction<T1, T2, T3, TResult>(string name, string argname1 = "a1", string argname2 = "a2", string argname3 = "a3")
		{
			var fn = new Fn<T1, T2, T3, TResult>(name, argname1, argname2, argname3);
			Append(fn);
			return fn;
		}

		public override string ToString()
		{
			var sb = new StringBuilder();
			sb.AppendLine("#version 450");
			sb.Append(base.ToString());
			return sb.ToString();
		}
	}
}
