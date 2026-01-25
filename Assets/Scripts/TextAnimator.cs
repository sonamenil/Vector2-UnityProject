using DG.Tweening;
using Nekki.Vector.Core.Audio;
using Nekki.Vector.Core.Localization;
using UnityEngine;

public class TextAnimator : MonoBehaviour
{
	private LabelAlias _Target;

	[SerializeField]
	private float _Delay;

	[SerializeField]
	private float _OneCharSpeed;

	[SerializeField]
	private bool _AlwaysUse = true;

	[SerializeField]
	private bool _IgnoreSource = true;

	[SerializeField]
	private ScrambleMode _ScrambleMode;

	private Tween _Tween;

	private string _LastText;

	private bool _ForceUse;

	public LabelAlias Target
	{
		set
		{
			_Target = value;
		}
	}

	private void OnEnable()
	{
		if (!(_Target == null) && _LastText != null)
		{
			if (_Tween != null)
			{
				_Tween.Kill();
				_Tween = null;
			}
			Play(_LastText);
		}
	}

	public void Arm()
	{
		_ForceUse = true;
	}

	public void Play(string p_text)
	{
		if (_ForceUse || _AlwaysUse)
		{
			if (!(_Target == null))
			{
				if (_Tween != null)
				{
					_Tween.Kill();
				}
				_LastText = p_text;
				if (_IgnoreSource)
				{
					_Target.text = string.Empty;
				}
				Sequence sequence = DOTween.Sequence();
				sequence.AppendCallback(delegate
				{
					AudioManager.PlaySound("type_loop", 1f, true);
				});
				if (_Delay > 1E-05f)
				{
					sequence.AppendInterval(_Delay);
					ScrambleMode scrambleMode = _ScrambleMode;
					sequence.Append(_Target.DOText(p_text, (float)p_text.Length * _OneCharSpeed, true, scrambleMode));
				}
				else
				{
					ScrambleMode scrambleMode = _ScrambleMode;
					sequence.Append(_Target.DOText(p_text, (float)p_text.Length * _OneCharSpeed, true, scrambleMode));
				}
				sequence.AppendCallback(delegate
				{
					AudioManager.StopSound("type_loop");
				});
				_Tween = sequence;
				_Tween.OnKill(delegate
				{
					_Tween = null;
				});
				_Tween.Play();
				_ForceUse = false;
			}
		}
		else
		{
			_LastText = p_text;
			_Target.text = p_text;
			if (_Tween != null)
			{
				_Tween.Kill();
			}
		}
	}
}
