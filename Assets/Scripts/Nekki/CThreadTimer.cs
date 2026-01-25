using System;
using System.Timers;

namespace Nekki
{
	public class CThreadTimer
	{
		public delegate void VoidDelegate();

		private VoidDelegate _timerCallback;

		private Timer _myTimer;

		private bool _completed;

		public bool completed
		{
			get
			{
				return _completed;
			}
		}

		public CThreadTimer(VoidDelegate callback, float timerTime, bool startTimer)
		{
			_timerCallback = (VoidDelegate)Delegate.Combine(_timerCallback, callback);
			_myTimer = new Timer();
			_myTimer.Elapsed += timerEnd;
			_myTimer.Interval = timerTime;
			if (startTimer)
			{
				this.startTimer();
			}
		}

		public void startTimer()
		{
			_completed = false;
			_myTimer.Start();
		}

		public void startTimer(float timerTime)
		{
			_completed = false;
			_myTimer.Interval = timerTime;
			_myTimer.Start();
		}

		public bool pauseTimer()
		{
			if (!_completed)
			{
				_myTimer.Enabled = false;
			}
			return !_completed;
		}

		public bool continueTimer()
		{
			if (!_completed)
			{
				_myTimer.Enabled = true;
			}
			return !_completed;
		}

		private void timerEnd(object source, ElapsedEventArgs e)
		{
			_completed = false;
			_timerCallback();
			_myTimer.Dispose();
		}
	}
}
