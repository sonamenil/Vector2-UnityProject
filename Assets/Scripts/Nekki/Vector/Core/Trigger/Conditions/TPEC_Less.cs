using System.Xml;

namespace Nekki.Vector.Core.Trigger.Conditions
{
	public class TPEC_Less : TPEC_Compare
	{
		public const string NodeName = "Less";

		public TPEC_Less(TriggerPassiveEffectLoop p_parent, XmlNode p_node)
			: base(p_parent, p_node)
		{
		}

		public override bool Check()
		{
			if (_IsNot)
			{
				return !_Value1.IsLess(_Value2);
			}
			return _Value1.IsLess(_Value2);
		}

		public override string ToString()
		{
			string text = "Less: " + ((!_IsNot) ? "(" : "!(");
			string text2 = text;
			return text2 + _Value1.ToString() + "<" + _Value2.ToString() + ")";
		}
	}
}
