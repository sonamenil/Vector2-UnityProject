using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PlateScroller : ScrollRect
{
	[SerializeField]
	private bool _IsVertical = true;

	[SerializeField]
	private GridLayoutGroup _GridContent;

	private List<GameObject> _UpEmptyGO;

	private List<GameObject> _DownEmptyGO;

	private bool _IsDrag;

	private RectTransform _ScrollerTransform;

	private RectTransform _ContentTransform;

	private Sequence _MoveTweener;

	private Sequence _ReturnTweener;

	public Action<int, float> OnMove;

	public Action<int> OnStop;

	private float _PrevPosition;

	private float[] _Speeds = new float[10];

	private int _CurSpeedIndex;

	private float ScrollerLen
	{
		get
		{
			return (!_IsVertical) ? _ScrollerTransform.sizeDelta.x : _ScrollerTransform.sizeDelta.y;
		}
	}

	private float PlateLen
	{
		get
		{
			return (!_IsVertical) ? _GridContent.cellSize.x : _GridContent.cellSize.y;
		}
	}

	private float ContentPosition
	{
		get
		{
			return (!_IsVertical) ? (0f - _ContentTransform.anchoredPosition.x) : _ContentTransform.anchoredPosition.y;
		}
		set
		{
			if (_IsVertical)
			{
				_ContentTransform.anchoredPosition = new Vector2(0f, value);
			}
			else
			{
				_ContentTransform.anchoredPosition = new Vector2(0f - value, 0f);
			}
		}
	}

	private float MinPosition
	{
		get
		{
			return PositionByPlate(0);
		}
	}

	private float MaxPosition
	{
		get
		{
			return PositionByPlate(PlateCount - 1);
		}
	}

	private float CalculateOffset
	{
		get
		{
			if (_IsVertical)
			{
				if (ContentPosition < MinPosition)
				{
					return ContentPosition - MinPosition;
				}
				if (ContentPosition > MaxPosition)
				{
					return ContentPosition - MaxPosition;
				}
				return 0f;
			}
			return 0f;
		}
	}

	private int PlateCount
	{
		get
		{
			return _ContentTransform.childCount - _DownEmptyGO.Count - _UpEmptyGO.Count;
		}
	}

	private float AvergeSpeed
	{
		get
		{
			float num = 0f;
			for (int i = 0; i < _Speeds.Length; i++)
			{
				num += _Speeds[i];
			}
			return num / (float)_Speeds.Length;
		}
	}

	public void SetPlateToCenter(int p_plate, bool p_withAnim = false, int p_plateStart = 0)
	{
		if (p_plate < 0)
		{
			p_plate = 0;
		}
		if (p_plate >= PlateCount)
		{
			p_plate = PlateCount - 1;
		}
		float num = PositionByPlate(p_plate);
		if (!p_withAnim)
		{
			ContentPosition = num;
			if (OnMove != null)
			{
				OnMove(p_plate, 1f);
			}
			if (OnStop != null)
			{
				OnStop(p_plate);
			}
			return;
		}
		if (_MoveTweener != null)
		{
			_MoveTweener.Kill();
		}
		_MoveTweener = DOTween.Sequence();
		if (_IsVertical)
		{
			_MoveTweener.Append(_ContentTransform.DOLocalMoveY(num, 0.2f * (float)Mathf.Abs(p_plateStart - p_plate)).SetEase(Ease.InOutSine));
		}
		else
		{
			_MoveTweener.Append(_ContentTransform.DOLocalMoveX(num, 0.2f * (float)Mathf.Abs(p_plateStart - p_plate)).SetEase(Ease.InOutSine));
		}
		_MoveTweener.OnKill(OnMoveTweenerKill);
		_MoveTweener.OnComplete(OnTweenerComplete);
		_MoveTweener.Play();
	}

	public void AddChild(Transform p_transform)
	{
		p_transform.SetParent(base.content, false);
	}

	public void CreateEmpty(int p_up, int p_down, GameObject p_prefab)
	{
		if (_UpEmptyGO.Count == 0 && _DownEmptyGO.Count == 0)
		{
			for (int i = 0; i < p_up; i++)
			{
				GameObject gameObject = UnityEngine.Object.Instantiate(p_prefab);
				gameObject.transform.SetParent(base.content, false);
				gameObject.GetComponent<PlateScrollerEmptyPlate>().Init(CreateToMinReturnTweener);
				_UpEmptyGO.Add(gameObject);
			}
			for (int j = 0; j < p_down; j++)
			{
				GameObject gameObject2 = UnityEngine.Object.Instantiate(p_prefab);
				gameObject2.transform.SetParent(base.content, false);
				gameObject2.GetComponent<PlateScrollerEmptyPlate>().Init(CreateToMaxReturnTweener);
				_DownEmptyGO.Add(gameObject2);
			}
		}
		for (int k = 0; k < _UpEmptyGO.Count; k++)
		{
			_UpEmptyGO[k].transform.SetSiblingIndex(0);
		}
		for (int l = 0; l < _DownEmptyGO.Count; l++)
		{
			_DownEmptyGO[l].transform.SetSiblingIndex(100);
		}
	}

	private float PositionByPlate(int p_index)
	{
		if (p_index < 0)
		{
			p_index = 0;
		}
		if (p_index > PlateCount - 1)
		{
			p_index = PlateCount - 1;
		}
		return (0f - ScrollerLen) / 2f + PlateLen / 2f + (float)(p_index + _UpEmptyGO.Count) * PlateLen;
	}

	private int PlateByPosition(float p_position)
	{
		float num = p_position + ScrollerLen / 2f;
		return (!(num < 0f)) ? ((int)(num / PlateLen) - _UpEmptyGO.Count) : (-1);
	}

	private void CalcSpeed(float p_delta)
	{
		_CurSpeedIndex++;
		if (_CurSpeedIndex >= _Speeds.Length)
		{
			_CurSpeedIndex = 0;
		}
		_Speeds[_CurSpeedIndex] = (_PrevPosition - ContentPosition) / p_delta;
		_PrevPosition = ContentPosition;
	}

	private void ResetSpeed()
	{
		_CurSpeedIndex = 0;
		for (int i = 0; i < _Speeds.Length; i++)
		{
			_Speeds[i] = 0f;
		}
	}

	protected override void Awake()
	{
		base.Awake();
		_UpEmptyGO = new List<GameObject>();
		_DownEmptyGO = new List<GameObject>();
		_ScrollerTransform = GetComponent<RectTransform>();
		_ContentTransform = base.content;
		_GridContent.constraint = (_IsVertical ? GridLayoutGroup.Constraint.FixedColumnCount : GridLayoutGroup.Constraint.FixedRowCount);
		base.vertical = _IsVertical;
		base.horizontal = !_IsVertical;
	}

	private static float RubberDelta(float overStretching, float viewSize)
	{
		return (1f - 1f / (Mathf.Abs(overStretching) * 0.55f / viewSize + 1f)) * viewSize * Mathf.Sign(overStretching);
	}

	public override void OnInitializePotentialDrag(PointerEventData eventData)
	{
		base.OnInitializePotentialDrag(eventData);
		if (_MoveTweener != null)
		{
			_MoveTweener.Kill();
		}
		if (_ReturnTweener != null)
		{
			_ReturnTweener.Kill();
		}
	}

	public override void OnBeginDrag(PointerEventData eventData)
	{
		base.OnBeginDrag(eventData);
		_IsDrag = true;
		_PrevPosition = ContentPosition;
		ResetSpeed();
	}

	public override void OnDrag(PointerEventData eventData)
	{
		base.OnDrag(eventData);
		float calculateOffset = CalculateOffset;
		float num = ScrollerLen / 2f;
		if (calculateOffset != 0f)
		{
			float contentPosition = ContentPosition;
			contentPosition -= calculateOffset;
			ContentPosition = contentPosition + RubberDelta(calculateOffset, ScrollerLen);
		}
		if (ContentPosition > MaxPosition + num)
		{
			ContentPosition = MaxPosition + num;
		}
		if (ContentPosition < MinPosition - num)
		{
			ContentPosition = MinPosition - num;
		}
	}

	public override void OnEndDrag(PointerEventData eventData)
	{
		base.OnEndDrag(eventData);
		_IsDrag = false;
		CheckMovePosition();
		if (_ReturnTweener == null)
		{
			CreateMoveTweener();
		}
	}

	protected override void LateUpdate()
	{
		base.LateUpdate();
		float deltaTime = Time.deltaTime;
		if (_IsDrag || _MoveTweener != null)
		{
			CalcSpeed(deltaTime);
			SendMoveAction();
		}
		if (_MoveTweener != null)
		{
			CheckMovePosition();
		}
	}

	private void CheckMovePosition()
	{
		float contentPosition = ContentPosition;
		if (contentPosition < MinPosition)
		{
			CreateReturTweener(true);
		}
		else if (contentPosition > MaxPosition)
		{
			CreateReturTweener(false);
		}
	}

	private void SendMoveAction()
	{
		if (OnMove != null)
		{
			float contentPosition = ContentPosition;
			int num = PlateByPosition(contentPosition);
			float num2 = 0f;
			num2 = (PositionByPlate(num) - contentPosition) / (PlateLen / 2f);
			if (num < 0)
			{
				num = 0;
			}
			if (num >= PlateCount)
			{
				num = PlateCount - 1;
			}
			if (OnMove != null)
			{
				OnMove(num, num2);
			}
		}
	}

	private void CreateMoveTweener()
	{
		float avergeSpeed = AvergeSpeed;
		float p_position = ContentPosition - avergeSpeed;
		p_position = PositionByPlate(PlateByPosition(p_position));
		float duration = Mathf.Max(0.1f, 0.2f * Mathf.Abs(ContentPosition - p_position) / PlateLen);
		_MoveTweener = DOTween.Sequence();
		if (_IsVertical)
		{
			_MoveTweener.Append(_ContentTransform.DOLocalMoveY(p_position, duration).SetEase(Ease.OutSine));
		}
		else
		{
			_MoveTweener.Append(_ContentTransform.DOLocalMoveX(p_position, duration).SetEase(Ease.OutSine));
		}
		_MoveTweener.OnKill(OnMoveTweenerKill);
		_MoveTweener.OnComplete(OnTweenerComplete);
		_MoveTweener.Play();
	}

	private void CreateReturTweener(bool p_isMin, bool p_withoutFirst = false)
	{
		_MoveTweener.Kill();
		float num = AvergeSpeed * 0.1f;
		float num2 = ContentPosition - num;
		float num3 = ScrollerLen / 2f;
		if (num2 > MaxPosition + num3)
		{
			num2 = MaxPosition + num3;
		}
		if (num2 < MinPosition - num3)
		{
			num2 = MinPosition - num3;
		}
		_ReturnTweener = DOTween.Sequence();
		if (_IsVertical)
		{
			if (!p_withoutFirst)
			{
				_ReturnTweener.Append(_ContentTransform.DOLocalMoveY(num2, 0.1f).SetEase(Ease.OutSine));
			}
			_ReturnTweener.Append(_ContentTransform.DOLocalMoveY((!p_isMin) ? MaxPosition : MinPosition, 0.3f).SetEase(Ease.InSine));
		}
		else
		{
			if (!p_withoutFirst)
			{
				_ReturnTweener.Append(_ContentTransform.DOLocalMoveX(num2, 0.1f).SetEase(Ease.OutSine));
			}
			_ReturnTweener.Append(_ContentTransform.DOLocalMoveX((!p_isMin) ? MaxPosition : MinPosition, 0.1f).SetEase(Ease.InSine));
		}
		_ReturnTweener.OnKill(OnReturnTweenerKill);
		_ReturnTweener.OnComplete(OnTweenerComplete);
		_ReturnTweener.Play();
	}

	private void CreateToMinReturnTweener()
	{
		CreateReturTweener(true, true);
	}

	private void CreateToMaxReturnTweener()
	{
		CreateReturTweener(false, true);
	}

	private void OnTweenerComplete()
	{
		if (OnStop != null)
		{
			OnStop(PlateByPosition(ContentPosition));
		}
	}

	private void OnMoveTweenerKill()
	{
		_MoveTweener = null;
	}

	private void OnReturnTweenerKill()
	{
		_ReturnTweener = null;
	}
}
