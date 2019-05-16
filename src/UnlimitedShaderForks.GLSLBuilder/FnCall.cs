namespace UnlimitedShaderForks.GLSLBuilder
{
	public class FnCall<TResult> : Value<TResult>, IStatement
	{
		private Fn<TResult> fn;

		public FnCall(Fn<TResult> fn)
		{
			this.fn = fn;
		}

		public override string ToString()
		{
			return $"{fn.Name}()";
		}
	}

	public class FnCall<T1, TResult> : Value<TResult>, IStatement
	{
		private Fn<T1, TResult> fn;
		Value<T1> a1;

		public FnCall(Fn<T1, TResult> fn, Value<T1> a1)
		{
			this.fn = fn;
			this.a1 = a1;
		}

		public override string ToString()
		{
			return $"{fn.Name}({a1})";
		}
	}

	public class FnCall<T1, T2, TResult> : Value<TResult>, IStatement
	{
		private Fn<T1, T2, TResult> fn;
		Value<T1> a1;
		Value<T2> a2;

		public FnCall(Fn<T1, T2, TResult> fn, Value<T1> a1, Value<T2> a2)
		{
			this.fn = fn;
			this.a1 = a1;
			this.a2 = a2;
		}

		public override string ToString()
		{
			return $"{fn.Name}({a1}, {a2})";
		}
	}

	public class FnCall<T1, T2, T3, TResult> : Value<TResult>, IStatement
	{
		private Fn<T1, T2, T3, TResult> fn;
		Value<T1> a1;
		Value<T2> a2;
		Value<T3> a3;

		public FnCall(Fn<T1, T2, T3, TResult> fn, Value<T1> a1, Value<T2> a2, Value<T3> a3)
		{
			this.fn = fn;
			this.a1 = a1;
			this.a2 = a2;
			this.a3 = a3;
		}

		public override string ToString()
		{
			return $"{fn.Name}({a1}, {a2}, {a3})";
		}
	}
}
