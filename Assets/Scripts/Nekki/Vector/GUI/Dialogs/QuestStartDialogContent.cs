using System;
using System.Collections.Generic;
using Nekki.Vector.Core.GameManagement;
using Nekki.Vector.Core.Localization;
using Nekki.Vector.Core.Quest;
using Nekki.Vector.GUI.Common;
using UnityEngine;

namespace Nekki.Vector.GUI.Dialogs
{
	public class QuestStartDialogContent : DialogContent
	{
		[SerializeField]
		private LabelAlias _Title;

		[SerializeField]
		private LabelAlias _ObjectiveLabel;

		[SerializeField]
		private LabelAlias _RewardLabel;

		[SerializeField]
		private ResolutionImage _Image;

		[SerializeField]
		private RectTransform _CardAnchor;

		[SerializeField]
		private BaseCardUI _BaseCardPrefab;

		private Action _OnClose;

		private string _QuestName;

		public void Init(string p_title, string p_objectiveText, Quest p_quest, Action p_onClose, bool p_noDetails)
		{
			_OnClose = p_onClose;
			_QuestName = p_quest.Name;
			List<DialogButtonData> list = new List<DialogButtonData>();
			if (p_noDetails)
			{
				list.Add(new DialogButtonData(OnCloseTap, "^GUI.Buttons.Ok^", ButtonUI.Type.Green));
			}
			else
			{
				list.Add(new DialogButtonData(OnDetailsTap, "^GUI.Buttons.Details^", ButtonUI.Type.Blue));
				list.Add(new DialogButtonData(OnCloseTap, "^GUI.Buttons.Close^", ButtonUI.Type.Red));
			}
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
			_ObjectiveLabel.SetAlias(p_objectiveText);
			_RewardLabel.SetAlias(p_quest.RewardVisualName);
		}

		private void OnCloseTap(BaseDialog p_dialog)
		{
			if (_OnClose != null)
			{
				_OnClose();
			}
			base.Parent.Dismiss();
		}

		private void OnDetailsTap(BaseDialog p_dialog)
		{
			if (_OnClose != null)
			{
				_OnClose();
			}
			base.Parent.Dismiss();
			Manager.OpenQuestLog(_QuestName);
		}
	}
}
