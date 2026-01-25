using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Nekki.Vector.Core.Game;
using Nekki.Vector.Core.GameManagement;
using Nekki.Vector.Core.User;
using UnityEngine;

namespace Nekki.Vector.GUI.Scenes.Run
{
	public class Gadget : Item
	{
		private const int MaxCharges = 12;

		[SerializeField]
		private ResolutionImage _BackgroundSprite;

		[SerializeField]
		private ResolutionImage _GlowSprite;

		[SerializeField]
		private ResolutionImage _ProgressBarCharges;

		[SerializeField]
		private ResolutionImage _ProgressBarRed;

		[SerializeField]
		private RectTransform _ContentWidget;

		private Tweener _RedTweener;

		private int _CurrentCharges = -1;

		private bool _isDark;

		private bool animationPlayed;

		private void Update()
		{
			if (!animationPlayed)
			{
				UpdateCounter(false);
			}
		}

		public override void Init(UserItem item, float p_scale)
		{
			base.Init(item, p_scale);
			ItemGroupAttributes attributeByGroupName = item.GetAttributeByGroupName("ST_MyGadgets");
			string p_value = string.Empty;
			if (attributeByGroupName.TryGetStrValue("ST_Slot", ref p_value))
			{
				List<string> list = new List<string>(p_value.Split('|'));
				p_value = ((list.Count <= 0) ? "Head" : list[0]);
				foreach (KeyValuePair<string, Settings.GadgetSlot> gadgetSlot in Settings.GadgetSlots)
				{
					if (gadgetSlot.Value.Name == p_value)
					{
						base.name = string.Format("{0:D2}{1}Gadget", gadgetSlot.Value.LevelGUIOrder, gadgetSlot.Value.Name);
						break;
					}
				}
			}
			_ProgressBarRed.fillAmount = 1f / 12f;
			_ProgressBarRed.gameObject.SetActive(false);
			UpdateCounter(true);
		}

		protected override void InitSizes(float p_scale)
		{
			_ContentWidget.localPosition += new Vector3(0f, (0f - _BackgroundSprite.rectTransform.rect.height) / 2f);
		}

		private void UpdateCounter(bool onInit = false)
		{
			if (base.CurrentItem == null || !base.CurrentItem.ContainsGroup("ST_GadgetBase"))
			{
				_ProgressBarCharges.fillAmount = 1f;
				return;
			}
			int num = 12;
			int num2 = base.CurrentItem.GetIntValueAttribute("ST_ChargesCurrent", "ST_GadgetBase") + base.CurrentItem.GetIntValueAttribute("ST_ChargesBonus", "ST_GadgetBase", 0);
			if (num2 != num && num2 != _CurrentCharges)
			{
				if (_CurrentCharges != -1 && !onInit)
				{
					HideCharge();
				}
				StartCoroutine(UpdateCharges(num2, num, !onInit));
			}
		}

		private IEnumerator UpdateCharges(int chargesCurrent, int chargesTotal, bool recharged)
		{
			if (recharged)
			{
				animationPlayed = true;
				yield return new WaitForSeconds(0.5f);
			}
			_ProgressBarCharges.fillAmount = (float)chargesCurrent * 1f / (float)chargesTotal;
			int stepsTotal = 12;
			int stepsPassed = stepsTotal - Mathf.RoundToInt(_ProgressBarCharges.fillAmount * (float)stepsTotal);
			if (stepsPassed > 2)
			{
				_ProgressBarCharges.rectTransform.localRotation = Quaternion.Euler(0f, 0f, -360f / (float)stepsTotal * (float)Mathf.FloorToInt((float)(stepsPassed - 1) / 2f));
			}
			_CurrentCharges = chargesCurrent;
			_ProgressBarRed.gameObject.SetActive(false);
			if (_RedTweener != null)
			{
				_RedTweener.Complete();
				_RedTweener = null;
			}
			UpdateDarkness();
			animationPlayed = false;
		}

		private void HideCharge()
		{
			int num = 12;
			int num2 = num - Mathf.RoundToInt(_ProgressBarCharges.fillAmount * (float)num);
			float num3 = 360f / (float)num * (float)Mathf.FloorToInt((float)(num2 - 1) / 2f);
			if (num2 > 2)
			{
				_ProgressBarRed.rectTransform.localRotation = Quaternion.Euler(0f, 0f, (_CurrentCharges % 2 != 0) ? (0f - num3 - (float)(_CurrentCharges - 1) * 30f) : (0f - num3));
			}
			_ProgressBarRed.gameObject.SetActive(true);
			if (_RedTweener != null)
			{
				_RedTweener.Complete();
			}
			_RedTweener = _ProgressBarRed.DOFade(0f, 1f / 9f);
			_RedTweener.SetLoops(6, LoopType.Yoyo);
			_RedTweener.Play();
		}

		private void UpdateDarkness()
		{
			if (_ProgressBarCharges.fillAmount == 0f)
			{
				if (!_isDark)
				{
					_isDark = true;
					EnableDarkness();
				}
			}
			else if (_isDark)
			{
				_isDark = false;
				DisableDarkness();
			}
		}

		public void EnableDarkness()
		{
			_Image.Alpha = 0.3f;
			_GlowSprite.Alpha = 0f;
			_BackgroundSprite.Alpha = 0f;
		}

		public void DisableDarkness()
		{
			if (!_isDark)
			{
				_Image.Alpha = 1f;
				_GlowSprite.Alpha = 1f;
				_BackgroundSprite.Alpha = 1f;
			}
		}
	}
}
