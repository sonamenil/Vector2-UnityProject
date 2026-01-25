using System.Collections.Generic;
using System.Xml;
using Nekki.Vector.Core.User;
using Nekki.Vector.Core.Variables;

namespace Nekki.Vector.Core.Trigger
{
	public class TriggerForEach
	{
		protected TriggerRunnerLoop _Parent;

		protected VariableItem _ObjectVar;

		protected Variable _InVariable;

		public TriggerForEach(XmlNode p_node, TriggerRunnerLoop p_parent)
		{
			_Parent = p_parent;
			Variable parentVar = _Parent.GetParentVar(XmlUtils.ParseString(p_node.Attributes["Variable"]));
			if (parentVar.Type == VariableType.Item)
			{
				_ObjectVar = parentVar as VariableItem;
				_InVariable = Variable.CreateVariable(XmlUtils.ParseString(p_node.Attributes["In"]), string.Empty, _Parent.ParentTrigger);
			}
		}

		public void ProcessEvent(TriggerEvent p_event)
		{
			HashSet<UserItem> hashSet = null;
			if (_InVariable == null)
			{
				hashSet = DataLocal.Current.AllItems;
			}
			else if (_InVariable.ValueString == "Stash")
			{
				hashSet = DataLocal.Current.Stash;
			}
			else if (_InVariable.ValueString == "Equipped")
			{
				hashSet = DataLocal.Current.Equipped;
			}
			VectorLog.Untab(1);
			VectorLog.RunLog("EXECUTE foreach Loop: " + _Parent.Name);
			VectorLog.Tab(1);
			foreach (UserItem item in hashSet)
			{
				_ObjectVar.Item = item;
				if (!_Parent.CheckEvent(p_event))
				{
					continue;
				}
				foreach (TriggerRunnerAction action in _Parent.Actions)
				{
					bool isRunNext = false;
					action.Activate(ref isRunNext);
				}
			}
			VectorLog.Untab(1);
			VectorLog.Tab(1);
		}
	}
}
