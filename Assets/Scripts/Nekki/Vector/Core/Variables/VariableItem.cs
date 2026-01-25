using Nekki.Vector.Core.User;

namespace Nekki.Vector.Core.Variables
{
	public class VariableItem : Variable
	{
		private UserItem _Item;

		public UserItem Item
		{
			get
			{
				return _Item;
			}
			set
			{
				_Item = value;
			}
		}

		public override string ValueString
		{
			get
			{
				if (Item != null)
				{
					return Item.Name;
				}
				return "NULL";
			}
		}

		public override string DebugStringValue
		{
			get
			{
				return (Item == null) ? "NULL" : Item.Name;
			}
		}

		protected internal VariableItem(string p_name, IVariableParent p_parent)
			: base(p_name, p_parent)
		{
			_Type = VariableType.Item;
		}
	}
}
