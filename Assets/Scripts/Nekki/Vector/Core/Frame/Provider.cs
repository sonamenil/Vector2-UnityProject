using UnityEngine;

namespace Nekki.Vector.Core.Frame
{
	public class Provider
	{
		private Vector3[][] _Data;

		public Vector3[][] Data
		{
			get
			{
				return _Data;
			}
		}

		public int Length
		{
			get
			{
				return _Data.Length;
			}
		}

		public Vector3[] this[int p_index]
		{
			get
			{
				return _Data[p_index];
			}
		}

		public Provider(int Count)
		{
			_Data = new Vector3[Count][];
		}

		public void Add(Vector3[] p_item, int p_index)
		{
			_Data[p_index] = p_item;
		}
	}
}
