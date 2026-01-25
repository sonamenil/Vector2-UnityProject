using System.Collections.Generic;
using Nekki.Vector.Core.Models;
using Nekki.Vector.Core.Node;
using Nekki.Vector.Core.Runners;

namespace Nekki.Vector.Core.Controllers
{
	public class ControllerArea
	{
		private ModelHuman _Model;

		private ModelNode _ComNode;

		private List<AreaRunner> _ActiveAreas = new List<AreaRunner>();

		public List<AreaRunner> ActiveAreas
		{
			get
			{
				return _ActiveAreas;
			}
		}

		public ControllerArea(ModelHuman p_parent)
		{
			_Model = p_parent;
			_ComNode = p_parent.Node("COM");
		}

		private bool IsInside(AreaRunner p_value)
		{
			return p_value.Hit(_ComNode.Start);
		}

		private bool IsActive(AreaRunner p_value)
		{
			return ActiveAreas.Contains(p_value);
		}

		private void Activate(AreaRunner p_value)
		{
			_ActiveAreas.Add(p_value);
			p_value.Activate(_Model);
			_Model.OnActiveArea();
		}

		private void Deactivate(AreaRunner p_value)
		{
			_Model.CheckDelayAction(p_value);
			p_value.Deactivate(_Model);
			_ActiveAreas.Remove(p_value);
		}

		public void Check(AreaRunner p_value)
		{
			bool flag = IsInside(p_value);
			if (IsActive(p_value))
			{
				if (!flag)
				{
					Deactivate(p_value);
				}
			}
			else if (flag)
			{
				Activate(p_value);
			}
		}

		public void RemoveArea(AreaRunner p_value)
		{
			if (_ActiveAreas.Contains(p_value))
			{
				_ActiveAreas.Remove(p_value);
			}
		}

		public void Reset()
		{
			_ActiveAreas.Clear();
		}
	}
}
