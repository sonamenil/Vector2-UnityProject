using System;
using UnityEngine;

namespace GamepadInput
{
	public static class GamePad
	{
		public enum Button
		{
			A = 0,
			B = 1,
			Y = 2,
			X = 3,
			RightShoulder = 4,
			LeftShoulder = 5,
			RightStick = 6,
			LeftStick = 7,
			Back = 8,
			Start = 9
		}

		public enum Trigger
		{
			LeftTrigger = 0,
			RightTrigger = 1
		}

		public enum Axis
		{
			LeftStick = 0,
			RightStick = 1,
			Dpad = 2
		}

		public enum Index
		{
			Any = 0,
			One = 1,
			Two = 2,
			Three = 3,
			Four = 4
		}

		public static bool GetButtonDown(Button button, Index controlIndex)
		{
			KeyCode keycode = GetKeycode(button, controlIndex);
			return Input.GetKeyDown(keycode);
		}

		public static bool GetButtonUp(Button button, Index controlIndex)
		{
			KeyCode keycode = GetKeycode(button, controlIndex);
			return Input.GetKeyUp(keycode);
		}

		public static bool GetButton(Button button, Index controlIndex)
		{
			KeyCode keycode = GetKeycode(button, controlIndex);
			return Input.GetKey(keycode);
		}

		public static Vector2 GetAxis(Axis axis, Index controlIndex, bool raw = false)
		{
			string axisName = string.Empty;
			string axisName2 = string.Empty;
			switch (axis)
			{
			case Axis.Dpad:
				axisName = "DPad_XAxis_" + (int)controlIndex;
				axisName2 = "DPad_YAxis_" + (int)controlIndex;
				break;
			case Axis.LeftStick:
				axisName = "L_XAxis_" + (int)controlIndex;
				axisName2 = "L_YAxis_" + (int)controlIndex;
				break;
			case Axis.RightStick:
				axisName = "R_XAxis_" + (int)controlIndex;
				axisName2 = "R_YAxis_" + (int)controlIndex;
				break;
			}
			Vector2 result = Vector3.zero;
			try
			{
				if (!raw)
				{
					result.x = Input.GetAxis(axisName);
					result.y = 0f - Input.GetAxis(axisName2);
				}
				else
				{
					result.x = Input.GetAxisRaw(axisName);
					result.y = 0f - Input.GetAxisRaw(axisName2);
				}
			}
			catch (Exception message)
			{
				AdvLog.LogError(message);
				AdvLog.LogWarning("Have you set up all axes correctly? \nThe easiest solution is to replace the InputManager.asset with version located in the GamepadInput package. \nWarning: do so will overwrite any existing input");
			}
			return result;
		}

		public static float GetTrigger(Trigger trigger, Index controlIndex, bool raw = false)
		{
			string axisName = string.Empty;
			switch (trigger)
			{
			case Trigger.LeftTrigger:
				axisName = "TriggersL_" + (int)controlIndex;
				break;
			case Trigger.RightTrigger:
				axisName = "TriggersR_" + (int)controlIndex;
				break;
			}
			float result = 0f;
			try
			{
				result = (raw ? Input.GetAxisRaw(axisName) : Input.GetAxis(axisName));
			}
			catch (Exception message)
			{
				AdvLog.LogError(message);
				AdvLog.LogWarning("Have you set up all axes correctly? \nThe easiest solution is to replace the InputManager.asset with version located in the GamepadInput package. \nWarning: do so will overwrite any existing input");
			}
			return result;
		}

		private static KeyCode GetKeycode(Button button, Index controlIndex)
		{
			switch (controlIndex)
			{
			case Index.One:
				switch (button)
				{
				case Button.A:
					return KeyCode.Joystick1Button0;
				case Button.B:
					return KeyCode.Joystick1Button1;
				case Button.X:
					return KeyCode.Joystick1Button2;
				case Button.Y:
					return KeyCode.Joystick1Button3;
				case Button.RightShoulder:
					return KeyCode.Joystick1Button5;
				case Button.LeftShoulder:
					return KeyCode.Joystick1Button4;
				case Button.Back:
					return KeyCode.Joystick1Button6;
				case Button.Start:
					return KeyCode.Joystick1Button7;
				case Button.LeftStick:
					return KeyCode.Joystick1Button8;
				case Button.RightStick:
					return KeyCode.Joystick1Button9;
				}
				break;
			case Index.Two:
				switch (button)
				{
				case Button.A:
					return KeyCode.Joystick2Button0;
				case Button.B:
					return KeyCode.Joystick2Button1;
				case Button.X:
					return KeyCode.Joystick2Button2;
				case Button.Y:
					return KeyCode.Joystick2Button3;
				case Button.RightShoulder:
					return KeyCode.Joystick2Button5;
				case Button.LeftShoulder:
					return KeyCode.Joystick2Button4;
				case Button.Back:
					return KeyCode.Joystick2Button6;
				case Button.Start:
					return KeyCode.Joystick2Button7;
				case Button.LeftStick:
					return KeyCode.Joystick2Button8;
				case Button.RightStick:
					return KeyCode.Joystick2Button9;
				}
				break;
			case Index.Three:
				switch (button)
				{
				case Button.A:
					return KeyCode.Joystick3Button0;
				case Button.B:
					return KeyCode.Joystick3Button1;
				case Button.X:
					return KeyCode.Joystick3Button2;
				case Button.Y:
					return KeyCode.Joystick3Button3;
				case Button.RightShoulder:
					return KeyCode.Joystick3Button5;
				case Button.LeftShoulder:
					return KeyCode.Joystick3Button4;
				case Button.Back:
					return KeyCode.Joystick3Button6;
				case Button.Start:
					return KeyCode.Joystick3Button7;
				case Button.LeftStick:
					return KeyCode.Joystick3Button8;
				case Button.RightStick:
					return KeyCode.Joystick3Button9;
				}
				break;
			case Index.Four:
				switch (button)
				{
				case Button.A:
					return KeyCode.Joystick4Button0;
				case Button.B:
					return KeyCode.Joystick4Button1;
				case Button.X:
					return KeyCode.Joystick4Button2;
				case Button.Y:
					return KeyCode.Joystick4Button3;
				case Button.RightShoulder:
					return KeyCode.Joystick4Button5;
				case Button.LeftShoulder:
					return KeyCode.Joystick4Button4;
				case Button.Back:
					return KeyCode.Joystick4Button6;
				case Button.Start:
					return KeyCode.Joystick4Button7;
				case Button.LeftStick:
					return KeyCode.Joystick4Button8;
				case Button.RightStick:
					return KeyCode.Joystick4Button9;
				}
				break;
			case Index.Any:
				switch (button)
				{
				case Button.A:
					return KeyCode.JoystickButton0;
				case Button.B:
					return KeyCode.JoystickButton1;
				case Button.X:
					return KeyCode.JoystickButton2;
				case Button.Y:
					return KeyCode.JoystickButton3;
				case Button.RightShoulder:
					return KeyCode.JoystickButton5;
				case Button.LeftShoulder:
					return KeyCode.JoystickButton4;
				case Button.Back:
					return KeyCode.JoystickButton6;
				case Button.Start:
					return KeyCode.JoystickButton7;
				case Button.LeftStick:
					return KeyCode.JoystickButton8;
				case Button.RightStick:
					return KeyCode.JoystickButton9;
				}
				break;
			}
			return KeyCode.None;
		}

		public static GamepadState GetState(Index controlIndex, bool raw = false)
		{
			GamepadState gamepadState = new GamepadState();
			gamepadState.A = GetButton(Button.A, controlIndex);
			gamepadState.B = GetButton(Button.B, controlIndex);
			gamepadState.Y = GetButton(Button.Y, controlIndex);
			gamepadState.X = GetButton(Button.X, controlIndex);
			gamepadState.RightShoulder = GetButton(Button.RightShoulder, controlIndex);
			gamepadState.LeftShoulder = GetButton(Button.LeftShoulder, controlIndex);
			gamepadState.RightStick = GetButton(Button.RightStick, controlIndex);
			gamepadState.LeftStick = GetButton(Button.LeftStick, controlIndex);
			gamepadState.Start = GetButton(Button.Start, controlIndex);
			gamepadState.Back = GetButton(Button.Back, controlIndex);
			gamepadState.LeftStickAxis = GetAxis(Axis.LeftStick, controlIndex, raw);
			gamepadState.rightStickAxis = GetAxis(Axis.RightStick, controlIndex, raw);
			gamepadState.dPadAxis = GetAxis(Axis.Dpad, controlIndex, raw);
			gamepadState.Left = gamepadState.dPadAxis.x < 0f;
			gamepadState.Right = gamepadState.dPadAxis.x > 0f;
			gamepadState.Up = gamepadState.dPadAxis.y > 0f;
			gamepadState.Down = gamepadState.dPadAxis.y < 0f;
			gamepadState.LeftTrigger = GetTrigger(Trigger.LeftTrigger, controlIndex, raw);
			gamepadState.RightTrigger = GetTrigger(Trigger.RightTrigger, controlIndex, raw);
			return gamepadState;
		}
	}
}
