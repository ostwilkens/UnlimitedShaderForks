using System;
using System.Collections.Generic;
using System.Linq;

namespace UnlimitedShaderForks
{
	public class Timeline
	{
		public List<Span> Spans { get; set; } = new List<Span>();
		public TimeSpan TotalDuration => Spans.Where(s => !s.IsForever).DefaultIfEmpty(new Span()).Max(s => s.End);
		public TimeSpan TimeModDuration(TimeSpan time) => TimeSpan.FromTicks(time.Ticks % TotalDuration.Ticks);
		public int RepeatCount(TimeSpan time) => (int)(time.Ticks / TotalDuration.Ticks);
		public event EventHandler OnRepeat;

		public IEnumerable<IState> GetActiveStates(TimeSpan time)
		{
			time = TimeModDuration(time);

			var states = new List<IState>();
			foreach (var span in Spans)
			{
				foreach (var state in span.States)
				{
					if (state.IsActive)
					{
						states.Add(state);
					}
				}
			}

			return states.OrderBy(s => s.ZIndex);
		}

		private int previousRepeatCount = 0;
		public void Execute(TimeSpan time)
		{
			int repeatCount = RepeatCount(time);
			if (repeatCount != previousRepeatCount)
			{
				previousRepeatCount = repeatCount;
				OnRepeat?.Invoke(null, EventArgs.Empty);
			}

			time = TimeModDuration(time);

			foreach (var span in Spans)
			{
				bool shouldBeActive = time > span.Start && time < span.End;

				foreach (var state in span.States)
				{
					if (state.IsActive && !shouldBeActive)
					{
						state.Leave();
					}
					else if (!state.IsActive && shouldBeActive)
					{
						state.Enter();
					}
				}
			}
		}
	}

	public class Span
	{
		public TimeSpan Start { get; set; }
		public TimeSpan End { get; set; }
		public bool IsForever => this.End == TimeSpan.MaxValue;
		public List<IState> States { get; set; } = new List<IState>();

		public Span(TimeSpan start, TimeSpan end)
		{
			this.Start = start;
			this.End = end;
		}

		public Span(double start, double end)
		{
			this.Start = TimeSpan.FromSeconds(start);
			this.End = TimeSpan.FromSeconds(end);
		}

		/// <summary>
		/// Forever
		/// </summary>
		public Span()
		{
			this.Start = TimeSpan.Zero;
			this.End = TimeSpan.MaxValue;
		}
	}

	//public class DisplayTextState : IState
	//{
	//	private QFont qFont;
	//	private QFontDrawing qFontDrawing;
	//	private Vector2 pos;
	//	private Func<string> textFunc;
	//	private QFontAlignment alignment;

	//	private bool isActive;
	//	public bool IsActive => isActive;

	//	public int ZIndex => 2;

	//	public DisplayTextState(string font, float size, Vector2 pos, Matrix4 projectionMatrix, Func<string> textFunc, QFontAlignment alignment = QFontAlignment.Left)
	//	{
	//		qFont = new QFont(font, size, new QFontBuilderConfiguration(true));
	//		qFontDrawing = new QFontDrawing();
	//		this.pos = pos;
	//		qFontDrawing.ProjectionMatrix = projectionMatrix;
	//		this.textFunc = textFunc;
	//		this.alignment = alignment;
	//	}

	//	public void Enter()
	//	{
	//		this.isActive = true;
	//	}

	//	public void Load()
	//	{
	//	}

	//	public void Draw()
	//	{
	//		string text = textFunc();

	//		qFontDrawing.DrawingPrimitives.Clear();
	//		qFontDrawing.Print(qFont, text, new Vector3(pos), alignment);
	//		qFontDrawing.RefreshBuffers();
	//		qFontDrawing.Draw();
	//	}

	//	public void SetUniforms()
	//	{
	//	}

	//	public void Leave()
	//	{
	//		this.isActive = false;
	//	}
	//}

	public class FragmentCodeState : IState
	{
		private Window _window;
		private string FragmentCode { get; set; }

		private bool isActive;
		public bool IsActive => isActive;

		public int ZIndex => 1;

		public FragmentCodeState(Window window, string fragmentCode)
		{
			this._window = window;
			this.FragmentCode = fragmentCode;
		}

		public void Enter()
		{
			this.isActive = true;
			_window.FragmentCode = FragmentCode;
		}

		public void Leave()
		{
			this.isActive = false;
		}

		public void Draw()
		{
		}

		public void SetUniforms()
		{
		}
	}

	//public class FragmentShaderState : IState
	//{
	//	private Action<ShaderProgram> setCurrentProgramAction;
	//	private ShaderProgram program;
	//	public ShaderProgram Program => program;
	//	//private IUniform[] uniforms;

	//	private bool isActive;
	//	public bool IsActive => isActive;

	//	public int ZIndex => 1;

	//	public FragmentShaderState(Action<ShaderProgram> setCurrentProgramAction, ShaderProgram program)
	//	{
	//		this.setCurrentProgramAction = setCurrentProgramAction;
	//		this.program = program;
	//		//this.uniforms = uniforms;
	//		this.isActive = false;

	//		Load();
	//	}

	//	public void Enter()
	//	{
	//		setCurrentProgramAction(program);
	//		this.isActive = true;
	//	}

	//	public void Load()
	//	{
	//		//foreach (var uniform in uniforms)
	//		//{
	//		//	uniform.Id = GL.GetUniformLocation(program.Id, uniform.Name);
	//		//	program.CheckUniformLocation(uniform.Name);
	//		//}
	//	}

	//	public void Draw()
	//	{
	//	}

	//	public void SetUniforms()
	//	{
	//		foreach (var uniform in uniforms)
	//		{
	//			if (uniform is Uniform<float>)
	//			{
	//				GL.Uniform1(uniform.Id.Value, (float)uniform.GetValue());
	//			}
	//			else if (uniform is Uniform<Vector2>)
	//			{
	//				GL.Uniform2(uniform.Id.Value, (Vector2)uniform.GetValue());
	//			}
	//			else
	//			{
	//				throw new NotImplementedException("Uniform type not implemented");
	//			}
	//		}
	//	}

	//	public void Leave()
	//	{
	//		this.isActive = false;
	//	}
	//}

	public interface IState
	{
		bool IsActive { get; }
		void Enter();
		//void Load();
		void Draw();
		void SetUniforms();
		void Leave();
		int ZIndex { get; }
	}
}
