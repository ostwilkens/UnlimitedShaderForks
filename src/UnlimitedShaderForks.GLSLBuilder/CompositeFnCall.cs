namespace UnlimitedShaderForks.GLSLBuilder
{
	public class CompositeFnCall<T1, TResult> : Value<TResult>, IStatement
	{
		private CompositeFn<T1, TResult> fn;
		Value<T1> a1;

		public CompositeFnCall(CompositeFn<T1, TResult> fn, Value<T1> a1)
		{
			this.fn = fn;
			this.a1 = a1;
		}

		public override string ToString()
		{
			return fn.Func(a1).ToString();
		}
	}

	public class CompositeFnCall<T1, T2, TResult> : Value<TResult>, IStatement
	{
		private CompositeFn<T1, T2, TResult> fn;
		Value<T1> a1;
		Value<T2> a2;

		public CompositeFnCall(CompositeFn<T1, T2, TResult> fn, Value<T1> a1, Value<T2> a2)
		{
			this.fn = fn;
			this.a1 = a1;
			this.a2 = a2;
		}

		public override string ToString()
		{
			return fn.Func(a1, a2).ToString();
		}
	}

	public class CompositeFnCall<T1, T2, T3, TResult> : Value<TResult>, IStatement
	{
		private CompositeFn<T1, T2, T3, TResult> fn;
		Value<T1> a1;
		Value<T2> a2;
		Value<T3> a3;

		public CompositeFnCall(CompositeFn<T1, T2, T3, TResult> fn, Value<T1> a1, Value<T2> a2, Value<T3> a3)
		{
			this.fn = fn;
			this.a1 = a1;
			this.a2 = a2;
			this.a3 = a3;
		}

		public override string ToString()
		{
			return fn.Func(a1, a2, a3).ToString();
		}
	}
}
