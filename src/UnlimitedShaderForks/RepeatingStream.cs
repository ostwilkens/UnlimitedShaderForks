using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnlimitedShaderForks
{
	public class RepeatingStream : WaveStream
	{
		WaveStream sourceStream;
		public event EventHandler OnRepeat;

		public RepeatingStream(WaveStream sourceStream)
		{
			this.sourceStream = sourceStream;
		}

		public override WaveFormat WaveFormat
		{
			get { return sourceStream.WaveFormat; }
		}

		public override long Length
		{
			get { return sourceStream.Length; }
		}

		public override long Position
		{
			get { return sourceStream.Position; }
			set { sourceStream.Position = value; }
		}

		public override int Read(byte[] buffer, int offset, int count)
		{
			int read = 0;
			while(read < count)
			{
				int required = count - read;
				int readThisTime = sourceStream.Read(buffer, offset + read, required);
				if(readThisTime < required)
				{
					sourceStream.Position = 0;
				}

				//if(sourceStream.Position >= sourceStream.Length)
				//{
				//	sourceStream.Position = 0;
				//}
				read += readThisTime;
			}
			return read;
		}

		//public override int Read(byte[] buffer, int offset, int count)
		//{
		//	int totalBytesRead = 0;

		//	while(totalBytesRead < count)
		//	{
		//		int bytesRead = sourceStream.Read(buffer, offset + totalBytesRead, count - totalBytesRead);
		//		if(bytesRead <= 0)
		//		{
		//			if(sourceStream.Position == 0)
		//			{
		//				// something wrong with the source stream
		//				break;
		//			}
		//			// loop
		//			sourceStream.Position = 0;

		//			//bytesRead += Read(buffer, 0, count - totalBytesRead);
		//			//sourceStream.Position = 8000;
		//			//bytesRead += sourceStream.Read(buffer, 0 + totalBytesRead, count - totalBytesRead);
		//			//OnRepeat?.Invoke(this, EventArgs.Empty);
		//		}
		//		totalBytesRead += bytesRead;
		//	}
		//	return totalBytesRead;
		//}
	}
}
