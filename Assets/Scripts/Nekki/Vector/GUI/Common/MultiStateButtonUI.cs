using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Nekki.Vector.GUI.Common
{
	[ExecuteInEditMode]
	public class MultiStateButtonUI : MonoBehaviour
	{
		[SerializeField]
		private Button _Button;

		[SerializeField]
		private ResolutionImage _Image;

		[SerializeField]
		private MultiStateButtonUIParameters _Parameters;

		private List<ButtonData> _States = new List<ButtonData>();

		private int _CurrentState;

		public Button Button
		{
			get
			{
				return _Button;
			}
		}

		public ResolutionImage Image
		{
			get
			{
				return _Image;
			}
		}

		public MultiStateButtonUIParameters Parameters
		{
			get
			{
				return _Parameters;
			}
		}

		public void Init(MultiStateButtonUIParameters p_parameters, List<ButtonData> p_states)
		{
			_Parameters = p_parameters;
			_States = p_states;
			_Image.SpriteName = _States[0].ImageAlias;
		}

		private void SetSound(string p_soundAlias, string p_defaultSoundAlias)
		{
			ButtonSoundClick componentInChildren = GetComponentInChildren<ButtonSoundClick>();
			if (p_soundAlias == null)
			{
				p_soundAlias = p_defaultSoundAlias;
			}
			if (componentInChildren != null)
			{
				componentInChildren.SoundAlias = p_soundAlias;
			}
		}

		private void Awake()
		{
			_CurrentState = 0;
			Sequence sequence = DOTween.Sequence();
			sequence.AppendInterval(_Parameters.ShowTime);
			sequence.AppendCallback(delegate
			{
				ChangeState();
			});
			sequence.Play();
		}

		public void ChangeState()
		{
			Sequence sequence = DOTween.Sequence();
			float num = 0.2f;
			sequence.Append(_Image.DOFade(num, _Parameters.ChangeStateAnimationTime * (1f - num) / 2f));
			sequence.AppendCallback(delegate
			{
				_Button.interactable = false;
			});
			sequence.Append(_Image.DOFade(0f, _Parameters.ChangeStateAnimationTime * num / 2f));
			sequence.AppendCallback(delegate
			{
				_CurrentState = (_CurrentState + 1) % _States.Count;
				_Image.SpriteName = _States[_CurrentState].ImageAlias;
			});
			sequence.Append(_Image.DOFade(num, _Parameters.ChangeStateAnimationTime * num / 2f));
			sequence.AppendCallback(delegate
			{
				_Button.interactable = true;
			});
			sequence.Append(_Image.DOFade(1f, _Parameters.ChangeStateAnimationTime * (1f - num) / 2f));
			sequence.AppendInterval(_Parameters.ShowTime);
			sequence.AppendCallback(delegate
			{
				ChangeState();
			});
			sequence.Play();
		}

		public void OnTap()
		{
			if (_States[_CurrentState].Callback != null)
			{
				_States[_CurrentState].Callback();
			}
		}
	}
}
