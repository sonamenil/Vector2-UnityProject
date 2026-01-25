using System.Collections.Generic;
using Nekki.Vector.Core.Variables;

namespace Nekki.Vector.Core.Localization
{
	public class LocalizationAlias
	{
		private string _Text;

		private List<Variable> _Vars;

		public string Text
		{
			get
			{
				return MakeText();
			}
			set
			{
				_Text = value;
			}
		}

		public LocalizationAlias(string p_text)
		{
			_Text = p_text;
		}

		public void SetParams(string p_params)
		{
			_Vars = new List<Variable>();
			List<string> funcArgs = VariableFunction.GetFuncArgs(p_params, ',', new string[2] { "[]", "{}" });
			for (int i = 0; i < funcArgs.Count; i++)
			{
				Variable item = Variable.CreateVariable(funcArgs[i], string.Empty);
				_Vars.Add(item);
			}
		}

		private string MakeText()
		{
			if (_Vars == null)
			{
				return _Text;
			}
			string text = _Text;
			for (int i = 0; i < _Vars.Count; i++)
			{
				string valueString = _Vars[i].ValueString;
				text = ((valueString.IndexOf("^") != 0 || valueString.LastIndexOf("^") != valueString.Length - 1) ? text.Replace("%" + (i + 1), valueString) : text.Replace("%" + (i + 1), LocalizationManager.GetPhrase(valueString.Replace("^", string.Empty))));
			}
			return text;
		}
	}
}
