using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Veldrid;
using Veldrid.SPIRV;

namespace UnlimitedShaderForks
{
	public struct TextureRendererArgs
	{
		public string FragmentCode { get; set; }
		public Stopwatch Stopwatch { get; set; }
		public View View { get; set; }

		public TextureRendererArgs(string fragmentCode, Stopwatch stopwatch, View view)
		{
			this.FragmentCode = fragmentCode;
			this.Stopwatch = stopwatch;
			this.View = view;
		}
	}

	public class TextureRenderer : RendererBase<TextureRendererArgs>
	{
		protected DeviceBuffer _timeBuffer;
		protected DeviceBuffer _offsetBuffer;
		protected DeviceBuffer _zoomBuffer;
		protected Texture _texture;
		public Texture Texture => _texture;
		public string FragmentCode { get; set; }
		public Stopwatch Stopwatch { get; set; }
		public View View { get; set; }

		public TextureRenderer(GraphicsDevice gd, string fragmentCode, Stopwatch sw, View view) : base(gd, new TextureRendererArgs(fragmentCode, sw, view))
		{
		}

		protected override void Initialize(TextureRendererArgs args)
		{
			this.FragmentCode = args.FragmentCode;
			this.Stopwatch = args.Stopwatch;
			this.View = args.View;

			_timeBuffer = _factory.CreateBuffer(new BufferDescription(16, BufferUsage.UniformBuffer));
			_offsetBuffer = _factory.CreateBuffer(new BufferDescription(16, BufferUsage.UniformBuffer));
			_zoomBuffer = _factory.CreateBuffer(new BufferDescription(16, BufferUsage.UniformBuffer));

			TextureDescription textureDesc = TextureDescription.Texture2D(
				_gd.SwapchainFramebuffer.Width / 4,
				_gd.SwapchainFramebuffer.Height / 4,
				1,
				1,
				PixelFormat.R16_G16_B16_A16_Float,
				TextureUsage.RenderTarget | TextureUsage.Sampled);
			_texture = _factory.CreateTexture(textureDesc);
		}

		protected override ResourceLayout GetResourceLayout()
		{
			return _factory.CreateResourceLayout(new ResourceLayoutDescription(
				new ResourceLayoutElementDescription("_Time", ResourceKind.UniformBuffer, ShaderStages.Fragment),
				new ResourceLayoutElementDescription("_Offset", ResourceKind.UniformBuffer, ShaderStages.Fragment),
				new ResourceLayoutElementDescription("_Zoom", ResourceKind.UniformBuffer, ShaderStages.Fragment)
				));
		}

		protected override ResourceSet GetResourceSet(ResourceLayout resourceLayout)
		{
			return _factory.CreateResourceSet(new ResourceSetDescription(resourceLayout, _timeBuffer, _offsetBuffer, _zoomBuffer));
		}

		protected override Framebuffer GetFramebuffer()
		{
			var framebufferDesc = new FramebufferDescription(null, _texture);
			return _factory.CreateFramebuffer(ref framebufferDesc);
		}

		protected override Shader[] GetShaders()
		{
			return _factory.CreateFromSpirv(
				new ShaderDescription(ShaderStages.Vertex, Encoding.UTF8.GetBytes(DefaultShaders.VertexCode), "main"),
				new ShaderDescription(ShaderStages.Fragment, Encoding.UTF8.GetBytes(FragmentCode), "main"), 
				new CrossCompileOptions(false, true));
		}

		public override void UpdateResources()
		{
			_gd.UpdateBuffer(_timeBuffer, 0, (float)Stopwatch.Elapsed.TotalSeconds);
			_gd.UpdateBuffer(_offsetBuffer, 0, View.Offset);
			_gd.UpdateBuffer(_zoomBuffer, 0, View.Zoom);
		}
	}
}
