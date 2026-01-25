namespace Nekki.Vector.Core.Node
{
	public class CameraNode
	{
		private ModelNode _Node;

		public ModelNode Node
		{
			get
			{
				return _Node;
			}
		}

		public Vector3f End
		{
			get
			{
				return (_Node != null) ? _Node.End : new Vector3f(0f, 0f, 0f);
			}
		}

		public Vector3f Start
		{
			get
			{
				return (_Node != null) ? _Node.Start : new Vector3f(0f, 0f, 0f);
			}
		}

		public CameraNode(ModelNode p_node)
		{
			_Node = p_node;
			Redraw();
		}

		private void Redraw()
		{
			if (_Node != null)
			{
				_Node.Radius = 8;
			}
		}
	}
}
