using System.Collections.Generic;
using System.Xml;
using Nekki.Vector.Core.Runners;
using Nekki.Vector.Core.Trigger.Events;
using Nekki.Vector.Core.Variables;

namespace Nekki.Vector.Core.Trigger.Actions
{
	public class TRA_Activate : TriggerRunnerAction
	{
		private Variable _ActivationID;

		private Variable _AboveLevel;

		private Variable _DispatchLevel;

		private TRA_Activate(TRA_Activate p_copyAction)
			: base(p_copyAction)
		{
			_ActivationID = p_copyAction._ActivationID;
			_AboveLevel = p_copyAction._AboveLevel;
			_DispatchLevel = p_copyAction._DispatchLevel;
		}

		public TRA_Activate(XmlNode p_node, TriggerRunnerLoop p_parent)
			: base(p_parent)
		{
			string value = p_node.Attributes["ActionID"].Value;
			TriggerRunnerAction.InitActionVar(p_parent.ParentTrigger, ref _ActivationID, value);
			if (p_node.Attributes["AboveLevel"] != null)
			{
				TriggerRunnerAction.InitActionVar(p_parent.ParentTrigger, ref _AboveLevel, p_node.Attributes["AboveLevel"].Value);
			}
			if (p_node.Attributes["Dispatch"] != null)
			{
				TriggerRunnerAction.InitActionVar(p_parent.ParentTrigger, ref _DispatchLevel, p_node.Attributes["Dispatch"].Value);
			}
		}

		public override void Activate(ref bool p_isRunNext)
		{
			base.Activate(ref p_isRunNext);
			p_isRunNext = true;
			if (_AboveLevel == null)
			{
				ActivEvents(_ParentLoop.ParentTrigger.ParentElements.Parent, _ActivationID.ValueString);
				return;
			}
			switch (_DispatchLevel.ValueString)
			{
			case "Upward":
				ActivUpward();
				ActivEvents(_ParentLoop.ParentTrigger.ParentElements.Parent, _ActivationID.ValueString, false);
				break;
			case "Two-way":
				ActivUpward();
				ActivEvents(_ParentLoop.ParentTrigger.ParentElements.Parent, _ActivationID.ValueString);
				break;
			case "Full":
			{
				ObjectRunner objectRunner = GetObjectRunner();
				ActivEvents(objectRunner, _ActivationID.ValueString);
				break;
			}
			}
		}

		private void ActivUpward()
		{
			ObjectRunner parent = _ParentLoop.ParentTrigger.ParentElements.Parent;
			for (int i = 0; i < _AboveLevel.ValueInt; i++)
			{
				parent = parent.Parent;
				if (parent == null)
				{
					break;
				}
				if (parent.IsReference)
				{
					i--;
				}
				ActivEvents(parent, _ActivationID.ValueString, false);
			}
		}

		private ObjectRunner GetObjectRunner()
		{
			if (_AboveLevel == null)
			{
				return _ParentLoop.ParentTrigger.ParentElements.Parent;
			}
			return GetObjectRunner(_ParentLoop.ParentTrigger.ParentElements.Parent, _AboveLevel.ValueInt);
		}

		private ObjectRunner GetObjectRunner(ObjectRunner p_obj, int p_aboveLevel)
		{
			if (p_aboveLevel <= 0)
			{
				return p_obj;
			}
			ObjectRunner parent = p_obj.Parent;
			if (parent == null)
			{
				return p_obj;
			}
			return GetObjectRunner(parent, (!parent.IsReference) ? (p_aboveLevel - 1) : p_aboveLevel);
		}

		public static void ActivEvents(ObjectRunner p_object, string p_actionID, bool p_sendChild = true)
		{
			List<TriggerRunner> triggers = p_object.Element.Triggers;
			TRE_Activate p_event = new TRE_Activate(p_actionID);
			for (int i = 0; i < triggers.Count; i++)
			{
				triggers[i].CheckEvent(p_event, null);
			}
			if (p_sendChild)
			{
				List<ObjectRunner> childs = p_object.Childs;
				for (int j = 0; j < childs.Count; j++)
				{
					ActivEvents(childs[j], p_actionID);
				}
			}
		}

		public override TriggerRunnerAction Copy()
		{
			return new TRA_Activate(this);
		}

		public override string ToString()
		{
			return "Activate ID=" + _ActivationID.DebugStringValue + ((_AboveLevel != null) ? (" AboveLevel=" + _AboveLevel.ValueInt + " Dispatch=" + _DispatchLevel.ValueString) : string.Empty);
		}

		protected override void Log()
		{
			base.Log();
			VectorLog.RunLog("Action: Activate");
			VectorLog.Tab(1);
			VectorLog.RunLog("ActivationID", _ActivationID);
			VectorLog.RunLog("AboveLevel", _AboveLevel);
			VectorLog.Untab(1);
		}
	}
}
