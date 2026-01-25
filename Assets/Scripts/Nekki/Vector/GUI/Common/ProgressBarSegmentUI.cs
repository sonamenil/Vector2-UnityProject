using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Nekki.Vector.GUI.Common
{
	public class ProgressBarSegmentUI : MonoBehaviour
	{
		private const int _DefaultHeight = 175;

		[SerializeField]
		private Image _BackgroundLine;

		[SerializeField]
		private Image _ProgressLine;

		[SerializeField]
		private Image _AnimLine;

		[SerializeField]
		private LayoutElement _LayoutElement;

		[SerializeField]
		private ResolutionImage _BoostGlow;

		private Sequence _AnimSequence;

		[SerializeField]
		private ProgressBarSegmentUIParameters _Parameters;

		private int _CurrentProgress;

		public ProgressBarSegmentUIParameters Parameters
		{
			get
			{
				return _Parameters;
			}
			set
			{
				if (_Parameters != value && _Parameters != null)
				{
					_Parameters = value;
					Refresh();
				}
			}
		}

		public bool ShowAnimation
		{
			get
			{
				return _Parameters.ShowAnimation;
			}
			set
			{
				if (_Parameters.ShowAnimation != value)
				{
					_Parameters.ShowAnimation = value;
					Refresh();
				}
			}
		}

		public int CurrentProgress
		{
			get
			{
				return _CurrentProgress;
			}
			set
			{
				if (_CurrentProgress != value)
				{
					_CurrentProgress = Mathf.Clamp(value, 0, _Parameters.MaxProgress);
					UpdateUI();
				}
			}
		}

		public void SetParametersAndProgress(ProgressBarSegmentUIParameters p_parameters, int p_value)
		{
			_Parameters = p_parameters;
			_CurrentProgress = Mathf.Clamp(p_value, 0, _Parameters.MaxProgress);
			Refresh();
		}

		public void Refresh()
		{
			StopAnim();
			SetupUI();
			UpdateUI();
		}

		private void SetupUI()
		{
			_BackgroundLine.color = _Parameters.BackgroundColor;
			_AnimLine.color = _Parameters.ProgressColor;
			_AnimLine.enabled = ShowAnimation;
			_AnimLine.rectTransform.sizeDelta = new Vector2(_Parameters.LineWidth, _Parameters.LineHeight);
			_BackgroundLine.rectTransform.sizeDelta = new Vector2(_Parameters.LineWidth, _Parameters.LineHeight);
			_ProgressLine.rectTransform.sizeDelta = new Vector2(_Parameters.LineWidth, _Parameters.LineHeight);
			_LayoutElement.minHeight = _Parameters.LineHeight;
			_LayoutElement.preferredHeight = -1f;
			_LayoutElement.minWidth = _Parameters.LineWidth;
			_LayoutElement.preferredWidth = -1f;
		}

		private void UpdateUI()
		{
			float y = _BackgroundLine.rectTransform.sizeDelta.y;
			float num = 0f;
			_BoostGlow.enabled = false;
			_ProgressLine.enabled = true;
			_BackgroundLine.enabled = true;
			_AnimLine.enabled = ShowAnimation;
			switch (Parameters.FillType)
			{
			case ProgressBarSegmentFillType.Boosted:
				num = 1f;
				_BoostGlow.SpriteName = _Parameters.BoostGlowSpriteName;
				_BoostGlow.enabled = true;
				_ProgressLine.enabled = false;
				_BackgroundLine.enabled = false;
				break;
			case ProgressBarSegmentFillType.Empty:
				num = 0f;
				break;
			case ProgressBarSegmentFillType.Filled:
				num = ((!_Parameters.ShowAnimation) ? 1f : 0f);
				break;
			case ProgressBarSegmentFillType.UseProgress:
				num = (float)_CurrentProgress / (float)_Parameters.MaxProgress;
				break;
			}
			_ProgressLine.rectTransform.sizeDelta = new Vector2(_Parameters.LineWidth, num * y);
			_ProgressLine.color = _Parameters.ProgressColor;
			if (ShowAnimation)
			{
				float num2 = 1f;
				if (Parameters.FillType != ProgressBarSegmentFillType.Filled)
				{
					num2 = (float)_Parameters.AnimatedProgress / (float)_Parameters.MaxProgress;
				}
				_AnimLine.rectTransform.sizeDelta = new Vector2(_Parameters.LineWidth, num2 * y);
				StartAnim();
			}
		}

		public void StopAnim()
		{
			if (_AnimSequence != null)
			{
				_AnimSequence.Kill(true);
				_AnimSequence = null;
			}
		}

		private void StartAnim()
		{
			Color color = _AnimLine.color;
			_AnimLine.color = new Color(color.r, color.g, color.b, 1f);
			_AnimSequence = DOTween.Sequence();
			_AnimSequence.Append(_AnimLine.DOFade(0f, 0.8f).SetEase(Ease.InOutQuad));
			_AnimSequence.Append(_AnimLine.DOFade(1f, 0.8f).SetEase(Ease.InOutQuad));
			_AnimSequence.SetLoops(-1, LoopType.Restart);
			_AnimSequence.Play();
		}
	}
}
