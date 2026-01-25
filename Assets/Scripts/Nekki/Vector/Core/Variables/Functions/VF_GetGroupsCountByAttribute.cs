using Nekki.Vector.Core.GameManagement;
using Nekki.Vector.Core.User;

namespace Nekki.Vector.Core.Variables.Functions
{
	public class VF_GetGroupsCountByAttribute : VariableFunction
	{
		private Variable _ItemName;

		private Variable _AttributeName;

		public override int ValueInt
		{
			get
			{
				return CountGroups();
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

		protected internal VF_GetGroupsCountByAttribute(string p_value, string p_name, IVariableParent p_parent)
			: base(p_value, p_name, p_parent)
		{
			p_value = VariableFunction.TrimSpaces(p_value);
			int num = p_value.IndexOf("[") + 1;
			int num2 = p_value.LastIndexOf("]");
			string text = p_value.Substring(num, num2 - num);
			string[] array = text.Split(',');
			VariableFunction.InitFuncVar(p_parent, ref _ItemName, array[0]);
			VariableFunction.InitFuncVar(p_parent, ref _AttributeName, array[1]);
		}

		private int CountGroups()
		{
			UserItem itemByName = DataLocal.Current.GetItemByName(_ItemName.ValueString);
			int num = 0;
			foreach (ItemGroupAttributes group in itemByName.Groups)
			{
				if (group.HasValue(_AttributeName.ValueString))
				{
					num++;
				}
			}
			return num;
		}

		public override void SimplifyArguments()
		{
			SimplifyArgument(ref _ItemName);
			SimplifyArgument(ref _AttributeName);
		}
	}
}
