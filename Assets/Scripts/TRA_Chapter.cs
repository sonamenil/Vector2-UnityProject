using System.Xml;
using Nekki.Vector.Core.Trigger;
using Nekki.Vector.Core.Variables;
using Nekki.Vector.GUI.Dialogs;

public class TRA_Chapter : TriggerRunnerAction
{
	private Variable _Title;

	private Variable _Message;

	private Variable _HideBy;

	private Variable _ActionAfterHide;

	private ChapterWindowSettings _Settings;

	private TRA_Chapter(TRA_Chapter p_copy)
		: base(p_copy)
	{
		_Title = p_copy._Title;
		_Message = p_copy._Message;
		_HideBy = p_copy._HideBy;
		_ActionAfterHide = p_copy._ActionAfterHide;
		_Settings = p_copy._Settings;
	}

	public TRA_Chapter(XmlNode p_node, TriggerRunnerLoop p_parent)
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
		DialogNotificationManager.ShowChapterWindow(_Title.ValueString, _Message.ValueString, valueString, _ActionAfterHide.ValueString, _Settings, null, 100);
	}

	public void OnClickCallback()
	{
	}

	public override TriggerRunnerAction Copy()
	{
		return new TRA_Chapter(this);
	}
}
