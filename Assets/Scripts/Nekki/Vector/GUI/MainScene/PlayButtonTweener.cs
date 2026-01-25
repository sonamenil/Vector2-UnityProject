using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Nekki.Vector.GUI.MainScene
{
	public class PlayButtonTweener : MonoBehaviour
	{
		[SerializeField]
		private List<Graphic> _Elements_1;

		[SerializeField]
		private List<Graphic> _Elements_2;

		[SerializeField]
		private float _FadeTime = 1.5f;

		[SerializeField]
		private float _PauseTime = 1f;

		private int _State = -1;

		private float _Timer;

		private float _AlphaValue = 1f;

		private List<Graphic> _CurrElements;

		public void Run()
		{
			_Timer = 0f;
			_State = 0;
			_CurrElements = _Elements_2;
			_AlphaValue = 0f;
			SetAlpha();
			SwitchCurrElements();
			_AlphaValue = 1f;
			SetAlpha();
		}

		public void Stop()
		{
			_State = -1;
			_CurrElements = _Elements_1;
			_AlphaValue = 1f;
			SetAlpha();
			SwitchCurrElements();
			SetAlpha();
		}

		private void Update()
		{
			if (_State == -1)
			{
				return;
			}
			_Timer += Time.deltaTime;
			switch (_State)
			{
			case 0:
				_AlphaValue = Mathf.Lerp(1f, 0f, _Timer / _FadeTime);
				SetAlpha();
				if (_Timer > _FadeTime)
				{
					_State = 2;
					_Timer = 0f;
					SwitchCurrElements();
				}
				break;
			case 1:
				if (_Timer > _PauseTime)
				{
					_State = 0;
					_Timer = 0f;
				}
				break;
			case 2:
				_AlphaValue = Mathf.Lerp(0f, 1f, _Timer / _FadeTime);
				SetAlpha();
				if (_Timer > _FadeTime)
				{
					_State = 1;
					_Timer = 0f;
				}
				break;
			}
		}

		private void SetAlpha()
		{
			for (int i = 0; i < _CurrElements.Count; i++)
			{
				Color color = _CurrElements[i].color;
				color.a = _AlphaValue;
				_CurrElements[i].color = color;
			}
		}

		private void SwitchCurrElements()
		{
			if (_CurrElements == _Elements_1)
			{
				_CurrElements = _Elements_2;
			}
			else
			{
				_CurrElements = _Elements_1;
			}
		}
	}
}
