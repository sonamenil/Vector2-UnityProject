using System.Xml;
using Nekki.Vector.Core.Variables;
using Nekki.Vector.GUI.Tutorial;
using UnityEngine;

namespace Nekki.Vector.Core.Trigger.Actions
{
	public class TRA_TutorialSequence : TriggerRunnerAction
	{
		private Variable _NameVar;

		public TRA_TutorialSequence(XmlNode p_node, TriggerRunnerLoop p_parent)
			: base(p_parent)
		{
			XmlAttribute xmlAttribute = p_node.Attributes["Name"];
			TriggerRunnerAction.InitActionVar(p_parent.ParentTrigger, ref _NameVar, xmlAttribute.Value);
			CreateTutorial();
		}

		private TRA_TutorialSequence(TRA_TutorialSequence p_copyAction)
			: base(p_copyAction)
		{
			_NameVar = p_copyAction._NameVar;
		}

		public override void Activate(ref bool p_isRunNext)
		{
			base.Activate(ref p_isRunNext);
			p_isRunNext = true;
			Tutorial.Current.Play(_NameVar.ValueString);
		}

		public override TriggerRunnerAction Copy()
		{
			return new TRA_TutorialSequence(this);
		}

		public override string ToString()
		{
			string text = "TutorialSequence";
			return text + " Name=" + _NameVar.DebugStringValue;
		}

		protected override void Log()
		{
			base.Log();
			VectorLog.RunLog("Action: TutorialSequence");
			VectorLog.Tab(1);
			VectorLog.RunLog("Name", _NameVar);
			VectorLog.Untab(1);
		}

		private void CreateTutorial()
		{
			if (Tutorial.Current == null)
			{
				GameObject gameObject = new GameObject("TutorialController");
				gameObject.AddComponent<Tutorial>();
			}
		}
	}
}
