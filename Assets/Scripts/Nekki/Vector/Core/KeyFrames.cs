using System.Collections.Generic;
using UnityEngine;

namespace Nekki.Vector.Core
{
	public class KeyFrames
	{
		protected List<List<Vector3f>> _Data = new List<List<Vector3f>>();

		protected int _Size;

		protected int _ActiveFrame;

		public int Size
		{
			get
			{
				return _Size;
			}
		}

		public int ActiveFrame
		{
			get
			{
				return _ActiveFrame;
			}
		}

		public int SizeMinusActiveFrame
		{
			get
			{
				return _Size - _ActiveFrame;
			}
		}

		public KeyFrames()
		{
			ResizeData(2, 46);
		}

		public void NextActiveFrame()
		{
			_ActiveFrame++;
		}

		public void InterruptFramesSeted()
		{
			_Size = 2;
		}

		public List<Vector3f> GetFrame(int p_index)
		{
			if (_Size <= p_index)
			{
				return null;
			}
			return _Data[p_index];
		}

		public List<Vector3f> GetActiveFrame(int p_shift)
		{
			return _Data[_ActiveFrame + p_shift];
		}

		public void Reset()
		{
			_Size = 0;
			_ActiveFrame = 0;
		}

		public void SetFrames(int p_from, int p_to, Vector3[][] p_data)
		{
			for (int i = p_from; i <= p_to; i++)
			{
				SetFrame(p_data[i]);
			}
		}

		public void SetFrame(Vector3[] p_data)
		{
			_Size++;
			if (_Data.Count < _Size)
			{
				ResizeData(1, p_data.Length);
			}
			List<Vector3f> list = _Data[_Size - 1];
			if (list.Count != p_data.Length)
			{
				DebugUtils.LogError("Input and data size don't match");
				return;
			}
			for (int i = 0; i < list.Count; i++)
			{
				list[i].Set(p_data[i]);
			}
		}

		public void SetFrame(List<Vector3f> p_data)
		{
			_Size++;
			if (_Data.Count < _Size)
			{
				ResizeData(1, p_data.Count);
			}
			List<Vector3f> list = _Data[_Size - 1];
			if (list.Count != p_data.Count)
			{
				DebugUtils.LogError("Input and data size don't match");
				return;
			}
			for (int i = 0; i < list.Count; i++)
			{
				list[i].Set(p_data[i]);
			}
		}

		protected void ResizeData(int p_count, int p_size)
		{
			for (int i = 0; i < p_count; i++)
			{
				List<Vector3f> list = new List<Vector3f>(p_size);
				_Data.Add(list);
				for (int j = 0; j < p_size; j++)
				{
					list.Add(new Vector3f(0f, 0f, 0f));
				}
			}
		}
	}
}
