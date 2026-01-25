using Nekki.Vector.Core.User;

namespace Nekki.Vector.Core.GameManagement
{
	public class PresetResult
	{
		public enum ItemType
		{
			NullItem = 0,
			OldItem = 1,
			NewItem = 2,
			AddToStashItem = 3,
			AddToEquipItem = 4,
			AddToShopBascketItem = 5
		}

		public bool Result;

		public ItemType TypeItem;

		public UserItem Item;

		public PresetDialogData DialogData;

		public bool RunPreset;
	}
}
