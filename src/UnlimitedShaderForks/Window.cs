using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Numerics;
using System.Text;
using Veldrid;
using Veldrid.Sdl2;
using Veldrid.SPIRV;
using Veldrid.StartupUtilities;

namespace UnlimitedShaderForks
{
	public class View
	{
		public Vector2 Offset { get; set; } = new Vector2(0f);
		public float Zoom { get; set; } = 0f;
	}

	public class Window
	{
		private Sdl2Window _window;
		private GraphicsDevice _gd;
		private ResourceFactory _factory;
		private CommandList _cl;
		//private Stopwatch _swLifetime = new Stopwatch();
		private Stopwatch _swThisSecond = new Stopwatch();
		private int _framesThisSecond = 0;
		private TextureRenderer _textureRenderer;
		private PassthroughRenderer _passthroughRenderer;
		private Audio _audio;
		private Time _time = new Time();
		public Time Time => _time;
		private Timeline _timeline;

		public bool Exists => _window.Exists;
		public void Close() => _window.Close();
		//public Stopwatch Time => _swLifetime;

		public View View { get; set; } = new View();

		public void SyncAudio() => _audio.CurrentTime = _time.Elapsed;

		void BuildTimeline(Window window)
		{
			float b = 7.5f / 8.0f;

			_timeline = new Timeline();
			_timeline.OnRepeat += (obj, args) => this.Close();

			{
				var span = new Span();
				var code = File.ReadAllText("636948464617234550.frag");
				var state = new FragmentCodeState(window, code);
				span.States.Add(state);
				_timeline.Spans.Add(span);
			}

			//{
			//	var span = new Span(0, 12 * b);
			//	var code = File.ReadAllText("1.frag");
			//	var state = new FragmentCodeState(window, code);
			//	span.States.Add(state);
			//	_timeline.Spans.Add(span);
			//}

			//{
			//	var span = new Span(12 * b, 20 * b);
			//	var code = File.ReadAllText("636948352810076602.frag");
			//	var state = new FragmentCodeState(window, code);
			//	span.States.Add(state);
			//	_timeline.Spans.Add(span);
			//}

			//{
			//	var span = new Span(20 * b - 0.15f, 28 * b);
			//	var code = File.ReadAllText("636948465444846427.frag");
			//	var state = new FragmentCodeState(window, code);
			//	span.States.Add(state);
			//	_timeline.Spans.Add(span);
			//}

			//{
			//	var span = new Span(28 * b - 0.11f, 36 * b);
			//	var code = File.ReadAllText("636948464954324415.frag");
			//	var state = new FragmentCodeState(window, code);
			//	span.States.Add(state);
			//	_timeline.Spans.Add(span);
			//}

			//{
			//	var span = new Span(36 * b, 40 * b);
			//	var code = File.ReadAllText("636948464617234550.frag");
			//	var state = new FragmentCodeState(window, code);
			//	span.States.Add(state);
			//	_timeline.Spans.Add(span);
			//}

			//{
			//	var span = new Span(40 * b, 43.5 * b);
			//	var code = File.ReadAllText("636948386929941764.frag");
			//	var state = new FragmentCodeState(window, code);
			//	span.States.Add(state);
			//	_timeline.Spans.Add(span);
			//}

			//{
			//	var span = new Span(43.5 * b, 76 * b);
			//	var code = File.ReadAllText("2.frag");
			//	var state = new FragmentCodeState(window, code);
			//	span.States.Add(state);
			//	_timeline.Spans.Add(span);
			//}

			//{
			//	var span = new Span(76 * b - 0.1f, 80 * b);
			//	var code = File.ReadAllText("636948385754020679.frag");
			//	var state = new FragmentCodeState(window, code);
			//	span.States.Add(state);
			//	_timeline.Spans.Add(span);
			//}


			//{
			//	var span = new Span(80 * b - 0.1f, 84 * b);
			//	var code = File.ReadAllText("636948484085218078.frag");
			//	var state = new FragmentCodeState(window, code);
			//	span.States.Add(state);
			//	_timeline.Spans.Add(span);
			//}

			//{
			//	var span = new Span(84 * b - 0.15f, 92 * b);
			//	var code = File.ReadAllText("636948481894290834.frag");
			//	var state = new FragmentCodeState(window, code);
			//	span.States.Add(state);
			//	_timeline.Spans.Add(span);
			//}

			//{
			//	var span = new Span(92 * b - 0.1f, 94 * b);
			//	var code = File.ReadAllText("636948480007839155.frag");
			//	var state = new FragmentCodeState(window, code);
			//	span.States.Add(state);
			//	_timeline.Spans.Add(span);
			//}

			//{
			//	var span = new Span(94 * b - 0.1f, 96 * b);
			//	var code = File.ReadAllText("636948485405048997.frag"); 
			//	var state = new FragmentCodeState(window, code);
			//	span.States.Add(state);
			//	_timeline.Spans.Add(span);
			//}

			//{
			//	var span = new Span(96 * b - 0.1f, 98 * b);
			//	var code = File.ReadAllText("636948482043142647.frag");
			//	var state = new FragmentCodeState(window, code);
			//	span.States.Add(state);
			//	_timeline.Spans.Add(span);
			//}

			//{
			//	var span = new Span(98 * b - 0.1f, 100 * b);
			//	var code = File.ReadAllText("636948485201203365.frag");
			//	var state = new FragmentCodeState(window, code);
			//	span.States.Add(state);
			//	_timeline.Spans.Add(span);
			//}

			//{
			//	var span = new Span(100 * b, 102 * b);
			//	var code = File.ReadAllText("636948372115734603.frag");
			//	var state = new FragmentCodeState(window, code);
			//	span.States.Add(state);
			//	_timeline.Spans.Add(span);
			//}

			//{
			//	var span = new Span(102 * b, 104 * b);
			//	var code = File.ReadAllText("636948378712242059.frag");
			//	var state = new FragmentCodeState(window, code);
			//	span.States.Add(state);
			//	_timeline.Spans.Add(span);
			//}

			//{
			//	var span = new Span(104 * b, 106 * b);
			//	var code = File.ReadAllText("636948379994976533.frag");
			//	var state = new FragmentCodeState(window, code);
			//	span.States.Add(state);
			//	_timeline.Spans.Add(span);
			//}

			//{
			//	var span = new Span(106 * b, 108 * b);
			//	var code = File.ReadAllText("636948383617549899.frag");
			//	var state = new FragmentCodeState(window, code);
			//	span.States.Add(state);
			//	_timeline.Spans.Add(span);
			//}

			//{
			//	var span = new Span(108 * b, 112 * b);
			//	var code = File.ReadAllText("636948383663384620.frag");
			//	var state = new FragmentCodeState(window, code);
			//	span.States.Add(state);
			//	_timeline.Spans.Add(span);
			//}

			//{
			//	var span = new Span(112 * b, 116 * b);
			//	var code = File.ReadAllText("636948379889793952.frag");
			//	var state = new FragmentCodeState(window, code);
			//	span.States.Add(state);
			//	_timeline.Spans.Add(span);
			//}

			//{
			//	var span = new Span(116 * b, 120 * b);
			//	var code = File.ReadAllText("2.frag");
			//	var state = new FragmentCodeState(window, code);
			//	span.States.Add(state);
			//	_timeline.Spans.Add(span);
			//}
		}

		public Window(WindowCreateInfo windowCreateInfo, Audio audio)
		{
			BuildTimeline(this);
			_timeline.Execute(_time.Elapsed);
			//_swLifetime.Start();
			_swThisSecond.Start();
			_window = VeldridStartup.CreateWindow(ref windowCreateInfo);
			_window.Shown += _window_Shown;
			_gd = VeldridStartup.CreateGraphicsDevice(
				_window,
				new GraphicsDeviceOptions { PreferStandardClipSpaceYDirection = true, SyncToVerticalBlank = false }, 
				GraphicsBackend.OpenGL);
			_factory = _gd.ResourceFactory;
			_audio = audio;

			_window.CursorVisible = false;

			_cl = _factory.CreateCommandList();

			_textureRenderer = new TextureRenderer(_gd, DefaultShaders.FragmentCode, Time, View);
			_passthroughRenderer = new PassthroughRenderer(_gd, _textureRenderer.Texture, Time);

			audio.OnRepeat += (s, a) => SyncAudio();
			_time.OnStart += (s, a) => audio.Play();
			_time.OnStop += (s, a) => audio.Stop();
			_time.OnStep += (s, a) => SyncAudio();
			_time.OnTimescaleChanged += (s, a) => audio.PlaybackRate = (float)((Time)s).Timescale;

			//_time.SetTime(TimeSpan.FromSeconds(108));
		}

		private void _window_Shown()
		{
			_time.Start();
			SyncAudio();
			//_audio.Play();
		}

		private void UpdateFps()
		{
			double b = 7.5f / 8.0f;
			double t = _time.Elapsed.TotalSeconds * b;

			_framesThisSecond++;

			if (_swThisSecond.ElapsedMilliseconds >= 1000)
			{
				_window.Title = $"{_framesThisSecond} fps, {t:N2}";
				_framesThisSecond = 0;
				_swThisSecond.Restart();
			}
		}

		public InputSnapshot Update()
		{
			UpdateFps();

			_textureRenderer.UpdateResources();
			_passthroughRenderer.UpdateResources();

			_timeline.Execute(_time.Elapsed);
			Draw();
			return _window.PumpEvents();
		}

		private void Draw()
		{
			_cl.Begin();

			_textureRenderer.Draw(_cl);
			_passthroughRenderer.Draw(_cl);

			_cl.End();
			_gd.SubmitCommands(_cl);
			_gd.SwapBuffers();
		}

		public string FragmentCode
		{
			get => _textureRenderer.FragmentCode;
			set
			{
				_textureRenderer.FragmentCode = value;
				//Console.WriteLine(value);
				_textureRenderer.ReloadShaders();
			}
		}
	}
}
