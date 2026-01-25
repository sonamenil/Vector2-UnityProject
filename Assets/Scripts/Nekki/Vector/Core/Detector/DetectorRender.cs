using System.Collections.Generic;
using Nekki.Vector.Core.Game;
using Nekki.Vector.Core.Scripts.Geometry;
using UnityEngine;

namespace Nekki.Vector.Core.Detector
{
	public class DetectorRender
	{
		private Color _Color = new Color(0f, 0f, 0f, 1f);

		private GameObject _Layer;

		private bool _IsDebug;

		private GameObject _Object = new GameObject("Detector");

		private Edge _Edge = new Edge();

		private List<GameObject> _Lines = new List<GameObject>();

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
				_Object.transform.SetParent((!(_Layer != null)) ? null : _Layer.transform, false);
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
					_Object.SetActive(value);
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

		public Edge Edge
		{
			get
			{
				return _Edge;
			}
		}

		public DetectorRender()
		{
			_Object.AddComponent<Nekki.Vector.Core.Scripts.Geometry.Edge>();
			_Object.SetActive(false);
		}

		public void Add(Vector3fLine p_line)
		{
			p_line.Stroke = 1.75f;
			p_line.Color = _Color;
			GameObject gameObject = new GameObject("Line");
			Nekki.Vector.Core.Scripts.Geometry.Edge edge = gameObject.AddComponent<Nekki.Vector.Core.Scripts.Geometry.Edge>();
			edge.Base = p_line;
			edge.SortingLayerName = "Model";
			edge.SortingOrder = 0;
			_Lines.Add(gameObject);
			gameObject.transform.parent = _Object.transform;
		}
	}
}
