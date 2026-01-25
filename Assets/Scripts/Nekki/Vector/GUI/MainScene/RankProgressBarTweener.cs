using System;
using UIFigures;
using UnityEngine;

namespace Nekki.Vector.GUI.MainScene
{
	public class RankProgressBarTweener : MonoBehaviour
	{
		private enum State
		{
			Running = 0,
			Stopped = 1
		}

		private float dest;

		private float start;

		private float _Timer;

		private float _RotateTime;

		private State _State;

		private Action _OnFinish;

		private UIArcBorder _Parent;

		private float _Delta;

		private void Awake()
		{
			_Parent = GetComponent<UIArcBorder>();
		}

		public void RotateArc(float From, float To, Action p_OnFinish, bool adjustSpeed = true, float speedUpCoef = 1f, float p_baseTime = 0.75f)
		{
			_OnFinish = p_OnFinish;
			start = From;
			dest = To;
			_State = State.Running;
			_RotateTime = p_baseTime;
			_Timer = 0f;
			if (!adjustSpeed)
			{
				_RotateTime = speedUpCoef * _RotateTime * ((To - From) / ((float)Math.PI * 2f));
			}
			else
			{
				_RotateTime = speedUpCoef * _RotateTime;
			}
		}

		private void Update()
		{
			if (_State != 0)
			{
				return;
			}
			_Timer += Time.deltaTime;
			_Parent.To = Mathf.Lerp(start, dest, _Timer / _RotateTime);
			_Parent.Refresh();
			if (_Parent.To >= dest)
			{
				_State = State.Stopped;
				if (_OnFinish != null)
				{
					_OnFinish();
				}
			}
		}

		public void Stop(bool callOnFinishEvent)
		{
			_State = State.Stopped;
			if (_OnFinish != null && callOnFinishEvent)
			{
				_OnFinish();
			}
		}

		public void SetArc(float From, float To)
		{
			if (_State != 0)
			{
				_Parent.From = From;
				_Parent.To = To;
				_Parent.Refresh();
			}
		}
	}
}
