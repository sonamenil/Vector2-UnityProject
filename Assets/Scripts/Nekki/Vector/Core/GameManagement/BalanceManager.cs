using System.Collections.Generic;
using Nekki.Vector.Core.Variables;
using Nekki.Yaml;

namespace Nekki.Vector.Core.GameManagement
{
	public class BalanceManager
	{
		private const string _FileName = "balance.yaml";

		private YamlDocumentHandler _Handler;

		private static BalanceManager _Current;

		public static BalanceManager Current
		{
			get
			{
				if (_Current == null)
				{
					_Current = new BalanceManager();
				}
				return _Current;
			}
		}

		private BalanceManager()
		{
			_Handler = new YamlDocumentHandler("Balance", YamlUtils.OpenYamlFile(VectorPaths.GeneratorDataDefault, "balance.yaml"));
		}

		public static void Init()
		{
			if (_Current == null)
			{
				_Current = new BalanceManager();
			}
		}

		public static void Reset()
		{
			_Current = null;
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
