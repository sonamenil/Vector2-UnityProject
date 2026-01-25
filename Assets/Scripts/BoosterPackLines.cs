using DG.Tweening;
using UIFigures;
using UnityEngine;
using UnityEngine.UI;

public class BoosterPackLines : MonoBehaviour
{
	[SerializeField]
	private Material _DefaultMaterial;

	[SerializeField]
	private float _Delta;

	[SerializeField]
	private float _Gradient;

	[SerializeField]
	private Image _Image;

	[SerializeField]
	private Image _ImageR;

	[SerializeField]
	private bool _Flag;

	private void Awake()
	{
		UILine[] componentsInChildren = GetComponentsInChildren<UILine>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			Material material = new Material(_DefaultMaterial);
			float num = componentsInChildren[i].Points[componentsInChildren[i].Points.Count - 1].x - componentsInChildren[i].Points[0].x;
			material.SetFloat("_Gradient", _Gradient / num);
			componentsInChildren[i].material = material;
		}
	}

	public void Play()
	{
		_Image.rectTransform.anchoredPosition = new Vector2(-1200f, 0f);
		_Image.rectTransform.DOAnchorPosX(1300f, 1.8f).Play().SetEase(Ease.OutQuad);
	}

	private void Update()
	{
		UILine[] componentsInChildren = GetComponentsInChildren<UILine>();
		if (_Flag)
		{
			_Flag = false;
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				DebugUtils.Log(componentsInChildren[i].gameObject.name + "X:" + componentsInChildren[i].rectTransform.anchoredPosition);
			}
		}
		float x = _Image.rectTransform.anchoredPosition.x;
		float num = x + 450f;
		for (int j = 0; j < componentsInChildren.Length; j++)
		{
			Vector2 anchoredPosition = componentsInChildren[j].rectTransform.anchoredPosition;
			float num2 = componentsInChildren[j].Points[componentsInChildren[j].Points.Count - 1].x - componentsInChildren[j].Points[0].x;
			float num3 = (x - anchoredPosition.x) / num2;
			float num4 = (num - anchoredPosition.x) / num2;
			if (num3 < 0f)
			{
				num3 = 0f;
			}
			if (num3 > 1f)
			{
				num3 = 1f;
			}
			if (num4 < 0f)
			{
				num4 = 0f;
			}
			if (num4 > 1f)
			{
				num4 = 1f;
			}
			componentsInChildren[j].material.SetFloat("_Left", num3);
			componentsInChildren[j].material.SetFloat("_Right", num4);
		}
		Image[] componentsInChildren2 = GetComponentsInChildren<Image>();
		for (int k = 0; k < componentsInChildren2.Length; k++)
		{
			float num5 = componentsInChildren2[k].rectTransform.anchoredPosition.x - componentsInChildren2[k].rectTransform.sizeDelta.x / 2f;
			float num6 = componentsInChildren2[k].rectTransform.anchoredPosition.x + componentsInChildren2[k].rectTransform.sizeDelta.x / 2f;
			if (x < num5 && num6 < num)
			{
				Color color = componentsInChildren2[k].color;
				color.a = 1f;
				if (num5 < x + _Delta)
				{
					color.a = (num5 - x) / _Delta;
				}
				if (num6 > num - _Delta)
				{
					color.a = (num - num6) / _Delta;
				}
				componentsInChildren2[k].color = color;
			}
			else
			{
				Color color2 = componentsInChildren2[k].color;
				color2.a = 0f;
				componentsInChildren2[k].color = color2;
			}
		}
	}
}
