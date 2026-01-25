using DG.Tweening;
using Nekki.Vector.Core.Controllers;
using Nekki.Vector.Core.Localization;
using UnityEngine;

namespace Nekki.Vector.GUI.Scenes.Run
{
	public class TutorialUI : UIModule
	{
		[SerializeField]
		private LabelAlias _TextKeyboard;

		[SerializeField]
		private LabelAlias _TextMobile;

		[SerializeField]
		private ResolutionImage _ArrowSprite;

		[SerializeField]
		private ResolutionImage _HandSprite;

		[SerializeField]
		private ResolutionImage _FingerTrace;

		[SerializeField]
		private RectTransform _Mobile;

		[SerializeField]
		private RectTransform _Keyboard;

		public void SetTutorialStep(Key p_key, string p_textMobile, string p_textKeyboard)
		{
			Reset();
			if (DeviceInformation.IsMobile)
			{
				_Mobile.gameObject.SetActive(true);
				_TextMobile.SetAlias(p_textMobile);
				switch (p_key)
				{
				case Key.Up:
					SetupFingerTrace(new Vector2(-169f, 88f), 0f);
					RunPositionTween(new Vector2(340f, -90f), new Vector2(340f, 180f), 1f);
					break;
				case Key.Down:
					SetupFingerTrace(new Vector2(-167f, 29f), 180f);
					RunPositionTween(new Vector2(340f, 180f), new Vector2(340f, -90f), 1f);
					break;
				case Key.Left:
					SetupFingerTrace(new Vector2(-200f, 55f), 90f);
					RunPositionTween(new Vector2(670f, 47f), new Vector2(400f, 47f), 1f);
					break;
				case Key.Right:
					SetupFingerTrace(new Vector2(-136f, 55f), 270f);
					RunPositionTween(new Vector2(400f, 47f), new Vector2(670f, 47f), 1f);
					break;
				}
				RunAlphaTween(0f, 1f, 1f);
			}
			else
			{
				_Keyboard.gameObject.SetActive(true);
				_TextKeyboard.SetAlias(p_textKeyboard);
				switch (p_key)
				{
				case Key.Up:
					SetupHelpArrow(0f);
					break;
				case Key.Down:
					SetupHelpArrow(180f);
					break;
				case Key.Left:
					SetupHelpArrow(90f);
					break;
				case Key.Right:
					SetupHelpArrow(270f);
					break;
				}
			}
		}

		private void Reset()
		{
			DOTween.Kill(_HandSprite.rectTransform);
			DOTween.Kill(_FingerTrace);
			_Mobile.gameObject.SetActive(false);
			_Keyboard.gameObject.SetActive(false);
		}

		private void RunPositionTween(Vector2 p_from, Vector2 p_to, float p_duration)
		{
			_HandSprite.rectTransform.localPosition = p_from;
			_HandSprite.rectTransform.DOLocalMove(p_to, p_duration).SetLoops(100500, LoopType.Restart);
		}

		private void RunAlphaTween(float p_from, float p_to, float p_duration)
		{
			_FingerTrace.Alpha = p_from;
			_FingerTrace.DOFade(p_to, p_duration).SetLoops(100500, LoopType.Restart);
		}

		private void SetupFingerTrace(Vector2 p_pos, float p_angle)
		{
			_FingerTrace.rectTransform.localPosition = p_pos;
			_FingerTrace.rectTransform.localEulerAngles = new Vector3(0f, 0f, p_angle);
		}

		private void SetupHelpArrow(float p_angle)
		{
			_ArrowSprite.rectTransform.localEulerAngles = new Vector3(0f, 0f, p_angle);
		}
	}
}
