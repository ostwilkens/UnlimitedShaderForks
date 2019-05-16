using System;
using System.Collections.Generic;
using System.Text;
using Veldrid;
using Veldrid.SPIRV;

namespace UnlimitedShaderForks
{
	public struct PassthroughRendererArgs
	{
		public Texture Texture { get; set; }

		public PassthroughRendererArgs(Texture texture)
		{
			this.Texture = texture;
		}
	}

	public class PassthroughRenderer : RendererBase<PassthroughRendererArgs>
	{
		protected TextureView _textureBuffer;

		public PassthroughRenderer(GraphicsDevice gd, Texture texture) : base(gd, new PassthroughRendererArgs(texture))
		{
		}

		protected override void Initialize(PassthroughRendererArgs args)
		{
			_textureBuffer = _factory.CreateTextureView(args.Texture);
		}

		protected override ResourceLayout GetResourceLayout()
		{
			return _factory.CreateResourceLayout(new ResourceLayoutDescription(
				new ResourceLayoutElementDescription("SourceTexture", ResourceKind.TextureReadOnly, ShaderStages.Fragment),
				new ResourceLayoutElementDescription("SourceSampler", ResourceKind.Sampler, ShaderStages.Fragment)));
		}

		protected override ResourceSet GetResourceSet(ResourceLayout resourceLayout)
		{
			return _factory.CreateResourceSet(new ResourceSetDescription(_resourceLayout, _textureBuffer, _gd.PointSampler));
		}

		protected override Shader[] GetShaders()
		{
			return _factory.CreateFromSpirv(
				new ShaderDescription(ShaderStages.Vertex, Encoding.UTF8.GetBytes(DefaultShaders.VertexCode), "main"),
				new ShaderDescription(ShaderStages.Fragment, Encoding.UTF8.GetBytes(DefaultShaders.PassthroughFragmentCode), "main"));
		}
	}
}
