using System.Collections.Generic;
using Nekki.Vector.Core.Variables;
using Nekki.Yaml;

namespace Nekki.Vector.Core.GameManagement
{
	public class PostProcess : ZoneResource<PostProcess>
	{
		private Preset _PostProcessPreset;

		protected override string ResourceId
		{
			get
			{
				return "PostProcess";
			}
		}

		protected override void Parse()
		{
			YamlDocumentNekki yamlDocumentNekki = YamlUtils.OpenYamlFile(VectorPaths.GeneratorData, base.FilePath);
			_PostProcessPreset = Preset.Create(yamlDocumentNekki.GetRoot(0).GetMapping("Root"));
		}

		public bool GetPostProcessData(ref ReplacementData p_replacement, ref Dictionary<string, Variable> p_objectParams, ref bool p_postpone)
		{
			p_postpone = _PostProcessPreset.PostProcess(ref p_replacement, ref p_objectParams) == Pick.PickResult.Postpone;
			return p_replacement != null;
		}
	}
}
