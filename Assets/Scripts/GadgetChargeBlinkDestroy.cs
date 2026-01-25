using System;
using DG.Tweening;
using UIFigures;
using UnityEngine;

public class GadgetChargeBlinkDestroy : MonoBehaviour
{
	private Action<GadgetChargeBlinkDestroy> _CallbackOnComplete;

	private Action<GadgetChargeBlinkDestroy> _CallbackOnStart;

	public UIArcBorder Target;

	public void Init(UIArcBorder p_arc, Color p_color, float p_time, Action<GadgetChargeBlinkDestroy> p_callbackOnStart, Action<GadgetChargeBlinkDestroy> p_callbackOnComplete, int p_flashes)
	{
		_CallbackOnComplete = p_callbackOnComplete;
		_CallbackOnStart = p_callbackOnStart;
		Target = p_arc;
		float atPosition = (float)p_flashes * 0.2f + p_time + 0.3f;
		Sequence sequence = DOTween.Sequence();
		sequence.AppendCallback(delegate
		{
			Target.Width = 60f;
			Target.Refresh();
		});
		sequence.Join(Target.DOColor(p_color, 0.1f));
		for (int i = 0; i < p_flashes; i++)
		{
			sequence.Append(Target.DOFade(0f, 0.1f));
			sequence.Append(Target.DOFade(0.8f, 0.1f));
		}
		sequence.AppendInterval(p_time);
		sequence.Append(DOTween.To(() => Target.Width, delegate(float x)
		{
			Target.Width = x;
			Target.Refresh();
		}, 16f, 0.1f));
		sequence.Append(Target.gameObject.transform.DORotate(new Vector3(0f, 0f, 360f), 0.7f, RotateMode.LocalAxisAdd));
		sequence.Insert(atPosition, Target.DOFade(0f, 0.5f).SetEase(Ease.OutCubic));
		sequence.OnStart(SequenceReady);
		sequence.OnComplete(SequenceOver);
		sequence.Play();
	}

	private void SequenceOver()
	{
		_CallbackOnComplete(this);
	}

	private void SequenceReady()
	{
		_CallbackOnStart(this);
	}
}
