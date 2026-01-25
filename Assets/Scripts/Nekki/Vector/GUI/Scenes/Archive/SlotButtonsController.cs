using System.Collections.Generic;
using Nekki.Vector.Core.GameManagement;
using Nekki.Vector.GUI.MainScene;
using Nekki.Vector.GUI.ShopScene;
using UnityEngine;

namespace Nekki.Vector.GUI.Scenes.Archive
{
	public class SlotButtonsController : MonoBehaviour
	{
		private const string _NextPageId = "_NextPage_";

		private const string _PrevPageId = "_PrevPage_";

		[SerializeField]
		private GameObject _SlotPrefab;

		[SerializeField]
		private List<SlotButton> _Slots = new List<SlotButton>();

		private List<List<string>> _Pages = new List<List<string>>();

		private int _CurrentPage;

		private SlotButton _SelectedSlot;

		private ArchivePanel _Parent;

		private bool UseSupportedAspect
		{
			get
			{
				return (double)((float)Screen.width / (float)Screen.height) >= 1.59999999;
			}
		}

		public void Init(ArchivePanel p_parent)
		{
			if (!UseSupportedAspect)
			{
				base.gameObject.SetActive(false);
				return;
			}
			_Parent = p_parent;
			_Pages.Clear();
			List<string> list = new List<string>();
			list.Add(SlotItem.Slot.Head.GetName());
			list.Add(SlotItem.Slot.Torso.GetName());
			list.Add(SlotItem.Slot.Hands.GetName());
			list.Add(SlotItem.Slot.Legs.GetName());
			list.Add(SlotItem.Slot.Belt.GetName());
			list.Add("_NextPage_");
			_Pages.Add(list);
			list = new List<string>();
			list.Add("_PrevPage_");
			list.Add(SlotItem.Slot.Stunts.GetName());
			list.Add(SlotItem.Slot.Notes.GetName());
			list.Add(SlotItem.Slot.StoryItems.GetName());
			_Pages.Add(list);
			Refresh();
		}

		public void Refresh()
		{
			_CurrentPage = GetSlotTabContainingLastPageIndex(_Parent.CurrentSlot);
			UpdateSlots();
		}

		private void UpdateSlots()
		{
			RemoveSlots();
			bool flag = false;
			List<string> list = _Pages[_CurrentPage];
			foreach (string item in list)
			{
				CreateSlot(item);
				if (_Parent.CurrentSlot.GetName() == item)
				{
					flag = true;
				}
			}
			if (_Parent.CurrentSlot == SlotItem.Slot.NotSlot || !flag)
			{
				OnSlotButtonTap(GetFirstSlot());
			}
			else
			{
				ChangeSlot(_Parent.CurrentSlot);
			}
		}

		private void RemoveSlots()
		{
			foreach (SlotButton slot in _Slots)
			{
				Object.DestroyImmediate(slot.gameObject);
			}
			_Slots.Clear();
		}

		public void SelectNextPage()
		{
			_CurrentPage++;
			if (_CurrentPage >= _Pages.Count)
			{
				_CurrentPage = 0;
			}
			if (Manager.IsEquip)
			{
				Scene<Nekki.Vector.GUI.MainScene.MainScene>.Current.SwitchPage(UpdateSlots);
			}
			else
			{
				Scene<Nekki.Vector.GUI.ShopScene.ShopScene>.Current.SwitchPage(UpdateSlots);
			}
		}

		public void SelectPrevPage()
		{
			_CurrentPage--;
			if (_CurrentPage < 0)
			{
				_CurrentPage = _Pages.Count - 1;
			}
			if (Manager.IsEquip)
			{
				Scene<Nekki.Vector.GUI.MainScene.MainScene>.Current.SwitchPage(UpdateSlots);
			}
			else
			{
				Scene<Nekki.Vector.GUI.ShopScene.ShopScene>.Current.SwitchPage(UpdateSlots);
			}
		}

		private void CreateSlot(string p_slotName)
		{
			GameObject gameObject = Object.Instantiate(_SlotPrefab);
			gameObject.transform.SetParent(base.transform, false);
			gameObject.transform.SetAsLastSibling();
			gameObject.name = p_slotName;
			SlotButton component = gameObject.GetComponent<SlotButton>();
			if (p_slotName == "_NextPage_")
			{
				component.InitNext(OnSlotButtonTap);
			}
			else if (p_slotName == "_PrevPage_")
			{
				component.InitPrev(OnSlotButtonTap);
			}
			else
			{
				component.Init(p_slotName.GetSlotByName(), OnSlotButtonTap);
			}
			_Slots.Add(component);
		}

		private void OnSlotButtonTap(SlotButton p_tab)
		{
			if (!(_SelectedSlot == p_tab))
			{
				if (p_tab.SlotButtonMode == SlotButton.ButtonMode.NextPage)
				{
					SelectNextPage();
					return;
				}
				if (p_tab.SlotButtonMode == SlotButton.ButtonMode.PrevPage)
				{
					SelectPrevPage();
					return;
				}
				_Parent.SetCardsIsNew(false);
				RefreshCurrentSlot();
				ChangeSlot(p_tab);
				_Parent.CurrentSlot = p_tab.SlotType;
			}
		}

		public void ChangeSlot(SlotItem.Slot p_slot)
		{
			if (UseSupportedAspect)
			{
				if (_CurrentPage != GetSlotTabContainingLastPageIndex(p_slot))
				{
					Refresh();
				}
				else
				{
					ChangeSlot(GetSlotTabByType(p_slot));
				}
			}
		}

		private void ChangeSlot(SlotButton p_slotTap)
		{
			if (_SelectedSlot != null)
			{
				_SelectedSlot.MoveRight(0.2f);
			}
			_SelectedSlot = p_slotTap;
			_SelectedSlot.MoveLeft(0.2f);
		}

		private SlotButton GetSlotTabByType(SlotItem.Slot p_slotType)
		{
			foreach (SlotButton slot in _Slots)
			{
				if (slot.SlotType == p_slotType)
				{
					return slot;
				}
			}
			return null;
		}

		private int GetSlotTabContainingLastPageIndex(SlotItem.Slot p_selectedSlot)
		{
			if (p_selectedSlot == SlotItem.Slot.NotSlot)
			{
				return 0;
			}
			string item = p_selectedSlot.GetName();
			for (int num = _Pages.Count - 1; num >= 0; num--)
			{
				if (_Pages[num].Contains(item))
				{
					return num;
				}
			}
			return 0;
		}

		private SlotButton GetFirstSlot()
		{
			int i = 0;
			for (int count = _Slots.Count; i < count; i++)
			{
				if (_Slots[i].SlotButtonMode == SlotButton.ButtonMode.Slot)
				{
					return _Slots[i];
				}
			}
			return null;
		}

		public void RefreshSlots()
		{
			foreach (SlotButton slot in _Slots)
			{
				slot.Refresh();
			}
		}

		public void RefreshCurrentSlot()
		{
			if (_SelectedSlot != null)
			{
				_SelectedSlot.Refresh();
			}
		}
	}
}
