using System;
using DG.Tweening;
using Nekki.Vector.Core.Localization;
using Nekki.Vector.GUI.Common;
using UnityEngine;
using UnityEngine.UI;

namespace Nekki.Vector.GUI.Dialogs
{
	public class Tooltip : BaseDialog
	{
		public enum AnimationType
		{
			Instant = 0,
			Fade = 1
		}

		public enum Pivot
		{
			TopLeft = 0,
			TopRight = 1,
			BottomLeft = 2,
			BottomRight = 3
		}

		public enum State
		{
			Inactive = 0,
			Opening = 1,
			Active = 2,
			Closing = 3
		}

		public class UISettings
		{
			public string Text;

			public string Title;

			public RectTransform Parent;

			public Vector2 Shift = new Vector2(0f, 0f);

			public Pivot Pivot;

			public AnimationType OpenAnimation;

			public AnimationType CloseAnimation;

			public bool BlockTouches;

			public DialogQueueType QueueType = DialogQueueType.Notification;

			public Action OnDismiss;

			public DialogButtonData ButtonData;
		}

		private const float _AnimationDuration = 0.3f;

		private const float _ContentConstSize = 82f;

		private static float _LastCreatedTooltipTime;

		private static string _LastCreatedTooltipText;

		[SerializeField]
		private RectTransform _ContentRoot;

		[SerializeField]
		private CanvasGroup _CanvasGroup;

		[SerializeField]
		private RectTransform _Pivot;

		[SerializeField]
		private LabelAlias _Text;

		[SerializeField]
		private LabelAlias _Title;

		[SerializeField]
		private ButtonUI _Button;

		private UISettings _UISettings;

		private Sequence _OpenAnimation;

		private Sequence _CloseAnimation;

		private State _CurrentState;

		private float ContentHeight
		{
			get
			{
				return 82f + _Text.preferredHeight + _Title.preferredHeight + ((!_Button.gameObject.activeSelf) ? 0f : _Button.GetComponent<RectTransform>().sizeDelta.y);
			}
		}

		public void Init(UISettings p_uiSettings)
		{
			_UISettings = p_uiSettings;
			_CanvasGroup.alpha = 0f;
			if (_OpenAnimation != null)
			{
				_OpenAnimation.Kill();
			}
			if (_CloseAnimation != null)
			{
				_CloseAnimation.Kill();
			}
			SetContent();
			SetPosition();
		}

		private void SetContent()
		{
			_Text.SetAlias(_UISettings.Text);
			_Button.gameObject.SetActive(false);
			_Title.gameObject.SetActive(false);
			if (_UISettings.ButtonData != null)
			{
				_Button.gameObject.SetActive(true);
				_UISettings.ButtonData.InitButton(_Button);
				_Button.Button.interactable = true;
			}
			if (_UISettings.Title != null)
			{
				_Title.gameObject.SetActive(true);
				_Title.SetAlias(_UISettings.Title);
			}
		}

		private void SetPosition()
		{
			base.transform.SetParent(_UISettings.Parent.root, false);
			base.transform.SetAsLastSibling();
			_ContentRoot.position = _UISettings.Parent.position;
			RealignSettings();
			_ContentRoot.localPosition += (Vector3)_UISettings.Shift;
			switch (_UISettings.Pivot)
			{
			case Pivot.TopLeft:
				_ContentRoot.pivot = new Vector2(0f, 1f);
				_Pivot.localScale = new Vector3(1f, 1f, 1f);
				_Pivot.anchorMin = new Vector2(0f, 1f);
				_Pivot.anchorMax = new Vector2(0f, 1f);
				break;
			case Pivot.TopRight:
				_ContentRoot.pivot = new Vector2(1f, 1f);
				_Pivot.localScale = new Vector3(-1f, 1f, 1f);
				_Pivot.anchorMin = new Vector2(1f, 1f);
				_Pivot.anchorMax = new Vector2(1f, 1f);
				break;
			case Pivot.BottomLeft:
				_ContentRoot.pivot = new Vector2(0f, 0f);
				_Pivot.localScale = new Vector3(1f, -1f, 1f);
				_Pivot.anchorMin = new Vector2(0f, 0f);
				_Pivot.anchorMax = new Vector2(0f, 0f);
				break;
			case Pivot.BottomRight:
				_ContentRoot.pivot = new Vector2(1f, 0f);
				_Pivot.localScale = new Vector3(-1f, -1f, 1f);
				_Pivot.anchorMin = new Vector2(1f, 0f);
				_Pivot.anchorMax = new Vector2(1f, 0f);
				break;
			}
		}

		private void RealignSettings()
		{
			base.gameObject.SetActive(true);
			LayoutRebuilder.ForceRebuildLayoutImmediate(_ContentRoot);
			base.gameObject.SetActive(false);
			int num = (CheckHorizontalPosition() ? 1 : (-1));
			int num2 = (CheckVerticalPosition() ? 1 : (-1));
			_UISettings.Shift.x *= num;
			_UISettings.Shift.y *= num2;
			if (num == -1)
			{
				switch (_UISettings.Pivot)
				{
				case Pivot.TopLeft:
					_UISettings.Pivot = Pivot.TopRight;
					break;
				case Pivot.TopRight:
					_UISettings.Pivot = Pivot.TopLeft;
					break;
				case Pivot.BottomLeft:
					_UISettings.Pivot = Pivot.BottomRight;
					break;
				default:
					_UISettings.Pivot = Pivot.BottomLeft;
					break;
				}
			}
			if (num2 == -1)
			{
				switch (_UISettings.Pivot)
				{
				case Pivot.TopLeft:
					_UISettings.Pivot = Pivot.BottomLeft;
					break;
				case Pivot.TopRight:
					_UISettings.Pivot = Pivot.BottomRight;
					break;
				case Pivot.BottomLeft:
					_UISettings.Pivot = Pivot.TopLeft;
					break;
				default:
					_UISettings.Pivot = Pivot.TopRight;
					break;
				}
			}
		}

		private bool CheckHorizontalPosition()
		{
			Vector2 vector = (Vector2)_ContentRoot.localPosition + _UISettings.Shift;
			Vector2 vector2 = ((_UISettings.Pivot != Pivot.TopRight && _UISettings.Pivot != Pivot.BottomRight) ? new Vector2(vector.x, vector.x + _ContentRoot.sizeDelta.x) : new Vector2(vector.x - _ContentRoot.sizeDelta.x, vector.x));
			float num = (base.transform.parent as RectTransform).sizeDelta.x * 0.5f;
			return vector2.x > 0f - num && vector2.y < num;
		}

		private bool CheckVerticalPosition()
		{
			Vector2 vector = (Vector2)_ContentRoot.localPosition + _UISettings.Shift;
			Vector2 vector2 = ((_UISettings.Pivot != 0 && _UISettings.Pivot != Pivot.TopRight) ? new Vector2(vector.y, vector.y + ContentHeight) : new Vector2(vector.y - ContentHeight, vector.y));
			float num = (base.transform.parent as RectTransform).sizeDelta.y * 0.5f;
			return vector2.x > 0f - num && vector2.y < num;
		}

		public override void Show()
		{
			_CurrentState = State.Opening;
			base.gameObject.SetActive(true);
			if (_UISettings.BlockTouches)
			{
				DialogCanvasController.Current.BlockTouches();
			}
			switch (_UISettings.OpenAnimation)
			{
			case AnimationType.Instant:
				OpenInstant();
				break;
			case AnimationType.Fade:
				OpenFade();
				break;
			default:
				DebugUtils.LogFormat("[Tooltip]: trying to use unsupported OpenAnimation - {0}!", _UISettings.OpenAnimation);
				break;
			}
		}

		public void OpenInstant()
		{
			_CanvasGroup.alpha = 1f;
			OnOpenAnimationEnd();
		}

		public void OpenFade()
		{
			_OpenAnimation = DOTween.Sequence();
			_OpenAnimation.Append(_CanvasGroup.DOFade(1f, 0.3f));
			_OpenAnimation.OnComplete(OnOpenAnimationEnd);
			_OpenAnimation.Play();
		}

		public override void Dismiss()
		{
			_CurrentState = State.Closing;
			if (_CurrentState == State.Opening && _UISettings.OpenAnimation != 0)
			{
				_OpenAnimation.Kill();
			}
			if (_UISettings.OnDismiss != null)
			{
				_UISettings.OnDismiss();
			}
			switch (_UISettings.CloseAnimation)
			{
			case AnimationType.Instant:
				CloseInstant();
				break;
			case AnimationType.Fade:
				CloseFade();
				break;
			default:
				DebugUtils.LogFormat("[Tooltip]: trying to use unsupported CloseAnimation - {0}!", _UISettings.CloseAnimation);
				break;
			}
		}

		public void CloseInstant()
		{
			_CanvasGroup.alpha = 0f;
			OnCloseAnimationEnd();
		}

		public void CloseFade()
		{
			_CloseAnimation = DOTween.Sequence();
			_CloseAnimation.Append(_CanvasGroup.DOFade(0f, 0.3f));
			_CloseAnimation.OnComplete(OnCloseAnimationEnd);
			_CloseAnimation.Play();
		}

		private void OnOpenAnimationEnd()
		{
			_CurrentState = State.Active;
		}

		private void OnCloseAnimationEnd()
		{
			_CurrentState = State.Inactive;
			base.gameObject.SetActive(false);
			if (_UISettings.BlockTouches)
			{
				DialogCanvasController.Current.UnBlockTouches();
			}
			if (_UISettings.QueueType == DialogQueueType.Dialog)
			{
				DialogNotificationManager.DialogsQueue.ShowNext();
			}
			else if (_UISettings.QueueType == DialogQueueType.Notification)
			{
				DialogNotificationManager.NotificationsQueue.ShowNext();
			}
		}

		public static bool CheckTooltipDouplicate(UISettings p_uiSettings)
		{
			if (_LastCreatedTooltipText == p_uiSettings.Text && Time.realtimeSinceStartup - _LastCreatedTooltipTime < 0.4f)
			{
				return false;
			}
			_LastCreatedTooltipText = p_uiSettings.Text;
			_LastCreatedTooltipTime = Time.realtimeSinceStartup;
			return true;
		}

		private void Update()
		{
			if (_CurrentState != 0 && _CurrentState != State.Closing && (Input.touchCount > 0 || Input.GetMouseButton(0)))
			{
				Dismiss();
			}
		}

		public virtual void OnButtonTap()
		{
			_Button.Button.interactable = false;
			_UISettings.ButtonData.Activate(this);
		}

		protected override void OnDestroy()
		{
			base.OnDestroy();
			CloseInstant();
		}
	}
}
