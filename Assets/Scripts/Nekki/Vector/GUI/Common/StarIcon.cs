using Nekki.Vector.Core.GameManagement;
using Nekki.Vector.Core.Utilites;
using UnityEngine;

namespace Nekki.Vector.GUI.Common
{
	public class StarIcon : MonoBehaviour
	{
		[SerializeField]
		private ResolutionImage _IconBorder;

		[SerializeField]
		private ResolutionImage _IconFiller;

		private float _FillAmount;

		private float _FillDuration;

		public float FillDuration
		{
			get
			{
				return _FillDuration;
			}
			set
			{
				_FillDuration = value;
			}
		}

		public void Init(int p_rarity = 0)
		{
			_IconFiller.fillAmount = 0f;
			SetColor(p_rarity);
		}

		private void Update()
		{
			if (_IconFiller.fillAmount < _FillAmount)
			{
				_IconFiller.fillAmount += 1f / _FillDuration * Time.deltaTime;
			}
		}

		public void SetCompleteStarIcon(float p_percent, bool p_forced = false)
		{
			_IconFiller.gameObject.SetActive(true);
			_FillAmount = p_percent;
			if ((!Manager.IsRun && !StarsManager.IsShowSequence) || p_forced)
			{
				_IconFiller.fillAmount = p_percent;
			}
		}

		private void SetColor(int p_rarity)
		{
			switch (p_rarity)
			{
			case 1:
				_IconBorder.color = ColorUtils.FromHex("7B9DAD");
				_IconFiller.color = ColorUtils.FromHex("7B9DAD");
				break;
			case 2:
				_IconBorder.color = ColorUtils.FromHex("DA660E");
				_IconFiller.color = ColorUtils.FromHex("DA660E");
				break;
			case 3:
				_IconBorder.color = ColorUtils.FromHex("C11527");
				_IconFiller.color = ColorUtils.FromHex("C11527");
				break;
			}
		}
	}
}
