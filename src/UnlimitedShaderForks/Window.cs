using System;
using System.Collections.Generic;
using System.Diagnostics;
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

		public bool Exists => _window.Exists;
		public void Close() => _window.Close();
		//public Stopwatch Time => _swLifetime;

		public View View { get; set; } = new View();

		public void SyncAudio() => _audio.CurrentTime = _time.Elapsed;

		public Window(WindowCreateInfo windowCreateInfo, Audio audio)
		{
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

			_cl = _factory.CreateCommandList();

			_textureRenderer = new TextureRenderer(_gd, DefaultShaders.FragmentCode, Time, View);
			_passthroughRenderer = new PassthroughRenderer(_gd, _textureRenderer.Texture, Time);

			audio.OnRepeat += (s, a) => SyncAudio();
			_time.OnStart += (s, a) => audio.Play();
			_time.OnStop += (s, a) => audio.Stop();
			_time.OnStep += (s, a) => SyncAudio();
			_time.OnTimescaleChanged += (s, a) => audio.PlaybackRate = (float)((Time)s).Timescale;
		}

		private void _window_Shown()
		{
			_time.Start();
			SyncAudio();
			//_audio.Play();
		}

		private void UpdateFps()
		{
			_framesThisSecond++;

			if (_swThisSecond.ElapsedMilliseconds >= 1000)
			{
				_window.Title = $"{_framesThisSecond} fps";
				_framesThisSecond = 0;
				_swThisSecond.Restart();
			}
		}

		public InputSnapshot Update()
		{
			UpdateFps();

			_textureRenderer.UpdateResources();
			_passthroughRenderer.UpdateResources();

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
