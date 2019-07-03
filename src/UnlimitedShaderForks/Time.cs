using System;
using System.Diagnostics;

namespace UnlimitedShaderForks
{
	public class Time
	{
		private long startTicks = 0;
		private double timeScale = 1.0;
		private Stopwatch sw = new Stopwatch(); // count time since startTicks

		public long ElapsedTicks => startTicks + (long)(sw.Elapsed.Ticks * timeScale);
		public TimeSpan Elapsed => TimeSpan.FromTicks(ElapsedTicks);
		public double ElapsedSeconds => Elapsed.TotalSeconds;
		public double ElapsedMilliseconds => Elapsed.TotalMilliseconds;
		public event EventHandler OnStart, OnStop, OnStep, OnTimescaleChanged;

		public void Start()
		{
			sw.Start();
			OnStart?.Invoke(this, EventArgs.Empty);
		}

		public void Stop()
		{
			startTicks += (long)(sw.Elapsed.Ticks * timeScale);
			sw.Reset();
			OnStop?.Invoke(this, EventArgs.Empty);
		}

		public void Toggle()
		{
			if (sw.IsRunning)
			{
				Stop();
			}
			else
			{
				Start();
			}
		}

		public void Step(double seconds)
		{
			this.startTicks += TimeSpan.FromSeconds(seconds).Ticks;

			if (ElapsedTicks < 0)
				SetTime(TimeSpan.Zero);

			OnStep?.Invoke(this, EventArgs.Empty);
		}

		public void Restart()
		{
			startTicks = 0;

			if (sw.IsRunning)
			{
				sw.Restart();
			}
			else
			{
				sw.Reset();
			}

			OnStep?.Invoke(this, EventArgs.Empty);
		}

		public double Timescale
		{
			get => timeScale;
			set
			{
				if (value < 0.1)
					return;

				if (sw.IsRunning)
				{
					startTicks += (long)(sw.Elapsed.Ticks * timeScale);
					sw.Restart();
					timeScale = value;
				}
				else
				{
					timeScale = value;
				}
				OnTimescaleChanged?.Invoke(this, EventArgs.Empty);
				OnStep?.Invoke(this, EventArgs.Empty);
			}
		}

		/// <summary>
		/// Set specific time, for audio syncing purposes
		/// </summary>
		/// <param name="time">Absolute time</param>
		public void SetTime(TimeSpan time)
		{
			this.startTicks = time.Ticks;

			if (sw.IsRunning)
			{
				sw.Restart();
			}
			else
			{
				sw.Reset();
			}

			OnStep?.Invoke(this, EventArgs.Empty);
		}

		public float ShaderStartOffset { get; set; } = 0f;
	}
}
