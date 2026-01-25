using System.Collections.Generic;
using System.Xml;
using UnityEngine;

namespace Nekki.Vector.Core.Trigger
{
	public class TemplateModule
	{
		private static TemplateModule current = new TemplateModule();

		private Dictionary<string, Template> templates = new Dictionary<string, Template>();

		private XmlDocument document;

		private TextAsset asset;

		public TemplateModule()
		{
			document = XmlUtils.OpenXMLDocument(VectorPaths.RunDataTemplates, "trigger_templates.xml");
			parseTemplates();
		}

		public static XmlNode getTemplateXmlNode(string p_name)
		{
			if (!current.templates.ContainsKey(p_name))
			{
				return null;
			}
			return current.templates[p_name].mainTemplateNode;
		}

		public void parseTemplates()
		{
			XmlNode xmlNode = document["Templates"];
			foreach (XmlNode childNode in xmlNode.ChildNodes)
			{
				string value = childNode.Attributes["Name"].Value;
				Template value2 = new Template(childNode);
				templates[value] = value2;
			}
		}

		public static XmlNode getTemplateActionsXML(string p_name)
		{
			string[] array = p_name.Split('.');
			Template template = current.templates[array[0]];
			foreach (XmlNode childNode in template.mainTemplateNode.ChildNodes)
			{
				if (childNode.Name.Equals("Loop") && childNode.Attributes["Name"].Value.Equals(array[1]))
				{
					return childNode["Actions"];
				}
			}
			return null;
		}

		public static XmlNode getTemplateConditionsXML(string p_name)
		{
			string[] array = p_name.Split('.');
			Template template = current.templates[array[0]];
			foreach (XmlNode childNode in template.mainTemplateNode.ChildNodes)
			{
				if (childNode.Name.Equals("Loop") && childNode.Attributes["Name"].Value.Equals(array[1]))
				{
					return childNode["Conditions"];
				}
			}
			return null;
		}

		public static XmlNode getTemplateLoopXML(string p_name)
		{
			string[] array = p_name.Split('.');
			Template template = current.templates[array[0]];
			foreach (XmlNode childNode in template.mainTemplateNode.ChildNodes)
			{
				if (childNode.Name.Equals("Loop") && childNode.Attributes["Name"].Value.Equals(array[1]))
				{
					return childNode;
				}
			}
			return null;
		}

		public static XmlNode getTemplateEventsXml(string p_name)
		{
			string[] array = p_name.Split('.');
			Template template = current.templates[array[0]];
			if (template == null)
			{
				return null;
			}
			foreach (XmlNode childNode in template.mainTemplateNode.ChildNodes)
			{
				if (childNode.Name.Equals("Loop") && childNode.Attributes["Name"].Value.Equals(array[1]))
				{
					return childNode["Events"];
				}
			}
			return null;
		}
	}
}
