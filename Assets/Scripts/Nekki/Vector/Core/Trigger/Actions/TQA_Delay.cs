using System.Collections;
using System.Xml;
using Nekki.Vector.Core.Variables;
using Nekki.Vector.GUI.Dialogs;
using UnityEngine;

namespace Nekki.Vector.Core.Trigger.Actions
{
	public class TQA_Delay : TriggerQuestAction
	{
		private Variable _DelayTime;

		private bool _BlockAllTouches;

		public TQA_Delay(XmlNode p_node, TriggerQuestLoop p_parent)
			: base(p_parent)
		{
			_DelayTime = Variable.CreateVariable(XmlUtils.ParseString(p_node.Attributes["TimeInSeconds"]), "0");
			_BlockAllTouches = XmlUtils.ParseBool(p_node.Attributes["BlockAllTouches"]);
		}

		public override void Activate(ref bool p_isRunNext)
		{
			if (_DelayTime.ValueFloat == 0f)
			{
				p_isRunNext = true;
				return;
			}
			p_isRunNext = false;
			if (_BlockAllTouches)
			{
				DialogCanvasController.Current.BlockTouches();
			}
			CoroutineManager.Current.StartCoroutine(LaunchDelay());
		}

		private IEnumerator LaunchDelay()
		{
			yield return new WaitForSeconds(_DelayTime.ValueFloat);
			if (_BlockAllTouches)
			{
				DialogCanvasController.Current.UnBlockTouches();
			}
			_Parent.ActivateActions();
		}
	}
}
