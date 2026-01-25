using System.Collections.Generic;
using System.Xml;
using Nekki.Vector.Core.Counter;
using Nekki.Vector.Core.GameManagement;
using Nekki.Vector.Core.Variables;
using UnityEngine;

namespace Nekki.Vector.Core.Runners
{
	public class Placeholder : Runner
	{
		public enum SortTypeEnum
		{
			None = 0,
			ByX = 1,
			ByXRevers = 2
		}

		private SortTypeEnum _SortType;

		private bool _IsDummy;

		private CounterActions _CounterActions;

		private StringActions _StringActions;

		private int _CounterRoomNumber;

		private int _CounterRoomNumberReversed;

		private int _CounterIterationCount;

		public SortTypeEnum SortType
		{
			get
			{
				return _SortType;
			}
		}

		public bool IsSorted
		{
			get
			{
				return _SortType != SortTypeEnum.None;
			}
		}

		public bool IsDummy
		{
			get
			{
				return _IsDummy;
			}
		}

		public override Vector3 LocalPosition
		{
			get
			{
				return new Vector3(_DefautPosition.X, _DefautPosition.Y);
			}
		}

		public Placeholder(float p_x, float p_y, Element p_elements, XmlNode p_node)
			: base(p_x, p_y, p_elements)
		{
			XmlNode xmlNode = p_node["Properties"];
			_CounterActions = CounterActions.Create((xmlNode != null) ? xmlNode["CounterActions"] : null, "ST_Default");
			_StringActions = StringActions.Create((xmlNode != null) ? xmlNode["StringActions"] : null);
			_CounterRoomNumber = CounterController.Current.CounterRoomNumber;
			_CounterRoomNumberReversed = CounterController.Current.CounterRoomNumberReversed;
			ParseSortedType(p_node.Attributes["Sorted"]);
			_IsDummy = XmlUtils.ParseBool(p_node.Attributes["Dummy"]);
		}

		private void ParseSortedType(XmlAttribute p_attrType)
		{
			if (p_attrType != null)
			{
				_SortType = ParseSortedType(XmlUtils.ParseString(p_attrType));
			}
		}

		private SortTypeEnum ParseSortedType(string p_type)
		{
			switch (p_type)
			{
			case "ByX":
				return SortTypeEnum.ByX;
			case "ByXReverse":
				return SortTypeEnum.ByXRevers;
			default:
				return SortTypeEnum.None;
			}
		}

		public override bool Render()
		{
			return true;
		}

		public void PostProcessDummy()
		{
			_CounterIterationCount++;
			CounterController.Current.CounterRoomNumber = _CounterRoomNumber;
			CounterController.Current.CounterRoomNumberReversed = _CounterRoomNumberReversed;
			CounterController.Current.CounterPlaceholderIterationCount = _CounterIterationCount;
			ObjectRunner.CurrentLocalNamespace = ParentElements.Parent.GetCurrentLocalNamespace();
			ActivateOnEnterActions();
			ActivateOnExitActions();
			DestroyUnityObjects();
		}

		public void PostProcess(Dictionary<string, string> p_ChoisesDictionary, ref bool p_postpone)
		{
			_CounterIterationCount++;
			CounterController.Current.CounterRoomNumber = _CounterRoomNumber;
			CounterController.Current.CounterRoomNumberReversed = _CounterRoomNumberReversed;
			CounterController.Current.CounterPlaceholderIterationCount = _CounterIterationCount;
			ObjectRunner.CurrentLocalNamespace = ParentElements.Parent.GetCurrentLocalNamespace();
			ActivateOnEnterActions();
			ReplacementData p_replacement = null;
			Dictionary<string, Variable> p_objectParams = null;
			if (!ZoneResource<Nekki.Vector.Core.GameManagement.PostProcess>.Current.GetPostProcessData(ref p_replacement, ref p_objectParams, ref p_postpone))
			{
				ActivateOnExitActions();
				DestroyUnityObjects();
				return;
			}
			XmlNode xmlNodeFromPreset = Sets.ObjectNode(p_replacement.Name.ValueString, p_replacement.Filename.ValueString).GetXmlNodeFromPreset(p_objectParams);
			ObjectRunner objectRunner = new ObjectRunner();
			objectRunner.Parse(xmlNodeFromPreset, p_ChoisesDictionary);
			objectRunner.SetPositionByPlaceholder(this);
			ParentElements.Parent.AddChild(objectRunner);
			ActivateOnExitActions();
			DestroyUnityObjects();
		}

		private void ActivateOnEnterActions()
		{
			if (_CounterActions != null)
			{
				_CounterActions.ActivateOnEnter();
			}
			if (_StringActions != null)
			{
				_StringActions.ActivateOnEnter();
			}
		}

		private void ActivateOnExitActions()
		{
			if (_CounterActions != null)
			{
				_CounterActions.ActivateOnExit();
			}
			if (_StringActions != null)
			{
				_StringActions.ActivateOnExit();
			}
		}
	}
}
