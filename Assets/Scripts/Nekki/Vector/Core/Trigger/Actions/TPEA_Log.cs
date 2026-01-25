using System.Xml;
using Nekki.Vector.Core.Variables;

namespace Nekki.Vector.Core.Trigger.Actions
{
	public class TPEA_Log : TriggerPassiveEffectAction
	{
		public const string NodeName = "Log";

		private Variable _Value;

		private TPEA_Log(TPEA_Log p_copyAction)
			: base(p_copyAction)
		{
			_Value = p_copyAction._Value;
		}

		public TPEA_Log(XmlNode p_node, TriggerPassiveEffectLoop p_parent)
			: base(p_parent)
		{
			TriggerPassiveEffectAction.InitActionVar(p_parent.Parent, ref _Value, p_node.Attributes["Value"].Value);
		}

		public override void Activate(ref bool p_isRunNext)
		{
			p_isRunNext = true;
			DebugUtils.Log(_Value.ValueString);
		}

		public override TriggerPassiveEffectAction Copy()
		{
			return new TPEA_Log(this);
		}
	}
}
