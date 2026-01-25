using System;
using System.Collections.Generic;
using Nekki.Vector.Core.GameManagement;
using Nekki.Vector.Core.User;
using UnityEngine;

namespace Nekki.Vector.GUI.Common
{
	public class GadgetUIPanel : MonoBehaviour
	{
		[SerializeField]
		private GameObject _GadgetUIPrefab;

		[SerializeField]
		private float _GadgetSize;

		[SerializeField]
		private bool _CreateEmpty;

		[SerializeField]
		private float _MoveDistance;

		[SerializeField]
		private bool _AllowUnselect;

		private GadgetUI _HeadSlot;

		private GadgetUI _TorsoSlot;

		private GadgetUI _HandsSlot;

		private GadgetUI _BeltSlot;

		private GadgetUI _LegsSlot;

		private List<GadgetUI> _AllSlots = new List<GadgetUI>();

		private Action<GadgetUI, bool> _OnGadgetTap;

		private GadgetUI _SelectedGadget;

		public void Init(List<GadgetItem> p_items, Action<GadgetUI, bool> p_onGadgetTap, bool p_selectFirst = true)
		{
			p_items.Sort((GadgetItem p_g1, GadgetItem p_g2) => (p_g1.SlotPriority <= p_g2.SlotPriority) ? 1 : (-1));
			_OnGadgetTap = p_onGadgetTap;
			ClearAll();
			if (_CreateEmpty)
			{
				InitCreateAll(p_items);
			}
			else
			{
				InitCreateExist(p_items);
			}
			if (p_selectFirst)
			{
				SelectedFirst();
			}
		}

		private void InitCreateExist(List<GadgetItem> p_items)
		{
			for (int i = 0; i < p_items.Count; i++)
			{
				GadgetUI gadgetUI = CreateSlotByName(p_items[i].SlotName);
				gadgetUI.Init(p_items[i], _GadgetSize, _GadgetSize, OnGadgetTap);
				if (Manager.IsRun)
				{
					gadgetUI.GadgetFrameColor = new Color(0.216f, 0.38f, 0.459f, 1f);
					gadgetUI.SetTapEnable(false);
				}
			}
		}

		private void InitCreateAll(List<GadgetItem> p_items)
		{
			CreateAllSlots();
			for (int i = 0; i < p_items.Count; i++)
			{
				GadgetUI gadgetUI = GetGadgetUI(p_items[i].SlotName);
				gadgetUI.Init(p_items[i], _GadgetSize, _GadgetSize, OnGadgetTap);
				if (Manager.IsRun)
				{
					gadgetUI.GadgetFrameColor = new Color(0.216f, 0.38f, 0.459f, 1f);
					gadgetUI.SetTapEnable(false);
				}
			}
			if (_HeadSlot.Gadget == null)
			{
				_HeadSlot.Init(null, _GadgetSize, _GadgetSize, null, "shop.head_empty");
			}
			if (_TorsoSlot.Gadget == null)
			{
				_TorsoSlot.Init(null, _GadgetSize, _GadgetSize, null, "shop.torso_empty");
			}
			if (_HandsSlot.Gadget == null)
			{
				_HandsSlot.Init(null, _GadgetSize, _GadgetSize, null, "shop.hands_empty");
			}
			if (_BeltSlot.Gadget == null)
			{
				_BeltSlot.Init(null, _GadgetSize, _GadgetSize, null, "shop.belt_empty");
			}
			if (_LegsSlot.Gadget == null)
			{
				_LegsSlot.Init(null, _GadgetSize, _GadgetSize, null, "shop.legs_empty");
			}
		}

		private GadgetUI CreateSlotByName(string p_slotName)
		{
			GadgetUI component = UnityEngine.Object.Instantiate(_GadgetUIPrefab).GetComponent<GadgetUI>();
			component.transform.SetParent(base.transform, false);
			switch (p_slotName)
			{
			case "Head":
				_HeadSlot = component;
				break;
			case "Torso":
				_TorsoSlot = component;
				break;
			case "Hands":
				_HandsSlot = component;
				break;
			case "Belt":
				_BeltSlot = component;
				break;
			case "Legs":
				_LegsSlot = component;
				break;
			}
			_AllSlots.Add(component);
			return component;
		}

		private void CreateAllSlots()
		{
			_HeadSlot = CreateSlotByName("Head");
			_TorsoSlot = CreateSlotByName("Torso");
			_HandsSlot = CreateSlotByName("Hands");
			_BeltSlot = CreateSlotByName("Belt");
			_LegsSlot = CreateSlotByName("Legs");
		}

		public GadgetUI GetGadgetUI(string p_slotName)
		{
			switch (p_slotName)
			{
			case "Head":
				return _HeadSlot;
			case "Torso":
				return _TorsoSlot;
			case "Hands":
				return _HandsSlot;
			case "Belt":
				return _BeltSlot;
			case "Legs":
				return _LegsSlot;
			default:
				return null;
			}
		}

		public string GetSelectedSlotName()
		{
			return _SelectedGadget.Gadget.SlotName;
		}

		private void ClearAll()
		{
			int i = 0;
			for (int count = _AllSlots.Count; i < count; i++)
			{
				UnityEngine.Object.Destroy(_AllSlots[i].gameObject);
			}
			_AllSlots.Clear();
			_HeadSlot = (_HandsSlot = (_TorsoSlot = (_BeltSlot = (_LegsSlot = null))));
		}

		public void SelectedFirst()
		{
			int i = 0;
			for (int count = _AllSlots.Count; i < count; i++)
			{
				if (_AllSlots[i].Gadget != null)
				{
					OnGadgetTap(_AllSlots[i], true);
					break;
				}
			}
		}

		public void SelectBySlotName(string p_name)
		{
			int i = 0;
			for (int count = _AllSlots.Count; i < count; i++)
			{
				if (_AllSlots[i].Gadget != null && _AllSlots[i].Gadget.SlotName == p_name)
				{
					OnGadgetTap(_AllSlots[i], true);
					break;
				}
			}
		}

		public void SetTapEnable(bool p_value)
		{
			int i = 0;
			for (int count = _AllSlots.Count; i < count; i++)
			{
				_AllSlots[i].SetTapEnable(p_value);
			}
		}

		public void RefreshCharges()
		{
			int i = 0;
			for (int count = _AllSlots.Count; i < count; i++)
			{
				_AllSlots[i].RefreshCharges();
			}
		}

		public Transform GetTransformGadget(UserItem p_item)
		{
			int i = 0;
			for (int count = _AllSlots.Count; i < count; i++)
			{
				if (_AllSlots[i].Gadget.CurrItem == p_item)
				{
					return _AllSlots[i].transform;
				}
			}
			return null;
		}

		public void OnGadgetTap(GadgetUI p_gadget)
		{
			OnGadgetTap(p_gadget, false);
		}

		public void OnGadgetTap(GadgetUI p_gadget, bool p_instant)
		{
			if (p_gadget == _SelectedGadget)
			{
				if (_AllowUnselect)
				{
					if (_SelectedGadget != null)
					{
						_SelectedGadget.MoveContentTo(0f);
						_SelectedGadget.OnDeselect();
						_SelectedGadget = null;
					}
					if (_OnGadgetTap != null)
					{
						_OnGadgetTap(null, p_instant);
					}
				}
			}
			else
			{
				if (_SelectedGadget != null)
				{
					_SelectedGadget.MoveContentTo(0f);
					_SelectedGadget.OnDeselect();
				}
				_SelectedGadget = p_gadget;
				_SelectedGadget.MoveContentTo(_MoveDistance);
				_SelectedGadget.OnSelect();
				if (_OnGadgetTap != null)
				{
					_OnGadgetTap(p_gadget, p_instant);
				}
			}
		}

		public void AddSegmentsToGadget(string p_slotName, int p_segments)
		{
			GadgetUI gadgetUI = GetGadgetUI(p_slotName);
			gadgetUI.AddSegments(p_segments);
			gadgetUI.BlinkGadgetFrame(1, 0.2f);
		}

		public void ClearSelection()
		{
			if (_SelectedGadget != null)
			{
				_SelectedGadget.MoveContentTo(0f);
				_SelectedGadget.OnDeselect();
				_SelectedGadget = null;
			}
		}
	}
}
