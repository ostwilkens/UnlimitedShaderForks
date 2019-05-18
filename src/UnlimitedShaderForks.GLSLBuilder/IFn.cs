namespace UnlimitedShaderForks.GLSLBuilder
{
	public interface IFn<TResult>
	{
		Value<TResult> Call();
	}

	public interface IFn<T1, TResult>
	{
		Value<TResult> Call(Value<T1> a1);
	}
	
	public interface IFn<T1, T2, TResult>
	{
		Value<TResult> Call(Value<T1> a1, Value<T2> a2);
	}

	public interface IFn<T1, T2, T3, TResult>
	{
		Value<TResult> Call(Value<T1> a1, Value<T2> a2, Value<T3> a3);
	}
}
