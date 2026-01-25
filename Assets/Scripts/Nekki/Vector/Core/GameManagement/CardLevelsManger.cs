using System.Collections.Generic;
using System.Text;
using Nekki.Yaml;

namespace Nekki.Vector.Core.GameManagement
{
	public class CardLevelsManger
	{
		private const string _FileName = "cards_levels.yaml";

		private YamlDocumentNekki _YamlDocument;

		private static CardLevelsManger _Current;

		public static CardLevelsManger Current
		{
			get
			{
				if (_Current == null)
				{
					_Current = new CardLevelsManger();
				}
				return _Current;
			}
		}

		private CardLevelsManger()
		{
			OpenYamlDocument();
		}

		public static void Reset()
		{
			_Current = null;
		}

		private void OpenYamlDocument()
		{
			_YamlDocument = YamlUtils.OpenYamlFile(VectorPaths.GeneratorDataDefault, "cards_levels.yaml");
		}

		public string GetCardParameter(string p_cardName, string p_key, int p_cardLevel)
		{
			List<string> list = new List<string>();
			list.Add(p_cardName);
			list.Add(p_key);
			list.Add(p_cardLevel.ToString());
			return GetCardParameter(list);
		}

		public string GetCardParameter(List<string> p_path)
		{
			Mapping cardMapping = GetCardMapping(p_path);
			Nekki.Yaml.Node nodeFast = cardMapping.GetNodeFast(p_path[p_path.Count - 1]);
			if (nodeFast == null)
			{
				nodeFast = cardMapping.GetNodeFast("1");
				if (nodeFast == null)
				{
					ShowError(p_path);
					return null;
				}
			}
			return nodeFast.value.ToString();
		}

		private Mapping GetCardMapping(List<string> p_path)
		{
			Mapping mapping = _YamlDocument.GetRoot(0);
			int i = 0;
			for (int num = p_path.Count - 1; i < num; i++)
			{
				Mapping mappingFast = mapping.GetMappingFast(p_path[i]);
				if (mappingFast == null)
				{
					mapping = mapping.GetMappingFast("Default");
					if (mapping == null)
					{
						ShowError(p_path, i + 1);
						return null;
					}
				}
				else
				{
					mapping = mappingFast;
				}
			}
			return mapping;
		}

		private void ShowError(List<string> p_path)
		{
			ShowError(p_path, p_path.Count);
		}

		private void ShowError(List<string> p_path, int p_errorI)
		{
			StringBuilder stringBuilder = new StringBuilder("cards_levels.yaml section not found: ");
			for (int i = 0; i < p_errorI; i++)
			{
				stringBuilder.Append(p_path[i]);
				stringBuilder.Append(".");
			}
			DebugUtils.Dialog(stringBuilder.ToString(), false);
		}
	}
}
