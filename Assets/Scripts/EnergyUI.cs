using System;
using System.Collections.Generic;
using Nekki.Vector.Core.GameManagement;
using Nekki.Vector.GUI.Dialogs;
using UnityEngine;

[ExecuteInEditMode]
public class EnergyUI : MonoBehaviour
{
	[SerializeField]
	private float _FreeSpace = 9f;

	[SerializeField]
	private float _Width = 365f;

	[SerializeField]
	private float _Height = 8f;

	[SerializeField]
	private float _Sdvig = 8f;

	[SerializeField]
	private int _Segments;

	[SerializeField]
	private RectTransform _Background;

	[SerializeField]
	private Color _FullColor;

	[SerializeField]
	private Color _EmptyColor;

	private List<UIPoligon> _Poligons = new List<UIPoligon>();

	public bool _Refresh;

	public int Segments
	{
		set
		{
			_Segments = value;
		}
	}

	public void ForcedUpdate(int p_segments)
	{
		_Segments = p_segments;
		CreatePoligons();
		UpdateColors();
	}

	public void Init(int p_segments)
	{
		_Segments = p_segments;
		CreatePoligons();
		UpdateColors();
		base.gameObject.AddComponent<EnergyManager.EnergyManagerRender>();
		EnergyManager.OnRecharge = (Action)Delegate.Combine(EnergyManager.OnRecharge, new Action(UpdateColors));
		EnergyManager.OnSpend = (Action)Delegate.Combine(EnergyManager.OnSpend, new Action(UpdateColors));
	}

	private void OnDestroy()
	{
		EnergyManager.OnRecharge = (Action)Delegate.Remove(EnergyManager.OnRecharge, new Action(UpdateColors));
		EnergyManager.OnSpend = (Action)Delegate.Remove(EnergyManager.OnSpend, new Action(UpdateColors));
	}

	private void UpdateColors()
	{
		int currentLevel = EnergyManager.CurrentLevel;
		for (int i = 0; i < _Poligons.Count; i++)
		{
			_Poligons[i].color = ((i + 1 > currentLevel) ? _EmptyColor : _FullColor);
		}
	}

	private void CreatePoligons()
	{
		for (int i = 0; i < _Poligons.Count; i++)
		{
			UnityEngine.Object.DestroyImmediate(_Poligons[i].gameObject);
		}
		_Poligons.Clear();
		float num = (_Width - (float)(_Segments - 1) * _FreeSpace) / (float)_Segments;
		for (int j = 0; j < _Segments; j++)
		{
			GameObject gameObject = new GameObject("Level");
			gameObject.AddComponent<CanvasRenderer>();	
			gameObject.transform.localPosition = new Vector3((float)j * (num + _FreeSpace), 0f);
			gameObject.transform.SetParent(_Background, false);
			UIPoligon uIPoligon = gameObject.AddComponent<UIPoligon>();
			uIPoligon.Points.Add(default(Vector2));
			uIPoligon.Points.Add(new Vector2(_Sdvig, _Height));
			uIPoligon.Points.Add(new Vector2(_Sdvig + num, _Height));
			uIPoligon.Points.Add(new Vector2(num, 0f));
			uIPoligon.Refresh();
			uIPoligon.raycastTarget = false;
			_Poligons.Add(uIPoligon);
		}
	}

	public void OnTap()
	{
		DialogNotificationManager.ShowEnergyDialog(null, 0);
	}
}
