using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnlimitedShaderForks.GLSLBuilder
{
	public class OutVarDeclare<T> : VarDeclare<T>
	{
		public OutVarDeclare(Var<T> var) : base(var)
		{
		}

		public override string ToString()
		{
			return $"out {base.ToString()}";
		}
	}
}
