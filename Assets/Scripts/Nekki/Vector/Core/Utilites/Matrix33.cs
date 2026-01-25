namespace Nekki.Vector.Core.Utilites
{
	public class Matrix33
	{
		private float _A11;

		private float _A12;

		private float _A13;

		private float _A21;

		private float _A22;

		private float _A23;

		private float _A31;

		private float _A32;

		private float _A33;

		public float D
		{
			get
			{
				return _A11 * _A22 * _A33 - _A11 * _A23 * _A32 - _A12 * _A21 * _A33 + _A12 * _A23 * _A31 + _A13 * _A21 * _A32 - _A13 * _A22 * _A31;
			}
		}

		public Matrix33(float A11 = 1f, float A12 = 0f, float A13 = 0f, float A21 = 0f, float A22 = 1f, float A23 = 0f, float A31 = 0f, float A32 = 0f, float A33 = 1f)
		{
			Set(A11, A12, A13, A21, A22, A23, A31, A32, A33);
		}

		public void Set(float A11 = 1f, float A12 = 0f, float A13 = 0f, float A21 = 0f, float A22 = 1f, float A23 = 0f, float A31 = 0f, float A32 = 0f, float A33 = 1f)
		{
			_A11 = A11;
			_A12 = A12;
			_A13 = A13;
			_A21 = A21;
			_A22 = A22;
			_A23 = A23;
			_A31 = A31;
			_A32 = A32;
			_A33 = A33;
		}

		public static Matrix33 GenerateInterpolationDeltaMatrix(Matrix33 matrix1, Matrix33 matrix2, int steps)
		{
			Matrix33 matrix3 = new Matrix33(1f, 0f, 0f, 0f, 1f, 0f, 0f, 0f, 1f);
			return (matrix2 - matrix1) / steps;
		}

		public Matrix33 GetInverseMatrix()
		{
			if (D != 0f)
			{
				float d = new Matrix22(_A22, _A23, _A32, _A33).D;
				float a = 0f - new Matrix22(_A21, _A23, _A31, _A33).D;
				float d2 = new Matrix22(_A21, _A22, _A31, _A32).D;
				float a2 = 0f - new Matrix22(_A12, _A13, _A32, _A33).D;
				float d3 = new Matrix22(_A11, _A13, _A31, _A33).D;
				float a3 = 0f - new Matrix22(_A11, _A12, _A31, _A32).D;
				float d4 = new Matrix22(_A12, _A13, _A22, _A23).D;
				float a4 = 0f - new Matrix22(_A11, _A13, _A21, _A23).D;
				float d5 = new Matrix22(_A11, _A12, _A21, _A22).D;
				Matrix33 matrix = new Matrix33(d, a2, d4, a, d3, a4, d2, a3, d5);
				return matrix / D;
			}
			return new Matrix33(1f, 0f, 0f, 0f, 1f, 0f, 0f, 0f, 1f);
		}

		public Matrix22 GetMatrix22()
		{
			return new Matrix22(_A11, _A12, _A21, _A22);
		}

		public static Matrix33 operator +(Matrix33 matrix1, Matrix33 matrix2)
		{
			Matrix33 matrix3 = new Matrix33(1f, 0f, 0f, 0f, 1f, 0f, 0f, 0f, 1f);
			matrix3._A11 = matrix1._A11 + matrix2._A11;
			matrix3._A12 = matrix1._A12 + matrix2._A12;
			matrix3._A13 = matrix1._A13 + matrix2._A13;
			matrix3._A21 = matrix1._A21 + matrix2._A21;
			matrix3._A22 = matrix1._A22 + matrix2._A22;
			matrix3._A23 = matrix1._A23 + matrix2._A23;
			matrix3._A31 = matrix1._A31 + matrix2._A31;
			matrix3._A32 = matrix1._A32 + matrix2._A32;
			matrix3._A33 = matrix1._A33 + matrix2._A33;
			return matrix3;
		}

		public static Matrix33 operator -(Matrix33 matrix1, Matrix33 matrix2)
		{
			Matrix33 matrix3 = new Matrix33(1f, 0f, 0f, 0f, 1f, 0f, 0f, 0f, 1f);
			matrix3._A11 = matrix1._A11 - matrix2._A11;
			matrix3._A12 = matrix1._A12 - matrix2._A12;
			matrix3._A13 = matrix1._A13 - matrix2._A13;
			matrix3._A21 = matrix1._A21 - matrix2._A21;
			matrix3._A22 = matrix1._A22 - matrix2._A22;
			matrix3._A23 = matrix1._A23 - matrix2._A23;
			matrix3._A31 = matrix1._A31 - matrix2._A31;
			matrix3._A32 = matrix1._A32 - matrix2._A32;
			matrix3._A33 = matrix1._A33 - matrix2._A33;
			return matrix3;
		}

		public static Matrix33 operator /(Matrix33 matrix, float arg2)
		{
			Matrix33 matrix2 = new Matrix33(1f, 0f, 0f, 0f, 1f, 0f, 0f, 0f, 1f);
			matrix2._A11 = matrix._A11 / arg2;
			matrix2._A12 = matrix._A12 / arg2;
			matrix2._A13 = matrix._A13 / arg2;
			matrix2._A21 = matrix._A21 / arg2;
			matrix2._A22 = matrix._A22 / arg2;
			matrix2._A23 = matrix._A23 / arg2;
			matrix2._A31 = matrix._A31 / arg2;
			matrix2._A32 = matrix._A32 / arg2;
			matrix2._A33 = matrix._A33 / arg2;
			return matrix2;
		}

		public static Matrix33 operator *(Matrix33 matrix, float arg2)
		{
			Matrix33 matrix2 = new Matrix33(1f, 0f, 0f, 0f, 1f, 0f, 0f, 0f, 1f);
			matrix2._A11 = matrix._A11 * arg2;
			matrix2._A12 = matrix._A12 * arg2;
			matrix2._A13 = matrix._A13 * arg2;
			matrix2._A21 = matrix._A21 * arg2;
			matrix2._A22 = matrix._A22 * arg2;
			matrix2._A23 = matrix._A23 * arg2;
			matrix2._A31 = matrix._A31 * arg2;
			matrix2._A32 = matrix._A32 * arg2;
			matrix2._A33 = matrix._A33 * arg2;
			return matrix2;
		}

		public static Vector3f operator *(Vector3f vector, Matrix33 matrix)
		{
			Vector3f vector3f = new Vector3f(0f, 0f, 0f);
			vector3f.X = matrix._A11 * vector.X + matrix._A21 * vector.Y + matrix._A31 * vector.Z;
			vector3f.Y = matrix._A12 * vector.X + matrix._A22 * vector.Y + matrix._A32 * vector.Z;
			vector3f.Z = matrix._A13 * vector.X + matrix._A23 * vector.Y + matrix._A33 * vector.Z;
			return vector3f;
		}

		public static Matrix33 operator *(Matrix33 matrix1, Matrix33 matrix2)
		{
			float a = matrix1._A11 * matrix2._A11 + matrix1._A12 * matrix2._A21 + matrix1._A13 * matrix2._A31;
			float a2 = matrix1._A11 * matrix2._A12 + matrix1._A12 * matrix2._A22 + matrix1._A13 * matrix2._A32;
			float a3 = matrix1._A11 * matrix2._A13 + matrix1._A12 * matrix2._A23 + matrix1._A13 * matrix2._A33;
			float a4 = matrix1._A21 * matrix2._A11 + matrix1._A22 * matrix2._A21 + matrix1._A23 * matrix2._A31;
			float a5 = matrix1._A21 * matrix2._A12 + matrix1._A22 * matrix2._A22 + matrix1._A23 * matrix2._A32;
			float a6 = matrix1._A21 * matrix2._A13 + matrix1._A22 * matrix2._A23 + matrix1._A23 * matrix2._A33;
			float a7 = matrix1._A31 * matrix2._A11 + matrix1._A32 * matrix2._A21 + matrix1._A33 * matrix2._A31;
			float a8 = matrix1._A31 * matrix2._A12 + matrix1._A32 * matrix2._A22 + matrix1._A33 * matrix2._A32;
			float a9 = matrix1._A31 * matrix2._A13 + matrix1._A32 * matrix2._A23 + matrix1._A33 * matrix2._A33;
			return new Matrix33(a, a2, a3, a4, a5, a6, a7, a8, a9);
		}
	}
}
