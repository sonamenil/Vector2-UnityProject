using System.Xml;
using Nekki.Vector.Core.Variables;
using UnityEngine;

namespace Nekki.Vector.Core.Trigger.Actions
{
	public class TRA_SetModelParameter : TriggerRunnerAction
	{
		private Variable _ModelName;

		private Variable _ParamName;

		private Variable _Type;

		private Variable _Value;

		public TRA_SetModelParameter(TRA_SetModelParameter p_copy)
			: base(p_copy)
		{
			_ModelName = p_copy._ModelName;
			_ParamName = p_copy._ParamName;
			_Value = p_copy._Value;
		}

		public TRA_SetModelParameter(XmlNode p_node, TriggerRunnerLoop p_parent)
			: base(p_parent)
		{
			if (p_node.Attributes != null)
			{
				TriggerRunnerAction.InitActionVar(p_parent.ParentTrigger, ref _ModelName, p_node.Attributes["ModelName"].Value);
				TriggerRunnerAction.InitActionVar(p_parent.ParentTrigger, ref _ParamName, p_node.Attributes["ParamName"].Value);
				TriggerRunnerAction.InitActionVar(p_parent.ParentTrigger, ref _Type, p_node.Attributes["Type"].Value);
				TriggerRunnerAction.InitActionVar(p_parent.ParentTrigger, ref _Value, p_node.Attributes["Value"].Value);
			}
		}

		public override void Activate(ref bool p_isRunNext)
		{
			base.Activate(ref p_isRunNext);
			p_isRunNext = true;
			Animator[] componentsInChildren = _ParentLoop.ParentTrigger.UnityObject.transform.parent.GetComponentsInChildren<Animator>();
			Animator[] array = componentsInChildren;
			foreach (Animator animator in array)
			{
				if (animator != null && animator.name == _ModelName.ValueString)
				{
					switch (_Type.ValueString.ToLower())
					{
					case "bool":
						animator.SetBool(_ParamName.ValueString, _Value.ValueInt != 0);
						break;
					case "int":
						animator.SetInteger(_ParamName.ValueString, _Value.ValueInt);
						break;
					case "float":
						animator.SetFloat(_ParamName.ValueString, _Value.ValueFloat);
						break;
					}
				}
			}
		}

		public override TriggerRunnerAction Copy()
		{
			return new TRA_SetModelParameter(this);
		}

		public override string ToString()
		{
			return "SetModelParameter ModelName=" + _ModelName.ValueString;
		}

		protected override void Log()
		{
			base.Log();
			VectorLog.RunLog("Action: SetModelParameter");
			VectorLog.Tab(1);
			VectorLog.RunLog("ModelName", _ModelName);
			VectorLog.RunLog("ParamName", _ParamName);
			VectorLog.RunLog("Type", _Type);
			VectorLog.RunLog("Value", _Value);
			VectorLog.Untab(1);
		}
	}
}
