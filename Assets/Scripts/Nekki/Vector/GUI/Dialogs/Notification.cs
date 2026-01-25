using System;
using DG.Tweening;
using Nekki.Vector.Core.Audio;
using Nekki.Vector.Core.GameManagement;
using UnityEngine;

namespace Nekki.Vector.GUI.Dialogs
{
	public abstract class Notification : BaseDialog
	{
		public enum Orientation
		{
			Top = 0,
			Bottom = 1,
			Right = 2,
			Left = 3,
			LeftBottom = 4
		}

		public enum HideBy
		{
			Click = 0,
			TimeOrClick = 1,
			TimeBlockClicks = 2,
			TimeDontBlockClicks = 3
		}

		public class Parameters
		{
			public Orientation Orientation;

			public HideBy HideBy;

			public string Image;

			public string Text = string.Empty;

			public string Card;

			public MissionItem Mission;

			public DialogQueueType QueueType;

			public Action Callback = delegate
			{
			};
		}

		protected const float _MoveTime = 0.7f;

		protected const float _DelayBeforeHideByTime = 2.5f;

		protected const float _MaxSize = 350f;

		[SerializeField]
		protected RectTransform _Root;

		[SerializeField]
		protected RectTransform _TweenRoot;

		protected Parameters _Parameters;

		protected Vector3 _ShowDeltaPosition;

		protected string _SoundName = "bottom_notify";

		protected Sequence _MoveSequence;

		protected bool _IgnoreActions;

		public override void Show()
		{
			AudioManager.PlaySound(_SoundName);
			PlayTweenForward();
		}

		public override void Dismiss()
		{
			base.gameObject.SetActive(false);
			_Parameters.Callback();
			DialogCanvasController.Current.UnBlockTouches();
			if (_Parameters.QueueType == DialogQueueType.Dialog)
			{
				DialogNotificationManager.DialogsQueue.ShowNext();
			}
			else if (_Parameters.QueueType == DialogQueueType.Notification)
			{
				DialogNotificationManager.NotificationsQueue.ShowNext();
			}
		}

		protected void SetRootPosition()
		{
			Vector2 vector = Vector2.zero;
			switch (_Parameters.Orientation)
			{
			case Orientation.Top:
				vector = new Vector2(0.5f, 1f);
				_ShowDeltaPosition = new Vector3(0f, _Root.sizeDelta.y, 0f);
				break;
			case Orientation.Bottom:
				vector = new Vector2(0.5f, 0f);
				_ShowDeltaPosition = new Vector3(0f, 0f - _Root.sizeDelta.y, 0f);
				break;
			case Orientation.Right:
				vector = new Vector2(1f, 0.5f);
				_ShowDeltaPosition = new Vector3(_Root.sizeDelta.x, 0f, 0f);
				break;
			case Orientation.Left:
				vector = new Vector2(0f, 0.5f);
				_ShowDeltaPosition = new Vector3(0f - _Root.sizeDelta.x, 0f, 0f);
				_SoundName = "notification";
				break;
			case Orientation.LeftBottom:
				vector = new Vector2(0f, 0.1f);
				_ShowDeltaPosition = new Vector3(0f - _Root.sizeDelta.x, 0f, 0f);
				_SoundName = "notification";
				break;
			}
			_Root.pivot = vector;
			_Root.anchorMin = vector;
			_Root.anchorMax = vector;
			_TweenRoot.pivot = vector;
			_TweenRoot.anchorMin = vector;
			_TweenRoot.anchorMax = vector;
		}

		public void PlayTweenForward()
		{
			if (_IgnoreActions)
			{
				return;
			}
			_IgnoreActions = true;
			if (_Parameters.HideBy == HideBy.Click || _Parameters.HideBy == HideBy.TimeBlockClicks || _Parameters.HideBy == HideBy.TimeOrClick)
			{
				DialogCanvasController.Current.BlockNotDialogTouches();
			}
			_TweenRoot.anchoredPosition = _ShowDeltaPosition;
			base.transform.SetAsLastSibling();
			base.gameObject.SetActive(true);
			_MoveSequence = DOTween.Sequence();
			_MoveSequence.Append(_TweenRoot.DOLocalMove(Vector3.zero, 0.7f));
			if (_Parameters.HideBy == HideBy.TimeOrClick)
			{
				_MoveSequence.AppendCallback(delegate
				{
					_IgnoreActions = false;
				});
			}
			if (_Parameters.HideBy == HideBy.TimeBlockClicks || _Parameters.HideBy == HideBy.TimeDontBlockClicks || _Parameters.HideBy == HideBy.TimeOrClick)
			{
				_MoveSequence.AppendInterval(2.5f);
				_MoveSequence.OnComplete(delegate
				{
					_IgnoreActions = false;
					PlayTweenBackward();
				});
			}
			if (_Parameters.HideBy == HideBy.Click)
			{
				_MoveSequence.OnComplete(delegate
				{
					_IgnoreActions = false;
				});
			}
			_MoveSequence.Play();
		}

		public void PlayTweenBackward()
		{
			if (!_IgnoreActions)
			{
				_IgnoreActions = true;
				_TweenRoot.anchoredPosition = Vector3.zero;
				_MoveSequence = DOTween.Sequence();
				_MoveSequence.Append(_TweenRoot.DOLocalMove(_ShowDeltaPosition, 0.7f));
				_MoveSequence.OnComplete(delegate
				{
					_IgnoreActions = false;
					Dismiss();
				});
				_MoveSequence.Play();
			}
		}

		private void Update()
		{
			if ((_Parameters.HideBy == HideBy.Click || _Parameters.HideBy == HideBy.TimeOrClick) && (Input.anyKeyDown || Input.touchCount > 0))
			{
				PlayTweenBackward();
			}
		}
	}
}
