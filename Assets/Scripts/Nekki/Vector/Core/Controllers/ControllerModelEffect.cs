using System.Collections.Generic;
using Nekki.Vector.Core.Controllers.ModelEffects;
using Nekki.Vector.Core.Models;
using Nekki.Vector.Core.Node;
using Nekki.Vector.Core.Runners;

namespace Nekki.Vector.Core.Controllers
{
	public class ControllerModelEffect
	{
		private ModelObject _ModelObject;

		private List<ModelEffect> _Effects = new List<ModelEffect>();

		private Dictionary<ModelLine, float> _StrokesOnInit = new Dictionary<ModelLine, float>();

		public ControllerModelEffect(ModelObject p_modelObject)
		{
			_ModelObject = p_modelObject;
			RunMainController.OnPause += Pause;
			RunMainController.OnSimulate += Simulate;
			foreach (ModelLine capsule in _ModelObject.Capsules)
			{
				_StrokesOnInit[capsule] = capsule.Stroke;
			}
		}

		public void RunEffect(string p_name, QuadRunner p_runner)
		{
			ModelEffect modelEffect = ModelEffect.Create(p_name, _ModelObject, p_runner);
			if (modelEffect != null)
			{
				_Effects.Add(modelEffect);
			}
		}

		public void Render()
		{
			if (_Effects.Count == 0)
			{
				return;
			}
			int count = _Effects.Count;
			for (int num = count - 1; num >= 0; num--)
			{
				if (_Effects[num].Render())
				{
					_Effects.RemoveAt(num);
				}
			}
		}

		public void Pause(bool p_pause)
		{
			for (int i = 0; i < _Effects.Count; i++)
			{
				_Effects[i].Pause(p_pause);
			}
		}

		public void Simulate(float p_time)
		{
			for (int i = 0; i < _Effects.Count; i++)
			{
				_Effects[i].Simulate(p_time);
			}
		}

		public void Reset()
		{
			foreach (ModelRender modelRender in _ModelObject.ModelRenders)
			{
				modelRender.MeshVisible = true;
			}
			foreach (ModelLine capsule in _ModelObject.Capsules)
			{
				capsule.Stroke = _StrokesOnInit[capsule];
			}
			_Effects.Clear();
		}

		public void End()
		{
			RunMainController.OnPause -= Pause;
			RunMainController.OnSimulate -= Simulate;
		}
	}
}
