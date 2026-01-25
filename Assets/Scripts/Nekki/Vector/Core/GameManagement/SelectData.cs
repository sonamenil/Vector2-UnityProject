using System.Collections.Generic;
using Nekki.Vector.Core.Variables;
using Nekki.Yaml;

namespace Nekki.Vector.Core.GameManagement
{
	public class SelectData
	{
		public const string NodeName = "Select";

		public static bool BreakFlag { get; set; }

		public SelectType Type { get; private set; }

		public Variable Root { get; private set; }

		public bool Random { get; private set; }

		public Variable Weight { get; private set; }

		public List<Variable> YamlPath { get; private set; }

		public Variable RangeValue { get; private set; }

		private SelectData(Mapping p_node)
		{
			Type = YamlUtils.GetEnumValue(p_node.GetText("Type"), SelectType.Balance);
			Root = Variable.CreateVariable(YamlUtils.GetStringValue(p_node.GetText("Root"), string.Empty), string.Empty);
			RangeValue = Variable.CreateVariable(YamlUtils.GetStringValue(p_node.GetText("RangeValue"), string.Empty), string.Empty);
			Random = YamlUtils.GetBoolValue(p_node.GetText("Random"));
			string stringValue = YamlUtils.GetStringValue(p_node.GetText("Weight"), string.Empty);
			if (stringValue != string.Empty)
			{
				Weight = Variable.CreateVariable(stringValue, string.Empty);
			}
			if (Type != 0 && Type != SelectType.Cards)
			{
				return;
			}
			YamlPath = new List<Variable>();
			string text = VariableFunction.TrimSpaces(Root.ValueString);
			if (string.IsNullOrEmpty(text))
			{
				return;
			}
			List<string> funcArgs = VariableFunction.GetFuncArgs(text, ',', new string[2] { "[]", "{}" });
			foreach (string item in funcArgs)
			{
				YamlPath.Add(Variable.CreateVariable(item, null));
			}
		}

		public static SelectData Create(Mapping p_node)
		{
			if (p_node == null)
			{
				return null;
			}
			return new SelectData(p_node);
		}
	}
}
