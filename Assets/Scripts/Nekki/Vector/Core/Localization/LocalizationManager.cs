using System;
using System.Collections.Generic;
using Nekki.Vector.Core.User;
using Nekki.Vector.Core.Variables;
using Nekki.Yaml;
using UnityEngine;

namespace Nekki.Vector.Core.Localization
{
	public static class LocalizationManager
	{
		private const string _FileName = "languages.yaml";

		private static SystemLanguage _DelayLanguage = SystemLanguage.English;

		private static LanguageData _CurrentLanguage;

		private static List<LanguageData> _Languages;

		private static Dictionary<SystemLanguage, LanguageData> _LanguagesDict;

		private static List<SystemLanguage> _SupportedLanguages;

		private static SystemLanguage _DefaultLanguage;

		public static bool IsInited
		{
			get
			{
				return _CurrentLanguage != null;
			}
		}

		public static SystemLanguage CurrentLanguage
		{
			get
			{
				return _CurrentLanguage.Name;
			}
			set
			{
				if (_CurrentLanguage != null && _CurrentLanguage.Name == value)
				{
					return;
				}
				if (!IsLanguageSupported(value))
				{
					DebugUtils.LogFormat("Try to set unsupported language -> {0}", value);
					return;
				}
				if (_CurrentLanguage != null)
				{
					_CurrentLanguage.CloseFile();
				}
				SetCurrentLanguage(value);
				LanguageData.LoadVars();
				if (LocalizationManager.OnLanguageChanged != null)
				{
					LocalizationManager.OnLanguageChanged(_CurrentLanguage.Name);
				}
			}
		}

		public static Font CurrentContentFont
		{
			get
			{
				return (_CurrentLanguage == null) ? null : _CurrentLanguage.ContentFont;
			}
		}

		public static Font CurrentTitleFont
		{
			get
			{
				return (_CurrentLanguage == null) ? null : _CurrentLanguage.TitleFont;
			}
		}

		public static Font CurrentButtonFont
		{
			get
			{
				return (_CurrentLanguage == null) ? null : _CurrentLanguage.ButtonFont;
			}
		}

		public static SystemLanguage DefaultLanguage
		{
			get
			{
				return _DefaultLanguage;
			}
		}

		public static List<SystemLanguage> SupportedLanguages
		{
			get
			{
				return _SupportedLanguages;
			}
		}

		public static event Action<SystemLanguage> OnLanguageChanged;

		public static void SetDelayLanguage()
		{
			if (_DelayLanguage != SystemLanguage.Unknown && _DelayLanguage != CurrentLanguage)
			{
				CurrentLanguage = _DelayLanguage;
				_DelayLanguage = SystemLanguage.Unknown;
			}
		}

		public static void Init()
		{
			LoadLanguages();
			SetupSupportedLanguages();
			SetupCurrentLanguage();
		}

		public static void Free()
		{
			_CurrentLanguage = null;
			_Languages.Clear();
			_Languages = null;
			_LanguagesDict.Clear();
			_LanguagesDict = null;
		}

		public static LanguageData GetLanguageData(SystemLanguage p_language)
		{
			LanguageData value = null;
			_LanguagesDict.TryGetValue(p_language, out value);
			return value;
		}

		public static bool IsLanguageSupported(SystemLanguage p_language)
		{
			return _SupportedLanguages.Contains(p_language);
		}

		public static string GetPhrase(string p_alias)
		{
			p_alias = ParseAlias(p_alias.Replace(" ", string.Empty));
			if (_CurrentLanguage == null)
			{
				DebugUtils.Dialog(string.Format("Try to get phrase [{0}] when language is null!", p_alias), false);
				return string.Empty;
			}
			return _CurrentLanguage.GetPhrase(p_alias);
		}

		public static string ParseAlias(string alias)
		{
			string text = string.Empty;
			string[] array = alias.Split('.');
			for (int i = 0; i < array.Length; i++)
			{
				text = ((array[i].Length <= 0 || array[i][0] != '?') ? (text + array[i]) : (text + Variable.CreateVariable(array[i], string.Empty).ValueString));
				if (i != array.Length - 1)
				{
					text += ".";
				}
			}
			return text;
		}

		private static void LoadLanguages()
		{
			YamlDocumentNekki yamlDocumentNekki = YamlUtils.OpenYamlFile(VectorPaths.Localization, "languages.yaml");
			if (yamlDocumentNekki == null)
			{
				DebugUtils.Dialog(string.Format("[LocalizationManager]: can't load {0}!", "languages.yaml"), true);
				return;
			}
			Sequence sequence = yamlDocumentNekki.GetRoot(0).GetSequence("Languages");
			_Languages = new List<LanguageData>();
			_LanguagesDict = new Dictionary<SystemLanguage, LanguageData>();
			foreach (Mapping item in sequence)
			{
				LanguageData languageData = new LanguageData(item);
				_Languages.Add(languageData);
				_LanguagesDict.Add(languageData.Name, languageData);
			}
		}

		private static void SetupSupportedLanguages()
		{
			_SupportedLanguages = new List<SystemLanguage>();
			foreach (LanguageData language in _Languages)
			{
				_SupportedLanguages.Add(language.Name);
			}
			SystemLanguage systemLanguage = Application.systemLanguage;
			_DefaultLanguage = ((!IsLanguageSupported(systemLanguage)) ? _SupportedLanguages[0] : systemLanguage);
		}

		private static void SetupCurrentLanguage()
		{
			SystemLanguage systemLanguage = DataLocal.Current.Settings.CurrentLanguage;
			if (!IsLanguageSupported(systemLanguage))
			{
				DataLocal.Current.Settings.CurrentLanguage = DefaultLanguage;
				systemLanguage = DefaultLanguage;
			}
			_DelayLanguage = SystemLanguage.Unknown;
			if (DeviceInformation.IsAndroid && systemLanguage == SystemLanguage.Russian)
			{
				_DelayLanguage = systemLanguage;
				systemLanguage = SystemLanguage.English;
			}
			CurrentLanguage = systemLanguage;
		}

		private static void SetCurrentLanguage(SystemLanguage p_language)
		{
			Debug.Log("SwitchTo: " + p_language);
			_CurrentLanguage = _Languages.Find((LanguageData p_data) => p_data.Name == p_language);
			_CurrentLanguage.LoadTexts();
		}
	}
}
