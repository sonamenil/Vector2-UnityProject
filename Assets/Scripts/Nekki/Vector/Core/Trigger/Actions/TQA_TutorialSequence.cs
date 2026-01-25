using System.Xml;
using Nekki.Vector.GUI.Tutorial;
using UnityEngine;

namespace Nekki.Vector.Core.Trigger.Actions
{
	public class TQA_TutorialSequence : TriggerQuestAction
	{
		private string _SequenceName;

		public TQA_TutorialSequence(XmlNode p_node, TriggerQuestLoop p_parent)
			: base(p_parent)
		{
			_SequenceName = XmlUtils.ParseString(p_node.Attributes["Name"]);
			CreateTutorial();
		}

		public override void Activate(ref bool p_runNext)
		{
			p_runNext = true;
			Tutorial.Current.Play(_SequenceName);
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
