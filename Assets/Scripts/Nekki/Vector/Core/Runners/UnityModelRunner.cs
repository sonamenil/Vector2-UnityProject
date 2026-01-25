using System.Collections.Generic;
using System.Xml;
using Nekki.Vector.Core.Scripts;
using UnityEngine;

namespace Nekki.Vector.Core.Runners
{
	public class UnityModelRunner : MatrixSupport
	{
		private class RendererData
		{
			private MeshRenderer _Renderer;

			private int _Order;

			public RendererData(MeshRenderer p_renderer)
			{
				_Renderer = p_renderer;
				_Order = ++Counter;
			}

			public void Update(string p_sortingLayerName)
			{
				_Renderer.sortingLayerName = p_sortingLayerName;
				_Renderer.sortingOrder = _Order;
			}
		}

		public static int Counter;

		private float _Factor;

		private GameObject _UnityModel;

		private List<RendererData> _Renderers = new List<RendererData>();

		private string _Layer = string.Empty;

		public UnityModelRunner(string name, float p_x, float p_y, float factor, Element p_elements, XmlNode p_node)
			: base(p_x, p_y, p_elements, p_node)
		{
			_TypeClass = TypeRunner.UnityModel;
			_Name = name;
			_Factor = factor;
			_Layer = XmlUtils.ParseString(p_node.Attributes["Layer"], string.Empty);
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
				_UnityModel = Object.Instantiate(@object);
			}
			else
			{
				_UnityModel = GameObject.CreatePrimitive(PrimitiveType.Sphere);
				_UnityModel.transform.localScale = new Vector3(1f, 1f, 0f);
				_UnityModel.GetComponent<Renderer>().material = null;
			}
			_UnityModel.transform.SetParent(base.UnityObject.transform, false);
			_UnityModel.name = _Name;
			if (Mathf.Abs(_Factor) > float.Epsilon)
			{
				_UnityModel.AddComponent<Parallax>().SetFactor(_Factor, _Factor);
			}
			List<MeshRenderer> list = new List<MeshRenderer>();
			_UnityModel.GetComponentsInChildren(true, list);
			foreach (MeshRenderer item in list)
			{
				_Renderers.Add(new RendererData(item));
			}
			UpdateLayer();
			Transform();
		}

		private void UpdateLayer()
		{
			foreach (RendererData renderer in _Renderers)
			{
				renderer.Update(_Layer);
			}
		}

		public override void TransformLayer(string p_layer)
		{
			_Layer = p_layer;
			UpdateLayer();
		}
	}
}
