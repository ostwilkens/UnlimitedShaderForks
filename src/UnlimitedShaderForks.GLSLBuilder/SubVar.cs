namespace UnlimitedShaderForks.GLSLBuilder
{
	public class SubVar<TBase, TField> : Value<TField>
	{
		private Value<TBase> baseValue;
		private string fieldName;

		public SubVar(Value<TBase> baseValue, string fieldName)
		{
			this.baseValue = baseValue;
			this.fieldName = fieldName;
		}

		public override string ToString()
		{
			return $"{baseValue}.{fieldName}";
		}
	}
}
