using DG.Tweening;
using UIFigures;
using UnityEngine;

namespace Nekki.Vector.GUI.Common
{
	public class BaseCardUISlot : MonoBehaviour
	{
		[SerializeField]
		private UICircle _Background;

		[SerializeField]
		private UICircleBorder _Border;

		[SerializeField]
		private ResolutionImage _Icon;

		public void Init(string p_iconName)
		{
			_Icon.SpriteName = p_iconName;
		}

		public void SetContentColor(Color p_color, float p_duration = 0f)
		{
			if (p_duration > 1E-05f)
			{
				_Border.DOColor(p_color, p_duration);
				_Icon.DOColor(p_color, p_duration);
			}
			else
			{
				_Border.color = p_color;
				_Icon.color = p_color;
			}
		}
	}
}
