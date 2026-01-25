using System.Collections.Generic;
using Nekki.Vector.Core.Models;
using Nekki.Vector.Core.Node;
using UnityEngine;

namespace Nekki.Vector.Core.Controllers
{
	public class ControllerStrike
	{
		private ModelObject _ModelObject;

		private List<ModelNode> _Nodes;

		public ControllerStrike(Model p_model)
		{
			_ModelObject = p_model.ModelObject;
			_Nodes = _ModelObject.CollisibleNodes;
		}

		public void Striking(ModelLine p_line, Vector3f p_point, Vector3f p_impulse)
		{
			Decrease();
			ModelNode start = p_line.Start;
			ModelNode end = p_line.End;
			if (!start.IsFixed || !end.IsFixed)
			{
				Vector3f start2 = start.Start;
				Vector3f start3 = end.Start;
				float num = Vector3f.Factor(p_point, start2, start3);
				Vector3f vector3f = new Vector3f(1f - num, 1f - num, 0f);
				Vector3f vector3f2 = new Vector3f(num, num, 0f);
				float slowModeValue = RunMainController.Scene.SlowModeValue;
				if (!start.IsFixed)
				{
					float p_x = start2.X + vector3f.X * p_impulse.X / Mathf.Sqrt(slowModeValue) / start.Weight;
					float p_y = start2.Y + vector3f.Y * p_impulse.Y / Mathf.Sqrt(slowModeValue) / start.Weight;
					start2.Set(p_x, p_y, start2.Z);
				}
				if (!end.IsFixed)
				{
					float p_x2 = start3.X + vector3f2.X * p_impulse.X / Mathf.Sqrt(slowModeValue) / end.Weight;
					float p_y2 = start3.Y + vector3f2.Y * p_impulse.Y / Mathf.Sqrt(slowModeValue) / end.Weight;
					start3.Set(p_x2, p_y2, start3.Z);
				}
			}
		}

		public void Decrease()
		{
			foreach (ModelNode node in _Nodes)
			{
				node.End.X = (node.End.X + node.Start.X) * 0.5f;
				node.End.Y = (node.End.Y + node.Start.Y) * 0.5f;
				node.End.Z = (node.End.Z + node.Start.Z) * 0.5f;
			}
		}
	}
}
