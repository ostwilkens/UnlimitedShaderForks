using System;
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
		static void Main()
		{
			var windowCreateInfo = new WindowCreateInfo(100, 100, 1280, 720, WindowState.Normal, "Demo");
			var window = new Window(windowCreateInfo);
			var gen = new GLSLGenerator();
			window.FragmentCode = gen.Generate();

			while (window.Exists)
			{
				var inputSnapshot = window.Update();

				foreach (var keyEvent in inputSnapshot.KeyEvents)
				{
					if (!keyEvent.Down)
						continue;

					switch (keyEvent.Key)
					{
						case Key.Escape:
							window.Close();
							break;
						case Key.R:
							window.FragmentCode = gen.Generate();
							break;
					}
				}
			}
		}
	}
}
