using System.Xml;
using Nekki.Vector.Core.Models;
using Nekki.Vector.Core.Runners;
using Nekki.Vector.Core.Variables;

namespace Nekki.Vector.Core.Trigger.Actions
{
	public class TRA_Spawn : TriggerRunnerAction
	{
		private Variable _SpawnVar;

		private Variable _ModelVar;

		private bool _UseSaveme;

		private bool _UseTeleport;

		private bool _PlayAnimation;

		private TRA_Spawn(TRA_Spawn p_copyAction)
			: base(p_copyAction)
		{
			_SpawnVar = p_copyAction._SpawnVar;
			_ModelVar = p_copyAction._ModelVar;
		}

		public TRA_Spawn(XmlNode p_node, TriggerRunnerLoop p_parent)
			: base(p_parent)
		{
			TriggerRunnerAction.InitActionVar(p_parent.ParentTrigger, ref _ModelVar, p_node.Attributes["Model"].Value);
			if (p_node.Attributes["Spawn"] != null)
			{
				TriggerRunnerAction.InitActionVar(p_parent.ParentTrigger, ref _SpawnVar, p_node.Attributes["Spawn"].Value);
			}
			_UseSaveme = XmlUtils.ParseBool(p_node.Attributes["UseSaveme"]);
			_UseTeleport = XmlUtils.ParseBool(p_node.Attributes["UseTeleport"]);
			_PlayAnimation = XmlUtils.ParseInt(p_node.Attributes["PlayAnimation"]) != 0;
		}

		public override void Activate(ref bool p_isRunNext)
		{
			base.Activate(ref p_isRunNext);
			p_isRunNext = true;
			string valueString = _ModelVar.ValueString;
			ModelHuman model = GetModel(valueString);
			if (model != null)
			{
				SpawnRunner spawnRunner = null;
				spawnRunner = ((_UseSaveme && _UseTeleport) ? RunMainController.Location.ControllerSpawns.GetNearestTeleportOrSaveMe(model.Position("NPivot", true).X) : (_UseSaveme ? RunMainController.Location.ControllerSpawns.GetNearestSaveMe(model.Position("NPivot", true).X) : ((!_UseTeleport) ? _ParentLoop.ParentTrigger.GetSpawnByName(_SpawnVar.ValueString) : RunMainController.Location.ControllerSpawns.GetNearestTeleport(model.Position("NPivot", true).X))));
				model.Reset();
				model.IsEnabled = true;
				model.PlaySpawn(spawnRunner);
				if (_PlayAnimation)
				{
					model.PlayBurnLaserReversed();
				}
			}
		}

		public override TriggerRunnerAction Copy()
		{
			return new TRA_Spawn(this);
		}

		public override string ToString()
		{
			return ("Spawn Model=" + _ModelVar.DebugStringValue + " Spawn" + _SpawnVar == null) ? "null" : _SpawnVar.DebugStringValue;
		}

		protected override void Log()
		{
			base.Log();
			VectorLog.RunLog("Action: Spawn");
			VectorLog.Tab(1);
			if (_SpawnVar != null)
			{
				VectorLog.RunLog("Spawn", _SpawnVar);
			}
			else
			{
				VectorLog.RunLog("Spawn", "null");
			}
			VectorLog.RunLog("Model", _ModelVar);
			VectorLog.Untab(1);
		}
	}
}
