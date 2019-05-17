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
	public class Window
	{
		private Sdl2Window _window;
		private GraphicsDevice _gd;
		private ResourceFactory _factory;
		private CommandList _cl;
		private Stopwatch _swLifetime = new Stopwatch();
		private Stopwatch _swThisSecond = new Stopwatch();
		private int _framesThisSecond = 0;
		private TextureRenderer _textureRenderer;
		private PassthroughRenderer _passthroughRenderer;

		public bool Exists => _window.Exists;
		public void Close() => _window.Close();

		public Window(WindowCreateInfo windowCreateInfo)
		{
			_swLifetime.Start();
			_swThisSecond.Start();
			_window = VeldridStartup.CreateWindow(ref windowCreateInfo);
			_gd = VeldridStartup.CreateGraphicsDevice(
				_window,
				new GraphicsDeviceOptions { PreferStandardClipSpaceYDirection = true }, 
				GraphicsBackend.OpenGL);
			_factory = _gd.ResourceFactory;

			_cl = _factory.CreateCommandList();

			_textureRenderer = new TextureRenderer(_gd, DefaultShaders.FragmentCode, _swLifetime);
			_passthroughRenderer = new PassthroughRenderer(_gd, _textureRenderer.Texture);
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
				Console.WriteLine(value);
				_textureRenderer.ReloadShaders();
			}
		}
	}
}
