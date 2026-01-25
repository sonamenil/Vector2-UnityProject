using System;
using System.Collections.Generic;
using Nekki.Yaml;
using UnityEngine;

namespace Nekki.Vector.Core.Localization
{
	public class LanguageData
	{
		private delegate void ParseDelegate(string p_key, string p_data);

		private const string _VarsFilename = "vars.yaml";

		private const string _Error = "<PhraseError-{0}>";

		private const string _FontsPath = "UI/Fonts/";

		private static Dictionary<string, LocalizationAlias> _Phrases = new Dictionary<string, LocalizationAlias>();

		public SystemLanguage Name { get; private set; }

		public string VisualName { get; private set; }

		public string File { get; private set; }

		public Font ContentFont { get; private set; }

		public Font TitleFont { get; private set; }

		public Font ButtonFont { get; private set; }

		public LanguageData(Mapping p_node)
		{
			Name = YamlUtils.GetEnumValue(p_node.GetText("Name"), SystemLanguage.English);
			VisualName = YamlUtils.GetStringValue(p_node.GetText("VisualName"), string.Empty);
			File = YamlUtils.GetStringValue(p_node.GetText("File"), string.Empty) + ".yaml";
			string stringValue = YamlUtils.GetStringValue(p_node.GetText("ContentFont"), string.Empty);
			string stringValue2 = YamlUtils.GetStringValue(p_node.GetText("TitleFont"), string.Empty);
			string stringValue3 = YamlUtils.GetStringValue(p_node.GetText("ButtonFont"), string.Empty);
			ContentFont = Resources.Load<Font>("UI/Fonts/" + stringValue);
			TitleFont = Resources.Load<Font>("UI/Fonts/" + stringValue2);
			ButtonFont = Resources.Load<Font>("UI/Fonts/" + stringValue3);
			if (ContentFont == null)
			{
				Debug.LogErrorFormat("Can't load content font [{0}] for [{1}] language", stringValue, Name);
			}
			if (TitleFont == null)
			{
				TitleFont = ContentFont;
				Debug.LogErrorFormat("Can't load title font [{0}] for [{1}] language", stringValue2, Name);
			}
			if (ButtonFont == null)
			{
				ButtonFont = ContentFont;
				Debug.LogErrorFormat("Can't load button font [{0}] for [{1}] language", stringValue3, Name);
			}
		}

		private static void LoadFile(string p_path, string p_file, ParseDelegate p_delegate)
		{
			YamlDocumentNekki yamlDocumentNekki = YamlUtils.OpenYamlFile(p_path, p_file);
			if (yamlDocumentNekki == null)
			{
				Debug.LogErrorFormat("[LanguageD]: can't load {0}!", p_file);
				return;
			}
			foreach (Nekki.Yaml.Node item in yamlDocumentNekki.GetRoot(0))
			{
				if (item is Scalar)
				{
					ParsePhrasesLeaf(item, string.Empty, p_delegate);
				}
				else
				{
					ParsePhrases(item as Mapping, string.Empty, p_delegate);
				}
			}
		}

		private static void ParsePhrases(Mapping p_node, string p_key, ParseDelegate p_delegate)
		{
			if (p_key.Length > 0)
			{
				p_key += ".";
			}
			foreach (Nekki.Yaml.Node item in p_node)
			{
				if (item is Scalar)
				{
					ParsePhrasesLeaf(item, p_key + p_node.key, p_delegate);
				}
				else
				{
					ParsePhrases(item as Mapping, p_key + p_node.key, p_delegate);
				}
			}
		}

		private static void ParsePhrasesLeaf(Nekki.Yaml.Node p_node, string p_key, ParseDelegate p_delegate)
		{
			if (p_key.Length > 0)
			{
				p_key += ".";
			}
			p_delegate(p_key + p_node.key, p_node.value.ToString());
		}

		public void LoadTexts()
		{
			LoadFile(VectorPaths.Localization, File, AddToPhrases);
		}

		private void AddToPhrases(string p_key, string p_value)
		{
			if (_Phrases.ContainsKey(p_key))
			{
				_Phrases[p_key].Text = p_value;
			}
			else
			{
				_Phrases.Add(p_key, new LocalizationAlias(p_value));
			}
		}

		public static void LoadVars()
		{
			LoadFile(VectorPaths.Localization, "vars.yaml", AddToVars);
		}

		private static void AddToVars(string p_key, string p_value)
		{
			if (_Phrases.ContainsKey(p_key))
			{
				_Phrases[p_key].SetParams(p_value);
			}
			else
			{
				DebugUtils.Dialog("No allias: " + p_key, true);
			}
		}

		public void CloseFile()
		{
			_Phrases.Clear();
			GC.Collect();
		}

		public string GetPhrase(string p_alias)
		{
			LocalizationAlias value = null;
			if (!_Phrases.TryGetValue(p_alias, out value))
			{
				return string.Format("<PhraseError-{0}>", p_alias);
			}
			return value.Text;
		}

		public override string ToString()
		{
			return string.Format("[LanguageData]: Name={0}, VisualName={1} File={2}, ContentFont={3}, TitleFont={4}, ButtonFont={5}", Name, VisualName, File, ContentFont, TitleFont, ButtonFont);
		}
	}
}
