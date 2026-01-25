using UnityEngine;
using UnityEngine.UI;

namespace Nekki.Vector.GUI
{
	public class MultiImageButton : Button
	{
		private Graphic[] Graphics
		{
			get
			{
				return base.targetGraphic.transform.GetComponentsInChildren<Graphic>();
			}
		}

		protected override void DoStateTransition(SelectionState p_state, bool p_instant)
		{
			if (base.transition == Transition.ColorTint)
			{
				if (base.gameObject.activeInHierarchy)
				{
					Color colorByState = GetColorByState(p_state);
					ColorTween(colorByState * base.colors.colorMultiplier, p_instant);
				}
			}
			else
			{
				base.DoStateTransition(p_state, p_instant);
			}
		}

		private Color GetColorByState(SelectionState p_state)
		{
			switch (p_state)
			{
			case SelectionState.Normal:
				return base.colors.normalColor;
			case SelectionState.Highlighted:
				return base.colors.highlightedColor;
			case SelectionState.Pressed:
				return base.colors.pressedColor;
			case SelectionState.Disabled:
				return base.colors.disabledColor;
			default:
				return Color.white;
			}
		}

		private void ColorTween(Color p_targetColor, bool p_instant)
		{
			if (!(base.targetGraphic == null))
			{
				Graphic[] graphics = Graphics;
				foreach (Graphic graphic in graphics)
				{
					graphic.CrossFadeColor(p_targetColor, p_instant ? 0f : base.colors.fadeDuration, true, true);
				}
			}
		}
	}
}
