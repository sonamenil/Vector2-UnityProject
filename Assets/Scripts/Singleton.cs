using UnityEngine;

public class Singleton<T> where T : Singleton<T>, new()
{
	protected static T _instance;

	public static T Instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = new T();
			}
			return _instance;
		}
	}

	public virtual void OnDelete()
	{
	}

	public static void Delete()
	{
		Debug.Log("Deleting Singleton with type: " + typeof(T));
		if (_instance != null)
		{
			_instance.OnDelete();
		}
		_instance = (T)null;
	}
}
