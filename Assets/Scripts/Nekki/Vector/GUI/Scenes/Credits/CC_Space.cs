using Nekki.Yaml;
using UnityEngine;
using UnityEngine.UI;

namespace Nekki.Vector.GUI.Scenes.Credits
{
	public class CC_Space : CreditsContent
	{
		private float _Value = 1f;

		public CC_Space(Mapping p_node)
		{
			_Value = YamlUtils.GetFloatValue(p_node.GetText("Space"), _Value);
		}

		public override void GenerateContentUI(Transform p_parent)
		{
			GameObject gameObject = CreateGO<CC_Space>(p_parent);
			gameObject.name = string.Format("CreditsContent: Space={0}", _Value);
			gameObject.GetComponent<LayoutElement>().minHeight *= _Value;
		}
	}
}
