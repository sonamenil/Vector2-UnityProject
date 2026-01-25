using System;
using UnityEngine;

public static class GUIHelper
{
	private static GUIStyle centerAlignedLabel;

	private static GUIStyle rightAlignedLabel;

	public static Rect ClientArea;

	private static void Setup()
	{
		if (centerAlignedLabel == null)
		{
			centerAlignedLabel = new GUIStyle(GUI.skin.label);
			centerAlignedLabel.alignment = TextAnchor.MiddleCenter;
			rightAlignedLabel = new GUIStyle(GUI.skin.label);
			rightAlignedLabel.alignment = TextAnchor.MiddleRight;
		}
	}

	public static void DrawArea(Rect area, bool drawHeader, Action action)
	{
		Setup();
		GUI.Box(area, string.Empty);
		GUILayout.BeginArea(area);
		if (drawHeader)
		{
			DrawCenteredText(SampleSelector.SelectedSample.DisplayName);
			GUILayout.Space(5f);
		}
		if (action != null)
		{
			action();
		}
		GUILayout.EndArea();
	}

	public static void DrawCenteredText(string msg)
	{
		Setup();
		GUILayout.BeginHorizontal();
		GUILayout.FlexibleSpace();
		GUILayout.Label(msg, centerAlignedLabel);
		GUILayout.FlexibleSpace();
		GUILayout.EndHorizontal();
	}

	public static void DrawRow(string key, string value)
	{
		Setup();
		GUILayout.BeginHorizontal();
		GUILayout.Label(key);
		GUILayout.FlexibleSpace();
		GUILayout.Label(value, rightAlignedLabel);
		GUILayout.FlexibleSpace();
		GUILayout.EndHorizontal();
	}
}
