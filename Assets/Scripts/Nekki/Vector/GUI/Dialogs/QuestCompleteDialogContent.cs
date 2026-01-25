using System;
using System.Collections.Generic;
using Nekki.Vector.Core.GameManagement;
using Nekki.Vector.Core.Localization;
using Nekki.Vector.Core.Quest;
using Nekki.Vector.GUI.Common;
using UnityEngine;

namespace Nekki.Vector.GUI.Dialogs
{
	public class QuestCompleteDialogContent : DialogContent
	{
		[SerializeField]
		private LabelAlias _Title;

		[SerializeField]
		private LabelAlias _RewardLabel;

		[SerializeField]
		private ResolutionImage _Image;

		[SerializeField]
		private RectTransform _CardAnchor;

		[SerializeField]
		private BaseCardUI _BaseCardPrefab;

		private Action _OnClose;

		public void Init(string p_title, Quest p_quest, Action p_onClose)
		{
			_OnClose = p_onClose;
			List<DialogButtonData> list = new List<DialogButtonData>();
			list.Add(new DialogButtonData(OnCollectTap, "^GUI.Buttons.Collect^", ButtonUI.Type.Green));
			list[0]._SoundAlias = "quest_complete_button";
			Init(list);
			bool flag = p_quest.RewardType == "Card";
			_Image.enabled = !flag;
			_CardAnchor.gameObject.SetActive(flag);
			if (flag)
			{
				CardsGroupAttribute card = CardsGroupAttribute.Create(p_quest.RewardItemName.ValueString);
				BaseCardUI baseCardUI = UnityEngine.Object.Instantiate(_BaseCardPrefab);
				baseCardUI.CardSize = 200;
				baseCardUI.Card = card;
				baseCardUI.transform.SetParent(_CardAnchor, false);
			}
			else if (!string.IsNullOrEmpty(p_quest.RewardImageName))
			{
				ImageResourceFinder.SetImage(_Image, p_quest.RewardImageName, true);
			}
			_Title.SetAlias(p_title);
			_RewardLabel.SetAlias(p_quest.RewardVisualName);
		}

		private void OnCollectTap(BaseDialog p_dialog)
		{
			if (_OnClose != null)
			{
				_OnClose();
			}
			base.Parent.Dismiss();
		}
	}
}
