using UnityEngine;
using UnityEngine.UI;

namespace UIFigures
{
	[ExecuteInEditMode]
	public class UIFigure : MaskableGraphic
	{
		public Sprite _Sprite;

		protected Vector2 _BottomLeft;

		protected Vector2 _UpRight;

		public override Texture mainTexture
		{
			get
			{
				return (!(_Sprite == null)) ? _Sprite.texture : Graphic.s_WhiteTexture;
			}
		}

		protected override void OnPopulateMesh(VertexHelper p_vertexHelper)
		{
			_BottomLeft = new Vector2(0f - base.rectTransform.pivot.x, 0f - base.rectTransform.pivot.y);
			_UpRight = new Vector2(1f - base.rectTransform.pivot.x, 1f - base.rectTransform.pivot.y);
			_BottomLeft.x *= base.rectTransform.rect.width;
			_BottomLeft.y *= base.rectTransform.rect.height;
			_UpRight.x *= base.rectTransform.rect.width;
			_UpRight.y *= base.rectTransform.rect.height;
		}

		public void Refresh()
		{
			SetVerticesDirty();
		}
	}
}
