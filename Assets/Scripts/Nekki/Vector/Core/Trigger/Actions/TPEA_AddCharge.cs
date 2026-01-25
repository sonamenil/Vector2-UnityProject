using System.Xml;
using Nekki.Vector.Core.GameManagement;
using Nekki.Vector.Core.User;
using Nekki.Vector.Core.Variables;

namespace Nekki.Vector.Core.Trigger.Actions
{
	public class TPEA_AddCharge : TriggerPassiveEffectAction
	{
		public const string NodeName = "AddCharge";

		private Variable _GadgetName;

		private Variable _Count;

		private Variable _ChargeType;

		private TPEA_AddCharge(TPEA_AddCharge p_copyAction)
			: base(p_copyAction)
		{
			_GadgetName = p_copyAction._GadgetName;
			_Count = p_copyAction._Count;
			_ChargeType = p_copyAction._ChargeType;
		}

		public TPEA_AddCharge(XmlNode p_node, TriggerPassiveEffectLoop p_parent)
			: base(p_parent)
		{
			TriggerPassiveEffectAction.InitActionVar(p_parent.Parent, ref _GadgetName, XmlUtils.ParseString(p_node.Attributes["GadgetName"], string.Empty));
			TriggerPassiveEffectAction.InitActionVar(p_parent.Parent, ref _Count, XmlUtils.ParseString(p_node.Attributes["Count"], "1"));
			TriggerPassiveEffectAction.InitActionVar(p_parent.Parent, ref _ChargeType, XmlUtils.ParseString(p_node.Attributes["ChargeType"], GadgetItem.ChargeType.Normal.ToString()));
		}

		public override void Activate(ref bool p_isRunNext)
		{
			p_isRunNext = true;
			GadgetItem gadget = DataLocalHelper.GetGadget(_GadgetName.ValueString);
			if (gadget != null)
			{
				switch (_ChargeType.ValueString.GetChargeTypeByName())
				{
				case GadgetItem.ChargeType.Normal:
					gadget.CurrentCharges += _Count.ValueInt;
					break;
				case GadgetItem.ChargeType.Bonus:
					gadget.BonusCharges += _Count.ValueInt;
					break;
				}
			}
		}

		public override TriggerPassiveEffectAction Copy()
		{
			return new TPEA_AddCharge(this);
		}

		public override string ToString()
		{
			return string.Format("AddCharge GadgetName={0}, Count={1}, ChargeType={2}", _GadgetName.DebugStringValue, _Count.DebugStringValue, _ChargeType.DebugStringValue);
		}
	}
}
