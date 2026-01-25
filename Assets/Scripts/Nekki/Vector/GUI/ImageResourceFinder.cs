using UnityEngine;

namespace Nekki.Vector.GUI
{
	public static class ImageResourceFinder
	{
		private static string[] _ImagesPaths = new string[5] { "UI/Textures/Protocol/", "UI/Textures/payment/", "UI/Textures/rewards/", "UI/Textures/misc/", "UI/Portraits/" };

		public static void SetImage(ResolutionImage p_image, string p_imageName, bool p_SetNativeSize = false)
		{
			p_image.SpriteName = p_imageName;
			if (string.IsNullOrEmpty(p_imageName))
			{
				return;
			}
			int i = 0;
			for (int num = _ImagesPaths.Length; i < num; i++)
			{
				if (!(p_image.sprite == null))
				{
					break;
				}
				Sprite sprite = ResourcesAndBundles.Load<Sprite>(_ImagesPaths[i] + p_imageName);
				if (sprite != null)
				{
					p_image.TexturePath = _ImagesPaths[i];
					p_image.SpriteName = p_imageName;
				}
			}
			if (p_SetNativeSize)
			{
				p_image.SetNativeSize();
			}
		}
	}
}
