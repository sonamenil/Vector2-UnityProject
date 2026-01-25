using Nekki.Vector.Core.Localization;

namespace Nekki.Vector.Core.Variables
{
	public class VariableLocalizationString : Variable
	{
		private string _ValueString;

		public override string ValueString
		{
			get
			{
				return _ValueString;
			}
		}

		public override string DebugStringValue
		{
			get
			{
				return _ValueString;
			}
		}

		protected internal VariableLocalizationString(string p_value, string p_name, IVariableParent p_parent)
			: base(p_name, p_parent)
		{
			_Type = VariableType.LocalizationString;
			SetValue(p_value);
		}

		public override void SetValue(string p_value)
		{
			_ValueString = LocalizationManager.ParseAlias(p_value.Replace(" ", string.Empty));
		}
	}
}
