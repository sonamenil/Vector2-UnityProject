using System;
using System.Collections.Generic;

namespace Nekki.Vector.Core.Transformations
{
	public class TransformationUtils
	{
		public static void CalcBezierPoints(List<Vector3f> p_points, int p_frames, ref List<Vector3f> p_result)
		{
			Vector3f vector3f = p_points[0];
			Vector3f vector3f2 = p_points[p_points.Count - 1];
			List<Vector3f> list = new List<Vector3f>();
			for (int i = 1; i < p_points.Count - 1; i++)
			{
				list.Add(p_points[i]);
			}
			p_result.Clear();
			float num = 0f;
			for (int j = 0; j < p_frames; j++)
			{
				double num2 = Math.Pow(1f - num, list.Count + 1);
				double num3 = num2 * (double)vector3f.X;
				double num4 = num2 * (double)vector3f.Y;
				int num5 = 0;
				for (num5 = 0; num5 < list.Count; num5++)
				{
					num2 = (double)(list.Count + 1) * Math.Pow(1f - num, list.Count - num5) * Math.Pow(num, num5 + 1);
					num3 += num2 * (double)list[num5].X;
					num4 += num2 * (double)list[num5].Y;
				}
				num2 = Math.Pow(num, num5 + 1);
				num3 += num2 * (double)vector3f2.X;
				num4 += num2 * (double)vector3f2.Y;
				p_result.Add(new Vector3f((float)num3, (float)num4, 0f));
				num += 1f / (float)p_frames;
			}
			p_result.Add(vector3f2);
		}

		public static void CalcSinusPoints(List<Vector3f> points, int frames, string quartersEnum, ref List<Vector3f> result)
		{
			if (points.Count < 2)
			{
				return;
			}
			result.Clear();
			Vector3f vector3f = points[0];
			Vector3f vector3f2 = points[1];
			List<int> quartersSequence = GetQuartersSequence(quartersEnum);
			List<Vector3f> list = new List<Vector3f>();
			int result2 = 0;
			if (!int.TryParse(quartersEnum[0].ToString(), out result2))
			{
				DebugUtils.Dialog("Detected an object with Sin moveInterval/SizeInterval with invalid \"Quarters\" field.\"Quarters\" field is enum, 1-st symbol is digit expected.", false);
				return;
			}
			int frames2 = frames / result2;
			int num = 0;
			for (int i = 0; i < quartersSequence.Count; i++)
			{
				if (quartersSequence[i] == 2 || quartersSequence[i] == 3)
				{
					num--;
				}
				else if (quartersSequence[i] == 1 || quartersSequence[i] == 4)
				{
					num++;
				}
			}
			if (num <= 0)
			{
				DebugUtils.Dialog("There's an object with Sin moveInterval/SizeInterval with bad quarters sequence.\nQuarters sequence has more (or equal) decreasing quarters than increasing quarters.\nShould be more increasing quarters.", false);
				return;
			}
			Vector3f vector3f3 = vector3f2 - vector3f;
			vector3f3.X /= num;
			vector3f3.Y /= num;
			vector3f3.Z /= num;
			for (int j = 0; j < quartersSequence.Count; j++)
			{
				Vector3f vector3f4 = vector3f;
				if (j > 0)
				{
					vector3f4 = list[list.Count - 1];
				}
				if (quartersSequence[j] == 2 || quartersSequence[j] == 3)
				{
					list.Add(vector3f4 - vector3f3);
				}
				else if (quartersSequence[j] == 1 || quartersSequence[j] == 4)
				{
					list.Add(vector3f4 + vector3f3);
				}
			}
			for (int k = 0; k < result2; k++)
			{
				int num2 = quartersSequence[k];
				Vector3f start = vector3f;
				if (k > 0)
				{
					start = list[k - 1];
				}
				Vector3f destination = list[k];
				bool slowAtStart = num2 == 2 || num2 == 4;
				AddQuarter(frames2, start, destination, slowAtStart, ref result);
			}
			result.Add(vector3f2);
		}

		public static void CalcSinusAngles(float angle, int frames, string quartersEnum, ref List<float> result)
		{
			result.Clear();
			float num = 0f;
			List<int> quartersSequence = GetQuartersSequence(quartersEnum);
			List<float> list = new List<float>();
			int result2 = 0;
			if (!int.TryParse(quartersEnum[0].ToString(), out result2))
			{
				DebugUtils.Dialog("Detected an object with Sin rotateInterval with invalid \"Quarters\" field.\"Quarters\" field is enum, 1-st symbol is digit expected.", false);
				return;
			}
			int frames2 = frames / result2;
			int num2 = 0;
			for (int i = 0; i < quartersSequence.Count; i++)
			{
				if (quartersSequence[i] == 2 || quartersSequence[i] == 3)
				{
					num2--;
				}
				else if (quartersSequence[i] == 1 || quartersSequence[i] == 4)
				{
					num2++;
				}
			}
			if (num2 <= 0)
			{
				DebugUtils.Dialog("There's an object with Sin rotateInterval with bad quarters sequence.\nQuarters sequence has more (or equal) decreasing quarters than increasing quarters.\nShould be more increasing quarters.", false);
				return;
			}
			float num3 = (angle - num) / (float)num2;
			for (int j = 0; j < quartersSequence.Count; j++)
			{
				float num4 = num;
				if (j > 0)
				{
					num4 = list[list.Count - 1];
				}
				if (quartersSequence[j] == 2 || quartersSequence[j] == 3)
				{
					list.Add(num4 - num3);
				}
				else if (quartersSequence[j] == 1 || quartersSequence[j] == 4)
				{
					list.Add(num4 + num3);
				}
			}
			for (int k = 0; k < result2; k++)
			{
				int num5 = quartersSequence[k];
				float start = num;
				if (k > 0)
				{
					start = list[k - 1];
				}
				float destination = list[k];
				bool slowAtStart = num5 == 2 || num5 == 4;
				AddQuarter(frames2, start, destination, slowAtStart, ref result);
			}
			result.Add(angle);
			for (int num6 = result.Count - 1; num6 > 0; num6--)
			{
				List<float> list2;
				List<float> list3 = (list2 = result);
				int index;
				int index2 = (index = num6);
				float num7 = list2[index];
				list3[index2] = num7 - result[num6 - 1];
			}
			if (result[0] == 0f)
			{
				result.RemoveAt(0);
			}
		}

		private static List<int> GetQuartersSequence(string quartersEnum)
		{
			List<int> list = new List<int>();
			switch (quartersEnum)
			{
			case "1QuarterAcc":
				list.Add(4);
				break;
			case "1QuarterDec":
				list.Add(1);
				break;
			case "2Quarters":
				list.Add(4);
				list.Add(1);
				break;
			case "2QuartersFastSlowFast":
				list.Add(1);
				list.Add(4);
				break;
			case "3Quarters":
				list.Add(3);
				list.Add(4);
				list.Add(1);
				break;
			}
			return list;
		}

		private static void AddQuarter(int frames, Vector3f start, Vector3f destination, bool slowAtStart, ref List<Vector3f> result)
		{
			double num = 0.0;
			if (slowAtStart)
			{
				num = -Math.PI / 2.0;
			}
			Vector3f vector3f = destination - start;
			double num2 = Math.PI / 2.0 / (double)frames;
			for (int i = 0; i < frames; i++)
			{
				double num3 = Math.Sin(num);
				if (slowAtStart)
				{
					num3 += 1.0;
				}
				double num4 = num3 * (double)vector3f.X + (double)start.X;
				double num5 = num3 * (double)vector3f.Y + (double)start.Y;
				result.Add(new Vector3f((float)num4, (float)num5, 0f));
				num += num2;
			}
		}

		private static void AddQuarter(int frames, float start, float destination, bool slowAtStart, ref List<float> result)
		{
			double num = 0.0;
			if (slowAtStart)
			{
				num = -Math.PI / 2.0;
			}
			float num2 = destination - start;
			double num3 = Math.PI / 2.0 / (double)frames;
			for (int i = 0; i < frames; i++)
			{
				double num4 = Math.Sin(num);
				if (slowAtStart)
				{
					num4 += 1.0;
				}
				float item = (float)(num4 * (double)num2 + (double)start);
				result.Add(item);
				num += num3;
			}
		}
	}
}
