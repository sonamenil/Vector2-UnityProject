using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoroutineManager : MonoBehaviour
{
	private class Coroutine
	{
		private static Dictionary<IEnumerator, Coroutine> _Coroutines = new Dictionary<IEnumerator, Coroutine>();

		public IEnumerator Routine { get; private set; }

		public bool IsRunning { get; private set; }

		public object Current
		{
			get
			{
				return Routine.Current;
			}
		}

		public Coroutine(IEnumerator p_routine)
		{
			Routine = p_routine;
			IsRunning = true;
			_Coroutines.Add(p_routine, this);
		}

		public static Coroutine GetCoroutine(IEnumerator p_routine)
		{
			Coroutine value = null;
			_Coroutines.TryGetValue(p_routine, out value);
			return value;
		}

		public bool MoveNext()
		{
			if (Routine != null && Routine.MoveNext())
			{
				return true;
			}
			Stop();
			return false;
		}

		public void Stop()
		{
			if (IsRunning)
			{
				IsRunning = false;
				_Coroutines.Remove(Routine);
			}
		}
	}

	private static CoroutineManager _Current;

	public static CoroutineManager Current
	{
		get
		{
			if (_Current == null)
			{
				_Current = new GameObject("[CoroutineManager]").AddComponent<CoroutineManager>();
				Object.DontDestroyOnLoad(_Current.gameObject);
			}
			return _Current;
		}
	}

	public bool IsPaused { get; private set; }

	public void StartRoutine(IEnumerator p_routine)
	{
		Coroutine p_coroutine = new Coroutine(p_routine);
		StartCoroutine(CallWrapper(p_coroutine));
	}

	public void StopRoutine(IEnumerator p_routine)
	{
		Coroutine coroutine = Coroutine.GetCoroutine(p_routine);
		if (coroutine != null)
		{
			coroutine.Stop();
		}
	}

	private IEnumerator CallWrapper(Coroutine p_coroutine)
	{
		yield return null;
		while (p_coroutine.IsRunning)
		{
			if (IsPaused)
			{
				yield return null;
			}
			else if (p_coroutine.MoveNext())
			{
				yield return p_coroutine.Current;
			}
		}
	}

	private void Awake()
	{
		IsPaused = false;
	}

	private void OnApplicationPause(bool p_pauseStatus)
	{
		IsPaused = p_pauseStatus;
	}
}
