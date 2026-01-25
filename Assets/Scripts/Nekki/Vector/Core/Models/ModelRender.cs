using System.Collections.Generic;
using Nekki.Vector.Core.Game;
using Nekki.Vector.Core.Node;
using Nekki.Vector.Core.Scripts.Geometry;
using Nekki.Vector.Core.Scripts.Projection;
using UnityEngine;

namespace Nekki.Vector.Core.Models
{
	public class ModelRender
	{
		private const string _SortingLayerName = "Model";

		private const int _SortingOrder_Capsules = 0;

		private const int _SortingOrder_Meshes = 1;

		private Color _Color;

		private GameObject _Layer;

		private bool _IsDebug;

		private bool _IsEnabled = true;

		private GameObject _Object = new GameObject("Object");

		private GameObject _CapsulesContainer = new GameObject("Capsules");

		private List<Capsule> _Capsules = new List<Capsule>();

		private GameObject _Mesh = new GameObject("Mesh");

		private MeshNode _MeshNode = new MeshNode();

		public Color Color
		{
			get
			{
				return _Color;
			}
			set
			{
				_Color = value;
			}
		}

		public string Name
		{
			get
			{
				return _Object.name;
			}
			set
			{
				_Object.name = value;
			}
		}

		public GameObject Layer
		{
			get
			{
				return _Layer;
			}
			set
			{
				_Layer = value;
				_Object.transform.SetParent((!(_Layer == null)) ? _Layer.transform : null, false);
			}
		}

		public bool IsDebug
		{
			get
			{
				return _IsDebug;
			}
			set
			{
				if (Settings.Visual.Model.Visible)
				{
					_IsDebug = value;
				}
			}
		}

		public bool IsEnabled
		{
			get
			{
				return _IsEnabled;
			}
			set
			{
				_IsEnabled = value;
				if (_Mesh != null)
				{
					_Mesh.SetActive(value);
				}
				foreach (Capsule capsule in _Capsules)
				{
					capsule.gameObject.SetActive(value);
				}
			}
		}

		public bool MeshVisible
		{
			set
			{
				if (_Mesh != null)
				{
					_Mesh.SetActive(value);
				}
			}
		}

		public GameObject Object
		{
			get
			{
				return _Object;
			}
		}

		public List<Capsule> Capsules
		{
			get
			{
				return _Capsules;
			}
		}

		public ModelRender()
		{
			_CapsulesContainer.transform.localPosition = new Vector2(0f, 0f);
			_Mesh.transform.localPosition = new Vector2(0f, 0f);
			_CapsulesContainer.transform.SetParent(_Object.transform, false);
			_Mesh.transform.SetParent(_Object.transform, false);
		}

		public void Init()
		{
			_MeshNode.Init();
			if (_MeshNode.Triangles.Length == 0)
			{
				_MeshNode = null;
				UnityEngine.Object.Destroy(_Mesh);
				return;
			}
			Nekki.Vector.Core.Scripts.Projection.Mesh mesh = _Mesh.AddComponent<Nekki.Vector.Core.Scripts.Projection.Mesh>();
			mesh.Base = _MeshNode;
			mesh.SortingLayerName = "Model";
			mesh.SortingOrder = 1;
		}

		public void Add(List<ModelNode> p_nodes)
		{
			foreach (ModelNode p_node in p_nodes)
			{
				Add(p_node);
			}
		}

		public void Add(ModelNode p_node)
		{
		}

		public void Add(List<ModelLine> p_lines)
		{
		}

		public void Add(ModelLine p_line)
		{
			GameObject gameObject = null;
			switch (p_line.Type)
			{
			case "Capsule":
			{
				gameObject = new GameObject(p_line.Name);
				Capsule capsule = gameObject.AddComponent<Capsule>();
				capsule.Base = p_line;
				capsule.SortingLayerName = "Model";
				capsule.SortingOrder = 0;
				_Capsules.Add(capsule);
				gameObject.transform.SetParent(_CapsulesContainer.transform, false);
				break;
			}
			}
			if (gameObject != null)
			{
				gameObject.SetActive(_IsEnabled);
			}
		}

		public void Add(ModelNode p_node1, ModelNode p_node2, ModelNode p_node3)
		{
			_MeshNode.AddTriangle(p_node1, p_node2, p_node3);
		}

		public void Add(Model p_model)
		{
		}

		public void Render()
		{
			for (int i = 0; i < _Capsules.Count; i++)
			{
				_Capsules[i].Render();
			}
		}
	}
}
