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
		private GraphicsDevice _graphicsDevice;
		private ResourceFactory _factory;
		private CommandList _cl;
		private DeviceBuffer _vertexBuffer;
		private DeviceBuffer _indexBuffer;
		private DeviceBuffer _timeBuffer;
		private Pipeline _pipeline;
		private ResourceLayout _resourceLayout;
		private ResourceSet _resourceSet;
		private VertexLayoutDescription _vertexLayoutDesc;
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
			_graphicsDevice = VeldridStartup.CreateGraphicsDevice(
				_window,
				new GraphicsDeviceOptions { PreferStandardClipSpaceYDirection = true }, 
				GraphicsBackend.Vulkan);
			_factory = _graphicsDevice.ResourceFactory;

			CreateResources();
			CreateGraphicsPipeline();
		}

		private void CreateResources()
		{
			Vector2[] quadVertices =
			{
				new Vector2(-1f, .7f),
				new Vector2(1f, .7f),
				new Vector2(-1f, -.7f),
				new Vector2(1f, -.7f),
			};

			ushort[] quadIndices = { 0, 1, 2, 3 };

			_vertexBuffer = _factory.CreateBuffer(new BufferDescription(4 * sizeof(float) * 2, BufferUsage.VertexBuffer));
			_indexBuffer = _factory.CreateBuffer(new BufferDescription(4 * sizeof(ushort), BufferUsage.IndexBuffer));

			_graphicsDevice.UpdateBuffer(_vertexBuffer, 0, quadVertices);
			_graphicsDevice.UpdateBuffer(_indexBuffer, 0, quadIndices);

			_resourceLayout = _factory.CreateResourceLayout(new ResourceLayoutDescription(
				new ResourceLayoutElementDescription("Time", ResourceKind.UniformBuffer, ShaderStages.Fragment)));
			_timeBuffer = _factory.CreateBuffer(new BufferDescription(16, BufferUsage.UniformBuffer));
			_resourceSet = _factory.CreateResourceSet(new ResourceSetDescription(_resourceLayout, _timeBuffer));

			_cl = _factory.CreateCommandList();

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
				ShaderSet = new ShaderSetDescription(new VertexLayoutDescription[] { _vertexLayoutDesc }, new Shader[] { }),
				Outputs = _graphicsDevice.SwapchainFramebuffer.OutputDescription,
			};
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

			_graphicsDevice.UpdateBuffer(_timeBuffer, 0, Time);
			Draw();
			return _window.PumpEvents();
		}

		private void Draw()
		{
			_cl.Begin();
			_cl.SetFramebuffer(_graphicsDevice.SwapchainFramebuffer);
			_cl.ClearColorTarget(0, RgbaFloat.Black);
			_cl.SetVertexBuffer(0, _vertexBuffer);
			_cl.SetIndexBuffer(_indexBuffer, IndexFormat.UInt16);
			_cl.SetPipeline(_pipeline);
			_cl.SetGraphicsResourceSet(0, _resourceSet);
			_cl.DrawIndexed(
				indexCount: 4,
				instanceCount: 1,
				indexStart: 0,
				vertexOffset: 0,
				instanceStart: 0);
			_cl.End();
			_graphicsDevice.SubmitCommands(_cl);
			_graphicsDevice.SwapBuffers();
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

		private GraphicsPipelineDescription _pipelineDesc;

		private void CreateGraphicsPipeline()
		{
			var shaders = _factory.CreateFromSpirv(
				new ShaderDescription(ShaderStages.Vertex, Encoding.UTF8.GetBytes(DefaultShaders.VertexCode), "main"),
				new ShaderDescription(ShaderStages.Fragment, Encoding.UTF8.GetBytes(FragmentCode), "main"));

			_pipelineDesc.ShaderSet.Shaders = shaders;
			_pipeline = _factory.CreateGraphicsPipeline(_pipelineDesc);
		}
	}
}
