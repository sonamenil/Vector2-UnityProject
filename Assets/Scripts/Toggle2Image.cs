using UnityEngine;
using UnityEngine.UI;

public class Toggle2Image : MonoBehaviour
{
	[SerializeField]
	private GameObject _OnObject;

	[SerializeField]
	private GameObject _OffObject;

	[SerializeField]
	private Toggle.ToggleEvent _OnValueChange;

	public bool State
	{
		get
		{
			return _OnObject.activeSelf;
		}
		set
		{
			_OnObject.SetActive(value);
			_OffObject.SetActive(!value);
		}
	}

	public void ChangeState()
	{
		State = !State;
		_OnValueChange.Invoke(State);
	}
}
