using System.Xml;
using Nekki.Vector.Core.Variables;
using Nekki.Vector.GUI;
using Nekki.Vector.GUI.Scenes.Run;

namespace Nekki.Vector.Core.Trigger.Actions
{
	public class TPEA_ShowPassiveEffect : TriggerPassiveEffectAction
	{
		public const string NodeName = "ShowPassiveEffect";

		private Variable _Name;

		private Variable _ImageName;

		private Variable _DelayFrames;

		private Variable _Color;

		private Variable _Quantity;

		private Variable _ActionLength;

		private Variable _RenewType;

		private Variable _StackType;

		private Variable _CounterName;

		private Variable _CounterValue;

		private TPEA_ShowPassiveEffect(TPEA_ShowPassiveEffect p_copyAction)
			: base(p_copyAction)
		{
			_Name = p_copyAction._Name;
			_ImageName = p_copyAction._ImageName;
			_DelayFrames = p_copyAction._DelayFrames;
			_Color = p_copyAction._Color;
			_Quantity = p_copyAction._Quantity;
			_ActionLength = p_copyAction._ActionLength;
			_RenewType = p_copyAction._RenewType;
			_StackType = p_copyAction._StackType;
			_CounterName = p_copyAction._CounterName;
			_CounterValue = p_copyAction._CounterValue;
		}

		public TPEA_ShowPassiveEffect(XmlNode p_node, TriggerPassiveEffectLoop p_parent)
			: base(p_parent)
		{
			TriggerPassiveEffectAction.InitActionVar(p_parent.Parent, ref _Name, p_node.Attributes["Name"].Value);
			TriggerPassiveEffectAction.InitActionVar(p_parent.Parent, ref _ImageName, p_node.Attributes["ImageName"].Value);
			TriggerPassiveEffectAction.InitActionVar(p_parent.Parent, ref _DelayFrames, p_node.Attributes["DelayFrames"].Value);
			TriggerPassiveEffectAction.InitActionVar(p_parent.Parent, ref _Color, p_node.Attributes["Color"].Value);
			TriggerPassiveEffectAction.InitActionVar(p_parent.Parent, ref _Quantity, XmlUtils.ParseString(p_node.Attributes["Quantity"], "0"));
			TriggerPassiveEffectAction.InitActionVar(p_parent.Parent, ref _ActionLength, XmlUtils.ParseString(p_node.Attributes["ActionLength"], "OneTime"));
			TriggerPassiveEffectAction.InitActionVar(p_parent.Parent, ref _RenewType, XmlUtils.ParseString(p_node.Attributes["RenewType"], "Queue"));
			TriggerPassiveEffectAction.InitActionVar(p_parent.Parent, ref _StackType, XmlUtils.ParseString(p_node.Attributes["StackType"], "Subs"));
			TriggerPassiveEffectAction.InitActionVar(p_parent.Parent, ref _CounterName, XmlUtils.ParseString(p_node.Attributes["CounterName"], string.Empty));
			TriggerPassiveEffectAction.InitActionVar(p_parent.Parent, ref _CounterValue, XmlUtils.ParseString(p_node.Attributes["CounterValue"], "0"));
		}

		public override void Activate(ref bool p_isRunNext)
		{
			p_isRunNext = true;
			HudPanel module = UIModule.GetModule<HudPanel>();
			if (module != null)
			{
				module.PanelStatusEffects.AddStatusEffect(_Name.ValueString, _ImageName.ValueString, _Color.ValueString, _DelayFrames.ValueInt, _Quantity.ValueInt, _ActionLength.ValueString, _RenewType.ValueString, _StackType.ValueString, _CounterName.ValueString, _CounterValue.ValueInt);
			}
		}

		public override TriggerPassiveEffectAction Copy()
		{
			return new TPEA_ShowPassiveEffect(this);
		}

		public override string ToString()
		{
			return "ShowStatusEffectName= " + _Name.DebugStringValue + "ImageName= " + _ImageName.DebugStringValue + "Color=" + _Color.DebugStringValue + "DelayFrames=" + _DelayFrames.DebugStringValue + "Quantity=" + _Quantity.DebugStringValue + "ActionLength=" + _ActionLength.DebugStringValue + "RenewType=" + _RenewType.DebugStringValue + "StackType=" + _StackType.DebugStringValue;
		}
	}
}
