using System.Xml;
using Nekki.Vector.Core.Variables;
using Nekki.Yaml;

namespace Nekki.Vector.Core.Counter
{
	public class CounterConditionRange : CounterCondition
	{
		private Variable _Min;

		private Variable _Max;

		private Variable _Equal;

		public int MinValue
		{
			get
			{
				return _Min.ValueInt;
			}
		}

		public string RangeString
		{
			get
			{
				return string.Format("[{0} {1}]", _Min.ValueInt, _Max.ValueInt);
			}
		}

		public CounterConditionRange(XmlNode p_node, string p_namespace)
			: base(p_node, p_namespace)
		{
			_Min = Variable.CreateVariable(XmlUtils.ParseString(p_node.Attributes["Min"], int.MinValue.ToString()), null);
			_Max = Variable.CreateVariable(XmlUtils.ParseString(p_node.Attributes["Max"], int.MaxValue.ToString()), null);
			XmlAttribute xmlAttribute = p_node.Attributes["Equal"];
			if (xmlAttribute != null)
			{
				_Equal = Variable.CreateVariable(XmlUtils.ParseString(xmlAttribute, "0"), null);
			}
			else
			{
				_Equal = null;
			}
		}

		public CounterConditionRange(Mapping p_node, string p_namespace)
			: base(p_node, p_namespace)
		{
			_Min = Variable.CreateVariable(YamlUtils.GetStringValue(p_node.GetText("Min"), int.MinValue.ToString()), null);
			_Max = Variable.CreateVariable(YamlUtils.GetStringValue(p_node.GetText("Max"), int.MaxValue.ToString()), null);
			Scalar text = p_node.GetText("Equal");
			if (text != null)
			{
				_Equal = Variable.CreateVariable(YamlUtils.GetStringValue(p_node.GetText("Equal"), "0"), null);
			}
			else
			{
				_Equal = null;
			}
		}

		private CounterConditionRange(CounterConditionRange p_copy)
			: base(p_copy)
		{
			_Min = p_copy._Min;
			_Max = p_copy._Max;
			_Equal = p_copy._Equal;
		}

		public override bool Check()
		{
			int p_value = CounterController.Current.GetUserCounter(base.Name, base.Namespace);
			return Check(p_value);
		}

		public override bool Check(int p_value)
		{
			bool flag = p_value >= _Min.ValueInt && p_value <= _Max.ValueInt && (_Equal == null || p_value == _Equal.ValueInt);
			return (!base.Not) ? flag : (!flag);
		}

		public override bool CheckRange(Point p_range)
		{
			if (_Equal != null)
			{
				return p_range.X <= (float)_Equal.ValueInt && (float)_Equal.ValueInt <= p_range.Y;
			}
			return !(p_range.X > (float)_Max.ValueInt) && !(p_range.Y < (float)_Min.ValueInt);
		}

		public override bool CheckByMax(int p_value)
		{
			bool flag = p_value <= _Max.ValueInt && (_Equal == null || p_value <= _Equal.ValueInt);
			return (!base.Not) ? flag : (!flag);
		}

		public override CounterCondition Copy()
		{
			return new CounterConditionRange(this);
		}

		public override string ToString()
		{
			return (_Equal != null) ? ("Condition Range Name:" + base.Name + " NS:" + base.Namespace + " Equal:" + _Equal.ValueInt) : ("Condition Range Name:" + base.Name + " NS:" + base.Namespace + " Min:" + _Min.ValueInt + " Max:" + _Max.ValueInt + " Not:" + base.Not);
		}

		public override string GetXmlNodeText(string p_tabs)
		{
			return string.Format("{0}<ConditionRange Name=\"{1}\" Min=\"{2}\" Max=\"{3}\" />", p_tabs, base.Name, _Min.ValueInt, _Max.ValueInt);
		}
	}
}
