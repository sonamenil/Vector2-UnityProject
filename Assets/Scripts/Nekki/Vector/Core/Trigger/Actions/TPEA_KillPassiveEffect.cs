using System.Xml;
using Nekki.Vector.Core.Variables;
using Nekki.Vector.GUI;
using Nekki.Vector.GUI.Scenes.Run;

namespace Nekki.Vector.Core.Trigger.Actions
{
	public class TPEA_KillPassiveEffect : TriggerPassiveEffectAction
	{
		public const string NodeName = "KillPassiveEffect";

		private Variable _Name;

		private TPEA_KillPassiveEffect(TPEA_KillPassiveEffect p_copyAction)
			: base(p_copyAction)
		{
			_Name = p_copyAction._Name;
		}

		public TPEA_KillPassiveEffect(XmlNode p_node, TriggerPassiveEffectLoop p_parent)
			: base(p_parent)
		{
			TriggerPassiveEffectAction.InitActionVar(p_parent.Parent, ref _Name, p_node.Attributes["Name"].Value);
		}

		public override void Activate(ref bool p_isRunNext)
		{
			p_isRunNext = true;
			HudPanel module = UIModule.GetModule<HudPanel>();
			if (module != null)
			{
				module.PanelStatusEffects.KillStatusEffect(_Name.ValueString);
			}
		}

		public override TriggerPassiveEffectAction Copy()
		{
			return new TPEA_KillPassiveEffect(this);
		}

		public override string ToString()
		{
			return "KillStatusEffect Name=" + _Name.DebugStringValue;
		}
	}
}
