using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Veldrid;
using Veldrid.SPIRV;

namespace UnlimitedShaderForks
{
	public struct PassthroughRendererArgs
	{
		public Texture Texture { get; set; }
		public Time Time { get; set; }

		public PassthroughRendererArgs(Texture texture, Time time)
		{
			this.Texture = texture;
			this.Time = time;
		}
	}

	public class PassthroughRenderer : RendererBase<PassthroughRendererArgs>
	{
		protected DeviceBuffer _timeBuffer;
		protected TextureView _textureBuffer;
		public Time Time { get; set; }

		public PassthroughRenderer(GraphicsDevice gd, Texture texture, Time time) : base(gd, new PassthroughRendererArgs(texture, time))
		{
		}

		protected override void Initialize(PassthroughRendererArgs args)
		{
			this.Time = args.Time;

			_textureBuffer = _factory.CreateTextureView(args.Texture);
			_timeBuffer = _factory.CreateBuffer(new BufferDescription(16, BufferUsage.UniformBuffer));
		}

		protected override ResourceLayout GetResourceLayout()
		{
			return _factory.CreateResourceLayout(new ResourceLayoutDescription(
				new ResourceLayoutElementDescription("SourceTexture", ResourceKind.TextureReadOnly, ShaderStages.Fragment),
				new ResourceLayoutElementDescription("SourceSampler", ResourceKind.Sampler, ShaderStages.Fragment),
				new ResourceLayoutElementDescription("_Time", ResourceKind.UniformBuffer, ShaderStages.Fragment)));
		}

		protected override ResourceSet GetResourceSet(ResourceLayout resourceLayout)
		{
			return _factory.CreateResourceSet(new ResourceSetDescription(_resourceLayout, _textureBuffer, _gd.PointSampler, _timeBuffer));
		}

		protected override Shader[] GetShaders()
		{
			return _factory.CreateFromSpirv(
				new ShaderDescription(ShaderStages.Vertex, Encoding.UTF8.GetBytes(DefaultShaders.VertexCode), "main"),
				new ShaderDescription(ShaderStages.Fragment, Encoding.UTF8.GetBytes(DefaultShaders.PassthroughFragmentCode), "main"));
		}

		public override void UpdateResources()
		{
			_gd.UpdateBuffer(_timeBuffer, 0, (float)Time.ElapsedSeconds);
		}
	}
}
