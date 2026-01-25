using System;
using GamepadInput;
using UnityEngine;

public class DemoScript : MonoBehaviour
{
	private void Examples()
	{
		GamePad.GetButtonDown(GamePad.Button.A, GamePad.Index.One);
		GamePad.GetAxis(GamePad.Axis.LeftStick, GamePad.Index.One);
		GamePad.GetTrigger(GamePad.Trigger.RightTrigger, GamePad.Index.One);
		GamepadState state = GamePad.GetState(GamePad.Index.One);
		MonoBehaviour.print("A: " + state.A);
	}

	private void OnGUI()
	{
		GUILayout.BeginArea(new Rect(0f, 20f, Screen.width, Screen.height));
		GUILayout.BeginHorizontal();
		GUILayout.FlexibleSpace();
		DrawLabels();
		for (int i = 0; i < 5; i++)
		{
			DrawState((GamePad.Index)i);
		}
		GUILayout.FlexibleSpace();
		GUILayout.EndHorizontal();
		GUILayout.EndArea();
	}

	private void DrawState(GamePad.Index controller)
	{
		GUILayout.Space(45f);
		GUILayout.BeginVertical();
		GamepadState state = GamePad.GetState(controller);
		GUILayout.Label("Gamepad " + controller);
		GUILayout.Label(string.Empty + state.A);
		GUILayout.Label(string.Empty + state.B);
		GUILayout.Label(string.Empty + state.X);
		GUILayout.Label(string.Empty + state.Y);
		GUILayout.Label(string.Empty + state.Start);
		GUILayout.Label(string.Empty + state.Back);
		GUILayout.Label(string.Empty + state.LeftShoulder);
		GUILayout.Label(string.Empty + state.RightShoulder);
		GUILayout.Label(string.Empty + state.Left);
		GUILayout.Label(string.Empty + state.Right);
		GUILayout.Label(string.Empty + state.Up);
		GUILayout.Label(string.Empty + state.Down);
		GUILayout.Label(string.Empty + state.LeftStick);
		GUILayout.Label(string.Empty + state.RightStick);
		GUILayout.Label(string.Empty);
		GUILayout.Label(string.Empty + Math.Round(state.LeftTrigger, 2));
		GUILayout.Label(string.Empty + Math.Round(state.RightTrigger, 2));
		GUILayout.Label(string.Empty);
		GUILayout.Label(string.Empty + state.LeftStickAxis);
		GUILayout.Label(string.Empty + state.rightStickAxis);
		GUILayout.Label(string.Empty + state.dPadAxis);
		GUILayout.EndVertical();
	}

	private void DrawLabels()
	{
		GUILayout.BeginVertical();
		GUILayout.Label(" ", GUILayout.Width(80f));
		GUILayout.Label("A");
		GUILayout.Label("B");
		GUILayout.Label("X");
		GUILayout.Label("Y");
		GUILayout.Label("Start");
		GUILayout.Label("Back");
		GUILayout.Label("Left Shoulder");
		GUILayout.Label("Right Shoulder");
		GUILayout.Label("Left");
		GUILayout.Label("Right");
		GUILayout.Label("Up");
		GUILayout.Label("Down");
		GUILayout.Label("LeftStick");
		GUILayout.Label("RightStick");
		GUILayout.Label(string.Empty);
		GUILayout.Label("LeftTrigger");
		GUILayout.Label("RightTrigger");
		GUILayout.Label(string.Empty);
		GUILayout.Label("LeftStickAxis");
		GUILayout.Label("rightStickAxis");
		GUILayout.Label("dPadAxis");
		GUILayout.EndVertical();
	}
}
