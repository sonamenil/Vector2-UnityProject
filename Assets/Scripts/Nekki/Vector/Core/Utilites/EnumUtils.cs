using System;
using UnityEngine;

namespace Nekki.Vector.Core.Utilites
{
	public static class EnumUtils
	{
		public static T Parse<T>(string p_str, T p_def)
		{
			try
			{
				return (T)Enum.Parse(typeof(T), p_str, true);
			}
			catch (Exception ex)
			{
				Debug.Log("Trying to parse unknown Enum: " + p_str + "Error: " + ex.ToString());
				return p_def;
			}
		}
	}
}
