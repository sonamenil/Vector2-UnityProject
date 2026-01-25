using UnityEngine;

namespace Nekki.Vector.Core.Scripts.Primitive
{
	[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
	public class RectangleDebugDraw : MonoBehaviour
	{
		[SerializeField]
		private readonly Material _Material;

		private float _Border = 1f;

		private Color _Color = new Color(0f, 0f, 0f, 1f);

		protected Rectangle _Base;

		public Color Color
		{
			get
			{
				return _Color;
			}
			set
			{
				_Color = value;
				_Material.SetVector("_Color", _Color);
			}
		}

		public float Border
		{
			get
			{
				return _Border;
			}
			set
			{
				_Border = value;
			}
		}

		public Rectangle Base
		{
			get
			{
				return _Base;
			}
			set
			{
				_Base = value;
			}
		}

		private void Start()
		{
			Update();
		}

		public void Update()
		{
			UnityEngine.Camera main = UnityEngine.Camera.main;
			if (!(_Base == null) && !(main == null))
			{
				Transform transform = base.gameObject.transform;
				transform.localPosition = new Vector3(_Base.MinX, _Base.MinY, 0f);
				transform.localScale = new Vector3(_Base.Size.Width, _Base.Size.Height, 1f);
				Vector3 position = transform.TransformPoint(new Vector3(_Base.MinX, _Base.MinY, 0f));
				position = main.WorldToScreenPoint(position);
				Vector3 position2 = transform.TransformPoint(new Vector3(_Base.MaxX, _Base.MaxY, 0f));
				position2 = main.WorldToScreenPoint(position2);
				_Material.SetVector("_Min", position);
				_Material.SetVector("_Max", position2);
			}
		}
	}
}
