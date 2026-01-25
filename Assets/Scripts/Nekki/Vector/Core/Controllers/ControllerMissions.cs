using System.Collections.Generic;
using Nekki.Vector.Core.Counter;
using Nekki.Vector.Core.GameManagement;
using Nekki.Vector.GUI.Dialogs;

namespace Nekki.Vector.Core.Controllers
{
	public class ControllerMissions
	{
		private HashSet<string> _CompletedMissions = new HashSet<string>();

		public ControllerMissions()
		{
			CounterController.Current.GetCounterNamespace("MissionProgress").OnCounterChanged += OnMissionProgressChanged;
		}

		public void End()
		{
			CounterController.Current.GetCounterNamespace("MissionProgress").OnCounterChanged -= OnMissionProgressChanged;
		}

		private void OnMissionProgressChanged(string p_counterName, string p_namespaceName)
		{
			if (MissionsManager.IsMissionsEnabled && !_CompletedMissions.Contains(p_counterName))
			{
				MissionItem missionItem = MissionsManager.MissionItems.Find((MissionItem p_item) => p_item.CounterName == p_counterName);
				if (missionItem != null && missionItem.IsCompletedCounters)
				{
					DebugUtils.Log("[ControllerMissions]: MissionCompleted - " + p_counterName);
					_CompletedMissions.Add(p_counterName);
					missionItem.Complete();
					Notification.Parameters parameters = new Notification.Parameters();
					parameters.Orientation = Notification.Orientation.Bottom;
					parameters.HideBy = Notification.HideBy.TimeDontBlockClicks;
					parameters.QueueType = DialogQueueType.Notification;
					parameters.Mission = missionItem;
					DialogNotificationManager.ShowMissionNotification(parameters);
				}
			}
		}
	}
}
