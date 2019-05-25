using NAudio.Wave;
using UnlimitedShaderForks.SoundTouch;
using System;

namespace UnlimitedShaderForks
{
	public class Audio : IDisposable
	{
		private WaveStream stream;
		private VarispeedSampleProvider speedControl;
		private IWavePlayer player;
		private float baseSpeed;
		public float BaseSpeed => baseSpeed;
		public event EventHandler OnRepeat;

		private Audio(string path, float speed, bool repeat)
		{
			baseSpeed = speed;
			stream = new AudioFileReader(path);
			if(repeat)
			{
				stream = new RepeatingStream(stream);
				((RepeatingStream)stream).OnRepeat += (s, a) => this.OnRepeat?.Invoke(s, a);
			}
			speedControl = new VarispeedSampleProvider(stream.ToSampleProvider(), 10, new SoundTouchProfile(false, true));
			speedControl.PlaybackRate = baseSpeed;
			//player = new WaveOutEvent() { DesiredLatency = 120, NumberOfBuffers = 2 };
			//player = new DirectSoundOut(40);
			player = new DirectSoundOut(100);
			//PlaybackRate = 1; 
			//player = new WaveOutEvent() { DesiredLatency = 200, NumberOfBuffers = 2 };
			player.Init(speedControl);
		}

		public static Audio Load(string path, float speed = 1, bool repeat = false)
		{
			var audio = new Audio(path, speed, repeat);
			return audio;
		}

		public void Play()
		{
			player.Play();
		}

		public void Stop()
		{
			player.Pause();
		}

		public TimeSpan CurrentTime
		{
			get => stream.CurrentTime;
			set => stream.CurrentTime = TimeSpan.FromTicks(value.Ticks % stream.TotalTime.Ticks);
		}

		public TimeSpan TotalLength => stream.TotalTime;

		public float PlaybackRate
		{
			get => speedControl.PlaybackRate;
			set => speedControl.PlaybackRate = value * baseSpeed;
		}

		public void Dispose()
		{
			player?.Dispose();
			stream?.Dispose();
			speedControl?.Dispose();
		}
	}
}
