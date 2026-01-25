using System;
using UnityEngine;

public class AffineDecomposition
{
	public float ScaleX1;

	public float ScaleY1;

	public float ScaleX2;

	public float ScaleY2;

	public float Angle1;

	public float Angle2;

	public float ScaleMultyX
	{
		get
		{
			return ScaleX1 * ScaleX2;
		}
	}

	public float ScaleMultyY
	{
		get
		{
			return ScaleY1 * ScaleY2;
		}
	}

	public float AngleSum
	{
		get
		{
			return Angle1 + Angle2;
		}
	}

	public AffineDecomposition(Matrix4x4 p_matrix)
	{
		float num = p_matrix[0, 0] * p_matrix[1, 1] - p_matrix[0, 1] * p_matrix[1, 0];
		float num2 = Mathf.Sqrt(p_matrix[0, 0] * p_matrix[0, 0] + p_matrix[0, 1] * p_matrix[0, 1]);
		if (num2 != 0f)
		{
			float scaleY = num / num2;
			float num3 = (p_matrix[0, 0] * p_matrix[1, 0] + p_matrix[0, 1] * p_matrix[1, 1]) / num;
			float f = Mathf.Atan2(p_matrix[0, 1], p_matrix[0, 0]);
			float num4 = num3 / 2f;
			float num5 = 0.5f * Mathf.Atan(num4);
			float f2 = -(float)Math.PI / 4f - num5;
			float num6 = (float)Math.PI / 4f - num5;
			float num7 = Mathf.Sqrt(num4 * num4 + 1f);
			float scaleX = num7 - num4;
			float scaleY2 = num7 + num4;
			Matrix4x4 identity = Matrix4x4.identity;
			identity[0, 0] = Mathf.Cos(f2);
			identity[0, 1] = Mathf.Sin(f2);
			identity[1, 0] = 0f - identity[0, 1];
			identity[1, 1] = identity[0, 0];
			Matrix4x4 identity2 = Matrix4x4.identity;
			identity2[0, 0] = Mathf.Cos(f);
			identity2[0, 1] = Mathf.Sin(f);
			identity2[1, 0] = 0f - identity2[0, 1];
			identity2[1, 1] = identity2[0, 0];
			Matrix4x4 matrix4x = identity2 * identity;
			float num8 = Mathf.Atan2(matrix4x[0, 1], matrix4x[0, 0]);
			ScaleX1 = num2;
			ScaleX2 = scaleX;
			ScaleY1 = scaleY;
			ScaleY2 = scaleY2;
			Angle1 = 57.29578f * num6;
			Angle2 = 57.29578f * num8;
		}
	}
}
