using Nekki.Vector.Core.User;
using UnityEngine;

namespace Nekki.Vector.GUI.Scenes.Run
{
	public class Item : MonoBehaviour
	{
		[SerializeField]
		protected ResolutionImage _Image;

		public UserItem CurrentItem { get; private set; }

		public virtual void Init(UserItem item, float p_scale)
		{
			CurrentItem = item;
			InitSizes(p_scale);
			if (item.GetAttributeByGroupName("ST_VisualData") != null && _Image != null)
			{
				string strValueAttribute = item.GetStrValueAttribute("ST_ItemImage", "ST_VisualData");
				_Image.SpriteName = strValueAttribute;
			}
		}

		protected virtual void InitSizes(float p_scale)
		{
			if (_Image != null)
			{
				ResizeWidgets(p_scale, _Image.rectTransform);
			}
		}

		protected void ResizeWidgets(float p_scale, params RectTransform[] p_widgets)
		{
			for (int i = 0; i < p_widgets.Length; i++)
			{
				p_widgets[i].sizeDelta *= p_scale;
			}
		}
	}
}
