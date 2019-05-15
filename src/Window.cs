using System;
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
		private DeviceBuffer _vertexBuffer;
		private DeviceBuffer _indexBuffer;
		private DeviceBuffer _timeBuffer;
		private Framebuffer _textureFb;
		private Pipeline _texturePipeline;
		private Pipeline _swapchainPipeline;
		private ResourceLayout _resourceLayout;
		private ResourceLayout _passthroughResourceLayout;
		private ResourceSet _resourceSet;
		private ResourceSet _passthroughResourceSet;
		private VertexLayoutDescription _vertexLayoutDesc;
		private GraphicsPipelineDescription _texturePipelineDesc;
		private GraphicsPipelineDescription _swapchainPipelineDesc;
		private Stopwatch _swLifetime = new Stopwatch();
		private Stopwatch _swThisSecond = new Stopwatch();
		private int _framesThisSecond = 0;

		public float Time => (float)_swLifetime.Elapsed.TotalSeconds;
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
				GraphicsBackend.Vulkan);
			_factory = _gd.ResourceFactory;

			CreateResources();
			CreateGraphicsPipeline();
		}

		private void CreateResources()
		{
			_cl = _factory.CreateCommandList();

			Vector2[] quadVertices =
			{
				new Vector2(-1f, 1f),
				new Vector2(1f, 1f),
				new Vector2(-1f, -1f),
				new Vector2(1f, -1f),
			};

			ushort[] quadIndices = { 0, 1, 2, 3 };

			_vertexBuffer = _factory.CreateBuffer(new BufferDescription(4 * sizeof(float) * 2, BufferUsage.VertexBuffer));
			_indexBuffer = _factory.CreateBuffer(new BufferDescription(4 * sizeof(ushort), BufferUsage.IndexBuffer));

			_gd.UpdateBuffer(_vertexBuffer, 0, quadVertices);
			_gd.UpdateBuffer(_indexBuffer, 0, quadIndices);

			_resourceLayout = _factory.CreateResourceLayout(new ResourceLayoutDescription(
				new ResourceLayoutElementDescription("Time", ResourceKind.UniformBuffer, ShaderStages.Fragment)));
			_timeBuffer = _factory.CreateBuffer(new BufferDescription(16, BufferUsage.UniformBuffer));
			_resourceSet = _factory.CreateResourceSet(new ResourceSetDescription(_resourceLayout, _timeBuffer));


			var vertexElements = new[] { new VertexElementDescription("Position", VertexElementSemantic.TextureCoordinate, VertexElementFormat.Float2) };
			_vertexLayoutDesc = new VertexLayoutDescription(vertexElements);


			TextureDescription textureDesc = TextureDescription.Texture2D(
				_gd.SwapchainFramebuffer.Width / 8,
				_gd.SwapchainFramebuffer.Height / 8,
				1,
				1,
				PixelFormat.R16_G16_B16_A16_Float,
				TextureUsage.RenderTarget | TextureUsage.Sampled);
			var texture = _factory.CreateTexture(textureDesc);
			var fbDesc = new FramebufferDescription(null, texture);
			_textureFb = _factory.CreateFramebuffer(ref fbDesc);

			var passthroughShaders = _factory.CreateFromSpirv(
				new ShaderDescription(ShaderStages.Vertex, Encoding.UTF8.GetBytes(DefaultShaders.VertexCode), "main"),
				new ShaderDescription(ShaderStages.Fragment, Encoding.UTF8.GetBytes(DefaultShaders.PassthroughFragmentCode), "main"));

			_passthroughResourceLayout = _factory.CreateResourceLayout(new ResourceLayoutDescription(
				new ResourceLayoutElementDescription("SourceTexture", ResourceKind.TextureReadOnly, ShaderStages.Fragment),
				new ResourceLayoutElementDescription("SourceSampler", ResourceKind.Sampler, ShaderStages.Fragment)));
			var textureBuffer = _factory.CreateTextureView(texture);
			_passthroughResourceSet = _factory.CreateResourceSet(new ResourceSetDescription(_passthroughResourceLayout, textureBuffer, _gd.PointSampler));


			_texturePipelineDesc = new GraphicsPipelineDescription
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
				ShaderSet = new ShaderSetDescription(new VertexLayoutDescription[] { _vertexLayoutDesc }, new Shader[] { }),
				Outputs = _textureFb.OutputDescription,
			};

			_swapchainPipelineDesc = new GraphicsPipelineDescription
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
				ResourceLayouts = new[] { _passthroughResourceLayout },
				ShaderSet = new ShaderSetDescription(new VertexLayoutDescription[] { _vertexLayoutDesc }, passthroughShaders),
				Outputs = _gd.SwapchainFramebuffer.OutputDescription,
			};

			_swapchainPipeline = _factory.CreateGraphicsPipeline(_swapchainPipelineDesc);
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

			_gd.UpdateBuffer(_timeBuffer, 0, Time);
			Draw();
			return _window.PumpEvents();
		}

		private void Draw()
		{
			_cl.Begin();

			_cl.SetPipeline(_texturePipeline);
			_cl.SetFramebuffer(_textureFb);
			_cl.SetGraphicsResourceSet(0, _resourceSet);
			_cl.SetVertexBuffer(0, _vertexBuffer);
			_cl.SetIndexBuffer(_indexBuffer, IndexFormat.UInt16);
			_cl.ClearColorTarget(0, RgbaFloat.Black);
			_cl.DrawIndexed(
				indexCount: 4,
				instanceCount: 1,
				indexStart: 0,
				vertexOffset: 0,
				instanceStart: 0);

			_cl.SetPipeline(_swapchainPipeline);
			_cl.SetFramebuffer(_gd.SwapchainFramebuffer);
			_cl.SetGraphicsResourceSet(0, _passthroughResourceSet);
			_cl.DrawIndexed(
				indexCount: 4,
				instanceCount: 1,
				indexStart: 0,
				vertexOffset: 0,
				instanceStart: 0);

			_cl.End();
			_gd.SubmitCommands(_cl);
			_gd.SwapBuffers();
		}

		private string _fragmentCode = DefaultShaders.FragmentCode;
		public string FragmentCode
		{
			get => _fragmentCode;
			set
			{
				_fragmentCode = value;
				CreateGraphicsPipeline();
			}
		}

		private void CreateGraphicsPipeline()
		{
			var shaders = _factory.CreateFromSpirv(
				new ShaderDescription(ShaderStages.Vertex, Encoding.UTF8.GetBytes(DefaultShaders.VertexCode), "main"),
				new ShaderDescription(ShaderStages.Fragment, Encoding.UTF8.GetBytes(FragmentCode), "main"));

			_texturePipelineDesc.ShaderSet.Shaders = shaders;
			_texturePipeline = _factory.CreateGraphicsPipeline(_texturePipelineDesc);
		}
	}
}
