using System.Xml;
using DigitalRuby.ThunderAndLightning;
using Nekki.Vector.Core.Utilites;
using UnityEngine;

namespace Nekki.Vector.Core.Runners
{
	public class LightningRunner : MatrixSupport
	{
		private const string _PrefabName = "LightningBoltPathPrefab";

		private GameObject _GameObject;

		private LightningBoltPathScript _Lighting;

		private string _Layer;

		private XmlNode _PropertiesNode;

		public LightningRunner(string p_name, float p_x, float p_y, Element p_elements, XmlNode p_node)
			: base(p_x, p_y, p_elements, p_node)
		{
			_TypeClass = TypeRunner.Lightning;
			_Name = p_name;
			_Layer = XmlUtils.ParseString(p_node.Attributes["Layer"], string.Empty);
			_PropertiesNode = p_node["Properties"];
		}

		private void ParseProperties()
		{
			ParsePath(_PropertiesNode["Static"]["Path"]);
			XmlNode xmlNode = _PropertiesNode["Static"];
			if (xmlNode["StartColor"] != null)
			{
				_Lighting.LightningTintColor = ColorUtils.FromHex(xmlNode["StartColor"].Attributes["Color"].Value);
			}
			if (xmlNode["GlowColor"] != null)
			{
				_Lighting.GlowTintColor = ColorUtils.FromHex(xmlNode["GlowColor"].Attributes["Color"].Value);
			}
			if (xmlNode["Parts"] != null)
			{
				_Lighting.Generations = Mathf.Clamp(XmlUtils.ParseInt(xmlNode["Parts"].Attributes["Value"]), 1, 8);
			}
			if (xmlNode["LightningCount"] != null)
			{
				_Lighting.CountRange.Minimum = XmlUtils.ParseInt(xmlNode["LightningCount"].Attributes["Min"], _Lighting.CountRange.Minimum);
				_Lighting.CountRange.Maximum = XmlUtils.ParseInt(xmlNode["LightningCount"].Attributes["Max"], _Lighting.CountRange.Maximum);
			}
			_PropertiesNode = null;
		}

		private void ParsePath(XmlNode p_node)
		{
			if (p_node == null)
			{
				return;
			}
			for (int i = 0; i < p_node.ChildNodes.Count; i++)
			{
				int num = XmlUtils.ParseInt(p_node.ChildNodes[i].Attributes["X"]);
				int num2 = XmlUtils.ParseInt(p_node.ChildNodes[i].Attributes["Y"]);
				GameObject gameObject = new GameObject();
				gameObject.transform.localPosition = new Vector3(num, num2, 0f);
				if (i < 2)
				{
					_Lighting.LightningPath.List[i] = gameObject;
				}
				else
				{
					_Lighting.LightningPath.List.Add(gameObject);
				}
				gameObject.transform.SetParent(_GameObject.transform, false);
			}
		}

		public override bool Render()
		{
			return false;
		}

		protected override void GenerateObject()
		{
			base.GenerateObject();
			_CachedTransform.localPosition = _DefautPosition;
			GameObject @object = GlobalLoad.GetObject("Prefab/Scene/Run/" + _Name, string.Empty);
			if (@object != null)
			{
				_GameObject = Object.Instantiate(@object);
				_Lighting = _GameObject.GetComponent<LightningBoltPathScript>();
				ParseProperties();
			}
			else
			{
				_GameObject = GameObject.CreatePrimitive(PrimitiveType.Sphere);
				_GameObject.transform.localScale = new Vector3(1f, 1f, 0f);
				_GameObject.GetComponent<Renderer>().material = null;
			}
			_GameObject.transform.SetParent(base.UnityObject.transform, false);
			_GameObject.name = _Name;
			UpdateLayer();
			Transform();
		}

		private void UpdateLayer()
		{
			_Lighting.SortingLayerName = _Layer;
			if (_Lighting.LightningOriginParticleSystem != null)
			{
				_Lighting.LightningOriginParticleSystem.GetComponent<ParticleSystemRenderer>().sortingLayerName = _Layer;
			}
			if (_Lighting.LightningDestinationParticleSystem != null)
			{
				_Lighting.LightningDestinationParticleSystem.GetComponent<ParticleSystemRenderer>().sortingLayerName = _Layer;
			}
		}
	}
}
