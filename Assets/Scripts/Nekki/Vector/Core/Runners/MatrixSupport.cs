using System.Xml;
using Nekki.Vector.Core.Utilites;
using UnityEngine;

namespace Nekki.Vector.Core.Runners
{
	public abstract class MatrixSupport : Runner
	{
		protected Matrix4x4 _Transformation = Matrix4x4.identity;

		protected MatrixSupport(float p_x, float p_y, Element p_elements, XmlNode p_node)
			: base(p_x, p_y, p_elements)
		{
			ParseTransform(p_node);
		}

		private void ParseTransform(XmlNode Node)
		{
			if (Node != null && Node["Properties"] != null && Node["Properties"]["Static"] != null && Node["Properties"]["Static"]["Matrix"] != null)
			{
				XmlNode xmlNode = Node["Properties"]["Static"]["Matrix"];
				_Transformation[0, 0] = float.Parse(xmlNode.Attributes["A"].Value);
				_Transformation[0, 1] = float.Parse(xmlNode.Attributes["B"].Value);
				_Transformation[1, 0] = float.Parse(xmlNode.Attributes["C"].Value);
				_Transformation[1, 1] = float.Parse(xmlNode.Attributes["D"].Value);
				_Transformation[2, 2] = 1f;
				_Transformation[3, 3] = 1f;
				_DefautPosition.X += float.Parse(xmlNode.Attributes["Tx"].Value);
				_DefautPosition.Y += float.Parse(xmlNode.Attributes["Ty"].Value);
			}
		}

		protected virtual void Transform()
		{
			if (!(_SupportUnityObject == null))
			{
				AffineDecomposition affineDecomposition = new AffineDecomposition(_Transformation);
				_SupportUnityObject.transform.localScale = new Vector3(affineDecomposition.ScaleX1, affineDecomposition.ScaleY1, 1f);
				_SupportUnityObject.transform.Rotate(0f, 0f, affineDecomposition.Angle1);
				_CachedTransform.localScale = new Vector3(affineDecomposition.ScaleX2, affineDecomposition.ScaleY2, 1f);
				_CachedTransform.Rotate(0f, 0f, affineDecomposition.Angle2);
			}
		}

		protected override void GenerateObject()
		{
			base.GenerateObject();
			if (!Matrix.IsIdentity(_Transformation))
			{
				Matrix4x4 transpose = _Transformation.transpose;
				QRDecomposition qRDecomposition = new QRDecomposition(transpose);
				if (qRDecomposition.ContainsSkew())
				{
					_SupportUnityObject = new GameObject
					{
						name = "Support"
					};
					_SupportUnityObject.transform.SetParent(_CachedTransform, false);
					return;
				}
				Matrix4x4 rotation = qRDecomposition.Rotation;
				Quaternion quaternion = default(Quaternion);
				int num = ((rotation[0, 0] * rotation[1, 1] - rotation[0, 1] * rotation[1, 0] > 0f) ? 1 : (-1));
				quaternion = ((num >= 0) ? Quaternion.LookRotation(rotation.GetColumn(2), rotation.GetColumn(1)) : Quaternion.LookRotation(-rotation.GetColumn(2), rotation.GetColumn(1)));
				_CachedTransform.localRotation = quaternion;
				_CachedTransform.localScale = new Vector3(qRDecomposition.ScaleX, qRDecomposition.ScaleY, 1f);
			}
		}

		public override void TransformRotateX(float p_angle)
		{
			Vector3 localEulerAngles = _CachedTransform.localEulerAngles;
			localEulerAngles.x += p_angle;
			_CachedTransform.localEulerAngles = localEulerAngles;
		}

		public override void TransformRotateY(float p_angle)
		{
			Vector3 localEulerAngles = _CachedTransform.localEulerAngles;
			localEulerAngles.y += p_angle;
			_CachedTransform.localEulerAngles = localEulerAngles;
		}

		public override void TransformRotateZ(float p_angle)
		{
			Vector3 localEulerAngles = _CachedTransform.localEulerAngles;
			localEulerAngles.z += p_angle;
			_CachedTransform.localEulerAngles = localEulerAngles;
		}

		public override void DestroyUnityObjects()
		{
			base.DestroyUnityObjects();
			if (_SupportUnityObject != null)
			{
				Object.DestroyObject(_SupportUnityObject);
				_SupportUnityObject = null;
			}
		}
	}
}
