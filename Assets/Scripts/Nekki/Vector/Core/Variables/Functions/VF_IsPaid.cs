using Nekki.Vector.Core.User;

namespace Nekki.Vector.Core.Variables.Functions
{
	public class VF_IsPaid : VariableFunction
	{
		public override int ValueInt
		{
			get
			{
				return DataLocal.Current.IsPaidVersion ? 1 : 0;
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

		public override string ValueForSave
		{
			get
			{
				return (!_IsPointer) ? "?isPaid[]" : "?_isPaid[]";
			}
		}

		protected internal VF_IsPaid(string p_value, string p_name, IVariableParent p_parent)
			: base(p_value, p_name, p_parent)
		{
		}
	}
}
