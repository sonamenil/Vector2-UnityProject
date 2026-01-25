using Nekki.Vector.Core.GameManagement;
using UnityEngine;
using UnityEngine.UI;

namespace Nekki.Vector.GUI.Scenes.Run
{
	public class KeyUI : MonoBehaviour
	{
		[SerializeField]
		private ResolutionImage _Icon;

		[SerializeField]
		private ResolutionImage _Border;

		[SerializeField]
		private Text _CountLabel;

		public KeyItem CurrentItem { get; private set; }

		public void Init(KeyItem p_item)
		{
			CurrentItem = p_item;
			_Icon.SpriteName = CurrentItem.Image;
			_CountLabel.color = CurrentItem.LabelColor;
			RefreshQuantity();
		}

		public void RefreshQuantity()
		{
			string text = CurrentItem.Quantity.ToString();
			if (_CountLabel.text != text)
			{
				_CountLabel.text = text;
			}
		}
	}
}
