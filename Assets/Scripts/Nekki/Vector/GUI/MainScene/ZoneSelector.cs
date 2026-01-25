using Nekki.Vector.Core.GameManagement;
using Nekki.Vector.Core.Utilites;
using UIFigures;
using UnityEngine;

namespace Nekki.Vector.GUI.MainScene
{
	public class ZoneSelector : MonoBehaviour
	{
		[SerializeField]
		private UICircle _Circle;

		private Zone _Zone;

		private static Color _ActiveColor = ColorUtils.FromHex("d2ebed");

		private static Color _InactiveColor = ColorUtils.FromHex("526778");

		public Zone Zone
		{
			get
			{
				return _Zone;
			}
		}

		public void Init(Zone p_zone)
		{
			_Zone = p_zone;
			Unselect();
		}

		public void Select()
		{
			_Circle.color = _ActiveColor;
		}

		public void Unselect()
		{
			_Circle.color = _InactiveColor;
		}
	}
}
