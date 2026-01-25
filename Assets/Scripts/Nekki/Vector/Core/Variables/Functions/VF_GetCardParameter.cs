using System.Collections.Generic;
using Nekki.Vector.Core.GameManagement;
using Nekki.Vector.Core.User;

namespace Nekki.Vector.Core.Variables.Functions
{
	public class VF_GetCardParameter : VariableFunction
	{
		private Variable _CardName;

		private Variable _Key;

		public override int ValueInt
		{
			get
			{
				return TryGetString().ValueInt;
			}
		}

		public override float ValueFloat
		{
			get
			{
				return TryGetString().ValueFloat;
			}
		}

		public override string ValueString
		{
			get
			{
				return TryGetString().ValueString;
			}
		}

		protected internal VF_GetCardParameter(string p_value, string p_name, IVariableParent p_parent)
			: base(p_value, p_name, p_parent)
		{
			_IsStringFunction = true;
			p_value = VariableFunction.TrimSpaces(p_value);
			int num = p_value.IndexOf("[") + 1;
			int num2 = p_value.LastIndexOf("]");
			string p_value2 = p_value.Substring(num, num2 - num);
			List<string> funcArgs = VariableFunction.GetFuncArgs(p_value2, ',', new string[2] { "[]", "{}" });
			VariableFunction.InitFuncVar(p_parent, ref _CardName, funcArgs[0]);
			VariableFunction.InitFuncVar(p_parent, ref _Key, funcArgs[1]);
		}

		private Variable TryGetString()
		{
			int cardLevel = GetCardLevel();
			return Variable.CreateVariable(CardLevelsManger.Current.GetCardParameter(_CardName.ValueString, _Key.ValueString, cardLevel), string.Empty);
		}

		private int GetCardLevel()
		{
			CardsGroupAttribute card = DataLocalHelper.GetCard(_CardName.ValueString);
			return (card != null) ? card.UserCardTotalLevel : 0;
		}
	}
}
