using System.Collections.Generic;
using Nekki.Vector.Core.User;

namespace Nekki.Vector.Core.Variables.Functions
{
	public class VF_SelectItemsByAttributeValue : VariableFunction
	{
		private static List<UserItem> _SelectedItems = new List<UserItem>();

		private Variable _AttributeName;

		private Variable _AttributeValue;

		private Variable _AttributeState;

		private Variable _Param;

		public override int ValueInt
		{
			get
			{
				SelectItems(_AttributeState.ValueString);
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

		protected internal VF_SelectItemsByAttributeValue(string p_value, string p_name, IVariableParent p_parent)
			: base(p_value, p_name, p_parent)
		{
			p_value = VariableFunction.TrimSpaces(p_value);
			string p_name2 = p_value.Split('.')[1];
			int num = p_value.IndexOf("[") + 1;
			int num2 = p_value.LastIndexOf("]");
			string text = p_value.Substring(num, num2 - num);
			string[] array = text.Split(',');
			VariableFunction.InitFuncVar(p_parent, ref _AttributeName, array[0]);
			VariableFunction.InitFuncVar(p_parent, ref _AttributeValue, array[1]);
			if (array.Length > 2)
			{
				VariableFunction.InitFuncVar(p_parent, ref _AttributeState, array[2]);
			}
			else
			{
				VariableFunction.InitFuncVar(p_parent, ref _AttributeState, "Equipped");
			}
			VariableFunction.InitFuncVar(p_parent, ref _Param, p_name2);
		}

		private void SelectItems(string type)
		{
			_SelectedItems.Clear();
			HashSet<UserItem> hashSet = null;
			switch (type)
			{
			case "Stash":
				hashSet = DataLocal.Current.Stash;
				break;
			case "Equipped":
				hashSet = DataLocal.Current.Equipped;
				break;
			case "AllItems":
				hashSet = DataLocal.Current.AllItems;
				break;
			}
			foreach (UserItem item in hashSet)
			{
				if (item.IsContainsAttributeWithValue(_AttributeName.ValueString, _AttributeValue.ValueInt))
				{
					_SelectedItems.Add(item);
				}
			}
		}

		public override void SimplifyArguments()
		{
			SimplifyArgument(ref _AttributeName);
			SimplifyArgument(ref _AttributeValue);
			SimplifyArgument(ref _AttributeState);
			SimplifyArgument(ref _Param);
		}
	}
}
