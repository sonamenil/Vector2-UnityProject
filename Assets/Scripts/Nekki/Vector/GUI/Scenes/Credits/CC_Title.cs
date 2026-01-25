using Nekki.Yaml;
using UnityEngine;
using UnityEngine.UI;

namespace Nekki.Vector.GUI.Scenes.Credits
{
	public class CC_Title : CreditsContent
	{
		private string _Value;

		public CC_Title(Mapping p_node)
		{
			_Value = YamlUtils.GetStringValue(p_node.GetText("Title"), string.Empty);
		}

		public override void GenerateContentUI(Transform p_parent)
		{
			GameObject gameObject = CreateGO<CC_Title>(p_parent);
			gameObject.name = string.Format("CreditsContent: Title={0}", _Value);
			gameObject.GetComponent<Text>().text = _Value;
		}
	}
}
