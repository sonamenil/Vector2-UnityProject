using System.Xml;
using Nekki.Vector.Core.Variables;

namespace Nekki.Vector.Core.Trigger.Events
{
	public class TRE_ChangeVar : TriggerRunnerEvent
	{
		private bool _IsInit;

		private Variable _Var;

		private int _OldValue;

		public TRE_ChangeVar(XmlNode p_node)
		{
			_Type = EventType.TRE_VAR_CHANGE;
			_Var = Variable.CreateVariable(p_node.Attributes["Value"].Value, string.Empty);
		}

		private void Init()
		{
			_IsInit = true;
			_OldValue = _Var.ValueInt;
		}

		public void Reset()
		{
			_IsInit = false;
		}

		public bool IsChange()
		{
			if (!_IsInit)
			{
				Init();
				return false;
			}
			if (_Parent.ParentTrigger.IsActive)
			{
				return _OldValue != _Var.ValueInt;
			}
			return false;
		}
	}
}
