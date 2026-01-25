using System.Xml;
using Nekki.Vector.Core.Variables;

namespace Nekki.Vector.Core.Trigger.Conditions
{
	public class TRC_Compare : TriggerRunnerCondition
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

		public TRC_Compare(TriggerRunnerLoop p_parent, XmlNode p_node)
			: base(p_parent, p_node)
		{
			Parse(p_node);
		}

		protected void Parse(XmlNode p_node)
		{
			_Value1 = GetOrCreateVar(p_node.Attributes["Value1"].Value);
			_Value2 = GetOrCreateVar(p_node.Attributes["Value2"].Value);
		}

		public override bool Check()
		{
			return false;
		}

		protected override string TypeString()
		{
			return "Compare";
		}

		public override void Log(bool result)
		{
			base.Log(result);
			VectorLog.RunLog("Value1", Value1);
			VectorLog.RunLog("Value2", Value2);
			VectorLog.RunLog("Result: " + result);
			VectorLog.Untab(1);
			VectorLog.Untab(1);
		}
	}
}
