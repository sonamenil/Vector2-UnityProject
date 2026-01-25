using System.Collections.Generic;
using System.Xml;
using Nekki.Vector.Core.Utilites;
using Nekki.Vector.Core.Variables;
using Nekki.Vector.GUI;
using Nekki.Vector.GUI.Scenes.Run;
using UnityEngine;

namespace Nekki.Vector.Core.Trigger.Actions
{
	public class TRA_FloatingText : TriggerRunnerAction
	{
		private FloatingText _UI;

		private Variable _TextVar;

		private Variable _ColorVar;

		private Variable _SizeVar;

		private Variable _IsItalicVar;

		private Variable _DelayVar;

		private Variable _FramesVar;

		private Variable _ShiftXVar;

		private Variable _ShiftYVar;

		private Variable _ImageVar;

		private Variable _ImageAlignVar;

		private Variable _ImageWidthVar;

		private Variable _ImageHeightVar;

		private Variable _OriginTypeVar;

		private Variable _OffsetXVar;

		private Variable _OffsetYVar;

		private Variable _AnchorItem;

		private Variable _ImageColor;

		private Variable _BorderColor;

		private List<Variable> _VarsInText;

		public TRA_FloatingText(XmlNode p_node, TriggerRunnerLoop p_parent)
			: base(p_parent)
		{
			XmlAttribute xmlAttribute = p_node.Attributes["Text"];
			XmlAttribute xmlAttribute2 = p_node.Attributes["Vars"];
			XmlAttribute xmlAttribute3 = p_node.Attributes["Color"];
			XmlAttribute xmlAttribute4 = p_node.Attributes["Size"];
			XmlAttribute xmlAttribute5 = p_node.Attributes["IsItalic"];
			XmlAttribute xmlAttribute6 = p_node.Attributes["Frames"];
			XmlAttribute xmlAttribute7 = p_node.Attributes["Delay"];
			XmlAttribute xmlAttribute8 = p_node.Attributes["ShiftX"];
			XmlAttribute xmlAttribute9 = p_node.Attributes["ShiftY"];
			XmlAttribute xmlAttribute10 = p_node.Attributes["Image"];
			XmlAttribute xmlAttribute11 = p_node.Attributes["ImageAlign"];
			XmlAttribute xmlAttribute12 = p_node.Attributes["ImageWidth"];
			XmlAttribute xmlAttribute13 = p_node.Attributes["ImageHeight"];
			XmlAttribute xmlAttribute14 = p_node.Attributes["OriginType"];
			XmlAttribute xmlAttribute15 = p_node.Attributes["OffsetX"];
			XmlAttribute xmlAttribute16 = p_node.Attributes["OffsetY"];
			XmlAttribute xmlAttribute17 = p_node.Attributes["AnchorItem"];
			XmlAttribute xmlAttribute18 = p_node.Attributes["ImageColor"];
			XmlAttribute xmlAttribute19 = p_node.Attributes["BorderColor"];
			if (xmlAttribute != null)
			{
				string p_nameOrValue = xmlAttribute.Value.Replace("(nl)", "\n");
				TriggerRunnerAction.InitActionVar(p_parent.ParentTrigger, ref _TextVar, p_nameOrValue);
			}
			if (xmlAttribute2 != null)
			{
				string text = xmlAttribute2.Value.Replace(" ", string.Empty);
				string[] array = text.Split('|');
				_VarsInText = new List<Variable>();
				Variable p_var = null;
				int i = 0;
				for (int num = array.Length; i < num; i++)
				{
					TriggerRunnerAction.InitActionVar(p_parent.ParentTrigger, ref p_var, array[i]);
					_VarsInText.Add(p_var);
				}
			}
			if (xmlAttribute3 != null)
			{
				TriggerRunnerAction.InitActionVar(p_parent.ParentTrigger, ref _ColorVar, xmlAttribute3.Value);
			}
			if (xmlAttribute4 != null)
			{
				TriggerRunnerAction.InitActionVar(p_parent.ParentTrigger, ref _SizeVar, xmlAttribute4.Value);
			}
			if (xmlAttribute5 != null)
			{
				TriggerRunnerAction.InitActionVar(p_parent.ParentTrigger, ref _IsItalicVar, xmlAttribute5.Value);
			}
			if (xmlAttribute6 != null)
			{
				TriggerRunnerAction.InitActionVar(p_parent.ParentTrigger, ref _FramesVar, xmlAttribute6.Value);
			}
			if (xmlAttribute7 != null)
			{
				TriggerRunnerAction.InitActionVar(p_parent.ParentTrigger, ref _DelayVar, xmlAttribute7.Value);
			}
			if (xmlAttribute8 != null)
			{
				TriggerRunnerAction.InitActionVar(p_parent.ParentTrigger, ref _ShiftXVar, xmlAttribute8.Value);
			}
			if (xmlAttribute9 != null)
			{
				TriggerRunnerAction.InitActionVar(p_parent.ParentTrigger, ref _ShiftYVar, xmlAttribute9.Value);
			}
			if (xmlAttribute10 != null)
			{
				TriggerRunnerAction.InitActionVar(p_parent.ParentTrigger, ref _ImageVar, xmlAttribute10.Value);
			}
			if (xmlAttribute11 != null)
			{
				TriggerRunnerAction.InitActionVar(p_parent.ParentTrigger, ref _ImageAlignVar, xmlAttribute11.Value);
			}
			if (xmlAttribute12 != null)
			{
				TriggerRunnerAction.InitActionVar(p_parent.ParentTrigger, ref _ImageWidthVar, xmlAttribute12.Value);
			}
			if (xmlAttribute13 != null)
			{
				TriggerRunnerAction.InitActionVar(p_parent.ParentTrigger, ref _ImageHeightVar, xmlAttribute13.Value);
			}
			if (xmlAttribute14 != null)
			{
				TriggerRunnerAction.InitActionVar(p_parent.ParentTrigger, ref _OriginTypeVar, xmlAttribute14.Value);
			}
			else
			{
				TriggerRunnerAction.InitActionVar(p_parent.ParentTrigger, ref _OriginTypeVar, "Local");
			}
			if (xmlAttribute15 != null)
			{
				TriggerRunnerAction.InitActionVar(p_parent.ParentTrigger, ref _OffsetXVar, xmlAttribute15.Value);
			}
			if (xmlAttribute16 != null)
			{
				TriggerRunnerAction.InitActionVar(p_parent.ParentTrigger, ref _OffsetYVar, xmlAttribute16.Value);
			}
			if (xmlAttribute17 != null)
			{
				_AnchorItem = p_parent.GetParentVar(p_node.Attributes["AnchorItem"].Value);
			}
			if (xmlAttribute18 != null)
			{
				TriggerRunnerAction.InitActionVar(p_parent.ParentTrigger, ref _ImageColor, xmlAttribute18.Value);
			}
			if (xmlAttribute19 != null)
			{
				TriggerRunnerAction.InitActionVar(p_parent.ParentTrigger, ref _BorderColor, xmlAttribute19.Value);
			}
			CreateUI();
		}

		private TRA_FloatingText(TRA_FloatingText p_copyAction)
			: base(p_copyAction)
		{
			_TextVar = p_copyAction._TextVar;
			_VarsInText = p_copyAction._VarsInText;
			_ColorVar = p_copyAction._ColorVar;
			_IsItalicVar = p_copyAction._IsItalicVar;
			_SizeVar = p_copyAction._SizeVar;
			_FramesVar = p_copyAction._FramesVar;
			_DelayVar = p_copyAction._DelayVar;
			_ShiftXVar = p_copyAction._ShiftXVar;
			_ShiftYVar = p_copyAction._ShiftYVar;
			_ImageVar = p_copyAction._ImageVar;
			_ImageAlignVar = p_copyAction._ImageAlignVar;
			_ImageWidthVar = p_copyAction._ImageWidthVar;
			_ImageHeightVar = p_copyAction._ImageHeightVar;
			_OriginTypeVar = p_copyAction._OriginTypeVar;
			_OffsetXVar = p_copyAction._OffsetXVar;
			_OffsetYVar = p_copyAction._OffsetYVar;
			_AnchorItem = p_copyAction._AnchorItem;
			_ImageColor = p_copyAction._ImageColor;
			_BorderColor = p_copyAction._BorderColor;
		}

		public override void Activate(ref bool p_isRunNext)
		{
			base.Activate(ref p_isRunNext);
			p_isRunNext = true;
			string textString = GetTextString();
			if (_UI.Text != textString)
			{
				_UI.Text = GetTextString();
			}
			if (_BorderColor != null)
			{
				_UI.BorderColor = ColorUtils.FromHex(_BorderColor.ValueString);
			}
			if (_ImageColor != null)
			{
				_UI.ImageColor = GetColor(_ImageColor.ValueString);
			}
			if (_ColorVar != null)
			{
				_UI.FontColor = ColorUtils.FromHex(_ColorVar.ValueString);
			}
			if (_IsItalicVar != null)
			{
				_UI.isItalic = ((_IsItalicVar.ValueString == "1") ? FontStyle.Italic : FontStyle.Normal);
			}
			if (_SizeVar != null)
			{
				_UI.FontSize = _SizeVar.ValueInt;
			}
			if (_FramesVar != null)
			{
				_UI.Time = (float)_FramesVar.ValueInt / (float)Application.targetFrameRate;
			}
			if (_DelayVar != null)
			{
				_UI.Delay = (float)_DelayVar.ValueInt / (float)Application.targetFrameRate;
			}
			if (_ShiftXVar != null || _ShiftYVar != null)
			{
				_UI.Shift = GetShift();
			}
			if (_ImageVar != null)
			{
				_UI.Image = _ImageVar.ValueString;
			}
			if (_ImageAlignVar != null)
			{
				_UI.ImageAlign = ((!(_ImageAlignVar.ValueString == "Left")) ? FT_ImageAlign.Right : FT_ImageAlign.Left);
			}
			if (_ImageWidthVar != null)
			{
				_UI.ImageWidth = _ImageWidthVar.ValueInt;
			}
			if (_ImageHeightVar != null)
			{
				_UI.ImageHeight = _ImageHeightVar.ValueInt;
			}
			if (_OriginTypeVar != null)
			{
				_UI.Position = GetPosition();
			}
			_UI.Play();
		}

		public override TriggerRunnerAction Copy()
		{
			return new TRA_FloatingText(this);
		}

		public override string ToString()
		{
			string text = "FloatingText";
			if (_TextVar != null)
			{
				text = text + " Text=" + _TextVar.DebugStringValue;
			}
			if (_ColorVar != null)
			{
				text = text + " Color=" + _ColorVar.DebugStringValue;
			}
			if (_SizeVar != null)
			{
				text = text + " Size=" + _SizeVar.DebugStringValue;
			}
			if (_FramesVar != null)
			{
				text = text + " Frames=" + _FramesVar.DebugStringValue;
			}
			if (_DelayVar != null)
			{
				text = text + " Delay=" + _DelayVar.DebugStringValue;
			}
			if (_ShiftXVar != null)
			{
				text = text + " ShiftX=" + _ShiftXVar.DebugStringValue;
			}
			if (_ShiftYVar != null)
			{
				text = text + " ShiftY=" + _ShiftYVar.DebugStringValue;
			}
			if (_ImageVar != null)
			{
				text = text + " Image=" + _ImageVar.DebugStringValue;
			}
			if (_ImageAlignVar != null)
			{
				text = text + " ImageAlign=" + _ImageAlignVar.DebugStringValue;
			}
			if (_ImageWidthVar != null)
			{
				text = text + " ImageWidth=" + _ImageWidthVar.DebugStringValue;
			}
			if (_ImageHeightVar != null)
			{
				text = text + " ImageHeight=" + _ImageHeightVar.DebugStringValue;
			}
			if (_OriginTypeVar != null)
			{
				text = text + "OriginType=" + _OriginTypeVar.DebugStringValue;
			}
			if (_OffsetXVar != null)
			{
				text = text + "OffsetX=" + _OffsetXVar.DebugStringValue;
			}
			if (_OffsetYVar != null)
			{
				text = text + " OffsetY=" + _OffsetYVar.DebugStringValue;
			}
			if (_AnchorItem != null)
			{
				text = text + " AnchorItem=" + _OffsetYVar.DebugStringValue;
			}
			if (_BorderColor != null)
			{
				text = text + " BorderColor=" + _BorderColor.DebugStringValue;
			}
			return text;
		}

		protected override void Log()
		{
			base.Log();
			VectorLog.RunLog("Action: FloatingText");
			VectorLog.Tab(1);
			if (_TextVar != null)
			{
				VectorLog.RunLog("Text", _TextVar);
			}
			if (_ColorVar != null)
			{
				VectorLog.RunLog("Color", _ColorVar);
			}
			if (_SizeVar != null)
			{
				VectorLog.RunLog("Size", _SizeVar);
			}
			if (_FramesVar != null)
			{
				VectorLog.RunLog("Frames", _FramesVar);
			}
			if (_DelayVar != null)
			{
				VectorLog.RunLog("Delay", _DelayVar);
			}
			if (_ShiftXVar != null)
			{
				VectorLog.RunLog("ShiftX", _ShiftXVar);
			}
			if (_ShiftYVar != null)
			{
				VectorLog.RunLog("ShiftY", _ShiftYVar);
			}
			if (_ImageVar != null)
			{
				VectorLog.RunLog("Image", _ImageVar);
			}
			if (_ImageAlignVar != null)
			{
				VectorLog.RunLog("ImageAlign", _ImageAlignVar);
			}
			if (_ImageWidthVar != null)
			{
				VectorLog.RunLog("ImageWidth", _ImageWidthVar);
			}
			if (_ImageHeightVar != null)
			{
				VectorLog.RunLog("ImageHeight", _ImageHeightVar);
			}
			if (_OriginTypeVar != null)
			{
				VectorLog.RunLog("OriginType=", _OriginTypeVar);
			}
			if (_OffsetXVar != null)
			{
				VectorLog.RunLog("OffsetX=", _OffsetXVar);
			}
			if (_OffsetYVar != null)
			{
				VectorLog.RunLog(" OffsetY=", _OffsetYVar);
			}
			if (_AnchorItem != null)
			{
				VectorLog.RunLog(" AnchorItem=", _AnchorItem);
			}
			if (_BorderColor != null)
			{
				VectorLog.RunLog("BorderColor=", _BorderColor);
			}
			VectorLog.Untab(1);
		}

		private void CreateUI()
		{
			_UI = FloatingText.Create(_OriginTypeVar.ValueString, _ParentLoop.ParentTrigger.Name, GetKeyForCaching());
		}

		private Color GetColor(string color)
		{
			switch (color)
			{
			case "blue":
				return new Color(0.25f, 0.77f, 0.88f);
			case "orange":
				return new Color(0.9f, 0.43f, 0f);
			case "grey":
				return new Color(0.62f, 0.62f, 0.62f);
			case "red":
				return new Color(0.65f, 0.078f, 0.027f);
			case "green":
				return new Color(0.435f, 0.792f, 0f);
			default:
				return new Color(0.62f, 0.62f, 0.62f);
			}
		}

		private string GetTextString()
		{
			if (_VarsInText == null)
			{
				return _TextVar.ValueString;
			}
			string text = _TextVar.ValueString;
			int i = 0;
			for (int count = _VarsInText.Count; i < count; i++)
			{
				text = text.Replace("%" + (i + 1), _VarsInText[i].ValueString);
			}
			return text;
		}

		private Vector3 GetPosition()
		{
			Vector3 zero = Vector3.zero;
			if (_OffsetXVar != null)
			{
				zero.x = _OffsetXVar.ValueFloat;
			}
			if (_OffsetYVar != null)
			{
				zero.y = _OffsetYVar.ValueFloat;
			}
			switch (_OriginTypeVar.ValueString)
			{
			case "Local":
				return _ParentLoop.ParentTrigger.Center + zero;
			case "Screen":
			case "Subtitles":
				return zero;
			case "Gadget":
			{
				HudPanel module = UIModule.GetModule<HudPanel>();
				if (module != null)
				{
					Transform transform = module.FindGadget((_AnchorItem as VariableItem).Item);
					if (transform != null)
					{
						return FloatingText.ConvertWorldToScreenCoords(transform.position) + zero;
					}
				}
				return zero;
			}
			default:
				return zero;
			}
		}

		private Vector3 GetShift()
		{
			Vector2 zero = Vector2.zero;
			if (_ShiftXVar != null)
			{
				zero.x = _ShiftXVar.ValueFloat;
			}
			if (_ShiftYVar != null)
			{
				zero.y = _ShiftYVar.ValueFloat;
			}
			return zero;
		}

		private string GetKeyForCaching()
		{
			if (_VarsInText == null && !_TextVar.IsFunctionVar && _ImageVar == null)
			{
				return GetTextString();
			}
			return null;
		}
	}
}
