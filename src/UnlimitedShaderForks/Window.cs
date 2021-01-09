using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
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

			////{
			////	var span = new Span();
			////	var code = File.ReadAllText("636978760143850876.frag");
			////	var state = new FragmentCodeState(window, code);
			////	span.States.Add(state);
			////	_timeline.Spans.Add(span);
			////}


			{
				var span = new Span(0, 48 * b);
				var code = File.ReadAllText("636977216337209132.frag");
				var state = new FragmentCodeState(window, code);
				span.States.Add(state);
				_timeline.Spans.Add(span);
			}

			{
				var span = new Span(48 * b, 56 * b);
				var code = File.ReadAllText("636977750001312665.frag");
				var state = new FragmentCodeState(window, code);
				span.States.Add(state);
				_timeline.Spans.Add(span);
			}

			{
				var span = new Span(56 * b, 64 * b);
				var code = File.ReadAllText("636977752020441833.frag");
				var state = new FragmentCodeState(window, code);
				span.States.Add(state);
				_timeline.Spans.Add(span);
			}

			{
				var span = new Span(64 * b, 72 * b);
				var code = File.ReadAllText("636977754472853332.frag");
				var state = new FragmentCodeState(window, code);
				span.States.Add(state);
				_timeline.Spans.Add(span);
			}

			{
				var span = new Span(72 * b, 80 * b);
				var code = File.ReadAllText("636978491880687336.frag");
				var state = new FragmentCodeState(window, code);
				span.States.Add(state);
				_timeline.Spans.Add(span);
			}

			{
				var span = new Span(80 * b, 96 * b);
				var code = File.ReadAllText("636978754797521954.frag");
				var state = new FragmentCodeState(window, code);
				span.States.Add(state);
				_timeline.Spans.Add(span);
			}

			{
				var span = new Span(96 * b, 112 * b);
				var code = File.ReadAllText("636979550217165941.frag");
				var state = new FragmentCodeState(window, code);
				span.States.Add(state);
				_timeline.Spans.Add(span);
			}

			{
				var span = new Span(112 * b, 120 * b);
				var code = File.ReadAllText("636979550950767061.frag");
				var state = new FragmentCodeState(window, code);
				span.States.Add(state);
				_timeline.Spans.Add(span);
			}

			{
				var span = new Span(120 * b, 136.5 * b);
				var code = File.ReadAllText("636979551664200367.frag");
				var state = new FragmentCodeState(window, code);
				span.States.Add(state);
				_timeline.Spans.Add(span);
			}


			////{
			////	var span = new Span(72 * b, 80 * b);
			////	var code = File.ReadAllText("636977752990043352.frag");
			////	var state = new FragmentCodeState(window, code);
			////	span.States.Add(state);
			////	_timeline.Spans.Add(span);
			////}

			////{
			////	var span = new Span(80 * b, 88 * b);
			////	var code = File.ReadAllText("636977754573933250.frag");
			////	var state = new FragmentCodeState(window, code);
			////	span.States.Add(state);
			////	_timeline.Spans.Add(span);
			////}


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
				new GraphicsDeviceOptions { PreferStandardClipSpaceYDirection = true, SyncToVerticalBlank = true }, 
				GraphicsBackend.OpenGL);
			_factory = _gd.ResourceFactory;
			_audio = audio;

			_window.CursorVisible = true;

			_cl = _factory.CreateCommandList();

			_textureRenderer = new TextureRenderer(_gd, DefaultShaders.FragmentCode, Time, View);
			_passthroughRenderer = new PassthroughRenderer(_gd, _textureRenderer.Texture, Time);

			audio.OnRepeat += (s, a) => SyncAudio();
			//_time.OnStart += (s, a) => audio.Play();
			_time.OnStop += (s, a) => audio.Stop();
			_time.OnStep += (s, a) => SyncAudio();
			_time.OnTimescaleChanged += (s, a) => audio.PlaybackRate = (float)((Time)s).Timescale;

			//_time.SetTime(TimeSpan.FromSeconds(67));
			//_time.SetTime(TimeSpan.FromSeconds(59));
			//_time.SetTime(TimeSpan.FromSeconds(70));
		}

		private void PrecompileShaders()
		{
			DateTime a = DateTime.Now;

			var allStates = _timeline.Spans.SelectMany(s => s.States);
			var codeStates = allStates.Select(s => s as FragmentCodeState).Where(s => s != null);

			foreach(var state in codeStates)
			{
				_textureRenderer.FragmentCode = state.FragmentCode;
				_textureRenderer.ReloadShaders();
				this.Draw();
			}

			Console.WriteLine("Shaders compiled in " + (DateTime.Now - a).TotalSeconds.ToString("N3"));
		}

		private void _window_Shown()
		{
			PrecompileShaders();

			_time.Start();
			SyncAudio();
			//_audio.Play();
		}

		int timesResyncedAudio = 0;

		private void UpdateFps()
		{
			double b = 7.5f / 8.0f;
			double t = _time.Elapsed.TotalSeconds * b;

			_framesThisSecond++;

			if (_swThisSecond.ElapsedMilliseconds >= 1000)
			{
				var syncError = _audio.CurrentTime - _time.Elapsed;
				_window.Title = $"{_framesThisSecond} fps, {t:N2}, {syncError.TotalMilliseconds:N0}";

				if(Math.Abs(syncError.TotalMilliseconds) > 100 && timesResyncedAudio < 1)
				{
					Console.WriteLine("Resyncing audio");
					SyncAudio();
					timesResyncedAudio++;
				}

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
