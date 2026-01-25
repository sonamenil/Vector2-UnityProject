using System;
using System.Xml;
using Nekki.Vector.Core.Trigger;
using Nekki.Vector.Core.Variables;
using Nekki.Vector.GUI.Dialogs;

public class TQA_Chapter : TriggerQuestAction
{
	private Variable _Title;

	private Variable _Message;

	private Variable _HideBy;

	private Variable _ActionAfterHide;

	private ChapterWindowSettings _Settings;

	public TQA_Chapter(XmlNode p_node, TriggerQuestLoop p_parent)
		: base(p_parent)
	{
		_Title = Variable.CreateVariable(XmlUtils.ParseString(p_node.Attributes["Title"]), string.Empty);
		_Message = Variable.CreateVariable(XmlUtils.ParseString(p_node.Attributes["Message"]), string.Empty);
		_Settings = new ChapterWindowSettings();
		string text = XmlUtils.ParseString(p_node.Attributes["DarknessFadeIn"]);
		if (text != null)
		{
			_Settings.DarknessFadeInVar = Variable.CreateVariable(text, string.Empty);
		}
		text = XmlUtils.ParseString(p_node.Attributes["TitleFadeIn"]);
		if (text != null)
		{
			_Settings.TitleFadeInVar = Variable.CreateVariable(text, string.Empty);
		}
		text = XmlUtils.ParseString(p_node.Attributes["ContentFadeIn"]);
		if (text != null)
		{
			_Settings.ContentFadeInVar = Variable.CreateVariable(text, string.Empty);
		}
		text = XmlUtils.ParseString(p_node.Attributes["TitleDelay"]);
		if (text != null)
		{
			_Settings.TitleDelayVar = Variable.CreateVariable(text, string.Empty);
		}
		text = XmlUtils.ParseString(p_node.Attributes["ContentDelay"]);
		if (text != null)
		{
			_Settings.ContentDelayVar = Variable.CreateVariable(text, string.Empty);
		}
		text = XmlUtils.ParseString(p_node.Attributes["DarknessFadeOut"]);
		if (text != null)
		{
			_Settings.DarknessFadeOutVar = Variable.CreateVariable(text, string.Empty);
		}
		text = XmlUtils.ParseString(p_node.Attributes["TitleFadeOut"]);
		if (text != null)
		{
			_Settings.TitleFadeOutVar = Variable.CreateVariable(text, string.Empty);
		}
		text = XmlUtils.ParseString(p_node.Attributes["ContentFadeOut"]);
		if (text != null)
		{
			_Settings.ContentFadeOutVar = Variable.CreateVariable(text, string.Empty);
		}
		text = XmlUtils.ParseString(p_node.Attributes["ContentFadeOutDelay"]);
		if (text != null)
		{
			_Settings.ContentFadeOutDelayVar = Variable.CreateVariable(text, string.Empty);
		}
		text = XmlUtils.ParseString(p_node.Attributes["DarknessFadeOutDelay"]);
		if (text != null)
		{
			_Settings.DarknessFadeOutDelayVar = Variable.CreateVariable(text, string.Empty);
		}
		text = XmlUtils.ParseString(p_node.Attributes["HideDelay"]);
		if (text != null)
		{
			_Settings.HideDelay = Variable.CreateVariable(text, string.Empty);
		}
		_HideBy = Variable.CreateVariable(XmlUtils.ParseString(p_node.Attributes["HideBy"], string.Empty), string.Empty);
		_ActionAfterHide = Variable.CreateVariable(XmlUtils.ParseString(p_node.Attributes["ActionAfterHide"], string.Empty), string.Empty);
	}

	public override void Activate(ref bool p_runNext)
	{
		string valueString = _HideBy.ValueString;
		p_runNext = ChapterWindow.NeedToHideManually(valueString);
		Action additionalAction = OnClickCallback;
		if (p_runNext)
		{
			additionalAction = null;
		}
		DialogNotificationManager.ShowChapterWindow(_Title.ValueString, _Message.ValueString, valueString, _ActionAfterHide.ValueString, _Settings, additionalAction, 100);
	}

	public void OnClickCallback()
	{
		_Parent.ActivateActions();
	}
}
