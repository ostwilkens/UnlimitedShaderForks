using System;
using System.IO;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using UnlimitedShaderForks.GLSLBuilder;
using Veldrid;
using Veldrid.Sdl2;
using Veldrid.SPIRV;
using Veldrid.StartupUtilities;

namespace UnlimitedShaderForks
{
	class Program
	{
		static void Main(params string[] args)
		{
			var audio = Audio.Load("audio.mp3", 1f, true);
			var windowCreateInfo = new WindowCreateInfo(100, 100, 1280, 720, WindowState.Normal, "Demo");
			var window = new Window(windowCreateInfo, audio);
			var gen = new GLSLGenerator(1012);
			//var gen = new GLSLGenerator(1017);
			window.FragmentCode = args.Length > 0 ? File.ReadAllText(args[0]) : gen.Generate3(0);

			Vector2 lastGrabPos = new Vector2(0f);
			bool grabbing = false;
			int it = 0;
			while (window.Exists)
			{
				//int newit = (int)((window.Time.ElapsedSeconds + 0.4f) / 7.5f * 4f);
				//if(newit != it)
				//{
				//	it = newit;
				//	window.FragmentCode = gen.Generate(it);
				//}

				var inputSnapshot = window.Update();

				window.View.Zoom -= inputSnapshot.WheelDelta * 0.15f;

				if (grabbing)
				{
					window.View.Offset = (inputSnapshot.MousePosition - lastGrabPos) * 0.005f;
				}

				foreach (var mouseEvent in inputSnapshot.MouseEvents)
				{
					if (mouseEvent.Down)
					{
						switch (mouseEvent.MouseButton)
						{
							case MouseButton.Left:
								grabbing = true;
								lastGrabPos = inputSnapshot.MousePosition - window.View.Offset / 0.005f;
								break;
						}
					}
					else
					{
						switch (mouseEvent.MouseButton)
						{
							case MouseButton.Left:
								grabbing = false;
								break;
						}
					}
				}

				foreach (var keyEvent in inputSnapshot.KeyEvents)
				{
					if (keyEvent.Down)
					{
						switch (keyEvent.Key)
						{
							case Key.Escape:
								window.Close();
								break;
							case Key.R:
								window.Time.Restart();
								break;
							case Key.N:
								it++;
								window.FragmentCode = gen.Generate3(it);
								window.View.Offset = new Vector2(0f);
								window.View.Zoom = 0f;
								window.Time.Restart();
								break;
							case Key.J:
								window.Time.Step(-2);
								break;
							case Key.L:
								window.Time.Step(2);
								break;
							case Key.Comma:
								window.Time.Step(-1d / 60d);
								break;
							case Key.Period:
								window.Time.Step(1d / 60d);
								break;
							case Key.Z:
								window.Time.Timescale += -0.1d;
								break;
							case Key.X:
								window.Time.Timescale += 0.1d;
								break;
							case Key.C:
								window.Time.Timescale = 1.0d;
								break;
							//case Key.S:
							//	window.SyncAudio();
							//	break;
							//case Key.C:
							//	window.View.Offset = new Vector2(0f);
							//	window.View.Zoom = 0f;
							//	window.Time.Restart();
							//	audio.CurrentTime = window.Time.Elapsed;
							//	break;
							case Key.K:
							case Key.Space:
								window.Time.Toggle();
								break;
							case Key.S:
								File.WriteAllText($@"{DateTime.Now.Ticks}.frag", window.FragmentCode);
								break;
						}
					}
					//else
					//{
					//	switch (keyEvent.Key)
					//	{
					//		case Key.Space:
					//			spaceIsDown = false;
					//			break;
					//	}
					//}
				}
			}
		}
	}
}
