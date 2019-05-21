using System;
using System.IO;
using System.Numerics;
using System.Text;
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
			var windowCreateInfo = new WindowCreateInfo(100, 100, 1280, 720, WindowState.Normal, "Demo");
			var window = new Window(windowCreateInfo);
			var gen = new GLSLGenerator();
			window.FragmentCode = args.Length > 0 ? File.ReadAllText(args[0]) : gen.Generate();

			Vector2 lastGrabPos = new Vector2(0f);
			bool grabbing = false;
			while (window.Exists)
			{
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
								window.FragmentCode = gen.Generate();
								window.View.Offset = new Vector2(0f);
								window.View.Zoom = 0f;
								break;
							case Key.C:
								window.View.Offset = new Vector2(0f);
								window.View.Zoom = 0f;
								window.Time.Restart();
								break;
							case Key.Space:
								if(window.Time.IsRunning)
									window.Time.Stop();
								else
									window.Time.Start();
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
