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

		public TextureRendererArgs(string fragmentCode, Stopwatch stopwatch)
		{
			this.FragmentCode = fragmentCode;
			this.Stopwatch = stopwatch;
		}
	}

	public class TextureRenderer : RendererBase<TextureRendererArgs>
	{
		protected DeviceBuffer _timeBuffer;
		protected Texture _texture;
		public Texture Texture => _texture;
		public string FragmentCode { get; set; }
		public Stopwatch Stopwatch { get; set; }

		public TextureRenderer(GraphicsDevice gd, string fragmentCode, Stopwatch sw) : base(gd, new TextureRendererArgs(fragmentCode, sw))
		{
		}

		protected override void Initialize(TextureRendererArgs args)
		{
			this.FragmentCode = args.FragmentCode;
			this.Stopwatch = args.Stopwatch;

			_timeBuffer = _factory.CreateBuffer(new BufferDescription(16, BufferUsage.UniformBuffer));

			TextureDescription textureDesc = TextureDescription.Texture2D(
				_gd.SwapchainFramebuffer.Width / 8,
				_gd.SwapchainFramebuffer.Height / 8,
				1,
				1,
				PixelFormat.R16_G16_B16_A16_Float,
				TextureUsage.RenderTarget | TextureUsage.Sampled);
			_texture = _factory.CreateTexture(textureDesc);
		}

		protected override ResourceLayout GetResourceLayout()
		{
			return _factory.CreateResourceLayout(new ResourceLayoutDescription(
				new ResourceLayoutElementDescription("Time", ResourceKind.UniformBuffer, ShaderStages.Fragment)));
		}

		protected override ResourceSet GetResourceSet(ResourceLayout resourceLayout)
		{
			return _factory.CreateResourceSet(new ResourceSetDescription(resourceLayout, _timeBuffer));
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
				new ShaderDescription(ShaderStages.Fragment, Encoding.UTF8.GetBytes(FragmentCode), "main"));
		}

		public override void UpdateResources()
		{
			_gd.UpdateBuffer(_timeBuffer, 0, (float)Stopwatch.Elapsed.TotalSeconds);
		}
	}
}
