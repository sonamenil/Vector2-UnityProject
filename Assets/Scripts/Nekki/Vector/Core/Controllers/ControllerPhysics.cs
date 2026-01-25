using System.Collections.Generic;
using Nekki.Vector.Core.Models;
using Nekki.Vector.Core.Node;

namespace Nekki.Vector.Core.Controllers
{
	public class ControllerPhysics
	{
		private int _Iterative = 1;

		private ModelObject _ModelObject;

		private bool _IsPhysics;

		private double _SlowMode = 1.0;

		private List<ModelNode> _Nodes;

		private List<ModelLine> _Edges;

		private static float _Gravity = 0.4f;

		public ModelObject ModelObject
		{
			get
			{
				return _ModelObject;
			}
		}

		public bool IsPhysics
		{
			get
			{
				return _IsPhysics;
			}
		}

		public double SlowMode
		{
			get
			{
				return _SlowMode;
			}
		}

		public List<ModelNode> Nodes
		{
			get
			{
				return _Nodes;
			}
		}

		public List<ModelLine> Edges
		{
			get
			{
				return _Edges;
			}
		}

		public static float Gravity
		{
			get
			{
				return _Gravity / RunMainController.Scene.SlowModeValue;
			}
		}

		public ControllerPhysics(ModelObject p_modelObject)
		{
			_ModelObject = p_modelObject;
			_Nodes = _ModelObject.NodesAll;
			_Edges = _ModelObject.EdgesAll;
		}

		public void Render()
		{
			TimeStep();
			IterativeProcess();
			NodeReset();
		}

		public void Start()
		{
			_IsPhysics = true;
		}

		public void Stop()
		{
			_IsPhysics = false;
		}

		public void TimeStep()
		{
			ModelNode modelNode = null;
			for (int i = 0; i < _Nodes.Count; i++)
			{
				modelNode = _Nodes[i];
				if (!modelNode.IsFixed && (_IsPhysics || modelNode.IsPhysics))
				{
					modelNode.TimeStep(Gravity);
				}
			}
		}

		public void IterativeProcess()
		{
			for (int i = 0; i < _Iterative; i++)
			{
				Iterative();
			}
		}

		public void Iterative()
		{
			ModelLine modelLine = null;
			for (int i = 0; i < _Edges.Count; i++)
			{
				modelLine = _Edges[i];
				if (modelLine.Start != null && modelLine.End != null)
				{
					IterativeLine(modelLine, _IsPhysics);
				}
			}
		}

		public void IterativeLine(ModelLine p_line, bool p_IsPhysics)
		{
			if (IsPhysics || p_line.Start.IsPhysics || p_line.End.IsPhysics)
			{
				Vector3f p_vector = p_line.Iterative(p_line.Start.Start);
				Vector3f p_vector2 = p_line.Iterative(p_line.End.Start);
				IterativeNode(p_line.Start, p_vector, p_IsPhysics);
				IterativeNode(p_line.End, p_vector2, p_IsPhysics);
			}
		}

		public void IterativeNode(ModelNode p_node, Vector3f p_vector, bool p_IsPhysics)
		{
			if (p_node.IsType && !p_node.IsFixed && (p_IsPhysics || p_node.IsPhysics))
			{
				p_node.PositionStart(p_vector);
			}
		}

		public void NodeReset()
		{
			if (_IsPhysics)
			{
				_ModelObject.PositionToPivot(_ModelObject.DetectorHorizontalNode);
				_ModelObject.PositionToPivot(_ModelObject.DetectorVerticalNode);
				_ModelObject.PositionToPivot(_ModelObject.CameraNode.Node);
				_ModelObject.PositionToPivot(_ModelObject.CenterOfMassNode);
			}
		}
	}
}
