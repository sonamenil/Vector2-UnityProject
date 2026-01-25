using System.Xml;

namespace Nekki.Vector.Core.Transformations
{
	public class IntervalActivation : PrototypeInterval
	{
		private enum TypeActivation
		{
			On = 0,
			Off = 1,
			Switch = 2
		}

		private TypeActivation _TypeActivation;

		private bool _Restore;

		public IntervalActivation()
		{
			_Type = IntervalType.Activation;
		}

		protected override void Parse(XmlNode p_node)
		{
			base.Parse(p_node);
			if (p_node.Attributes["Restore"] != null)
			{
				_Restore = XmlUtils.ParseBool(p_node.Attributes["Restore"]);
			}
			switch (p_node.Attributes["Type"].Value)
			{
			case "On":
				_TypeActivation = TypeActivation.On;
				break;
			case "Off":
				_TypeActivation = TypeActivation.Off;
				break;
			case "Switch":
				_TypeActivation = TypeActivation.Switch;
				break;
			}
		}

		public override bool Iteration(TransformInterface p_runner)
		{
			switch (_TypeActivation)
			{
			case TypeActivation.On:
				p_runner.TransformationOn(_Restore);
				break;
			case TypeActivation.Off:
				p_runner.TransformationOff();
				break;
			case TypeActivation.Switch:
				p_runner.TransformationSwitch(_Restore);
				break;
			}
			return false;
		}
	}
}
