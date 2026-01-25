using Nekki.Vector.Core.Variables;

namespace Nekki.Vector.GUI.Dialogs
{
	public class ChapterWindowSettings
	{
		private Variable _DarknessFadeIn;

		private Variable _TitleFadeIn;

		private Variable _ContentFadeIn;

		private Variable _TitleDelay;

		private Variable _ContentDelay;

		private Variable _DarknessFadeOut;

		private Variable _TitleFadeOut;

		private Variable _ContentFadeOut;

		private Variable _ContentFadeOutDelay;

		private Variable _DarknessFadeOutDelay;

		private Variable _HideDelay;

		public float DarknessFadeInFloat
		{
			get
			{
				return _DarknessFadeIn.ValueFloat;
			}
		}

		public float TitleFadeInFloat
		{
			get
			{
				return _TitleFadeIn.ValueFloat;
			}
		}

		public float ContentFadeInFloat
		{
			get
			{
				return _ContentFadeIn.ValueFloat;
			}
		}

		public float TitleDelayFloat
		{
			get
			{
				return _TitleDelay.ValueFloat;
			}
		}

		public float ContentDelayFloat
		{
			get
			{
				return _ContentDelay.ValueFloat;
			}
		}

		public float DarknessFadeOutFloat
		{
			get
			{
				return _DarknessFadeOut.ValueFloat;
			}
		}

		public float TitleFadeOutFloat
		{
			get
			{
				return _TitleFadeOut.ValueFloat;
			}
		}

		public float ContentFadeOutFloat
		{
			get
			{
				return _ContentFadeOut.ValueFloat;
			}
		}

		public float ContentFadeOutDelayFloat
		{
			get
			{
				return _ContentFadeOutDelay.ValueFloat;
			}
		}

		public float DarknessFadeOutDelayFloat
		{
			get
			{
				return _DarknessFadeOutDelay.ValueFloat;
			}
		}

		public float HideDelayFloat
		{
			get
			{
				return _HideDelay.ValueFloat;
			}
		}

		public Variable DarknessFadeInVar
		{
			set
			{
				_DarknessFadeIn = value;
			}
		}

		public Variable TitleFadeInVar
		{
			set
			{
				_TitleFadeIn = value;
			}
		}

		public Variable ContentFadeInVar
		{
			set
			{
				_ContentFadeIn = value;
			}
		}

		public Variable TitleDelayVar
		{
			set
			{
				_TitleDelay = value;
			}
		}

		public Variable ContentDelayVar
		{
			set
			{
				_ContentDelay = value;
			}
		}

		public Variable DarknessFadeOutVar
		{
			set
			{
				_DarknessFadeOut = value;
			}
		}

		public Variable TitleFadeOutVar
		{
			set
			{
				_TitleFadeOut = value;
			}
		}

		public Variable ContentFadeOutVar
		{
			set
			{
				_ContentFadeOut = value;
			}
		}

		public Variable ContentFadeOutDelayVar
		{
			set
			{
				_ContentFadeOutDelay = value;
			}
		}

		public Variable DarknessFadeOutDelayVar
		{
			set
			{
				_DarknessFadeOutDelay = value;
			}
		}

		public Variable HideDelay
		{
			set
			{
				_HideDelay = value;
			}
		}

		public ChapterWindowSettings()
		{
			_DarknessFadeIn = Variable.CreateVariable("0.5", string.Empty);
			_TitleFadeIn = Variable.CreateVariable("0.5", string.Empty);
			_ContentFadeIn = Variable.CreateVariable("0.5", string.Empty);
			_TitleDelay = Variable.CreateVariable("1.0", string.Empty);
			_ContentDelay = Variable.CreateVariable("1.0", string.Empty);
			_DarknessFadeOut = Variable.CreateVariable("0.5", string.Empty);
			_TitleFadeOut = Variable.CreateVariable("0.5", string.Empty);
			_ContentFadeOut = Variable.CreateVariable("0.5", string.Empty);
			_ContentFadeOutDelay = Variable.CreateVariable("1.0", string.Empty);
			_DarknessFadeOutDelay = Variable.CreateVariable("1.0", string.Empty);
			_HideDelay = Variable.CreateVariable("3.0", string.Empty);
		}
	}
}
