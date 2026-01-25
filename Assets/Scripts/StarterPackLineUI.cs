using System.Collections.Generic;
using Nekki.Vector.GUI.MainScene;
using UIFigures;
using UnityEngine;

public class StarterPackLineUI : MonoBehaviour
{
	[SerializeField]
	private UILine _Line;

	[SerializeField]
	private List<Vector2> _Points;

	[SerializeField]
	private float _Distance = 1500f;

	private float _Time;

	private bool _FirstRun = true;

	private int _State = -1;

	private float _cachedY;

	private float _TimeToDo;

	private FloorButton _LastItem;

	private void Update()
	{
		if (_State == -1)
		{
			return;
		}
		_Time += Time.deltaTime;
		switch (_State)
		{
		case 0:
		{
			float x;
			if (_Time > _TimeToDo)
			{
				x = _Points[2].x;
				_Time = 0f;
				_State = 1;
				_TimeToDo = Mathf.Abs(_cachedY - _Points[2].y) / _Distance;
			}
			else
			{
				x = Mathf.Lerp(_Points[3].x, _Points[2].x, _Time / _TimeToDo);
			}
			_Line.Points[3] = new Vector2(x, _Line.Points[3].y);
			break;
		}
		case 1:
		{
			float y;
			if (_Time > _TimeToDo)
			{
				y = _Points[2].y;
				_Time = 0f;
				_State = 2;
				_TimeToDo = Mathf.Abs(_Line.Points[2].x - _Points[3].x) / _Distance;
			}
			else
			{
				y = Mathf.Lerp(_cachedY, _Points[2].y, _Time / _TimeToDo);
			}
			_Line.Points[2] = new Vector2(_Points[2].x, y);
			_Line.Points[3] = new Vector2(_Line.Points[3].x, y);
			break;
		}
		case 2:
		{
			float x;
			if (_Time > _TimeToDo)
			{
				x = _Points[3].x;
				_Time = 0f;
				_State = -1;
			}
			else
			{
				x = Mathf.Lerp(_Points[2].x, _Points[3].x, _Time / _TimeToDo);
			}
			_Line.Points[3] = new Vector2(x, _Line.Points[3].y);
			break;
		}
		case 3:
		{
			float x;
			if (_Time > _TimeToDo)
			{
				x = _Points[1].x;
				_Time = 0f;
				_State = 1;
				_TimeToDo = Mathf.Abs(_cachedY - _Points[2].y) / _Distance;
			}
			else
			{
				x = Mathf.Lerp(_Points[0].x, _Points[1].x, _Time / _TimeToDo);
			}
			_Line.Points[1] = new Vector2(x, _Line.Points[0].y);
			_Line.Points[2] = new Vector2(x, _Line.Points[0].y);
			_Line.Points[3] = new Vector2(x, _Line.Points[0].y);
			break;
		}
		}
		_Line.Refresh();
	}

	public void SetLine(FloorButton p_item)
	{
		_LastItem = p_item;
		if (p_item.StarterPackItem == null || p_item.StarterPackItem.IsBlock)
		{
			_Line.Points[0] = new Vector2(365f, _Line.Points[0].y);
			_Points[0] = new Vector2(365f, _Line.Points[0].y);
		}
		else
		{
			_Line.Points[0] = new Vector2(220f, _Line.Points[0].y);
			_Points[0] = new Vector2(220f, _Line.Points[0].y);
		}
		if (p_item.StarterPackItem == null || p_item.StarterPackItem.IsBlock)
		{
			_Line.color = new Color(0.49f, 0.522f, 0.576f, 0.5f);
		}
		else
		{
			_Line.color = new Color(0.369f, 0.643f, 0.733f, 0.8f);
		}
		switch (_State)
		{
		case -1:
			if (_FirstRun)
			{
				_FirstRun = false;
				_State = 3;
				_TimeToDo = Mathf.Abs(_Points[1].x - _Line.Points[0].x) / _Distance;
			}
			else
			{
				_State = 0;
				_TimeToDo = Mathf.Abs(_Line.Points[3].x - _Points[2].x) / _Distance;
			}
			break;
		case 2:
			_State = 0;
			break;
		case 1:
			_TimeToDo = Mathf.Abs(_Line.Points[2].y - _Points[2].y) / _Distance;
			break;
		}
		_Time = 0f;
		if (_State != 3)
		{
			_cachedY = _Line.Points[2].y;
		}
		else
		{
			_cachedY = _Line.Points[1].y;
		}
		_Points[2] = new Vector2(_Points[2].x, p_item.transform.localPosition.y + 77f);
		_Points[3] = new Vector2(((RectTransform)p_item.transform.parent).anchoredPosition.x - 165f, p_item.transform.localPosition.y + 77f);
		_Line.Refresh();
	}

	public void UpdateLine()
	{
		if (_State == -1 && _LastItem != null)
		{
			Transform transform = _LastItem.transform;
			_Line.Points[3] = new Vector2(((RectTransform)transform.parent).anchoredPosition.x + transform.localPosition.x + _LastItem.Content.transform.localPosition.x - 125f, transform.localPosition.y + 77f);
			_Line.Refresh();
		}
	}
}
