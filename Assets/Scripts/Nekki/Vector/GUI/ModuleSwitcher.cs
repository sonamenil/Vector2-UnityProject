using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Nekki.Vector.GUI
{
	public class ModuleSwitcher : MonoBehaviour
	{
		[SerializeField]
		private EventSystem _EventSystem;

		[SerializeField]
		private Image _Background;

		[SerializeField]
		private float _FadeTime = 1.5f;

		[SerializeField]
		private float _PauseTime = 1f;

		private Action _SwitchFunc;

		private Action _ActionAfterSwitch;

		public void Switch(Action p_switchFunc, bool p_needFadeOut = true)
		{
			Switch(p_switchFunc, null, p_needFadeOut);
		}

		public void Switch(Action p_switchFunc, Action p_onEndSwitchFunc, bool p_needFadeOut = true)
		{
			_SwitchFunc = p_switchFunc;
			_ActionAfterSwitch = p_onEndSwitchFunc;
			Run(_FadeTime, _PauseTime, p_needFadeOut);
		}

		public void Switch(Action p_switchFunc, float p_fadeTime, float p_pauseTime, bool p_needFadeOut = true)
		{
			Switch(p_switchFunc, null, p_fadeTime, p_pauseTime, p_needFadeOut);
		}

		public void Switch(Action p_switchFunc, Action p_onEndSwitchFunc, float p_fadeTime, float p_pauseTime, bool p_needFadeOut = true)
		{
			_SwitchFunc = p_switchFunc;
			_ActionAfterSwitch = p_onEndSwitchFunc;
			Run(p_fadeTime, p_pauseTime, p_needFadeOut);
		}

		private void Run(float p_fadeTime, float p_pauseTime, bool p_needFadeOut)
		{
			ResetAlpha();
			_EventSystem.enabled = false;
			GetComponent<RectTransform>().SetSiblingIndex(10);
			Sequence sequence = DOTween.Sequence();
			sequence.Append(_Background.DOFade(1f, p_fadeTime));
			sequence.AppendInterval(p_pauseTime);
			sequence.AppendCallback(SwitchModules);
			if (p_needFadeOut)
			{
				sequence.Append(_Background.DOFade(0f, p_fadeTime));
			}
			sequence.AppendCallback(OnEndSwitchWrap);
			sequence.OnKill(Stop);
			sequence.Play();
		}

		public void Stop()
		{
			_EventSystem.enabled = true;
		}

		private void ResetAlpha()
		{
			Color color = _Background.color;
			color.a = 0f;
			_Background.color = color;
		}

		private void SwitchModules()
		{
			if (_SwitchFunc != null)
			{
				_SwitchFunc();
			}
		}

		private void OnEndSwitchWrap()
		{
			if (_ActionAfterSwitch != null)
			{
				_ActionAfterSwitch();
				_ActionAfterSwitch = null;
			}
		}
	}
}
