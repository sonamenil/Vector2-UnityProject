using System.Xml;
using Nekki.Vector.Core.GameManagement;
using Nekki.Vector.Core.Variables;

namespace Nekki.Vector.Core.Trigger.Actions
{
	public class TQA_SetString : TriggerQuestAction
	{
		private Variable _NameVar;

		private Variable _ValueVar;

		private bool _Collect;

		public TQA_SetString(XmlNode p_node, TriggerQuestLoop p_parent)
			: base(p_parent)
		{
			_NameVar = Variable.CreateVariable(XmlUtils.ParseString(p_node.Attributes["Name"], string.Empty), string.Empty);
			_ValueVar = Variable.CreateVariable(XmlUtils.ParseString(p_node.Attributes["Value"], string.Empty), string.Empty);
			_Collect = XmlUtils.ParseBool(p_node.Attributes["Collect"]);
		}

		public override void Activate(ref bool p_isRunNext)
		{
			p_isRunNext = true;
			StringBuffer.AddString(_NameVar.ValueString, _ValueVar.ValueString, _Collect);
		}

		public override string ToString()
		{
			return "SetString Name:" + _NameVar.DebugStringValue + " Value=" + _ValueVar.DebugStringValue;
		}
	}
}
