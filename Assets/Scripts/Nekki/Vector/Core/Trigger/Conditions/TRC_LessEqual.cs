using System.Xml;

namespace Nekki.Vector.Core.Trigger.Conditions
{
	public class TRC_LessEqual : TRC_Compare
	{
		public TRC_LessEqual(TriggerRunnerLoop p_parent, XmlNode p_node)
			: base(p_parent, p_node)
		{
		}

		public override bool Check()
		{
			if (_IsNot)
			{
				return !_Value1.IsLessEqual(_Value2);
			}
			return _Value1.IsLessEqual(_Value2);
		}

		protected override string TypeString()
		{
			return "LessEqual";
		}

		public override string ToString()
		{
			string text = "LessEqual: " + ((!_IsNot) ? "(" : "!(");
			string text2 = text;
			return text2 + _Value1.ToString() + ">=" + _Value2.ToString() + ")";
		}
	}
}
