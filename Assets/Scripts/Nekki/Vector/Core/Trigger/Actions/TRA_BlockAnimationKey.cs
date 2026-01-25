using System.Xml;
using Nekki.Vector.Core.Controllers;
using Nekki.Vector.Core.Models;
using Nekki.Vector.Core.Variables;

namespace Nekki.Vector.Core.Trigger.Actions
{
	public class TRA_BlockAnimationKey : TriggerRunnerAction
	{
		private Variable _Key;

		private Variable _Value;

		private TRA_BlockAnimationKey(TRA_BlockAnimationKey p_copyAction)
			: base(p_copyAction)
		{
			_Key = p_copyAction._Key;
			_Value = p_copyAction._Value;
		}

		public TRA_BlockAnimationKey(XmlNode p_node, TriggerRunnerLoop p_parent)
			: base(p_parent)
		{
			string value = p_node.Attributes["Key"].Value;
			string value2 = p_node.Attributes["Value"].Value;
			TriggerRunnerAction.InitActionVar(p_parent.ParentTrigger, ref _Value, value2);
			TriggerRunnerAction.InitActionVar(p_parent.ParentTrigger, ref _Key, value);
		}

		public override void Activate(ref bool p_isRunNext)
		{
			base.Activate(ref p_isRunNext);
			p_isRunNext = true;
			ModelHuman model = GetModel("Player");
			Key key = KeyVariables.Parse(_Key.ValueString);
			if (key != Key.None)
			{
				model.ControllerAnimation.BlockKey(key, _Value.ValueInt == 1);
			}
		}

		public override TriggerRunnerAction Copy()
		{
			return new TRA_BlockAnimationKey(this);
		}

		public override string ToString()
		{
			return "BlockAnimationKey: Key=" + _Key.DebugStringValue + " Value:" + _Value.DebugStringValue;
		}

		protected override void Log()
		{
			base.Log();
			VectorLog.RunLog("Action: BlockAnimationKey");
			VectorLog.Tab(1);
			VectorLog.RunLog("Key", _Key);
			VectorLog.RunLog("Value", _Value);
			VectorLog.Untab(1);
		}
	}
}
