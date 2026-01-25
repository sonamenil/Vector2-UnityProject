using System.Collections;
using UnityEngine;

public class Routiner : MonoBehaviour
{
	private static Routiner _instance;

	public static void Go(IEnumerator routine)
	{
		if (!_instance || !_instance.gameObject)
		{
			_instance = new GameObject("routine").AddComponent<Routiner>();
			Object.DontDestroyOnLoad(_instance.gameObject);
		}
		_instance.StartCoroutine(routine);
	}
}
