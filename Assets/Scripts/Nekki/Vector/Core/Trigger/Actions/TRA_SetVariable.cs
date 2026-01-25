using System.Xml;
using Nekki.Vector.Core.Variables;
using Nekki.Vector.Core.Variables.Functions;

namespace Nekki.Vector.Core.Trigger.Actions
{
	public class TRA_SetVariable : TriggerRunnerAction
	{
		private Variable _SetVar;

		private Variable _ValueVar;

		private bool _IsSetFuncVar;

		private TRA_SetVariable(TRA_SetVariable p_copyAction)
			: base(p_copyAction)
		{
			_SetVar = p_copyAction._SetVar;
			_ValueVar = p_copyAction._ValueVar;
		}

		public TRA_SetVariable(XmlNode p_node, TriggerRunnerLoop p_parent)
			: base(p_parent)
		{
			string value = p_node.Attributes["Name"].Value;
			string value2 = p_node.Attributes["Value"].Value;
			if (value2[0] == '_')
			{
				_ValueVar = _ParentLoop.GetParentVar(value2);
				if (_ValueVar == null)
				{
					DebugUtils.Dialog("Error create Action SetVariable no found : " + value2, true);
				}
			}
			else
			{
				_ValueVar = Variable.CreateVariable(value2, string.Empty, p_parent.ParentTrigger);
				if (_ValueVar.Type == VariableType.Function)
				{
					_IsSetFuncVar = (_ValueVar as VariableFunction).IsPointer;
				}
			}
			if (value[0] == '?')
			{
				_SetVar = Variable.CreateVariable(value, string.Empty, p_parent.ParentTrigger);
			}
			else
			{
				_SetVar = _ParentLoop.GetParentVar("_" + value);
			}
			if (_SetVar == null)
			{
				DebugUtils.Dialog("Error create Action SetVariable not found =" + value, true);
			}
		}

		public override void Activate(ref bool p_isRunNext)
		{
			base.Activate(ref p_isRunNext);
			p_isRunNext = true;
			if (_IsSetFuncVar)
			{
				SetPointerVar();
			}
			else
			{
				SetVar();
			}
		}

		private void SetVar()
		{
			switch (_ValueVar.Type)
			{
			case VariableType.Float:
			case VariableType.Function:
			case VariableType.Expression:
				_SetVar.SetValue(_ValueVar.ValueFloat);
				break;
			case VariableType.String:
				_SetVar.SetValue(_ValueVar.ValueString);
				break;
			default:
				_SetVar.SetValue(_ValueVar.ValueInt);
				break;
			}
		}

		private void SetPointerVar()
		{
			if (_SetVar.Type == VariableType.Function)
			{
				VF_GetItemAsGroup vF_GetItemAsGroup = _SetVar as VF_GetItemAsGroup;
				if (vF_GetItemAsGroup != null)
				{
					vF_GetItemAsGroup.setVariableToItem(_ValueVar);
				}
				else
				{
					DebugUtils.Dialog("Error on work Action SetVariable: SetVar != TF_GetItemAsGroup", true);
				}
			}
			else
			{
				_ParentLoop.ParentTrigger.ChangeVarByName(_SetVar.Name, _ValueVar);
			}
		}

		public override TriggerRunnerAction Copy()
		{
			return new TRA_SetVariable(this);
		}

		public override string ToString()
		{
			return "SetVariable Var:" + _SetVar.ToString() + " Value=" + _ValueVar.DebugStringValue;
		}

		protected override void Log()
		{
			base.Log();
			VectorLog.RunLog("Action: SetVariable");
			VectorLog.Tab(1);
			VectorLog.RunLog("Set", _SetVar);
			VectorLog.RunLog("Value", _ValueVar);
			VectorLog.Untab(1);
		}
	}
}
