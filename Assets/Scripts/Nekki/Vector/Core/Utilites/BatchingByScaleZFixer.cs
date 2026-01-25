using UnityEngine;

namespace Nekki.Vector.Core.Utilites
{
	public static class BatchingByScaleZFixer
	{
		public static void FixForBatching(Transform transform)
		{
			if (GetNegativeScalesInHierarchy(transform) % 2 != 0)
			{
				InvertScaleZ(transform);
			}
		}

		private static int GetNegativeScalesInHierarchy(Transform transform)
		{
			if (transform == transform.root)
			{
				return GetOwnNegativeScales(transform);
			}
			return GetOwnNegativeScales(transform) + GetNegativeScalesInHierarchy(transform.parent);
		}

		private static int GetOwnNegativeScales(Transform transform)
		{
			Vector3 localScale = transform.localScale;
			return ((localScale.x < 0f) ? 1 : 0) + ((localScale.y < 0f) ? 1 : 0) + ((localScale.z < 0f) ? 1 : 0);
		}

		private static void InvertScaleZ(Transform transform)
		{
			transform.localScale = Vector3.Scale(transform.localScale, new Vector3(1f, 1f, -1f));
		}
	}
}
