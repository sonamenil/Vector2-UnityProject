using Nekki.Vector.Core.User;

namespace Nekki.Vector.Core.GameManagement
{
	public static class BoosterpacksManager
	{
		private const string _BoosterPacksItemName = "Boosterpacks";

		private static UserItem _Item;

		public static int BoosterpackQuantity
		{
			get
			{
				if (_Item.Groups.Count == 0)
				{
					return 0;
				}
				ItemGroupAttributes itemGroupAttributes = _Item.Groups[0];
				int p_value = 0;
				itemGroupAttributes.TryGetIntValue("Quantity", ref p_value);
				return p_value;
			}
			set
			{
				if (_Item.Groups.Count != 0)
				{
					ItemGroupAttributes itemGroupAttributes = _Item.Groups[0];
					if (value == 0)
					{
						_Item.RemoveGroupAttributes(itemGroupAttributes.GroupName);
					}
					else
					{
						itemGroupAttributes.TrySetValue("Quantity", value);
					}
				}
			}
		}

		public static void Init()
		{
			_Item = DataLocal.Current.GetItemByName("Boosterpacks");
			if (_Item == null)
			{
				_Item = PresetsManager.GetPresetByName("Boosterpacks").RunPreset().Item;
				DataLocal.Current.Save(true);
			}
		}
	}
}
