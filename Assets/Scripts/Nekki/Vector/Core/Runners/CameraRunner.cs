using System.Xml;

namespace Nekki.Vector.Core.Runners
{
	public class CameraRunner : SpawnRunner
	{
		public CameraRunner(Element p_elements, XmlNode p_node)
			: base(p_elements, p_node)
		{
			_TypeClass = TypeRunner.Camera;
		}
	}
}
