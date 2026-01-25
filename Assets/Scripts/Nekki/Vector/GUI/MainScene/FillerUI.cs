using System;
using Nekki.Vector.Core.GameManagement;
using UnityEngine;

namespace Nekki.Vector.GUI.MainScene
{
	public class FillerUI : MonoBehaviour
	{
		[SerializeField]
		private ResolutionImage _Icon;

		private BuffGroupAttribute _BuffAttributes;

		private Action<FillerUI> _OnTap;

		public BuffGroupAttribute BuffAttributes
		{
			get
			{
				return _BuffAttributes;
			}
		}

		public void Init(BuffGroupAttribute p_byff, Action<FillerUI> p_onTap)
		{
			_OnTap = p_onTap;
			_BuffAttributes = p_byff;
			_Icon.SpriteName = p_byff.BuffImage;
		}

		public void OnFillerTap()
		{
			if (_OnTap != null)
			{
				_OnTap(this);
			}
		}
	}
}
