using System.Collections.Generic;
using Nekki.Vector.Core.User;

namespace Nekki.Vector.Core.Variables.Functions
{
	public class VF_SelectItemsByGroupName : VariableFunction
	{
		private static List<UserItem> _SelectedItems = new List<UserItem>();

		private Variable _GroupName;

		private Variable _AttributeValue;

		private Variable _Param;

		public override int ValueInt
		{
			get
			{
				SelectItems();
				if (_Param.ValueString == "count")
				{
					return _SelectedItems.Count;
				}
				int num = 0;
				for (int i = 0; i < _SelectedItems.Count; i++)
				{
					num += _SelectedItems[i].GetIntValueAttribute(_Param.ValueString);
				}
				return num;
			}
		}

		public override float ValueFloat
		{
			get
			{
				return ValueInt;
			}
		}

		public override string ValueString
		{
			get
			{
				return ValueInt.ToString();
			}
		}

		protected internal VF_SelectItemsByGroupName(string p_value, string p_name, IVariableParent p_parent)
			: base(p_value, p_name, p_parent)
		{
			p_value = VariableFunction.TrimSpaces(p_value);
			string p_name2 = p_value.Split('.')[1];
			int num = p_value.IndexOf("[") + 1;
			int num2 = p_value.LastIndexOf("]");
			string p_name3 = p_value.Substring(num, num2 - num);
			VariableFunction.InitFuncVar(p_parent, ref _GroupName, p_name3);
			VariableFunction.InitFuncVar(p_parent, ref _Param, p_name2);
		}

		private void SelectItems()
		{
			_SelectedItems.Clear();
			foreach (UserItem item in DataLocal.Current.Equipped)
			{
				if (item.ContainsGroup(_GroupName.ValueString))
				{
					_SelectedItems.Add(item);
				}
			}
		}

		public override void SimplifyArguments()
		{
			SimplifyArgument(ref _GroupName);
			SimplifyArgument(ref _AttributeValue);
			SimplifyArgument(ref _Param);
		}
	}
}
