namespace Nekki.Vector.Core.GameManagement
{
	public static class SlotExtension
	{
		public static string GetName(this SlotItem.Slot slot)
		{
			switch (slot)
			{
			case SlotItem.Slot.Stunts:
				return "Stunts";
			case SlotItem.Slot.Notes:
				return "Notes";
			case SlotItem.Slot.StoryItems:
				return "StoryItems";
			case SlotItem.Slot.Legs:
				return "Legs";
			case SlotItem.Slot.Torso:
				return "Torso";
			case SlotItem.Slot.Head:
				return "Head";
			case SlotItem.Slot.Hands:
				return "Hands";
			case SlotItem.Slot.Belt:
				return "Belt";
			default:
				return "NotSlot";
			}
		}

		public static SlotItem.Slot GetSlotByName(this string slot)
		{
			switch (slot)
			{
			case "Stunts":
				return SlotItem.Slot.Stunts;
			case "Notes":
				return SlotItem.Slot.Notes;
			case "StoryItems":
				return SlotItem.Slot.StoryItems;
			case "Legs":
				return SlotItem.Slot.Legs;
			case "Torso":
				return SlotItem.Slot.Torso;
			case "Head":
				return SlotItem.Slot.Head;
			case "Hands":
				return SlotItem.Slot.Hands;
			case "Belt":
				return SlotItem.Slot.Belt;
			default:
				return SlotItem.Slot.NotSlot;
			}
		}

		public static string GetImage(this SlotItem.Slot p_slot)
		{
			return BalanceManager.Current.GetBalance("Slots", p_slot.GetName(), "Image");
		}

		public static string GetIcon(this SlotItem.Slot p_slot)
		{
			return BalanceManager.Current.GetBalance("Slots", p_slot.GetName(), "IconImage");
		}
	}
}
