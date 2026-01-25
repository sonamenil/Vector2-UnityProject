using System;
using Nekki.Vector.Core.GameManagement;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Nekki.Vector.GUI.Common
{
	public class TimerUI : MonoBehaviour
	{
		[SerializeField]
		private string _Id;

		[SerializeField]
		private TimerFormat _Format;

		[SerializeField]
		private Text _Label;

		[SerializeField]
		private UnityEvent _OnExpire = new UnityEvent();

		private float _UpdateTime;

		public string Id
		{
			get
			{
				return _Id;
			}
			set
			{
				_Id = value;
				CheckTimer();
			}
		}

		public UnityEvent OnExpire
		{
			get
			{
				return _OnExpire;
			}
		}

		public TimerFormat Format
		{
			get
			{
				return _Format;
			}
			set
			{
				_Format = value;
			}
		}

		private void OnEnable()
		{
			_UpdateTime = 0f;
		}

		private void Update()
		{
			_UpdateTime -= Time.deltaTime;
			if (_UpdateTime <= 1E-06f)
			{
				CheckTimer();
			}
		}

		private bool CheckTimer()
		{
			float timerValueInSeconds = TimersManager.GetTimerValueInSeconds(_Id);
			if (timerValueInSeconds > 1E-06f)
			{
				TimeSpan p_timeLeft = TimeSpan.FromSeconds(timerValueInSeconds);
				if (_Label != null)
				{
					_Label.text = GetTimeText(p_timeLeft);
				}
				_UpdateTime = GetUpdatePeriod(p_timeLeft);
				return true;
			}
			if (base.enabled)
			{
				_OnExpire.Invoke();
			}
			base.enabled = false;
			if (_Label != null)
			{
				_Label.text = string.Empty;
			}
			return false;
		}

		private string GetTimeText(TimeSpan p_timeLeft)
		{
			switch (_Format)
			{
			case TimerFormat.Full:
				return GetTimeText_Full(p_timeLeft);
			case TimerFormat.Short:
				return GetTimeText_Short(p_timeLeft);
			default:
				return "UnknownFormat[ " + p_timeLeft.ToString() + "]";
			}
		}

		private string GetTimeText_Full(TimeSpan p_timeLeft)
		{
			string text = ((p_timeLeft.Days <= 0) ? string.Empty : string.Format("{0}d ", p_timeLeft.Days));
			return text + string.Format("{0:00}:{1:00}:{2:00}", p_timeLeft.Hours, p_timeLeft.Minutes, p_timeLeft.Seconds);
		}

		private string GetTimeText_Short(TimeSpan p_timeLeft)
		{
			if (p_timeLeft.Days > 0)
			{
				return string.Format("{0}d {1:00}h", p_timeLeft.Days, p_timeLeft.Hours);
			}
			if (p_timeLeft.Hours > 0)
			{
				return string.Format("{0:00}h {1:00}m", p_timeLeft.Hours, p_timeLeft.Minutes);
			}
			if (p_timeLeft.Minutes > 0)
			{
				return string.Format("{0:00}m {1:00}s", p_timeLeft.Minutes, p_timeLeft.Seconds);
			}
			return string.Format("   {0:00}s", p_timeLeft.Seconds);
		}

		private float GetUpdatePeriod(TimeSpan p_timeLeft)
		{
			switch (_Format)
			{
			case TimerFormat.Full:
				return 1f;
			case TimerFormat.Short:
				if (p_timeLeft.Days > 0)
				{
					return 3600f;
				}
				if (p_timeLeft.Hours > 0)
				{
					return 60f;
				}
				return 1f;
			default:
				return 1f;
			}
		}
	}
}
