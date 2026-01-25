using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExtentionBehaviour : MonoBehaviour
{
	public class CallEventArgs
	{
		public object Target { get; private set; }

		public int Event { get; private set; }

		public object Content { get; private set; }

		public CallEventArgs(int evt, object content, object target)
		{
			Target = target;
			Content = content;
			Event = evt;
		}

		public CallEventArgs SwitchTarget(object target)
		{
			Target = target;
			return this;
		}
	}

	public enum EventTypes
	{
		SimpeEvent = 0
	}

	private GameObject _gameObject;

	private Transform _transform;

	private Renderer _renderer;

	private Animator _animator;

	private readonly Dictionary<int, List<Action<CallEventArgs>>> _subscription = new Dictionary<int, List<Action<CallEventArgs>>>();

	public new GameObject gameObject
	{
		get
		{
			if (!_gameObject)
			{
				_gameObject = base.gameObject;
			}
			return _gameObject;
		}
	}

	public new Transform transform
	{
		get
		{
			if (!_transform)
			{
				_transform = base.transform;
			}
			return _transform;
		}
	}

	public Renderer renderer
	{
		get
		{
			if (!_renderer)
			{
				_renderer = GetComponent<Renderer>();
			}
			return _renderer;
		}
	}

	public Animator animator
	{
		get
		{
			if (!_animator)
			{
				_animator = GetComponent<Animator>();
			}
			return _animator;
		}
	}

	protected void Log(object message, UnityEngine.Object source = null)
	{
		AdvLog.Log(message, source ?? this);
	}

	protected void LogWarning(object message, UnityEngine.Object source = null)
	{
		AdvLog.LogWarning(message, source ?? this);
	}

	protected void LogError(object message, UnityEngine.Object source = null)
	{
		AdvLog.LogError(message, source ?? this);
	}

	protected void LogException(Exception ex, UnityEngine.Object source = null)
	{
		AdvLog.LogException(ex, source ?? this);
	}

	protected void Invoke(Action action, float t)
	{
		StartCoroutine(InvokeActionRoutine(action, t));
	}

	private IEnumerator InvokeActionRoutine(Action action, float t)
	{
		yield return new WaitForSeconds(t);
		action();
	}

	public void addEventListener(int evt, Action<CallEventArgs> callback)
	{
		if (!_subscription.ContainsKey(evt))
		{
			_subscription.Add(evt, new List<Action<CallEventArgs>>());
		}
		_subscription[evt].Add(callback);
	}

	public void addEventListener(int[] evt, Action<CallEventArgs> callback)
	{
		for (int i = 0; i < evt.Length; i++)
		{
			addEventListener(evt[i], callback);
		}
	}

	private void removeAllEventListener()
	{
		_subscription.Clear();
	}

	public void removeEvent(int evt)
	{
		if (_subscription.ContainsKey(evt))
		{
			_subscription.Remove(evt);
		}
	}

	public void removeEventListener(int evt, Action<CallEventArgs> callback)
	{
		if (_subscription.ContainsKey(evt))
		{
			while (_subscription[evt].Contains(callback))
			{
				_subscription[evt].Remove(callback);
			}
		}
	}

	protected virtual void OnDestroy()
	{
		removeAllEventListener();
	}

	public void callEvent(int evt, object content = null)
	{
		if (!_subscription.ContainsKey(evt))
		{
			return;
		}
		List<Action<CallEventArgs>> list = new List<Action<CallEventArgs>>(_subscription[evt]);
		for (int i = 0; i < list.Count; i++)
		{
			if (list[i] != null && _subscription.ContainsKey(evt) && _subscription[evt].Contains(list[i]))
			{
				list[i](new CallEventArgs(evt, content, this));
			}
		}
	}
}
