using System.Xml;
using Nekki.Vector.Core.Variables;

namespace Nekki.Vector.Core.Trigger.Actions
{
	public class TQA_SetVariable : TriggerQuestAction
	{
		private Variable _SetVar;

		private Variable _Value;

		public TQA_SetVariable(XmlNode p_node, TriggerQuestLoop p_parent)
			: base(p_parent)
		{
			_Value = Variable.CreateVariable(XmlUtils.ParseString(p_node.Attributes["Value"]), string.Empty);
			_SetVar = Variable.CreateVariable(XmlUtils.ParseString(p_node.Attributes["Name"]), string.Empty);
		}

		public override void Activate(ref bool p_runNext)
		{
			p_runNext = true;
			SetVar();
		}

		private void SetVar()
		{
			switch (_Value.Type)
			{
			case VariableType.Float:
			case VariableType.Function:
			case VariableType.Expression:
				_SetVar.SetValue(_Value.ValueFloat);
				break;
			case VariableType.String:
				_SetVar.SetValue(_Value.ValueString);
				break;
			default:
				_SetVar.SetValue(_Value.ValueInt);
				break;
			}
		}
	}
}
