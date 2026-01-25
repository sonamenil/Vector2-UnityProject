using System;
using UnityEngine;

public class PlateScrollerEmptyPlate : MonoBehaviour
{
	private Action _OnTap;

	public void Init(Action p_onTap)
	{
		_OnTap = p_onTap;
	}

	public void OnPlateTap()
	{
		if (_OnTap != null)
		{
			_OnTap();
		}
	}
}
