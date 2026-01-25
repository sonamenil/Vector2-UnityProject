using System.Xml;

namespace Nekki.Vector.Core.Transformations
{
	public class IntervalExecute : PrototypeInterval
	{
		public IntervalExecute()
		{
			_Type = IntervalType.Execute;
		}

		protected override void Parse(XmlNode p_node)
		{
			base.Parse(p_node);
		}

		public override bool Iteration(TransformInterface p_runner)
		{
			p_runner.TransformExecute();
			return false;
		}
	}
}
