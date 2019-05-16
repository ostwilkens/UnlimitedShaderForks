namespace UnlimitedShaderForks.GLSLBuilder
{
	public class ActualValue<T> : Value<T>
	{
		private T actualValue;

		public ActualValue(T actualValue)
		{
			this.actualValue = actualValue;
		}

		public override string ToString()
		{
			return this.Print();
		}

		public static implicit operator T(ActualValue<T> value) => value.actualValue;
	}
}
