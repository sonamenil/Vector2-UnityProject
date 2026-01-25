using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using Nekki.Vector.Core.Counter;
using Nekki.Vector.GUI.Common;
using Nekki.Vector.GUI.Scenes.Terminal;
using UnityEngine;
using UnityEngine.UI;

namespace Nekki.Vector.GUI.Tutorial
{
	public class Tutorial : MonoBehaviour
	{
		[Serializable]
		public class TutorialSequence
		{
			public string CounterName = string.Empty;

			public string CounterNamespace = "Tutorial";

			public int Value = 1;

			public TextAsset Sequence;
		}

		[SerializeField]
		public bool _PlayOnStart;

		[SerializeField]
		public List<TutorialSequence> Sequences = new List<TutorialSequence>();

		[SerializeField]
		public ModuleHolder _Scene;

		private static string SequencesPath = "TutorialSequences/";

		private List<TutorialStep> _Steps = new List<TutorialStep>();

		private int _СurrentStep;

		private int _CurrentIndex;

		public Dictionary<string, GameObject> Pointers = new Dictionary<string, GameObject>();

		public static Tutorial Current;

		public bool Started { get; set; }

		public static event Action OnTutorialEnd;

		static Tutorial()
		{
			Tutorial.OnTutorialEnd = delegate
			{
			};
		}

		private IEnumerator Start()
		{
			Current = this;
			yield return new WaitForEndOfFrame();
			if (_PlayOnStart)
			{
				CheckCounters();
			}
		}

		public void CheckCounters()
		{
			for (int i = 0; i < Sequences.Count; i++)
			{
				TutorialSequence tutorialSequence = Sequences[i];
				if ((int)CounterController.Current.GetUserCounter(tutorialSequence.CounterName, tutorialSequence.CounterNamespace) == tutorialSequence.Value)
				{
					_CurrentIndex = i;
					Play(tutorialSequence.Sequence);
					break;
				}
			}
		}

		public void Play(string p_name)
		{
			TextAsset p_sequense = LoadSequence(p_name);
			Play(p_sequense);
		}

		public void Play(TextAsset p_sequense)
		{
			if ((bool)p_sequense)
			{
				Stop();
				Started = true;
				ParsSteps(p_sequense);
				_СurrentStep = 0;
				NextStep();
			}
		}

		public void Stop()
		{
			Started = false;
			ClearForks();
			_Steps.Clear();
			_СurrentStep = 0;
		}

		public void NextStep()
		{
			bool p_runNext = false;
			if (_Steps == null)
			{
				return;
			}
			if (_СurrentStep == _Steps.Count)
			{
				Tutorial.OnTutorialEnd();
			}
			for (int i = _СurrentStep; i < _Steps.Count; i++)
			{
				_СurrentStep = i;
				_Steps[i].Activate(ref p_runNext);
				if (!p_runNext)
				{
					return;
				}
			}
			Started = false;
		}

		public void Reload()
		{
			TutorialSequence tutorialSequence = Sequences[_CurrentIndex];
			if ((int)CounterController.Current.GetUserCounter(tutorialSequence.CounterName, tutorialSequence.CounterNamespace) == tutorialSequence.Value)
			{
				_СurrentStep = 0;
				NextStep();
			}
		}

		public void ClickButtonStepOver(TS_ClickButton p_step)
		{
			p_step.RemoveArrow();
			p_step.RemoveDelegate();
			StepOver();
		}

		public void ClickGadgetStepOver(TS_ClickGadget p_step)
		{
			p_step.RemoveArrow();
			p_step.RemoveDelegate();
			StepOver();
		}

		public void ClickCardStepOver(TS_ClickCard p_step)
		{
			p_step.RemoveArrow();
			p_step.RemoveDelegate();
			StepOver();
		}

		public void ClearForks()
		{
			for (int i = _СurrentStep; i < _Steps.Count; i++)
			{
				if (_Steps[i].TypeStep == TutorialStep.Type.Fork)
				{
					((TS_Fork)_Steps[i]).RemovePrefabAndDelegate();
				}
			}
		}

		public void StepOver()
		{
			_СurrentStep++;
			NextStep();
		}

		public void Fork(TS_Fork.TutorialWay p_way)
		{
			p_way.Parent.RemovePrefabAndDelegate();
			if (!p_way.CurrentSequence)
			{
				ParsSteps(p_way.Sequence);
			}
			_СurrentStep = p_way.StepIndex;
			NextStep();
		}

		private void Awake()
		{
			Current = this;
		}

		private TextAsset LoadSequence(string p_name)
		{
			return Resources.Load<TextAsset>(SequencesPath + p_name);
		}

		private void ParsSteps(TextAsset p_sequence)
		{
			_Steps.Clear();
			XmlDocument xmlDocument = XmlUtils.OpenXMLDocFromTextAsset(p_sequence);
			foreach (XmlNode childNode in xmlDocument["Steps"].ChildNodes)
			{
				TutorialStep tutorialStep = TutorialStep.Create(childNode);
				if (tutorialStep != null)
				{
					_Steps.Add(tutorialStep);
				}
			}
		}

		public List<Button> GetButtons(string p_module, string p_buttonName)
		{
			UIModule moduleByName = _Scene.GetModuleByName(p_module);
			if (moduleByName == null)
			{
				return null;
			}
			return moduleByName.GetButtonsByName(p_buttonName);
		}

		public GadgetUIPanel GetGadgetUIPanel()
		{
			return _Scene.GetComponentInChildren<GadgetUIPanel>(true);
		}

		public PlateScroller[] GetPlateScrollers(string p_module)
		{
			UIModule moduleByName = _Scene.GetModuleByName(p_module);
			if (moduleByName == null)
			{
				return null;
			}
			return moduleByName.GetComponentsInChildren<PlateScroller>();
		}

		public BaseCardUI[] GetCards()
		{
			Component component = null;
			if (Manager.Scene != SceneKind.Run)
			{
				component = _Scene.GetComponentInChildren<TerminalItemsPanel>(true);
			}
			if (component == null)
			{
				Debug.Log("[Tutorial]: GetCards could not find prefab with BaseCardsUI");
				return null;
			}
			return component.GetComponentsInChildren<BaseCardUI>();
		}
	}
}
