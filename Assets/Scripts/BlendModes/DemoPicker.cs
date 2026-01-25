using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;

namespace BlendModes
{
	public class DemoPicker : MonoBehaviour
	{
		public BlendModeEffect TargetBlendMode;

		private Text targetText;

		private void Start()
		{
			targetText = TargetBlendMode.transform.Find("Text Blend Mode").GetComponent<Text>();
			List<Button> list = new List<Button>(22);
			foreach (Transform item in base.transform)
			{
				if ((bool)item.GetComponent<Button>())
				{
					list.Add(item.GetComponent<Button>());
				}
			}
			for (int i = 0; i < 22; i++)
			{
				int num = i;
				list[num].GetComponentInChildren<Text>().text = Regex.Replace(((BlendMode)num).ToString(), "(\\B[A-Z])", " $1");
				if (TargetBlendMode.BlendMode == (BlendMode)num)
				{
					targetText.text = Regex.Replace(((BlendMode)num).ToString(), "(\\B[A-Z])", " $1");
					list[num].GetComponent<Image>().color = Color.green;
				}
			}
		}
	}
}
