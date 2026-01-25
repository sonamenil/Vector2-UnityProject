using System.Collections.Generic;
using Nekki.Vector.Core.Variables;
using Nekki.Yaml;

namespace Nekki.Vector.Core.GameManagement
{
	public class PostProcessData
	{
		public const string NodeName = "PostProcessData";

		public ReplacementData Replacement { get; private set; }

		public Dictionary<string, Variable> ObjectParams { get; private set; }

		private PostProcessData(Mapping p_node)
		{
			Mapping mapping = p_node.GetMapping("Replacement");
			Mapping mapping2 = p_node.GetMapping("OverrideVariable");
			ParsePlaceholder(mapping);
			ParseObjectParams(mapping2);
		}

		public static PostProcessData Create(Mapping p_node)
		{
			if (p_node == null)
			{
				return null;
			}
			return new PostProcessData(p_node);
		}

		private void ParsePlaceholder(Mapping p_node)
		{
			if (p_node != null)
			{
				Replacement = new ReplacementData(Variable.CreateVariable(p_node.GetText("Name").text, string.Empty), Variable.CreateVariable(p_node.GetText("Filename").text, string.Empty));
			}
		}

		private void ParseObjectParams(Mapping p_node)
		{
			if (p_node == null)
			{
				return;
			}
			ObjectParams = new Dictionary<string, Variable>();
			foreach (Nekki.Yaml.Node item in p_node.nodesInside)
			{
				ObjectParams.Add(item.key, Variable.CreateVariable(item.value.ToString(), string.Empty));
			}
		}
	}
}
