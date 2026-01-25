using System;
using Nekki.Vector.Core.Trigger.Events;

namespace Nekki.Vector.Core
{
	public class GlobalTimer
	{
		private string _timeValue;

		private int _frameCount;

		private int _seconds;

		private int _milliseconds;

		private bool _isIncrement;

		private bool _isDecrement;

		public string Time
		{
			get
			{
				return _timeValue;
			}
		}

		public int FrameCount
		{
			get
			{
				return _frameCount;
			}
		}

		public int Seconds
		{
			get
			{
				return _seconds;
			}
		}

		public int Milliseconds
		{
			get
			{
				return _milliseconds;
			}
		}

		private void ConvertFrameCountToTime()
		{
			TimeSpan timeSpan = TimeSpan.FromSeconds((float)_frameCount * (1f / 60f));
			_timeValue = string.Format("{0:00}:{1:00}", timeSpan.Seconds, timeSpan.Milliseconds / 10);
			_milliseconds = timeSpan.Milliseconds;
			_seconds = timeSpan.Seconds;
		}

		public void Update()
		{
			if (_isDecrement)
			{
				_frameCount--;
				if (_frameCount < 0)
				{
					_frameCount = 0;
					_isDecrement = false;
					TRE_GlobalTimerTimeout.ActivateThisEvent();
				}
				else
				{
					ConvertFrameCountToTime();
				}
			}
			else if (_isIncrement)
			{
				_frameCount++;
				ConvertFrameCountToTime();
			}
		}

		public void StartCounting()
		{
			_isIncrement = true;
		}

		public void StartCountdown()
		{
			_isDecrement = true;
		}

		public void SetFramesCount(int frameCount)
		{
			_frameCount = frameCount;
		}

		public void ChangeFramesCount(int frameCount)
		{
			_frameCount += frameCount;
		}

		public void Pause()
		{
			_isIncrement = false;
			_isDecrement = false;
		}
	}
}
