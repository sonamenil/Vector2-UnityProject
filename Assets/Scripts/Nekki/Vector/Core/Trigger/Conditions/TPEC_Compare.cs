using System;
using System.Xml;
using Nekki.Vector.Core.Variables;

namespace Nekki.Vector.Core.Trigger.Conditions
{
	public class TPEC_Compare : TriggerPassiveEffectCondition
	{
		protected Variable _Value1;

		protected Variable _Value2;

		public Variable Value1
		{
			get
			{
				return _Value1;
			}
		}

		public Variable Value2
		{
			get
			{
				return _Value2;
			}
		}

		public TPEC_Compare(TriggerPassiveEffectLoop p_parent, XmlNode p_node)
			: base(p_node, p_parent)
		{
			Parse(p_node);
		}

		protected void Parse(XmlNode p_node)
		{
			_Value1 = _Parent.Parent.GetOrCreateVar(p_node.Attributes["Value1"].Value);
			_Value2 = _Parent.Parent.GetOrCreateVar(p_node.Attributes["Value2"].Value);
		}

		public override bool Check()
		{
			throw new NotImplementedException();
		}

		public override void Log(bool result)
		{
		}
	}
}
