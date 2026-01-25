using System.Xml;

namespace Nekki.Vector.Core.Transformations
{
	public class IntervalLayer : PrototypeInterval
	{
		private string _Layer;

		public IntervalLayer()
		{
			_Type = IntervalType.Layer;
		}

		protected override void Parse(XmlNode p_node)
		{
			base.Parse(p_node);
			_Layer = XmlUtils.ParseString(p_node.Attributes["ToLayer"], string.Empty);
		}

		public override bool Iteration(TransformInterface p_runner)
		{
			p_runner.TransformLayer(_Layer);
			return false;
		}
	}
}
