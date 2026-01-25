using System.Collections.Generic;
using Nekki.Vector.Core.Variables;
using Nekki.Yaml;

namespace Nekki.Vector.Core.GameManagement
{
	public class ZoneBalanceManager : ZoneResource<ZoneBalanceManager>
	{
		private YamlDocumentHandler _Handler;

		protected override string ResourceId
		{
			get
			{
				return "ZoneBalance";
			}
		}

		protected override void Parse()
		{
			_Handler = new YamlDocumentHandler("ZoneBalance", YamlUtils.OpenYamlFile(VectorPaths.GeneratorData, base.FilePath));
		}

		public bool HasBalanceElement(List<Variable> p_params, bool p_showErrors = true, int p_defaultable_node = -1)
		{
			return _Handler.HasElement(p_params, p_showErrors, p_defaultable_node);
		}

		public string GetBalance(List<Variable> p_params, int p_defaultable_node = -1)
		{
			return _Handler.GetElement(p_params, p_defaultable_node);
		}

		public string GetBalance(params string[] p_params)
		{
			return _Handler.GetElement(p_params);
		}

		public Mapping GetBalanceMapping(List<Variable> p_params, bool p_fullSearch = true)
		{
			return _Handler.GetMapping(p_params, p_fullSearch);
		}

		public Mapping GetBalanceMapping(params string[] p_params)
		{
			return _Handler.GetMapping(p_params, true);
		}

		public Mapping GetBalanceMapping(string[] p_params, bool p_fullsearch)
		{
			return _Handler.GetMapping(p_params, p_fullsearch);
		}
	}
}
