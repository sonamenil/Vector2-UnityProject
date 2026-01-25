using System.Text;
using System.Xml;
using Nekki.Vector.Core.Counter;
using UnityEngine;
using UnityEngine.SocialPlatforms;

namespace Nekki.Vector.Core.GameCenter
{
	public class AchievementData
	{
		public string Name { get; private set; }

		public string Id { get; private set; }

		public bool IsIncremental { get; private set; }

		public string CounterName { get; private set; }

		public string CounterNamespace { get; private set; }

		public int Threshold { get; private set; }

		public int Goal { get; private set; }

		public bool IsCompleted { get; private set; }

		public int Progress
		{
			get
			{
				return CounterController.Current.GetUserCounter(Name, "ST_Achievements");
			}
			set
			{
				CounterController.Current.CreateCounterOrSetValue(Name, value, "ST_Achievements");
			}
		}

		public int TemporaryData
		{
			get
			{
				return CounterController.Current.GetUserCounter(CounterName, CounterNamespace);
			}
			set
			{
				CounterController.Current.CreateCounterOrSetValue(CounterName, value, CounterNamespace);
			}
		}

		public int Step
		{
			get
			{
				return (!IsIncremental) ? TemporaryData : Progress;
			}
		}

		public AchievementData(XmlNode p_node)
		{
			Name = XmlUtils.ParseString(p_node.Attributes["Name"], string.Empty).Replace(" ", "_");
			Id = XmlUtils.ParseString(p_node.Attributes["Id"], string.Empty);
			IsIncremental = XmlUtils.ParseBool(p_node.Attributes["Incremental"]);
			CounterName = XmlUtils.ParseString(p_node.Attributes["CounterName"], string.Empty);
			CounterNamespace = XmlUtils.ParseString(p_node.Attributes["Namespace"], string.Empty);
			Threshold = XmlUtils.ParseInt(p_node.Attributes["Threshold"], 1);
			Goal = XmlUtils.ParseInt(p_node.Attributes["Goal"], 1);
			RestoreProgress();
		}

		public bool UpdateIncrementalAchievementStep()
		{
			if (IsIncremental)
			{
				if (TemporaryData > 0)
				{
					Progress = Mathf.Min(Progress + TemporaryData, Goal);
				}
				return true;
			}
			return false;
		}

		public void RestoreProgress()
		{
			IsCompleted = ((!IsIncremental) ? (Progress == 1) : (Progress >= Goal));
		}

		public void SendProgress()
		{
			SendProgress(Progress);
		}

		public void SendProgress(int p_progress)
		{
			if (IsCompleted)
			{
				GameCenterController.UnlockAchievement(Id);
			}
			else if (IsIncremental && p_progress >= Threshold)
			{
				GameCenterController.AchievementProgress(Id, p_progress);
			}
			DebugUtils.LogFormat("[AchievementProgress]: {0} - {1}/{2}", Name, p_progress, Goal);
		}

		public void UpdateProgress(bool p_notifyGameCenter = true)
		{
			int num = Mathf.Min(Step, Goal);
			if (num > 0)
			{
				IsCompleted = num >= Goal;
				if (IsCompleted)
				{
					Complete();
				}
				if (p_notifyGameCenter)
				{
					SendProgress(num);
				}
			}
		}

		public void SyncProgress(IAchievement p_achievementData)
		{
			if (IsCompleted)
			{
				Complete();
			}
			else if (IsIncremental)
			{
				Progress = Mathf.Max(Progress, (int)(p_achievementData.percentCompleted / 100.0 * (double)Goal));
			}
			SendProgress();
		}

		private void Complete()
		{
			Progress = ((!IsIncremental) ? 1 : Goal);
		}

		public void Log(StringBuilder p_sb)
		{
			if (IsCompleted)
			{
				p_sb.AppendLine(string.Format("[AchievementProgress]: {0}{1} - {2}/{3}", Name, (!IsIncremental) ? string.Empty : " (i)", Goal, Goal));
			}
			else
			{
				p_sb.AppendLine(string.Format("[AchievementProgress]: {0}{1} - {2}/{3}", Name, (!IsIncremental) ? string.Empty : " (i)", Step, Goal));
			}
		}

		public override string ToString()
		{
			return string.Format("[AchievmentData: Name={0}, Id={1}, IsIncremental={2}, CounterName={3}, CounterNamespace={4}, Threshold={5}, Goal={6}, Progress={7}]", Name, Id, IsIncremental, CounterName, CounterNamespace, Threshold, Goal, Progress);
		}
	}
}
