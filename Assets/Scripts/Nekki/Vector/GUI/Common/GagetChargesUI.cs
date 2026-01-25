using System;
using System.Collections.Generic;
using DG.Tweening;
using Nekki.Vector.Core.GameManagement;
using UIFigures;
using UnityEngine;

namespace Nekki.Vector.GUI.Common
{
	[ExecuteInEditMode]
	public class GagetChargesUI : MonoBehaviour
	{
		[SerializeField]
		private int _Segments = 10;

		[SerializeField]
		private int _BonusSegments;

		[SerializeField]
		private int _TotalSegments = 10;

		[Range(0f, 100f)]
		[SerializeField]
		private int _FreePercent;

		[SerializeField]
		private float _SegmentWidth = 10f;

		[SerializeField]
		private float _FrameWidth = 5f;

		[SerializeField]
		private Color _SegmentsColor = Color.black;

		[SerializeField]
		private Color _BonusSegmentsColor = Color.black;

		[SerializeField]
		private Color _FrameColor = Color.black;

		[SerializeField]
		private bool _Refresh;

		private RectTransform _RectTransform;

		private List<UIArcBorder> _SegmentsObject = new List<UIArcBorder>();

		public float angl;

		public int Segments
		{
			get
			{
				return _Segments;
			}
		}

		public int BonusSegments
		{
			get
			{
				return _BonusSegments;
			}
		}

		public Color FrameColor
		{
			get
			{
				return _FrameColor;
			}
			set
			{
				_FrameColor = value;
			}
		}

		private void Awake()
		{
			_RectTransform = base.gameObject.GetComponent<RectTransform>();
		}

		public void Init(GadgetItem p_gadget)
		{
			_Segments = Mathf.Min((p_gadget != null) ? p_gadget.CurrentCharges : 0, _TotalSegments);
			_BonusSegments = Mathf.Min((p_gadget != null) ? p_gadget.BonusCharges : 0, _TotalSegments - Segments);
			Init();
		}

		private void Init()
		{
			int i = 0;
			for (int count = _SegmentsObject.Count; i < count; i++)
			{
				UnityEngine.Object.DestroyImmediate(_SegmentsObject[i].gameObject);
			}
			_SegmentsObject.Clear();
			float num = (float)Math.PI * 2f / (float)_TotalSegments;
			float num2 = num * (1f - (float)_FreePercent / 100f);
			i = 0;
			for (int count = Mathf.Min(_Segments, _TotalSegments); i < count; i++)
			{
				CreateSegment(20, _SegmentWidth, num * (float)i, num * (float)i + num2, _SegmentsColor);
			}
			for (int count = Mathf.Min(i + _BonusSegments, _TotalSegments); i < count; i++)
			{
				CreateSegment(20, _SegmentWidth, num * (float)i, num * (float)i + num2, _BonusSegmentsColor);
			}
			if (_TotalSegments != _Segments + _BonusSegments)
			{
				if (_Segments == 0 && _BonusSegments == 0)
				{
					CreateSegment(60, _FrameWidth, 0f, (float)Math.PI * 2f, _FrameColor);
				}
				else
				{
					CreateSegment(60, _FrameWidth, num * (float)(_Segments + _BonusSegments), (float)Math.PI * 2f - (num - num2), _FrameColor);
				}
			}
			_RectTransform.localRotation = Quaternion.AngleAxis(-90f - 57.29578f * num2 / 2f, Vector3.forward);
		}

		private void CreateSegment(int p_segments, float p_width, float p_from, float p_to, Color p_color)
		{
			GameObject gameObject = new GameObject("Arc");
			gameObject.AddComponent<CanvasRenderer>();
			gameObject.transform.SetParent(base.transform, false);
			UIArcBorder uIArcBorder = gameObject.AddComponent<UIArcBorder>();
			uIArcBorder.Segments = p_segments;
			uIArcBorder.From = p_from;
			uIArcBorder.To = p_to;
			uIArcBorder.Width = p_width;
			uIArcBorder.SetAllColor = p_color;
			RectTransform rectTransform = uIArcBorder.rectTransform;
			rectTransform.anchorMax = new Vector2(1f, 1f);
			rectTransform.anchorMin = default(Vector2);
			rectTransform.offsetMax = default(Vector2);
			rectTransform.offsetMin = default(Vector2);
			rectTransform.sizeDelta = default(Vector2);
			_SegmentsObject.Add(uIArcBorder);
		}

		public void ChangeAllSegments(int p_segments, int p_bonusSegments)
		{
			p_segments = Mathf.Min(p_segments, _TotalSegments);
			p_bonusSegments = Mathf.Min(p_bonusSegments, _TotalSegments - p_segments);
			if ((_Segments == p_segments && _BonusSegments == p_bonusSegments) || (p_segments < 0 && p_bonusSegments < 0))
			{
				return;
			}
			if (p_segments > _Segments || p_bonusSegments > _BonusSegments)
			{
				_Segments = p_segments;
				_BonusSegments = p_bonusSegments;
				Init();
				return;
			}
			int num = _Segments - p_segments;
			_Segments = p_segments;
			int i = 0;
			int segments = _Segments;
			for (; i < num; i++)
			{
				_SegmentsObject[segments + i].gameObject.AddComponent<GadgetChargeBlinkDestroy>().Init(_SegmentsObject[segments + i], Color.red, 0f, HideSegment, KillSegment, 2);
			}
			num = _BonusSegments - p_bonusSegments;
			_BonusSegments = p_bonusSegments;
			i = 0;
			segments = _Segments + _BonusSegments;
			for (; i < num; i++)
			{
				_SegmentsObject[segments + i].gameObject.AddComponent<GadgetChargeBlinkDestroy>().Init(_SegmentsObject[segments + i], Color.red, 0f, HideSegment, KillSegment, 2);
			}
			BlinkAllSegments(1, Color.red, 0.1f);
		}

		private void KillSegment(GadgetChargeBlinkDestroy p_segment)
		{
			UnityEngine.Object.Destroy(p_segment.Target.gameObject);
			RefreshMainArc();
		}

		private void HideSegment(GadgetChargeBlinkDestroy p_segment)
		{
			_SegmentsObject.Remove(p_segment.Target);
			RefreshMainArc();
		}

		private void BlinkAllSegments(int p_times, Color p_color, float p_wait)
		{
			int i = 0;
			for (int count = _SegmentsObject.Count; i < count; i++)
			{
				if (i < _Segments || i == _SegmentsObject.Count - 1)
				{
					Sequence sequence = DOTween.Sequence();
					for (int j = 0; j < p_times; j++)
					{
						sequence.Append(_SegmentsObject[i].DOColor(p_color, 0f));
						sequence.AppendInterval(p_wait);
						sequence.Append(_SegmentsObject[i].DOColor(_SegmentsObject[i].color, 0f));
						sequence.AppendInterval(p_wait);
					}
					sequence.Append(_SegmentsObject[i].DOColor(_SegmentsObject[i].color, 0f));
					sequence.Play();
				}
			}
		}

		private void RefreshMainArc()
		{
			float num = (float)Math.PI * 2f / (float)_TotalSegments;
			float num2 = num * (1f - (float)_FreePercent / 100f);
			UIArcBorder uIArcBorder = _SegmentsObject[_SegmentsObject.Count - 1];
			if (_Segments == 0 && _BonusSegments == 0)
			{
				uIArcBorder.From = 0f;
				uIArcBorder.To = (float)Math.PI * 2f;
			}
			else
			{
				uIArcBorder.From = num * (float)(_SegmentsObject.Count - 1);
				uIArcBorder.To = (float)Math.PI * 2f - (num - num2);
			}
			uIArcBorder.color = FrameColor;
			uIArcBorder.Refresh();
		}

		private void Update()
		{
			if (_Refresh)
			{
				Init();
			}
		}
	}
}
