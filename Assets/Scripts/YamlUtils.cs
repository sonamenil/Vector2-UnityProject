using System;
using Nekki.Yaml;
using UnityEngine;

public static class YamlUtils
{
	public enum OpenYamlType
	{
		Normal = 0,
		ForcedResourced = 1,
		ForcedExternal = 2
	}

	public static YamlDocumentNekki OpenYamlFileIfExists(string p_path, string p_file = "", OpenYamlType p_mode = OpenYamlType.Normal)
	{
		string p_path2 = p_path + ((!(p_file != string.Empty)) ? string.Empty : ("/" + p_file));
		string text = string.Empty;
		switch (p_mode)
		{
		case OpenYamlType.Normal:
			text = ResourceManager.GetText(p_path2);
			break;
		case OpenYamlType.ForcedExternal:
			text = ResourceManager.GetTextFromExternal(p_path2);
			break;
		case OpenYamlType.ForcedResourced:
			text = ResourceManager.GetTextFromResources(p_path2);
			break;
		}
		if (string.IsNullOrEmpty(text))
		{
			return null;
		}
		return YamlDocumentNekki.FromYamlContent(text);
	}

	public static YamlDocumentNekki OpenYamlFile(string p_path, string p_file = "", OpenYamlType p_mode = OpenYamlType.Normal)
	{
		string p_path2 = p_path + ((!(p_file != string.Empty)) ? string.Empty : ("/" + p_file));
		switch (p_mode)
		{
		case OpenYamlType.Normal:
			return YamlDocumentNekki.FromYamlContent(ResourceManager.GetText(p_path2));
		case OpenYamlType.ForcedExternal:
			return YamlDocumentNekki.FromYamlContent(ResourceManager.GetTextFromExternal(p_path2));
		case OpenYamlType.ForcedResourced:
			return YamlDocumentNekki.FromYamlContent(ResourceManager.GetTextFromResources(p_path2));
		default:
			return null;
		}
	}

	public static YamlDocumentNekki OpenYamlFile(TextAsset p_asset)
	{
		return YamlDocumentNekki.FromYamlContent((!p_asset) ? string.Empty : p_asset.text);
	}

	public static int GetIntValue(Scalar p_value, int p_def = 0)
	{
		if (p_value == null)
		{
			return p_def;
		}
		return int.Parse(p_value.text);
	}

	public static float GetFloatValue(Scalar p_value, float p_def = 0f)
	{
		if (p_value == null)
		{
			return p_def;
		}
		return float.Parse(p_value.text);
	}

	public static bool GetBoolValue(Scalar p_value, bool p_def = false)
	{
		if (p_value == null)
		{
			return p_def;
		}
		return int.Parse(p_value.text) == 1;
	}

	public static string GetStringValue(Scalar p_value, string p_def = "")
	{
		if (p_value == null)
		{
			return p_def;
		}
		return p_value.text;
	}

	public static T GetEnumValue<T>(Scalar p_value, T p_def)
	{
		if (p_value == null)
		{
			return p_def;
		}
		try
		{
			return (T)Enum.Parse(typeof(T), p_value.text, true);
		}
		catch
		{
			return p_def;
		}
	}

	public static bool IsMapping(this Node p_node)
	{
		return p_node.typeNode == "Mapping";
	}

	public static bool IsSequence(this Node p_node)
	{
		return p_node.typeNode == "Sequence";
	}

	public static bool IsScalar(this Node p_node)
	{
		return p_node.typeNode == "Scalar";
	}

	public static Node GetNodeFast(this Mapping p_node, string p_name)
	{
		foreach (Node item in p_node)
		{
			if (item.key == p_name)
			{
				return item;
			}
		}
		return null;
	}

	public static Mapping GetMappingFast(this Mapping p_node, string p_name)
	{
		foreach (Node item in p_node)
		{
			if (item.IsMapping() && item.key == p_name)
			{
				return (Mapping)item;
			}
		}
		return null;
	}

	public static bool IsFileValid(string p_path)
	{
		try
		{
			OpenYamlFile(p_path, string.Empty, OpenYamlType.ForcedExternal);
			return true;
		}
		catch (Exception ex)
		{
			Debug.LogErrorFormat("[Validation]: error to load file - {0}, exception - {1}", p_path, ex.Message);
			return false;
		}
	}
}
