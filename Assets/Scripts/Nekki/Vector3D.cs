using System.Globalization;
using UnityEngine;

namespace Nekki
{
	public class Vector3D
	{
		public static Vector3 getDivisionPoint3D(Vector3 a, Vector3 b, float ratio)
		{
			return new Vector3(a.x + (b.x - a.x) * ratio, a.y + (b.y - a.y) * ratio, a.z + (b.z - a.z) * ratio);
		}

		public static Vector3 GetFromString(string vector3String)
		{
			string[] array = vector3String.Split(' ');
			if (array.Length < 3)
			{
				AdvLog.LogError("Wrong Vector3 string!!! - " + vector3String);
				return Vector3.zero;
			}
			return new Vector3(float.Parse(array[0], CultureInfo.InvariantCulture), float.Parse(array[1], CultureInfo.InvariantCulture), float.Parse(array[2], CultureInfo.InvariantCulture));
		}
	}
}
