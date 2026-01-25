using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ZoneScroll : ScrollRect
{
	private bool _IsDraging;

	private float _CanvasWidth;

	private int _Page;

	private Vector2 _Beginposition;

	private Vector2 _FixedPositio;

	private Vector3 prevPosition;

	protected override void Awake()
	{
		base.Awake();
	}

	protected override void Start()
	{
		base.Start();
		Canvas componentInParent = GetComponentInParent<Canvas>();
		_CanvasWidth = componentInParent.GetComponent<RectTransform>().sizeDelta.x;
	}

	public override void OnBeginDrag(PointerEventData eventData)
	{
		base.OnBeginDrag(eventData);
		_IsDraging = true;
		_Beginposition = base.content.anchoredPosition;
	}

	public override void OnDrag(PointerEventData eventData)
	{
		base.OnDrag(eventData);
	}

	public override void OnEndDrag(PointerEventData eventData)
	{
		base.OnEndDrag(eventData);
		_IsDraging = false;
		Vector2 anchoredPosition = base.content.anchoredPosition;
		float num = anchoredPosition.x - _Beginposition.x;
		int num2 = (int)Math.Round((0f - anchoredPosition.x) / _CanvasWidth, MidpointRounding.AwayFromZero);
		if (num2 == _Page)
		{
			if (num < 0f)
			{
				_Page++;
			}
			else
			{
				_Page--;
			}
		}
		else
		{
			_Page = num2;
		}
		if (_Page < 0)
		{
			_Page = 0;
		}
		DebugUtils.Log(_Page.ToString());
		_FixedPositio = new Vector2((float)(-_Page) * _CanvasWidth, 0f);
	}

	public override void OnScroll(PointerEventData data)
	{
		base.OnScroll(data);
		DebugUtils.Log("OnScroll:\n" + data.ToString());
	}

	protected override void LateUpdate()
	{
		float unscaledDeltaTime = Time.unscaledDeltaTime;
		Vector2 vector = _FixedPositio - base.content.anchoredPosition;
		vector.y = 0f;
		if (!_IsDraging && (vector != Vector2.zero || base.velocity != Vector2.zero))
		{
			Vector2 anchoredPosition = base.content.anchoredPosition;
			Vector2 vector2 = base.velocity;
			if (vector[0] != 0f)
			{
				float currentVelocity = vector2[0];
				anchoredPosition[0] = Mathf.SmoothDamp(base.content.anchoredPosition[0], base.content.anchoredPosition[0] + vector[0], ref currentVelocity, base.elasticity, float.PositiveInfinity, unscaledDeltaTime);
				vector2[0] = currentVelocity;
				base.velocity = vector2;
			}
			if (vector2 != Vector2.zero)
			{
				SetContentAnchoredPosition(anchoredPosition);
			}
		}
	}
}
