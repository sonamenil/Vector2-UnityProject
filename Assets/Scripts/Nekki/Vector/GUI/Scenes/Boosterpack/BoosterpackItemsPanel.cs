using System;
using System.Collections.Generic;
using DG.Tweening;
using Nekki.Vector.Core.GameManagement;
using UnityEngine;

namespace Nekki.Vector.GUI.Scenes.Boosterpack
{
	public class BoosterpackItemsPanel : MonoBehaviour
	{
		[SerializeField]
		private List<BoosterpackItemUI> _Items = new List<BoosterpackItemUI>();

		private BoosterpackPanel _Parent;

		private Sequence _MoveItemsToStartSequence;

		public void Init(BoosterpackPanel p_parent)
		{
			_Parent = p_parent;
			GetComponentsInChildren(_Items);
			foreach (BoosterpackItemUI item in _Items)
			{
				item.Init(_Parent);
			}
		}

		public void GenerateItems()
		{
			Reset();
			BoosterpackItemsManager.CreateBasketItems();
			List<BoosterpackItem> basketItems = BoosterpackItemsManager.BasketItems;
			int i = 0;
			for (int count = _Items.Count; i < count; i++)
			{
				_Items[i].RewardItem = basketItems[i];
			}
		}

		public void RestoreItems()
		{
			List<BoosterpackItem> basketItems = BoosterpackItemsManager.BasketItems;
			int i = 0;
			for (int count = _Items.Count; i < count; i++)
			{
				_Items[i].RewardItem = basketItems[i];
				if (!basketItems[i].IsGiven)
				{
					_Items[i].Prepare();
				}
			}
		}

		public void RestoreItemUI()
		{
			int i = 0;
			for (int count = _Items.Count; i < count; i++)
			{
				_Items[i].RetoreUI();
			}
		}

		public void MoveItemsToStart(Action p_OnEnd)
		{
			if (!_Items[0].IsOpened)
			{
				p_OnEnd();
				return;
			}
			float interval = 0.2f;
			float moveTime = 0.3f;
			_MoveItemsToStartSequence = DOTween.Sequence();
			_MoveItemsToStartSequence.AppendCallback(delegate
			{
				_Items[0].MoveToStartPosition(moveTime);
			});
			_MoveItemsToStartSequence.AppendInterval(interval);
			_MoveItemsToStartSequence.AppendCallback(delegate
			{
				_Items[1].MoveToStartPosition(moveTime);
			});
			_MoveItemsToStartSequence.AppendInterval(interval);
			_MoveItemsToStartSequence.AppendCallback(delegate
			{
				_Items[2].MoveToStartPosition(moveTime);
			});
			_MoveItemsToStartSequence.AppendInterval(interval);
			_MoveItemsToStartSequence.AppendCallback(delegate
			{
				p_OnEnd();
			});
			_MoveItemsToStartSequence.Play();
		}

		public void Reset()
		{
			foreach (BoosterpackItemUI item in _Items)
			{
				item.Reset();
			}
		}

		public void Prepare()
		{
			foreach (BoosterpackItemUI item in _Items)
			{
				item.Prepare();
			}
		}

		public void OpenAll()
		{
			foreach (BoosterpackItemUI item in _Items)
			{
				if (!item.IsOpened)
				{
					item.Open(false);
				}
			}
		}
	}
}
