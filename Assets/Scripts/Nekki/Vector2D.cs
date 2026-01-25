using UnityEngine;

namespace Nekki
{
	public class Vector2D
	{
		public static void getEquationLine(Vector2 pointA, Vector2 pointB, EquationLine equation)
		{
			float num = Vector2.Distance(pointA, pointB);
			equation.a = (pointA.y - pointB.y) / num;
			equation.b = (pointB.x - pointA.x) / num;
			equation.c = 0f - (equation.a * pointA.x + equation.b * pointA.y);
		}

		public static EquationLine getEquationLine(Vector2 pointA, Vector2 pointB)
		{
			EquationLine equationLine = null;
			getEquationLine(pointA, pointB, equationLine);
			return equationLine;
		}

		private static float ABSFloat(float value)
		{
			return (!(value < 0f)) ? value : (0f - value);
		}

		public static bool getIntersectOfCapsulesPoint2D(Vector2 a1, Vector2 a2, float radiusA, Vector2 b1, Vector2 b2, float radiusB, ref Vector2 resultA, ref Vector2 resultB, EquationLine elineA, EquationLine elineB)
		{
			Vector2 vector = a1;
			Vector2 vector2 = a2;
			Vector2 vector3 = b1;
			Vector2 vector4 = b2;
			float num = radiusA + radiusB;
			if (num == 0f)
			{
				if (getIntersectPoint2D(vector3, vector4, vector, vector2, resultA))
				{
					resultB = resultA;
					return true;
				}
				return false;
			}
			EquationLine equationLine = ((elineB == null) ? getEquationLine(b1, b2) : elineB);
			float num2 = equationLine.a * vector.x + equationLine.b * vector.y + equationLine.c;
			float num3 = equationLine.a * vector2.x + equationLine.b * vector2.y + equationLine.c;
			if (0f <= num2 * num3 && num < ABSFloat(num2) && num < ABSFloat(num3))
			{
				return false;
			}
			EquationLine equationLine2 = ((elineA == null) ? getEquationLine(a1, a2) : elineA);
			float num4 = equationLine2.a * vector3.x + equationLine2.b * vector3.y + equationLine2.c;
			float num5 = equationLine2.a * vector4.x + equationLine2.b * vector4.y + equationLine2.c;
			if (0f <= num4 * num5 && num < ABSFloat(num4) && num < ABSFloat(num5))
			{
				return false;
			}
			if (num4 * num5 < 0f && num2 * num3 < 0f)
			{
				float num6 = num4 / (num4 - num5);
				resultA = vector4 - vector3;
				resultA *= num6;
				resultA += vector3;
				resultB = resultA;
				return true;
			}
			if (isDistanceStrike(num2, num, equationLine, vector, ref resultB, vector3, vector4))
			{
				resultA = vector;
				return true;
			}
			if (isDistanceStrike(num3, num, equationLine, vector2, ref resultB, vector3, vector4))
			{
				resultA = vector2;
				return true;
			}
			if (isDistanceStrike(num4, num, equationLine2, vector3, ref resultB, vector, vector2))
			{
				resultA = vector3;
				resultB = vector3;
				return true;
			}
			if (isDistanceStrike(num5, num, equationLine2, vector4, ref resultB, vector, vector2))
			{
				resultA = vector4;
				resultB = vector4;
				return true;
			}
			return false;
		}

		public static bool isDistanceStrike(float distance, float Reach, EquationLine equation, Vector2 point, ref Vector2 basePoint, Vector2 start, Vector2 end)
		{
			if (ABSFloat(distance) <= Reach)
			{
				basePoint.x = point.x - distance * equation.a;
				basePoint.y = point.y - distance * equation.b;
				return (((end.x <= basePoint.x && basePoint.x <= start.x) || (start.x <= basePoint.x && basePoint.x <= end.x)) && ((end.y <= basePoint.y && basePoint.y <= start.y) || (start.y <= basePoint.y && basePoint.y <= end.y))) || (point.x - start.x) * (point.x - start.x) + (point.y - start.y) * (point.y - start.y) <= Reach * Reach || (point.x - end.x) * (point.x - end.x) + (point.y - end.y) * (point.y - end.y) <= Reach * Reach;
			}
			return false;
		}

		public static bool getIntersectPoint2D(Vector2 A, Vector2 B, Vector2 C, Vector2 D, Vector2 result)
		{
			if ((A.x == B.x && A.y == B.y) || (C.x == D.x && C.y == D.y))
			{
				return false;
			}
			float num = B.x - A.x;
			float num2 = B.y - A.y;
			float num3 = D.x - C.x;
			float num4 = D.y - C.y;
			float num5 = A.x - C.x;
			float num6 = A.y - C.y;
			float num7 = num4 * num - num3 * num2;
			float num8 = num3 * num6 - num4 * num5;
			float num9 = num * num6 - num2 * num5;
			if (num7 == 0f)
			{
				if (num8 != 0f && num9 != 0f)
				{
					return false;
				}
				float x;
				float x2;
				if (A.x < B.x)
				{
					x = A.x;
					x2 = B.x;
				}
				else
				{
					x = B.x;
					x2 = A.x;
				}
				float x3;
				float x4;
				if (C.x < D.x)
				{
					x3 = C.x;
					x4 = D.x;
				}
				else
				{
					x3 = D.x;
					x4 = C.x;
				}
				if (x > x4 || x3 > x2)
				{
					return false;
				}
				if (A.y < B.y)
				{
					x = A.y;
					x2 = B.y;
				}
				else
				{
					x = B.y;
					x2 = A.y;
				}
				if (C.y < D.y)
				{
					x3 = C.y;
					x4 = D.y;
				}
				else
				{
					x3 = D.y;
					x4 = C.y;
				}
				if (x > x4 || x3 > x2)
				{
					return false;
				}
				num7 = 1f;
			}
			num8 /= num7;
			num9 /= num7;
			if (num8 >= 0f && num8 <= 1f && num9 >= 0f && num9 <= 1f)
			{
				result.x = A.x + num8 * (B.x - A.x);
				result.y = A.y + num8 * (B.y - A.y);
				return true;
			}
			return false;
		}

		public static float GetAngle2DDegreeSigned(Vector2 impulse, Vector3 axis)
		{
			return GetAngle2DRadianSigned(impulse, axis) * 57.29578f;
		}

		public static float GetAngle2DRadianSigned(Vector2 a, Vector2 b)
		{
			float num = a.x * b.y - a.y * b.x;
			float num2 = a.x * b.x + a.y * b.y;
			float num3 = 1f / Mathf.Sqrt(num * num + num2 * num2);
			return Mathf.Atan2(num * num3, num2 * num3);
		}
	}
}
