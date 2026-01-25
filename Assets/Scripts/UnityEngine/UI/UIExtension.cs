namespace UnityEngine.UI
{
	public static class UIExtension
	{
		public static Vector3 WorldToCanvasPosition(this Canvas p_canvas, Vector3 p_worldPosition, Camera p_camera = null)
		{
			if (p_camera == null)
			{
				p_camera = Camera.main;
			}
			Vector3 vector = p_camera.WorldToViewportPoint(p_worldPosition);
			RectTransform component = p_canvas.GetComponent<RectTransform>();
			return new Vector2(vector.x * component.sizeDelta.x - component.sizeDelta.x * 0.5f, vector.y * component.sizeDelta.y - component.sizeDelta.y * 0.5f);
		}
	}
}
