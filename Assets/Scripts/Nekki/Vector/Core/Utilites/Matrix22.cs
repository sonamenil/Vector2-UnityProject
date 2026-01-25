namespace Nekki.Vector.Core.Utilites
{
	public class Matrix22
	{
		private float _A;

		private float _B;

		private float _C;

		private float _D;

		public float D
		{
			get
			{
				return _A * _D - _B * _C;
			}
		}

		public Matrix22(float a = 1f, float b = 0f, float c = 0f, float d = 1f)
		{
			Set(a, b, c, d);
		}

		public void Set(float a, float b, float c, float d)
		{
			_A = a;
			_B = b;
			_C = c;
			_D = d;
		}

		public static Matrix22 GenerateInterpolationDeltaMatrix(Matrix22 matrix1, Matrix22 matrix2, int steps)
		{
			Matrix22 matrix3 = new Matrix22(1f, 0f, 0f, 1f);
			return (matrix2 - matrix1) / steps;
		}

		public Matrix22 GetInverseMatrix()
		{
			if (D != 0f)
			{
				Matrix22 matrix = new Matrix22(_D, 0f - _B, 0f - _C, _A);
				return matrix * (1f / D);
			}
			return new Matrix22(1f, 0f, 0f, 1f);
		}

		public static Matrix22 operator +(Matrix22 matrix1, Matrix22 matrix2)
		{
			Matrix22 matrix3 = new Matrix22(1f, 0f, 0f, 1f);
			matrix3._A = matrix1._A + matrix2._A;
			matrix3._B = matrix1._B + matrix2._B;
			matrix3._C = matrix1._C + matrix2._C;
			matrix3._D = matrix1._D + matrix2._D;
			return matrix3;
		}

		public static Matrix22 operator -(Matrix22 matrix1, Matrix22 matrix2)
		{
			Matrix22 matrix3 = new Matrix22(1f, 0f, 0f, 1f);
			matrix3._A = matrix1._A - matrix2._A;
			matrix3._B = matrix1._B - matrix2._B;
			matrix3._C = matrix1._C - matrix2._C;
			matrix3._D = matrix1._D - matrix2._D;
			return matrix3;
		}

		public static Matrix22 operator /(Matrix22 matrix, float arg2)
		{
			Matrix22 matrix2 = new Matrix22(1f, 0f, 0f, 1f);
			matrix2._A = matrix._A / arg2;
			matrix2._B = matrix._B / arg2;
			matrix2._C = matrix._C / arg2;
			matrix2._D = matrix._D / arg2;
			return matrix2;
		}

		public static Matrix22 operator *(Matrix22 matrix, float arg2)
		{
			Matrix22 matrix2 = new Matrix22(1f, 0f, 0f, 1f);
			matrix2._A = matrix._A * arg2;
			matrix2._B = matrix._B * arg2;
			matrix2._C = matrix._C * arg2;
			matrix2._D = matrix._D * arg2;
			return matrix2;
		}

		public static Vector3f operator *(Vector3f vector, Matrix22 matrix)
		{
			Vector3f vector3f = new Vector3f(0f, 0f, 0f);
			vector3f.X = matrix._A * vector.X + matrix._C * vector.Y;
			vector3f.Y = matrix._B * vector.X + matrix._D * vector.Y;
			return vector3f;
		}

		public static Matrix22 operator *(Matrix22 matrix1, Matrix22 matrix2)
		{
			float a = matrix1._A * matrix2._A + matrix1._B * matrix2._C;
			float b = matrix1._A * matrix2._B + matrix1._B * matrix2._D;
			float c = matrix1._C * matrix2._A + matrix1._D * matrix2._C;
			float d = matrix1._C * matrix2._B + matrix1._D * matrix2._D;
			return new Matrix22(a, b, c, d);
		}
	}
}
