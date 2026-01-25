using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UIFigures.Utils
{
	public static class DrawFunctions
	{
		public static void DrawArc(VertexHelper p_pertexHelper, Vector2 p_position, Vector2 p_radius, float p_from, float p_to, int p_segments, Color p_color)
		{
			p_pertexHelper.Clear();
			List<UIVertex> list = new List<UIVertex>();
			float num = (p_to - p_from) / (float)p_segments;
			list.Add(CreateUIVertex(p_position, new Vector2(0.5f, 0.5f), p_color));
			for (int i = 0; i <= p_segments; i++)
			{
				float f = p_from + (float)i * num;
				float num2 = Mathf.Cos(f);
				float num3 = Mathf.Sin(f);
				float x = num2 * p_radius.x + p_position.x;
				float y = num3 * p_radius.y + p_position.y;
				Vector2 p_uv = new Vector2(num2 * 0.5f + 0.5f, num3 * 0.5f + 0.5f);
				list.Add(CreateUIVertex(new Vector2(x, y), p_uv, p_color));
			}
			p_pertexHelper.AddUIVertexStream(list, FigureTopology.TringleFanIndices(p_segments));
		}

		public static void DrawCone(VertexHelper p_vertexHelper, Vector2 p_position, Vector2 p_radius, float p_width, float p_from, float p_to, int p_segments, Color p_color, Color p_endColor)
		{
			p_vertexHelper.Clear();
			List<UIVertex> list = new List<UIVertex>();
			float num = (p_to - p_from) / (float)p_segments;
			Vector2 vector = new Vector2(p_radius.x - p_width, p_radius.y - p_width);
			for (int i = 0; i <= p_segments; i++)
			{
				float f = p_from + (float)i * num;
				float num2 = Mathf.Cos(f);
				float num3 = Mathf.Sin(f);
				Color p_color2 = Color.Lerp(p_color, p_endColor, (float)i / (float)p_segments);
				Vector2 p_position2 = new Vector2(num2 * p_radius.x + p_position.x, num3 * p_radius.y + p_position.y);
				Vector2 p_uv = new Vector2(num2 * 0.5f + 0.5f, num3 * 0.5f + 0.5f);
				list.Add(CreateUIVertex(p_position2, p_uv, p_color2));
				Vector2 p_position3 = new Vector2(num2 * vector.x + p_position.x, num3 * vector.y + p_position.y);
				Vector2 vector2 = new Vector2(1f - p_width / p_radius.x, 1f - p_width / p_radius.y);
				Vector2 p_uv2 = new Vector2(num2 * vector2.x * 0.5f + 0.5f, num3 * vector2.y * 0.5f + 0.5f);
				list.Add(CreateUIVertex(p_position3, p_uv2, p_color2));
			}
			p_vertexHelper.AddUIVertexStream(list, FigureTopology.TringleStripIndices(p_segments * 2));
		}

		public static void DrawLine(VertexHelper p_vertexHelper, List<Vector2> p_points, float p_width, Color p_color, bool p_singleTextureCord = false)
		{
			p_vertexHelper.Clear();
			List<UIVertex> list = new List<UIVertex>();
			List<Vector2> list2 = new List<Vector2>();
			if (p_points.Count < 2)
			{
				return;
			}
			float num = p_points[p_points.Count - 1].x - p_points[0].x;
			for (int i = 1; i < p_points.Count; i++)
			{
				list2.Add(p_points[i] - p_points[i - 1]);
				int index = i - 1;
				Vector2 vector = new Vector2(0f - list2[i - 1].y, list2[i - 1].x);
				list2[index] = vector.normalized;
				if (list2[i - 1].magnitude == 0f && i > 1)
				{
					list2[i - 1] = list2[i - 2];
				}
			}
			for (int num2 = p_points.Count - 2; num2 > 0; num2--)
			{
				if (list2[num2 - 1].magnitude == 0f && num2 > 0)
				{
					list2[num2 - 1] = list2[num2];
				}
			}
			int num3 = 1;
			for (int j = 0; j < p_points.Count; j++)
			{
				Vector2 vector2 = list2[(j <= 0) ? j : (j - 1)];
				Vector2 vector3 = list2[(j >= p_points.Count - 1) ? (j - 1) : j];
				Vector2 vector4 = p_points[j];
				float x = ((!p_singleTextureCord) ? ((float)((j % 2 == 0) ? 1 : 0)) : ((vector4.x - p_points[0].x) / num));
				Vector2 p_uv = new Vector2(x, 0f);
				float num4 = Mathf.Abs(Mathf.Cos((float)Math.PI / 180f * Vector2.Angle(vector2, vector3) / 2f));
				Vector2 vector5 = num3 * (vector2 + vector3).normalized / num4 * p_width;
				if (vector5.magnitude == 0f)
				{
					vector5 = num3 * vector2 * p_width;
					num3 *= -1;
				}
				Vector2 p_uv2 = new Vector2(x, 1f);
				list.Add(CreateUIVertex(vector4 - vector5 / 2f, p_uv, p_color));
				list.Add(CreateUIVertex(vector4 + vector5 / 2f, p_uv2, p_color));
			}
			p_vertexHelper.AddUIVertexStream(list, FigureTopology.TringleStripIndices((p_points.Count - 1) * 2));
		}

		private static UIVertex CreateUIVertex(Vector2 p_position, Vector2 p_uv, Color p_color)
		{
			UIVertex simpleVert = UIVertex.simpleVert;
			simpleVert.position = p_position;
			simpleVert.uv0 = p_uv;
			simpleVert.color = p_color;
			return simpleVert;
		}
	}
}
