using UnityEngine;

public class MonoBehaviourSingleton<T> : MonoBehaviour where T : MonoBehaviour
{
	protected static T _instance;

	public static T Instance
	{
		get
		{
			if (_instance == null)
			{
				Object @object = Object.FindObjectOfType(typeof(T));
				if (@object == null)
				{
					_instance = new GameObject(typeof(T).ToString()).AddComponent<T>();
				}
				else
				{
					_instance = @object as T;
				}
				if (_instance == null)
				{
					Debug.LogError(string.Concat("An instance of ", typeof(T), " is not found or cant created !!!"));
				}
			}
			return _instance;
		}
	}

	public static void Delete()
	{
		Debug.Log("Deleting MonoBehaviourSingleton with type: " + typeof(T));
		_instance = (T)null;
	}
}
