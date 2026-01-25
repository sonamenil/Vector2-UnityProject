using System;
using UnityEngine;

namespace Nekki.Vector.Core.Utilites
{
	public static class Matrix
	{
		public static bool IsIdentity(Matrix4x4 p_matrix)
		{
			for (int i = 0; i < 4; i++)
			{
				for (int j = 0; j < 4; j++)
				{
					if (p_matrix[i, j] != Matrix4x4.identity[i, j])
					{
						return false;
					}
				}
			}
			return true;
		}

		public static Vector3 ToRotate(Matrix4x4 Matrix)
		{
			Vector3 result = default(Vector3);
			result.x = Mathf.Atan2(Matrix[2, 1], Matrix[2, 2]);
			result.y = Mathf.Atan2(0f - Matrix[2, 0], Mathf.Sqrt(Matrix[2, 1] * Matrix[2, 1] + Matrix[2, 2] * Matrix[2, 2]));
			result.z = Mathf.Atan2(Matrix[1, 0], Matrix[0, 0]);
			result.x *= 180f / (float)Math.PI;
			result.y *= 180f / (float)Math.PI;
			result.z *= 180f / (float)Math.PI;
			return result;
		}

		public static Vector3 ToPosition(Matrix4x4 Matrix)
		{
			return Matrix.GetColumn(3);
		}

		public static Vector3 ToScale(Matrix4x4 Matrix)
		{
			float x = Mathf.Sqrt(Matrix[0, 0] * Matrix[0, 0] + Matrix[1, 0] * Matrix[1, 0]) * Mathf.Sign(Matrix[0, 0]);
			float y = Mathf.Sqrt(Matrix[0, 1] * Matrix[0, 1] + Matrix[1, 1] * Matrix[1, 1]) * Mathf.Sign(Matrix[1, 1]);
			float z = 1f;
			return new Vector3(x, y, z);
		}

		public static bool ContainsSkew(Matrix4x4 Matrix)
		{
			QRDecomposition qRDecomposition = new QRDecomposition(Matrix.transpose);
			return qRDecomposition.ContainsSkew();
		}
	}
}
