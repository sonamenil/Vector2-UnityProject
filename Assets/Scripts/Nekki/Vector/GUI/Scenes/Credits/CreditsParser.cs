using System.Collections.Generic;
using Nekki.Vector.Core;
using Nekki.Vector.Core.Localization;
using Nekki.Yaml;
using UnityEngine;

namespace Nekki.Vector.GUI.Scenes.Credits
{
	public static class CreditsParser
	{
		private static string CreditsFile
		{
			get
			{
				return VectorPaths.Credits + "/" + ((LocalizationManager.CurrentLanguage != SystemLanguage.Russian) ? "int.yaml" : "ru.yaml");
			}
		}

		public static List<CreditsBlock> GetCreditsContent()
		{
			YamlDocumentNekki yamlDocumentNekki = YamlUtils.OpenYamlFile(CreditsFile, string.Empty);
			if (yamlDocumentNekki == null || yamlDocumentNekki.GetRoot(0) == null)
			{
				return null;
			}
			return CreditsBlock.CreateList(yamlDocumentNekki.GetRoot(0).GetSequence("Credits"));
		}
	}
}
