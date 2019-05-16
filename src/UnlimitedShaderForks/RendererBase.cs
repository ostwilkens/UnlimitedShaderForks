using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using Veldrid;
using Veldrid.SPIRV;

namespace UnlimitedShaderForks
{
	public interface IRenderer
	{
		void Draw(CommandList cl);
		void UpdateResources();
	}

	public abstract class RendererBase<TArgs> : IRenderer
	{
		protected GraphicsDevice _gd;
		protected ResourceFactory _factory;
		private GraphicsPipelineDescription _pipelineDesc;
		private VertexLayoutDescription _vertexLayoutDesc;
		private Pipeline _pipeline;
		protected ResourceLayout _resourceLayout;
		protected ResourceSet _resourceSet;
		protected Framebuffer _framebuffer;
		private DeviceBuffer _vertexBuffer;
		private DeviceBuffer _indexBuffer;
		protected Shader[] _shaders;

		public RendererBase(GraphicsDevice gd, TArgs args)
		{
			_gd = gd;
			_factory = gd.ResourceFactory;
			Initialize(args);
			_framebuffer = GetFramebuffer();
			_shaders = GetShaders();

			var vertices = new[]
			{
				new Vector2(-1f, 1f),
				new Vector2(1f, 1f),
				new Vector2(-1f, -1f),
				new Vector2(1f, -1f),
			};

			var indices = new ushort[] { 0, 1, 2, 3 };

			_vertexBuffer = _factory.CreateBuffer(new BufferDescription(4 * sizeof(float) * 2, BufferUsage.VertexBuffer));
			_indexBuffer = _factory.CreateBuffer(new BufferDescription(4 * sizeof(ushort), BufferUsage.IndexBuffer));

			_gd.UpdateBuffer(_vertexBuffer, 0, vertices);
			_gd.UpdateBuffer(_indexBuffer, 0, indices);

			_resourceLayout = GetResourceLayout();
			_resourceSet = GetResourceSet(_resourceLayout);

			var vertexElements = new[] { new VertexElementDescription("Position", VertexElementSemantic.TextureCoordinate, VertexElementFormat.Float2) };
			_vertexLayoutDesc = new VertexLayoutDescription(vertexElements);

			_pipelineDesc = new GraphicsPipelineDescription
			{
				BlendState = BlendStateDescription.SingleOverrideBlend,
				DepthStencilState = new DepthStencilStateDescription(
					depthTestEnabled: false,
					depthWriteEnabled: true,
					comparisonKind: ComparisonKind.LessEqual),
				RasterizerState = new RasterizerStateDescription(
					cullMode: FaceCullMode.None,
					fillMode: PolygonFillMode.Solid,
					frontFace: FrontFace.Clockwise,
					depthClipEnabled: true,
					scissorTestEnabled: false),
				PrimitiveTopology = PrimitiveTopology.TriangleStrip,
				ResourceLayouts = new[] { _resourceLayout },
				ShaderSet = new ShaderSetDescription(new VertexLayoutDescription[] { _vertexLayoutDesc }, _shaders),
				Outputs = _framebuffer.OutputDescription,
			};

			_pipeline = _factory.CreateGraphicsPipeline(_pipelineDesc);
		}

		public void Draw(CommandList cl)
		{
			cl.SetPipeline(_pipeline);
			cl.SetFramebuffer(_framebuffer);
			cl.SetGraphicsResourceSet(0, _resourceSet);
			cl.SetVertexBuffer(0, _vertexBuffer);
			cl.SetIndexBuffer(_indexBuffer, IndexFormat.UInt16);
			cl.ClearColorTarget(0, RgbaFloat.Black);
			cl.DrawIndexed(
				indexCount: 4,
				instanceCount: 1,
				indexStart: 0,
				vertexOffset: 0,
				instanceStart: 0);
		}

		protected virtual ResourceLayout GetResourceLayout() => _factory.CreateResourceLayout(new ResourceLayoutDescription());
		protected virtual ResourceSet GetResourceSet(ResourceLayout resourceLayout) => _factory.CreateResourceSet(new ResourceSetDescription(resourceLayout));
		protected virtual Framebuffer GetFramebuffer() => _gd.SwapchainFramebuffer;
		protected abstract Shader[] GetShaders();
		protected abstract void Initialize(TArgs args);
		public virtual void UpdateResources() { }
	}
}
