using System.Xml;

namespace Nekki.Vector.Core.Trigger.Conditions
{
	public class TRC_Equal : TRC_Compare
	{
		public TRC_Equal(TriggerRunnerLoop p_parent, XmlNode p_node)
			: base(p_parent, p_node)
		{
		}

		public override bool Check()
		{
			if (_IsNot)
			{
				return !_Value2.IsEqual(_Value1);
			}
			return _Value2.IsEqual(_Value1);
		}

		protected override string TypeString()
		{
			return "Equal";
		}

		public override string ToString()
		{
			string text = "Equal: " + ((!_IsNot) ? "(" : "!(");
			string text2 = text;
			return text2 + _Value1.ToString() + "==" + _Value2.ToString() + ")";
		}
	}
}
