using System.Collections.Generic;
using System.Xml;
using Nekki.Vector.Core.Counter;
using Nekki.Vector.Core.Runners;

namespace Nekki.Vector.Core.Generator
{
	public class Room
	{
		public string Name;

		public string File;

		public string Id;

		public List<Variant> Variants;

		public XmlNode TmpNode;

		public ObjectRunner Object;

		protected GateRunner _In;

		protected GateRunner _Out;

		protected List<GateRunner> _Ins;

		protected List<GateRunner> _Outs;

		public CounterActions CounterActions;

		protected List<Choice> _Choices;

		public List<GeneratorLabel> GeneratorLabels;

		public bool _IsIncludeInPlayCommand;

		public string UniqueName
		{
			get
			{
				return Name + "_" + Id;
			}
		}

		public GateRunner CurrentIn
		{
			get
			{
				return _In;
			}
			set
			{
				_In = value;
			}
		}

		public GateRunner CurrentOut
		{
			get
			{
				return _Out;
			}
			set
			{
				_Out = value;
			}
		}

		public List<GateRunner> Ins
		{
			get
			{
				return _Ins;
			}
		}

		public List<GateRunner> Outs
		{
			get
			{
				return _Outs;
			}
		}

		public bool IsIncludeInPlayCommand
		{
			get
			{
				return _IsIncludeInPlayCommand;
			}
		}

		public Room(RoomData p_room, List<Variant> p_variants)
		{
			Name = p_room.Name;
			File = p_room.File;
			Id = LocationGenerator.GetGeneratedRoomId();
			_Choices = p_room.Choices;
			GeneratorLabels = p_room.GeneratorLabels;
			Variants = p_variants;
			_IsIncludeInPlayCommand = p_room.IsIncludeInPlayCommand;
		}

		public Point SwitchOutAndGetDelta(string p_name)
		{
			GateRunner gateRunner = null;
			foreach (GateRunner @out in _Outs)
			{
				if (@out.Name == p_name)
				{
					gateRunner = @out;
					break;
				}
			}
			if (gateRunner == null)
			{
				DebugUtils.Dialog("Out Name:" + p_name + " not exist", true);
				return null;
			}
			Point result = new Point(gateRunner.Position.x - _Out.Position.x, gateRunner.Position.y - _Out.Position.y);
			_Out = gateRunner;
			return result;
		}

		public void AddChoices(Dictionary<string, string> p_choices)
		{
			if (Variants != null)
			{
				SetPrefix();
				for (int i = 0; i < Variants.Count; i++)
				{
					Variants[i].AddToDictionary(p_choices);
				}
			}
		}

		private void SetPrefix()
		{
			foreach (Choice choice in _Choices)
			{
				choice.Prefix = UniqueName;
			}
		}

		public void CollectGates()
		{
			_Ins = new List<GateRunner>();
			_Outs = new List<GateRunner>();
			CollectGates(Object);
		}

		private void CollectGates(ObjectRunner p_object)
		{
			_Ins.AddRange(p_object.Element.Ins);
			_Outs.AddRange(p_object.Element.Outs);
			foreach (ObjectRunner child in p_object.Childs)
			{
				CollectGates(child);
			}
		}

		public WaypointRunner GetWaypointByName(string p_name)
		{
			if (Object == null)
			{
				return null;
			}
			return ObjectRunner.GetWaypointByName(p_name, Object);
		}

		public void SaveToXMLTest(XmlElement p_root)
		{
			XmlElement xmlElement = p_root.OwnerDocument.CreateElement("Room");
			xmlElement.SetAttribute("Name", Name);
			xmlElement.SetAttribute("ChoiceCount", (_Choices != null) ? _Choices.Count.ToString() : "0");
			p_root.AppendChild(xmlElement);
			if (Variants != null)
			{
				for (int i = 0; i < Variants.Count; i++)
				{
					XmlElement xmlElement2 = p_root.OwnerDocument.CreateElement("Variant");
					xmlElement.AppendChild(xmlElement2);
					xmlElement2.SetAttribute("Name", Variants[i]._Name);
					xmlElement2.SetAttribute("Choice", Variants[i].Parent.ChoiceName);
					xmlElement2.SetAttribute("ChildChoiceCount", Variants[i].ChildChoice.Count.ToString());
				}
			}
		}
	}
}
