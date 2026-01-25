using System.Collections.Generic;
using System.Xml;
using Nekki.Vector.Core.Models;
using Nekki.Vector.Core.Runners;
using Nekki.Vector.Core.Trigger.Actions;
using Nekki.Vector.Core.Variables;

namespace Nekki.Vector.Core.Trigger
{
	public abstract class TriggerRunnerAction
	{
		protected static Dictionary<string, string> newNamesVars;

		protected TriggerRunnerLoop _ParentLoop;

		protected bool _IsLastAction = true;

		public bool IsLastAction
		{
			set
			{
				_IsLastAction = value;
			}
		}

		public bool IsParentEnable
		{
			get
			{
				return _ParentLoop.ParentTrigger.IsEnabled;
			}
		}

		public virtual int Frames
		{
			get
			{
				return 0;
			}
		}

		protected TriggerRunnerAction(TriggerRunnerLoop p_parent)
		{
			_ParentLoop = p_parent;
		}

		protected TriggerRunnerAction(TriggerRunnerAction p_copy)
		{
			_ParentLoop = p_copy._ParentLoop;
			_IsLastAction = p_copy._IsLastAction;
		}

		public static TriggerRunnerAction Create(XmlNode p_node, TriggerRunnerLoop p_parent, Dictionary<string, string> p_prefix)
		{
			newNamesVars = p_prefix;
			switch (p_node.Name)
			{
			case "SoundSource":
				return new TRA_SoundSourceOn(p_node, p_parent);
			case "Camera":
				return new TRA_Camera(p_node, p_parent);
			case "Wait":
				return new TRA_Wait(p_node, p_parent);
			case "SetVariable":
				return new TRA_SetVariable(p_node, p_parent);
			case "AppendValue":
				return new TRA_AppendValue(p_node, p_parent);
			case "Press":
				return new TRA_Press(p_node, p_parent);
			case "ForceAnimation":
				return new TRA_ForceAnimation(p_node, p_parent);
			case "Control":
				return new TRA_Control(p_node, p_parent);
			case "EndGame":
				return new TRA_EndGame(p_node, p_parent);
			case "SetTimer":
				return new TRA_SetTimer(p_node, p_parent);
			case "Spawn":
				return new TRA_Spawn(p_node, p_parent);
			case "Transform":
				return new TRA_Transformation(p_node, p_parent, true);
			case "Choose":
				return new TRA_Choose(p_node, p_parent);
			case "Activate":
				return new TRA_Activate(p_node, p_parent);
			case "ModelExecute":
				return new TRA_ModelExecute(p_node, p_parent);
			case "Kill":
				return new TRA_Kill(p_node, p_parent);
			case "AddItem":
				return new TRA_AddItem(p_node, p_parent);
			case "Sound":
				return new TRA_Sound(p_node, p_parent);
			case "Music":
				return new TRA_Music(p_node, p_parent);
			case "MakeRoom":
				return new TRA_MakeRoom(p_node, p_parent);
			case "ChangeExit":
				return new TRA_ChangeExit(p_node, p_parent);
			case "Impulse":
				return new TRA_Impulse(p_node, p_parent);
			case "RunModelEffect":
				return new TRA_RunModelEffect(p_node, p_parent);
			case "Tutorial":
				return new TRA_Tutorial(p_node, p_parent);
			case "SetModelParameter":
				return new TRA_SetModelParameter(p_node, p_parent);
			case "FloatingText":
				return new TRA_FloatingText(p_node, p_parent);
			case "TutorialSequence":
				return new TRA_TutorialSequence(p_node, p_parent);
			case "GlobalTimer":
				return new TRA_GlobalTimer(p_node, p_parent);
			case "ExecuteCall":
				return new TRA_ExecuteCall(p_node, p_parent);
			case "Statistics":
				return new TRA_Statistics(p_node, p_parent);
			case "GUI":
				return new TRA_GUI(p_node, p_parent);
			case "Swarm":
				return new TRA_Swarm(p_node, p_parent);
			case "Chapter":
				return new TRA_Chapter(p_node, p_parent);
			case "ActivatePassiveEffect":
				return new TRA_ActivatePassiveEffect(p_node, p_parent);
			case "ActivateNearPlayer":
				return new TRA_ActivateNearPlayer(p_node, p_parent);
			case "BlockAnimationKey":
				return new TRA_BlockAnimationKey(p_node, p_parent);
			case "ShowLog":
				return null;
			default:
				return null;
			}
		}

		public static void Parse(XmlNode p_node, TriggerRunnerLoop p_loop, List<TriggerRunnerAction> p_actions, string p_prefix = null)
		{
			if (p_node == null)
			{
				return;
			}
			XmlAttribute xmlAttribute = p_node.Attributes["Template"];
			if (xmlAttribute != null)
			{
				Parse(TemplateModule.getTemplateActionsXML(xmlAttribute.Value), p_loop, p_actions);
				return;
			}
			foreach (XmlNode childNode in p_node.ChildNodes)
			{
				if (childNode.LocalName.Equals("#comment"))
				{
					continue;
				}
				if (childNode.LocalName.Equals("ActionBlock"))
				{
					string value = childNode.Attributes["Template"].Value;
					XmlNode templateActionsXML = TemplateModule.getTemplateActionsXML(value);
					string p_prefix2 = XmlUtils.ParseString(childNode.Attributes["Prefix"]);
					Parse(templateActionsXML, p_loop, p_actions, p_prefix2);
					continue;
				}
				Dictionary<string, string> dictionary = new Dictionary<string, string>();
				if (p_prefix != null)
				{
					XmlNode xmlNode2 = p_node.ParentNode["Using"];
					foreach (XmlNode childNode2 in xmlNode2.ChildNodes)
					{
						if (childNode2.Attributes["ComplexName"] != null)
						{
							string value2 = childNode2.Attributes["Name"].Value;
							string value3 = p_prefix + value2;
							dictionary[value2] = value3;
						}
					}
				}
				Dictionary<string, string> dictionary2 = newNamesVars;
				TriggerRunnerAction triggerRunnerAction = Create(childNode, p_loop, dictionary);
				if (triggerRunnerAction != null)
				{
					p_actions.Add(triggerRunnerAction);
				}
				newNamesVars = dictionary2;
			}
		}

		public abstract TriggerRunnerAction Copy();

		public virtual void Activate(ref bool isRunNext)
		{
			Log();
		}

		protected virtual void Log()
		{
		}

		public ModelHuman GetModel()
		{
			return RunMainController.Location.GetUserModel();
		}

		public ModelHuman GetModel(string p_modelName)
		{
			ModelHuman result = null;
			if (p_modelName.Length != 0 && p_modelName[0] == '_')
			{
				Variable parentVar = _ParentLoop.GetParentVar(p_modelName);
				if (parentVar != null)
				{
					result = RunMainController.Location.GetModelByName(parentVar.ValueString);
				}
			}
			else
			{
				result = RunMainController.Location.GetModelByName(p_modelName);
			}
			return result;
		}

		private static Variable GetTriggerVar(TriggerRunner p_parent, string p_name)
		{
			string text = p_name.Substring(1);
			string value = string.Empty;
			if (newNamesVars.TryGetValue(text, out value))
			{
				text = value;
			}
			return p_parent.GetVariable("_" + text);
		}

		protected static void InitActionVar(TriggerRunner p_parent, ref Variable p_var, string p_nameOrValue)
		{
			if (!string.IsNullOrEmpty(p_nameOrValue) && p_nameOrValue[0] == '_')
			{
				p_var = GetTriggerVar(p_parent, p_nameOrValue);
			}
			else
			{
				p_var = Variable.CreateVariable(p_nameOrValue, string.Empty, p_parent);
			}
		}

		public override string ToString()
		{
			return "Unknown TriggerAction";
		}
	}
}
