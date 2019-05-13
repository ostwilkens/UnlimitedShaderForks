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
		private CommandList _commandList;
		private DeviceBuffer _vertexBuffer;
		private DeviceBuffer _indexBuffer;
		private Pipeline _pipeline;
		private Stopwatch _swLifetime = new Stopwatch();
		private Stopwatch _swThisSecond = new Stopwatch();
		private int _framesThisSecond = 0;

		public bool Exists => _window.Exists;
		public void Close() => _window.Close();

		public Window(WindowCreateInfo windowCreateInfo)
		{
			_swLifetime.Start();
			_swThisSecond.Start();
			_window = VeldridStartup.CreateWindow(ref windowCreateInfo);
			var options = new GraphicsDeviceOptions { PreferStandardClipSpaceYDirection = true };
			_graphicsDevice = VeldridStartup.CreateGraphicsDevice(_window, options, GraphicsBackend.Vulkan);
			_factory = _graphicsDevice.ResourceFactory;

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

			SetGraphicsPipeline();
			_commandList = _factory.CreateCommandList();
		}

		public InputSnapshot Update()
		{
			_framesThisSecond++;

			if (_swThisSecond.ElapsedMilliseconds >= 1000)
			{
				_window.Title = $"{_framesThisSecond} fps";
				_framesThisSecond = 0;
				_swThisSecond.Restart();
			}

			Draw();
			return _window.PumpEvents();
		}

		private void Draw()
		{
			_commandList.Begin();
			_commandList.SetFramebuffer(_graphicsDevice.SwapchainFramebuffer);
			_commandList.ClearColorTarget(0, RgbaFloat.Black);
			_commandList.SetVertexBuffer(0, _vertexBuffer);
			_commandList.SetIndexBuffer(_indexBuffer, IndexFormat.UInt16);
			_commandList.SetPipeline(_pipeline);
			_commandList.DrawIndexed(
				indexCount: 4,
				instanceCount: 1,
				indexStart: 0,
				vertexOffset: 0,
				instanceStart: 0);
			_commandList.End();
			_graphicsDevice.SubmitCommands(_commandList);
			_graphicsDevice.SwapBuffers();
		}

		private string _fragmentCode = DefaultShaders.FragmentCode;
		public string FragmentCode
		{
			get => _fragmentCode;
			set
			{
				_fragmentCode = value;
				SetGraphicsPipeline();
			}
		}

		private void SetGraphicsPipeline()
		{
			var vertexElements = new[] { new VertexElementDescription("Position", VertexElementSemantic.TextureCoordinate, VertexElementFormat.Float2) };
			var vertexLayout = new VertexLayoutDescription(vertexElements);

			var vertexShaderDesc = new ShaderDescription(ShaderStages.Vertex, Encoding.UTF8.GetBytes(DefaultShaders.VertexCode), "main");
			var fragmentShaderDesc = new ShaderDescription(ShaderStages.Fragment, Encoding.UTF8.GetBytes(FragmentCode), "main");

			var shaders = _factory.CreateFromSpirv(vertexShaderDesc, fragmentShaderDesc);

			var pipelineDescription = new GraphicsPipelineDescription
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
				ResourceLayouts = Array.Empty<ResourceLayout>(),
				ShaderSet = new ShaderSetDescription(new VertexLayoutDescription[] { vertexLayout }, shaders),
				Outputs = _graphicsDevice.SwapchainFramebuffer.OutputDescription,
			};

			_pipeline = _factory.CreateGraphicsPipeline(pipelineDescription);
		}
	}
}
