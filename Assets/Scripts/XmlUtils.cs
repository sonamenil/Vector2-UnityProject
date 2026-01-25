using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Xml;
using Nekki.Vector.Core;
using Nekki.Vector.Core.DataValidation;
using UnityEngine;

public static class XmlUtils
{
	public enum OpenXmlType
	{
		Normal = 0,
		ForcedResourced = 1,
		ForcedExternal = 2
	}

	public const string Comment = "#comment";

	public static bool IsNodeComment(XmlNode p_node)
	{
		return p_node.NodeType == XmlNodeType.Comment;
	}

	public static int ParseInt(XmlAttribute p_attr, int p_defVal = 0)
	{
		if (p_attr == null)
		{
			return p_defVal;
		}
		return int.Parse(p_attr.Value);
	}

	public static long ParseLong(XmlAttribute p_attr, long p_defVal = 0)
	{
		if (p_attr == null)
		{
			return p_defVal;
		}
		return long.Parse(p_attr.Value);
	}

	public static uint ParseUint(XmlAttribute p_attr, uint p_defVal = 0)
	{
		if (p_attr == null)
		{
			return p_defVal;
		}
		return uint.Parse(p_attr.Value);
	}

	public static float ParseFloat(XmlAttribute p_attr, float p_defVal = 0f)
	{
		if (p_attr == null)
		{
			return p_defVal;
		}
		return float.Parse(p_attr.Value);
	}

	public static bool ParseBool(XmlAttribute p_attr, bool p_defVal = false)
	{
		if (p_attr == null)
		{
			return p_defVal;
		}
		return int.Parse(p_attr.Value) > 0;
	}

	public static string ParseString(XmlAttribute p_attr, string p_defVal = null)
	{
		if (p_attr == null)
		{
			return p_defVal;
		}
		return p_attr.Value;
	}

	public static T ParseEnum<T>(XmlAttribute p_attr, T p_def)
	{
		if (p_attr == null)
		{
			return p_def;
		}
		try
		{
			return (T)Enum.Parse(typeof(T), p_attr.Value, true);
		}
		catch
		{
			return p_def;
		}
	}

	public static Resolution ParseResolution(XmlAttribute resolution, Resolution p_def)
	{
        var resolutions = Screen.resolutions;
		Resolution returnresolution = p_def;
		
		if (resolution == null)
		{
			return returnresolution;
		}
		try
		{
            Regex regex = new Regex(@"(\d+)\s*x\s*(\d+)\s*@\s*(\d+(?:\.\d+)?)");
            Match match = regex.Match(resolution.Value);

            if (match.Success)
            {
                int x = int.Parse(match.Groups[1].Value);
                int y = int.Parse(match.Groups[2].Value);
                double hz = double.Parse(match.Groups[3].Value);

                for (int i = 0; i < resolutions.Length; i++)
                {
                    if (resolutions[i].width == x && resolutions[i].height == y && (resolutions[i].refreshRateRatio.value == hz || resolutions[i].refreshRateRatio.value + 1 == hz))
                    {
                        returnresolution = resolutions[i];
                    }
                }

            }
			else
			{
                return returnresolution;
            }
        }
		catch
		{
            return returnresolution;
        }

		return returnresolution;
    }

	public static XmlDocument OpenXMLDocumentFromBytes(byte[] p_bytes, bool p_ignoreComments = true)
	{
		XmlReaderSettings xmlReaderSettings = new XmlReaderSettings();
		xmlReaderSettings.IgnoreComments = p_ignoreComments;
		try
		{
			MemoryStream stream = new MemoryStream(p_bytes);
			using (XmlReader p_reader = XmlReader.Create(stream, xmlReaderSettings))
			{
				return OpenXMLDocument(p_reader);
			}
		}
		catch
		{
			return null;
		}
	}

	public static XmlDocument OpenXMLDocument(string p_path, string p_file = "", OpenXmlType p_openType = OpenXmlType.Normal, bool p_ignoreComments = true)
	{
		try
		{
			string p_path2 = p_path + ((!(p_file != string.Empty)) ? string.Empty : ("/" + p_file));
			XmlReaderSettings xmlReaderSettings = new XmlReaderSettings();
			xmlReaderSettings.IgnoreComments = p_ignoreComments;
			switch (p_openType)
			{
			case OpenXmlType.Normal:
				if (p_path.StartsWith(VectorPaths.CurrentStorage))
				{
					return OpenXMLDocFromString(ResourceManager.GetTextFromExternal(p_path2), xmlReaderSettings);
				}
				return OpenXMLDocFromString(ResourceManager.GetText(p_path2), xmlReaderSettings);
			case OpenXmlType.ForcedExternal:
				return OpenXMLDocFromString(ResourceManager.GetTextFromExternal(p_path2), xmlReaderSettings);
			case OpenXmlType.ForcedResourced:
				return OpenXMLDocFromString(ResourceManager.GetTextFromResources(p_path2), xmlReaderSettings);
			}
		}
		catch
		{
			return null;
		}
		return null;
	}

	public static XmlDocument OpenXMLDocFromTextAsset(TextAsset p_text, bool p_ignoreComments = true)
	{
		XmlReaderSettings xmlReaderSettings = new XmlReaderSettings();
		xmlReaderSettings.IgnoreComments = p_ignoreComments;
		using (XmlReader p_reader = XmlReader.Create(new StringReader(p_text.text), xmlReaderSettings))
		{
			XmlDocument xmlDocument = OpenXMLDocument(p_reader);
			if (xmlDocument == null)
			{
				DebugUtils.Dialog("Error open XML", true);
			}
			return xmlDocument;
		}
	}

	private static XmlDocument OpenXMLDocFromString(string p_data, XmlReaderSettings p_settings)
	{
		using (XmlReader p_reader = XmlReader.Create(new StringReader(p_data), p_settings))
		{
			XmlDocument xmlDocument = OpenXMLDocument(p_reader);
			if (xmlDocument == null)
			{
				DebugUtils.Dialog("Error read XML", true);
			}
			return xmlDocument;
		}
	}

	private static XmlDocument OpenXMLDocument(XmlReader p_reader)
	{
		try
		{
			XmlDocument xmlDocument = new XmlDocument();
			xmlDocument.Load(p_reader);
			return xmlDocument;
		}
		catch
		{
			return null;
		}
	}

	public static void CopyXmlFromResources(string p_from, string p_to)
	{
		XmlDocument xmlDocument = OpenXMLDocument(p_from, string.Empty);
		xmlDocument.Save(p_to);
	}

	public static XmlDocument OpenXMLDocumentAndCheckHash(string p_path, string p_file = "", OpenXmlType p_openType = OpenXmlType.Normal, bool p_ignoreComments = true)
	{
		XmlDocument xmlDocument = OpenXMLDocument(p_path, p_file, p_openType, p_ignoreComments);
		if (xmlDocument != null && ((!VectorPaths.UsingResources && p_openType == OpenXmlType.Normal) || p_openType == OpenXmlType.ForcedExternal))
		{
			string p_path2 = p_path + ((!(p_file != string.Empty)) ? string.Empty : ("/" + p_file));
			UserDataValidator.CheckFileHash(xmlDocument, p_path2);
		}
		return xmlDocument;
	}

	public static void SaveDocumentAndUpdateHash(XmlDocument p_doc, string p_filePath)
	{
		p_doc.Save(p_filePath);
		UserDataValidator.UpdateFileHash(p_doc, p_filePath);
	}

	public static void CopyXmlFromResourcesAndUpdateHash(string p_from, string p_to)
	{
		XmlDocument xmlDocument = OpenXMLDocument(p_from, string.Empty);
		xmlDocument.Save(p_to);
		UserDataValidator.UpdateFileHash(xmlDocument, p_to);
	}

	public static void CopyXmlAndTrimSpaces(string p_from, string p_to)
	{
		XmlDocument xmlDocument = OpenXMLDocument(p_from, string.Empty, OpenXmlType.ForcedExternal);
		if (xmlDocument == null)
		{
			DebugUtils.LogError("[XmlUtils]: try to trim incorrect xml - " + p_from);
			return;
		}
		XmlWriterSettings xmlWriterSettings = new XmlWriterSettings();
		xmlWriterSettings.Indent = false;
		xmlWriterSettings.NewLineChars = string.Empty;
		using (XmlWriter xmlWriter = XmlWriter.Create(p_to, xmlWriterSettings))
		{
			xmlDocument.Save(xmlWriter);
		}
	}

	public static void TrimSpacesFromXmlInDirectory(string p_path, bool p_recursively = false)
	{
		string[] files = Directory.GetFiles(p_path, "*.xml", p_recursively ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly);
		string[] array = files;
		foreach (string text in array)
		{
			CopyXmlAndTrimSpaces(text, text);
		}
	}

	public static bool IsFileValid(string p_path)
	{
		using (XmlTextReader xmlTextReader = new XmlTextReader(p_path))
		{
			try
			{
				while (xmlTextReader.Read())
				{
				}
			}
			catch (Exception ex)
			{
				Debug.LogErrorFormat("[Validation]: error to load file - {0}, exception - {1}", p_path, ex.Message);
				return false;
			}
		}
		return true;
	}
}
