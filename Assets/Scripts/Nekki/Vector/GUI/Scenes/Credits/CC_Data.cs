using System.Text;
using Nekki.Yaml;
using UnityEngine;
using UnityEngine.UI;

namespace Nekki.Vector.GUI.Scenes.Credits
{
	public class CC_Data : CreditsContent
	{
		private string _Value;

		public CC_Data(Mapping p_node)
		{
			StringBuilder stringBuilder = new StringBuilder();
			foreach (Scalar item in p_node.GetSequence("Data"))
			{
				stringBuilder.AppendLine(item.text);
			}
			_Value = stringBuilder.ToString();
		}

		public override void GenerateContentUI(Transform p_parent)
		{
			GameObject gameObject = CreateGO<CC_Data>(p_parent);
			gameObject.name = string.Format("CreditsContent: Data={0}", _Value.Replace('\n', ',').TrimEnd(','));
			gameObject.GetComponent<Text>().text = _Value;
		}
	}
}
