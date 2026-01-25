using System.Collections.Generic;
using System.Text;
using Nekki.Vector.Core.Variables;
using Nekki.Yaml;

namespace Nekki.Vector.Core.GameManagement
{
	public class YamlDocumentHandler
	{
		private string _Name;

		private YamlDocumentNekki _Document;

		public YamlDocumentHandler(string p_name, YamlDocumentNekki p_document)
		{
			_Name = p_name;
			_Document = p_document;
		}

		public bool HasElement(List<Variable> p_params, bool p_showErrors, int p_defaultable_node)
		{
			Mapping mapping = GetMapping(p_params, false, p_showErrors, p_defaultable_node);
			Nekki.Yaml.Node node = ((mapping == null) ? null : mapping.GetNodeFast(p_params[p_params.Count - 1].ValueString));
			return node != null;
		}

		public bool HasElement(string[] p_params, bool p_showErrors)
		{
			Mapping mapping = GetMapping(p_params, false, p_showErrors);
			Nekki.Yaml.Node node = ((mapping == null) ? null : mapping.GetNodeFast(p_params[p_params.Length - 1]));
			return node != null;
		}

		public string GetElement(List<Variable> p_params, int p_defaultable_node = -1)
		{
			Mapping mapping = GetMapping(p_params, false, true, p_defaultable_node);
			Nekki.Yaml.Node nodeFast = mapping.GetNodeFast(p_params[p_params.Count - 1].ValueString);
			if (nodeFast == null)
			{
				nodeFast = mapping.GetNodeFast("Default");
				if (nodeFast == null)
				{
					if (p_defaultable_node == -1)
					{
						ShowErrorMsg(p_params, p_params.Count);
						return string.Empty;
					}
					List<Variable> list = new List<Variable>(p_params);
					list[p_defaultable_node] = Variable.CreateVariable("Default", string.Empty);
					return GetElement(list);
				}
			}
			return Variable.CreateVariable(nodeFast.value.ToString(), string.Empty).ValueString;
		}

		public string GetElement(params string[] p_params)
		{
			Mapping mapping = GetMapping(p_params, false);
			Nekki.Yaml.Node nodeFast = mapping.GetNodeFast(p_params[p_params.Length - 1]);
			if (nodeFast == null)
			{
				nodeFast = mapping.GetNodeFast("Default");
				if (nodeFast == null)
				{
					ShowErrorMsg(p_params, p_params.Length);
					return string.Empty;
				}
			}
			return Variable.CreateVariable(nodeFast.value.ToString(), string.Empty).ValueString;
		}

		public Mapping GetMapping(List<Variable> p_params, bool p_fullSearch, bool p_showError = true, int p_defaultable_node = -1)
		{
			Mapping mapping = _Document.GetRoot(0);
			int i = 0;
			for (int num = ((!p_fullSearch) ? (p_params.Count - 1) : p_params.Count); i < num; i++)
			{
				Mapping mappingFast = mapping.GetMappingFast(p_params[i].ValueString);
				if (mappingFast == null)
				{
					mapping = mapping.GetMappingFast("Default");
					if (mapping != null)
					{
						continue;
					}
					if (p_defaultable_node == -1)
					{
						if (p_showError)
						{
							ShowErrorMsg(p_params, i + 1);
						}
						return null;
					}
					List<Variable> list = new List<Variable>(p_params);
					list[p_defaultable_node] = Variable.CreateVariable("Default", string.Empty);
					return GetMapping(list, p_fullSearch, p_showError);
				}
				mapping = mappingFast;
			}
			return mapping;
		}

		public Mapping GetMapping(string[] p_params, bool p_fullSearch, bool p_showError = true, int p_defaultable_node = -1)
		{
			Mapping mapping = _Document.GetRoot(0);
			int i = 0;
			for (int num = ((!p_fullSearch) ? (p_params.Length - 1) : p_params.Length); i < num; i++)
			{
				Mapping mappingFast = mapping.GetMappingFast(p_params[i]);
				if (mappingFast == null)
				{
					mapping = mapping.GetMappingFast("Default");
					if (mapping != null)
					{
						continue;
					}
					if (p_defaultable_node == -1)
					{
						if (p_showError)
						{
							ShowErrorMsg(p_params, i + 1);
						}
						return null;
					}
					p_params[p_defaultable_node] = "Default";
					return GetMapping(p_params, p_fullSearch, p_showError);
				}
				mapping = mappingFast;
			}
			return mapping;
		}

		private void ShowErrorMsg(List<Variable> p_params, int p_errorIndex)
		{
			StringBuilder stringBuilder = new StringBuilder(_Name + " section not found: ");
			for (int i = 0; i < p_errorIndex; i++)
			{
				stringBuilder.Append(p_params[i].ValueString);
				if (i < p_errorIndex - 1)
				{
					stringBuilder.Append(".");
				}
			}
			DebugUtils.Dialog(stringBuilder.ToString(), false);
		}

		private void ShowErrorMsg(string[] p_params, int p_errorIndex)
		{
			StringBuilder stringBuilder = new StringBuilder(_Name + " section not found: ");
			for (int i = 0; i < p_errorIndex; i++)
			{
				stringBuilder.Append(p_params[i]);
				if (i < p_errorIndex - 1)
				{
					stringBuilder.Append(".");
				}
			}
			DebugUtils.Dialog(stringBuilder.ToString(), false);
		}
	}
}
