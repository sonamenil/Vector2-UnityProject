using Nekki.Vector.Core.GameManagement;
using Nekki.Vector.Core.Localization;
using Nekki.Vector.Core.Quest;
using Nekki.Vector.GUI.Common;
using UnityEngine;

namespace Nekki.Vector.GUI.Journal
{
	public class QuestInfo : MonoBehaviour
	{
		[SerializeField]
		private LabelAlias _QuestName;

		[SerializeField]
		private LabelAlias _QuestText;

		[SerializeField]
		private LabelAlias _QuestProgress;

		[SerializeField]
		private LabelAlias _RewardName;

		[SerializeField]
		private ResolutionImage _RewardPic;

		[SerializeField]
		private BaseCardUISettings _CardUISettings = new BaseCardUISettings();

		[SerializeField]
		private GameObject _BaseCardUIPrefab;

		[SerializeField]
		private GameObject _CardHolder;

		private BaseCardUI _Card;

		public void Init(Quest p_quest)
		{
			if (_Card == null)
			{
				CreateCardUI();
			}
			_QuestName.SetAlias(p_quest.VisualName);
			_QuestText.SetAlias(p_quest.Description);
			_QuestProgress.SetAlias(p_quest.Progress);
			if (p_quest.RewardType == "Card")
			{
				_RewardPic.gameObject.SetActive(false);
				CardsGroupAttribute card = CardsGroupAttribute.Create(p_quest.RewardItemName.ValueString);
				_Card.Card = card;
				_Card.gameObject.SetActive(true);
			}
			else
			{
				_Card.gameObject.SetActive(false);
				ImageResourceFinder.SetImage(_RewardPic, p_quest.RewardImageName, true);
				_RewardPic.gameObject.SetActive(true);
			}
			_RewardName.SetAlias(p_quest.RewardVisualName);
		}

		private void CreateCardUI()
		{
			GameObject gameObject = Object.Instantiate(_BaseCardUIPrefab);
			gameObject.transform.SetParent(_CardHolder.transform, false);
			_Card = gameObject.GetComponent<BaseCardUI>();
			_Card.Init(_CardUISettings);
		}
	}
}
