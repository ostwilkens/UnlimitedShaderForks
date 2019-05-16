using System;
using System.Numerics;
using System.Text;
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

			while (window.Exists)
			{
				var inputSnapshot = window.Update();

				foreach(var keyEvent in inputSnapshot.KeyEvents)
				{
					if (!keyEvent.Down)
						continue;

					switch(keyEvent.Key)
					{
						case Key.Escape:
							window.Close();
							break;
						case Key.R:
							window.FragmentCode = @"
#version 450

layout(location = 0) in vec2 fsin_Position;
layout(location = 0) out vec4 fsout_Color;

void main()
{
	fsout_Color = vec4(fsin_Position.x);
}";
							break;
	}
				}
			}
		}
	}
}
