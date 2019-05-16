namespace UnlimitedShaderForks.GLSLBuilder
{
	public class Fn<TResult> : FnBase, IFn<TResult>
	{
		public Fn(string name) : base(name)
		{
		}

		public Value<TResult> Call()
		{
			return new FnCall<TResult>(this);
		}

		override internal string Head => $"{typeof(TResult).Print()} {Name}()";
	}

	public class Fn<T1, TResult> : FnBase, IFn<T1, TResult>
	{
		private Var<T1> a1;
		protected VarDeclare<T1> a1Decl;

		public Var<T1> A1 => a1;

		public Fn(string name, string argname1 = "a1") : base(name)
		{
			this.a1 = new Var<T1>(argname1);
			this.a1Decl = new VarDeclare<T1>(this.a1);
		}

		public Value<TResult> Call(Value<T1> a1)
		{
			return new FnCall<T1, TResult>(this, a1);
		}

		override internal string Head => $"{typeof(TResult).Print()} {Name}({a1Decl})";
	}

	public class Fn<T1, T2, TResult> : FnBase, IFn<T1, T2, TResult>
	{
		private Var<T1> a1;
		protected VarDeclare<T1> a1Decl;
		private Var<T2> a2;
		protected VarDeclare<T2> a2Decl;

		public Var<T1> A1 => a1;
		public Var<T2> A2 => a2;

		public Fn(string name, string argname1 = "a1", string argname2 = "a2") : base(name)
		{
			this.a1 = new Var<T1>(argname1);
			this.a1Decl = new VarDeclare<T1>(this.a1);
			this.a2 = new Var<T2>(argname2);
			this.a2Decl = new VarDeclare<T2>(this.a2);
		}

		public Value<TResult> Call(Value<T1> a1, Value<T2> a2)
		{
			return new FnCall<T1, T2, TResult>(this, a1, a2);
		}

		override internal string Head => $"{typeof(TResult).Print()} {Name}({a1Decl}, {a2Decl})";
	}

	public class Fn<T1, T2, T3, TResult> : FnBase, IFn<T1, T2, T3, TResult>
	{
		private Var<T1> a1;
		protected VarDeclare<T1> a1Decl;
		private Var<T2> a2;
		protected VarDeclare<T2> a2Decl;
		private Var<T3> a3;
		protected VarDeclare<T3> a3Decl;

		public Var<T1> A1 => a1;
		public Var<T2> A2 => a2;
		public Var<T3> A3 => a3;

		public Fn(string name, string argname1 = "a1", string argname2 = "a2", string argname3 = "a3") : base(name)
		{
			this.a1 = new Var<T1>(argname1);
			this.a1Decl = new VarDeclare<T1>(this.a1);
			this.a2 = new Var<T2>(argname2);
			this.a2Decl = new VarDeclare<T2>(this.a2);
			this.a3 = new Var<T3>(argname3);
			this.a3Decl = new VarDeclare<T3>(this.a3);
		}

		public Value<TResult> Call(Value<T1> a1, Value<T2> a2, Value<T3> a3)
		{
			return new FnCall<T1, T2, T3, TResult>(this, a1, a2, a3);
		}

		override internal string Head => $"{typeof(TResult).Print()} {Name}({a1Decl}, {a2Decl}, {a3Decl})";
	}
}