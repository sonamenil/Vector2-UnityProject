using System.Xml;
using Nekki.Vector.Core.Variables;

namespace Nekki.Vector.Core.Trigger.Actions
{
	public class TRA_AppendValue : TriggerRunnerAction
	{
		private Variable _SetVar;

		private Variable _ValueVar;

		private TRA_AppendValue(TRA_AppendValue p_copyAction)
			: base(p_copyAction)
		{
			_SetVar = p_copyAction._SetVar;
			_ValueVar = p_copyAction._ValueVar;
		}

		public TRA_AppendValue(XmlNode p_node, TriggerRunnerLoop p_parent)
			: base(p_parent)
		{
			_ValueVar = null;
			string value = p_node.Attributes["Name"].Value;
			string value2 = p_node.Attributes["Value"].Value;
			if (value2[0] == '_')
			{
				_ValueVar = _ParentLoop.GetParentVar(value2);
				if (_ValueVar == null)
				{
					DebugUtils.Dialog("Error create Action SetVariable not found" + value2, true);
				}
			}
			else
			{
				_ValueVar = Variable.CreateVariable(value2, string.Empty, p_parent.ParentTrigger);
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
				DebugUtils.Dialog("Error create Action SetVariable not found = " + value, true);
			}
		}

		public override void Activate(ref bool p_isRunNext)
		{
			base.Activate(ref p_isRunNext);
			p_isRunNext = true;
			switch (_ValueVar.Type)
			{
			case VariableType.Float:
				_SetVar.AppendValue(_ValueVar.ValueFloat);
				break;
			case VariableType.String:
				_SetVar.AppendValue(_ValueVar.ValueString);
				break;
			default:
				_SetVar.AppendValue(_ValueVar.ValueInt);
				break;
			}
		}

		public override TriggerRunnerAction Copy()
		{
			return new TRA_AppendValue(this);
		}

		public override string ToString()
		{
			return "AppendValue Var:" + _SetVar.ToString() + " Value:" + _ValueVar.DebugStringValue;
		}

		protected override void Log()
		{
			base.Log();
			VectorLog.RunLog("Action: AppendValue");
			VectorLog.Tab(1);
			VectorLog.RunLog("Set", _SetVar);
			VectorLog.RunLog("Value", _ValueVar);
			VectorLog.Untab(1);
		}
	}
}
