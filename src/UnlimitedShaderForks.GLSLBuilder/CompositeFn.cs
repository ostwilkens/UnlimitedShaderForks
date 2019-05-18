using System;

namespace UnlimitedShaderForks.GLSLBuilder
{
	public static class CompositeFn
	{
		public static CompositeFn<float, float> From(Func<Value<float>, Value<float>> func)
		{
			return new CompositeFn<float, float>(func);
		}

		public static CompositeFn<float, float, float> From(Func<Value<float>, Value<float>, Value<float>> func)
		{
			return new CompositeFn<float, float, float>(func);
		}
	}

	public class CompositeFn<T1, TResult> : IStatement, IFn<T1, TResult>
	{
		private Func<Value<T1>, Value<TResult>> func;
		public Func<Value<T1>, Value<TResult>> Func => func;

		public CompositeFn(Func<Value<T1>, Value<TResult>> func)
		{
			this.func = func;
		}

		public Value<TResult> Call(Value<T1> a1)
		{
			return new CompositeFnCall<T1, TResult>(this, a1);
		}
	}

	public class CompositeFn<T1, T2, TResult> : IStatement, IFn<T1, T2, TResult>
	{
		private Func<Value<T1>, Value<T2>, Value<TResult>> func;
		public Func<Value<T1>, Value<T2>, Value<TResult>> Func => func;

		public CompositeFn(Func<Value<T1>, Value<T2>, Value<TResult>> func)
		{
			this.func = func;
		}

		public Value<TResult> Call(Value<T1> a1, Value<T2> a2)
		{
			return new CompositeFnCall<T1, T2, TResult>(this, a1, a2);
		}
	}

	public class CompositeFn<T1, T2, T3, TResult> : IStatement, IFn<T1, T2, T3, TResult>
	{
		private Func<Value<T1>, Value<T2>, Value<T3>, Value<TResult>> func;
		public Func<Value<T1>, Value<T2>, Value<T3>, Value<TResult>> Func => func;

		public CompositeFn(Func<Value<T1>, Value<T2>, Value<T3>, Value<TResult>> func)
		{
			this.func = func;
		}

		public Value<TResult> Call(Value<T1> a1, Value<T2> a2, Value<T3> a3)
		{
			return new CompositeFnCall<T1, T2, T3, TResult>(this, a1, a2, a3);
		}
	}
}
