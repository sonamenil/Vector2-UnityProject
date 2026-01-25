using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Nekki.Vector.Core.Localization
{
	[AddComponentMenu("UI_Nekki/LabelAlias")]
	public class LabelAlias : Text
	{
		public enum Type
		{
			Content = 0,
			Title = 1,
			Button = 2
		}

		[Tooltip("If true target will use font from localization settings")]
		public bool UseLocalizationFont = true;

		[Tooltip("Type of font. Choose font from languages.yaml")]
		public Type FontType;

		[Tooltip("Text template with aliases")]
		[TextArea(4, 10)]
		public string Alias = string.Empty;

		[SerializeField]
		private bool _ToUpperCase;

		private TextAnimator _Animator;

		public bool ToUpperCase
		{
			get
			{
				return _ToUpperCase;
			}
			set
			{
				_ToUpperCase = value;
			}
		}

		public string Text
		{
			get
			{
				return text;
			}
			set
			{
				text = value;
				if (UseLocalizationFont)
				{
					base.font = LocalizedFont;
				}
			}
		}

		private string LocalizedText
		{
			get
			{
				string text = Alias;
				List<string> aliasesList = GetAliasesList();
				if (aliasesList != null)
				{
					foreach (string item in aliasesList)
					{
						text = text.Replace("^" + item + "^", LocalizationManager.GetPhrase(item));
					}
				}
				return text;
			}
		}

		private Font LocalizedFont
		{
			get
			{
				switch (FontType)
				{
				case Type.Content:
					return LocalizationManager.CurrentContentFont;
				case Type.Title:
					return LocalizationManager.CurrentTitleFont;
				case Type.Button:
					return LocalizationManager.CurrentButtonFont;
				default:
					return LocalizationManager.CurrentContentFont;
				}
			}
		}

		public float Alpha
		{
			get
			{
				return color.a;
			}
			set
			{
				Color color = this.color;
				color.a = value;
				this.color = color;
			}
		}

		private List<string> GetAliasesList()
		{
			List<string> list = new List<string>();
			string text = null;
			bool flag = false;
			if (Alias != null)
			{
				string alias = Alias;
				foreach (char c in alias)
				{
					if (flag)
					{
						if (c == '^')
						{
							list.Add(text);
							flag = false;
						}
						else
						{
							text += c;
						}
					}
					else if (c == '^')
					{
						text = string.Empty;
						flag = true;
					}
				}
				return list;
			}
			return null;
		}

		protected override void Awake()
		{
			base.Awake();
			_Animator = GetComponent<TextAnimator>();
			if (_Animator != null)
			{
				_Animator.Target = this;
			}
			LocalizationManager.OnLanguageChanged += OnLanguageChanged;
			if (LocalizationManager.IsInited && Alias != null && Alias.Length != 0)
			{
				OnLanguageChanged(LocalizationManager.CurrentLanguage);
			}
		}

		protected override void OnDestroy()
		{
			base.OnDestroy();
			LocalizationManager.OnLanguageChanged -= OnLanguageChanged;
		}

		private void OnLanguageChanged(SystemLanguage p_language)
		{
			string text = LocalizedText.TrimStart(' ');
			if (_ToUpperCase)
			{
				text = text.ToUpper();
			}
			if (_Animator != null)
			{
				_Animator.Play(text);
			}
			else
			{
				this.text = text;
			}
			if (UseLocalizationFont)
			{
				base.font = LocalizedFont;
			}
		}

		public void SetAlias(string text)
		{
			Alias = text;
			OnLanguageChanged(LocalizationManager.CurrentLanguage);
		}
	}
}
