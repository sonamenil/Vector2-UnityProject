using System;
using DG.Tweening;
using Nekki.Vector.Core.GameManagement;
using Nekki.Vector.Core.Localization;
using Nekki.Vector.Core.Utilites;
using Nekki.Vector.GUI.Common;
using UnityEngine;

namespace Nekki.Vector.GUI.Scenes.Terminal
{
	public class TerminalItemTab : MonoBehaviour
	{
		private const string _CountTextFormat = "+{0}";

		[SerializeField]
		private BaseCardUISettings _CardUISettings = new BaseCardUISettings();

		[SerializeField]
		private int _ItemRewardUISize = 210;

		[SerializeField]
		private float _MoveShift = 155f;

		[SerializeField]
		private float _OnDisappearShift = 300f;

		[SerializeField]
		private float _OnDisappearForwardShift = 50f;

		[SerializeField]
		private RectTransform _Content;

		[SerializeField]
		private GameObject _BaseCardUIPrefab;

		[SerializeField]
		private GameObject _ItemRewardUIPrefab;

		[SerializeField]
		private GameObject _EffectsHolder;

		[SerializeField]
		private LabelAlias _RequirementText;

		[SerializeField]
		private GameObject _RequirementCompleted;

		[SerializeField]
		private GameObject _RequirementIncompleted;

		[SerializeField]
		private LabelAlias _CountText;

		[SerializeField]
		private GameObject _CountPlate;

		private static Color _RequirementCompleteColor = ColorUtils.FromHex("abd8eb");

		private static Color _RequirementIncompleteColor = ColorUtils.FromHex("4e6a7e");

		private TerminalItemGroupAttribute _Item;

		private Action<TerminalItemTab> _OnTapAction;

		private BaseCardUI _CardRewardUI;

		private ItemRewardUI _ItemRewardUI;

		public float OnDisappearShift
		{
			get
			{
				return _OnDisappearShift;
			}
		}

		public float OnDisappearForwardShift
		{
			get
			{
				return _OnDisappearForwardShift;
			}
		}

		public RectTransform Content
		{
			get
			{
				return _Content;
			}
		}

		public TerminalItemGroupAttribute Item
		{
			get
			{
				return _Item;
			}
		}

		public void Init(TerminalItemGroupAttribute p_item, Action<TerminalItemTab> p_onTapAction)
		{
			_Item = p_item;
			_OnTapAction = p_onTapAction;
			_CardRewardUI = CreateRewardUI<BaseCardUI>(_BaseCardUIPrefab);
			_CardRewardUI.Init(_CardUISettings, OnCardTap);
			_ItemRewardUI = CreateRewardUI<ItemRewardUI>(_ItemRewardUIPrefab);
			_ItemRewardUI.Size = _ItemRewardUISize;
			RefreshReward();
			RefreshRequirement();
			if (_Item.IsExpired)
			{
				SetOffState();
			}
		}

		private T CreateRewardUI<T>(GameObject p_prefab) where T : MonoBehaviour
		{
			GameObject gameObject = UnityEngine.Object.Instantiate(p_prefab);
			gameObject.transform.SetParent(_EffectsHolder.transform, false);
			gameObject.transform.SetAsLastSibling();
			return gameObject.GetComponent<T>();
		}

		public void Reset()
		{
			if (_Item.IsExpired)
			{
				_Content.localPosition = new Vector3(Content.localPosition.x + OnDisappearShift, Content.localPosition.y, Content.localPosition.z);
				_CountPlate.SetActive(true);
				_RequirementText.gameObject.SetActive(true);
				RefreshRequirement();
				RefreshReward();
			}
		}

		public void RefreshReward()
		{
			_CardRewardUI.gameObject.SetActive(false);
			_ItemRewardUI.gameObject.SetActive(false);
			if (_Item.IsCardReward)
			{
				_CardRewardUI.Card = _Item.Card;
				_CardRewardUI.NeedShowProgressAnimation = true;
				_CardRewardUI.AnimationCardCount = _Item.Count;
				_CardRewardUI.gameObject.SetActive(true);
				if (_Item.Card.UserCardTotalLevel == 0 && _Item.Card.UserCardsSinceLastLevel == 0)
				{
					_CountPlate.SetActive(false);
				}
			}
			else
			{
				_ItemRewardUI.Init(_Item.ItemImage);
				_ItemRewardUI.gameObject.SetActive(true);
			}
			_CountText.SetAlias(string.Format("+{0}", _Item.Count));
		}

		public void GetBuyTween(Sequence target)
		{
			target.Append(Content.DOLocalMoveX(Content.localPosition.x + OnDisappearForwardShift, 0.4f));
			target.Append(Content.DOLocalMoveX(Content.localPosition.x - OnDisappearShift, 0.4f));
			target.AppendCallback(RequirementsOff);
			target.AppendCallback(delegate
			{
				_CountPlate.SetActive(false);
			});
		}

		private void SetOffState()
		{
			_Content.localPosition = new Vector3(Content.localPosition.x - OnDisappearShift, Content.localPosition.y, Content.localPosition.z);
			_CountPlate.SetActive(false);
			RequirementsOff();
		}

		private void RequirementsOff()
		{
			_RequirementCompleted.gameObject.SetActive(false);
			_RequirementIncompleted.gameObject.SetActive(false);
			_RequirementText.gameObject.SetActive(false);
		}

		public void TwinkleOff()
		{
			if (_Item.IsCardReward)
			{
				_CardRewardUI.NeedShowProgressAnimation = false;
				_CardRewardUI.NeedShowProgressNumbers = false;
				_CardRewardUI.Refresh();
			}
		}

		private void RefreshRequirement()
		{
			bool requirementIsCompleted = _Item.RequirementIsCompleted;
			if (Item.NoRequirements)
			{
				_RequirementCompleted.SetActive(false);
				_RequirementIncompleted.SetActive(false);
			}
			else
			{
				_RequirementCompleted.SetActive(requirementIsCompleted);
				_RequirementIncompleted.SetActive(!requirementIsCompleted);
			}
			_RequirementText.SetAlias(_Item.RequirementText);
			_RequirementText.color = ((!requirementIsCompleted) ? _RequirementIncompleteColor : _RequirementCompleteColor);
		}

		public void MoveLeft(float p_duration)
		{
			_Content.DOLocalMoveX(_Content.localPosition.x - _MoveShift, p_duration);
		}

		public void MoveRight(float p_duration)
		{
			_Content.DOLocalMoveX(_Content.localPosition.x + _MoveShift, p_duration);
		}

		private void OnCardTap(BaseCardUI p_card)
		{
			OnTap();
		}

		public void OnTap()
		{
			if (_OnTapAction != null)
			{
				_OnTapAction(this);
			}
		}
	}
}
