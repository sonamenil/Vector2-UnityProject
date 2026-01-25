using System;
using DG.Tweening;
using Nekki.Vector.Core.Localization;
using Nekki.Vector.Core.Quest;
using Nekki.Vector.Core.User;
using UnityEngine;
using UnityEngine.UI;

namespace Nekki.Vector.GUI.Journal
{
	public class QuestScrollerPlate : MonoBehaviour
	{
		[SerializeField]
		private Image _Content;

		[SerializeField]
		private ResolutionImage _QuestState;

		[SerializeField]
		private LabelAlias _QuestName;

		[SerializeField]
		private Graphic _ComplateIcon;

		[SerializeField]
		private Graphic _ProgressIcon;

		[SerializeField]
		private Graphic _ProgressNewIcon;

		private float _DefaultSizeX = -1f;

		private Quest _Quest;

		private bool _IsNewQuest;

		private Action<QuestScrollerPlate> _OnTap;

		public Quest PlateQuest
		{
			get
			{
				return _Quest;
			}
		}

		public void Init(Quest p_quest, Action<QuestScrollerPlate> p_onTap)
		{
			_Quest = p_quest;
			_OnTap = p_onTap;
			_QuestName.SetAlias(_Quest.VisualName);
			if (_Quest.IsCompleteState)
			{
				_ProgressIcon.gameObject.SetActive(false);
				_ProgressNewIcon.gameObject.SetActive(false);
			}
			if (_Quest.IsActiveState)
			{
				_ComplateIcon.gameObject.SetActive(false);
				if (!_Quest.IsNew)
				{
					_ProgressNewIcon.gameObject.SetActive(false);
				}
				else
				{
					_IsNewQuest = true;
				}
			}
			SetNormalColorToQuestName();
		}

		public void ResizeDelta(float p_persect, float p_time = 0f, bool p_canGrow = true, bool p_canShrink = true, bool p_isTimeProportional = false)
		{
			Vector3 vector = _Content.rectTransform.sizeDelta;
			if (_DefaultSizeX == -1f)
			{
				_DefaultSizeX = vector.x;
			}
			float num = _DefaultSizeX + 55f * p_persect;
			if ((p_canGrow || num - vector.x <= 0f) && (p_canShrink || num - vector.x >= 0f))
			{
				_Content.DOKill();
				_Content.rectTransform.DOKill();
				if (p_time == 0f)
				{
					_Content.rectTransform.sizeDelta = new Vector2(num, vector.y);
					return;
				}
				if (!p_isTimeProportional)
				{
					_Content.rectTransform.DOSizeDelta(new Vector2(num, vector.y), p_time);
					return;
				}
				float duration = p_time * (1f - Mathf.Abs(vector.x - _DefaultSizeX) / 55f);
				_Content.rectTransform.DOSizeDelta(new Vector2(num, vector.y), duration);
			}
		}

		public void SetSelectedColorToQuestName()
		{
			_QuestName.color = new Color(0.682f, 0.949f, 0.988f);
			_ComplateIcon.color = new Color(0.682f, 0.949f, 0.988f);
			_ProgressIcon.color = new Color(0.682f, 0.949f, 0.988f);
			_ProgressNewIcon.color = new Color(0.682f, 0.949f, 0.988f);
			if (_IsNewQuest)
			{
				_IsNewQuest = false;
				_ProgressNewIcon.gameObject.SetActive(false);
				_Quest.IsNew = false;
				DataLocal.Current.Save(false);
			}
		}

		public void SetNormalColorToQuestName()
		{
			float a = _QuestName.color.a;
			_QuestName.color = new Color(0.482f, 0.616f, 0.678f, a);
			_ComplateIcon.color = new Color(0.482f, 0.616f, 0.678f, a);
			_ProgressIcon.color = ((!_IsNewQuest) ? new Color(0.482f, 0.616f, 0.678f, a) : new Color(0.929f, 0.608f, 0.082f, a));
			_ProgressNewIcon.color = ((!_IsNewQuest) ? new Color(0.482f, 0.616f, 0.678f, a) : new Color(0.929f, 0.608f, 0.082f, a));
		}

		public void SetPlateAlpha(float p_value)
		{
			Color color = _Content.color;
			color.a = p_value;
			_Content.color = color;
		}

		public void SetQuestContentAlpha(float p_value)
		{
			Color color = _QuestName.color;
			color.a = p_value;
			_QuestName.color = color;
			color = _ProgressNewIcon.color;
			color.a = p_value;
			_ProgressNewIcon.color = color;
			_ComplateIcon.color = color;
			_ProgressIcon.color = color;
		}

		public void OnTap()
		{
			if (_OnTap != null)
			{
				_OnTap(this);
			}
		}
	}
}
