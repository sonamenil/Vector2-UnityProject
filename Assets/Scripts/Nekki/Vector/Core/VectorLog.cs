using System.Collections.Generic;
using Nekki.Vector.Core.Animation;
using Nekki.Vector.Core.Generator;
using Nekki.Vector.Core.Runners;
using Nekki.Vector.Core.Trigger;
using Nekki.Vector.Core.Variables;

namespace Nekki.Vector.Core
{
	public static class VectorLog
	{
		private const string _TextIndent = "  ";

		private static string _CurrentTab = string.Empty;

		private static uint _CurrentFrame;

		private static string _CurrentAnimation;

		public static void Init()
		{
		}

		public static void Tab(int count = 1)
		{
		}

		public static void Untab(int count = 1)
		{
		}

		public static void SetTabs(int count)
		{
		}

		public static void Log(object message, string stacktrace = null)
		{
		}

		public static void RunLog(object message, string stacktrace = null)
		{
		}

		public static void RunLog(AnimationReaction animationReaction, AnimationEventType type, string data)
		{
		}

		public static void RunLogAnimationFrame(int frame, bool previous = false)
		{
		}

		public static void RunLog(TriggerEvent triggerEvent, string name, Vector3f position, bool checking)
		{
		}

		public static void RunLog(TriggerRunner triggerRunner)
		{
		}

		public static void RunLog(TriggerLoop triggerLoop)
		{
		}

		public static void RunLog(string name, Variable variable)
		{
		}

		private static void SetFrame()
		{
		}

		public static void GeneratorStart()
		{
		}

		public static void GeneratorEnd()
		{
		}

		public static void GeneratorLog(object message, string stacktrace = null)
		{
		}

		public static void GeneratorLog(RoomConditionList p_conditions)
		{
		}

		public static void GeneratorLog(Room p_room)
		{
		}

		public static void GeneratorLogCounters()
		{
		}

		public static void GeneratorLog(List<Variant> p_variants, Dictionary<string, int> p_labels)
		{
		}
	}
}
