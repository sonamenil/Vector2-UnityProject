using System.Collections.Generic;
using Nekki.Vector.Core.Variables;
using Nekki.Yaml;

namespace Nekki.Vector.Core.GameManagement
{
	public class TimerActions
	{
		private abstract class TimerAction
		{
			private class SetTimer : TimerAction
			{
				protected Variable _Preset;

				public SetTimer(Mapping p_node)
					: base(p_node)
				{
					_Preset = ((p_node.GetText("Preset") == null) ? null : Variable.CreateVariable(YamlUtils.GetStringValue(p_node.GetText("Preset"), string.Empty), string.Empty));
				}

				public override void Activate()
				{
					TimersManager.CreateTimer(_Name.ValueString, _Value.ValueFloat, (_Preset == null) ? null : _Preset.ValueString);
				}
			}

			private class StopTimer : TimerAction
			{
				public StopTimer(Mapping p_node)
					: base(p_node)
				{
				}

				public override void Activate()
				{
					TimersManager.RemoveTimer(_Name.ValueString);
				}
			}

			protected Variable _Name;

			protected Variable _Value;

			protected TimerAction(Mapping p_node)
			{
				_Name = Variable.CreateVariable(YamlUtils.GetStringValue(p_node.GetText("Name"), string.Empty), string.Empty);
				_Value = ((p_node.GetText("Value") == null) ? null : Variable.CreateVariable(YamlUtils.GetStringValue(p_node.GetText("Value"), string.Empty), string.Empty));
			}

			public static TimerAction Create(Mapping p_node)
			{
				if (p_node == null)
				{
					return null;
				}
				TimerAction result = null;
				switch (p_node.GetText("Type").text)
				{
				case "Set":
					result = new SetTimer(p_node);
					break;
				case "Stop":
					result = new StopTimer(p_node);
					break;
				}
				return result;
			}

			public abstract void Activate();
		}

		public const string NodeName = "TimerActions";

		private List<TimerAction> _OnEnter;

		private List<TimerAction> _OnExit;

		public static TimerActions Create(Mapping p_node)
		{
			if (p_node == null)
			{
				return null;
			}
			TimerActions timerActions = new TimerActions();
			Sequence sequence = p_node.GetSequence("OnEnter");
			Sequence sequence2 = p_node.GetSequence("OnExit");
			if (sequence != null)
			{
				timerActions._OnEnter = new List<TimerAction>();
				foreach (Mapping item in sequence)
				{
					TimerAction timerAction = TimerAction.Create(item);
					if (timerAction != null)
					{
						timerActions._OnEnter.Add(timerAction);
					}
				}
			}
			if (sequence2 != null)
			{
				timerActions._OnExit = new List<TimerAction>();
				foreach (Mapping item2 in sequence2)
				{
					TimerAction timerAction2 = TimerAction.Create(item2);
					if (timerAction2 != null)
					{
						timerActions._OnExit.Add(timerAction2);
					}
				}
			}
			return timerActions;
		}

		public void ActivateOnEnter()
		{
			ActivateList(_OnEnter);
		}

		public void ActivateOnExit()
		{
			ActivateList(_OnExit);
		}

		private void ActivateList(List<TimerAction> p_list)
		{
			if (p_list == null)
			{
				return;
			}
			foreach (TimerAction item in p_list)
			{
				item.Activate();
			}
		}
	}
}
