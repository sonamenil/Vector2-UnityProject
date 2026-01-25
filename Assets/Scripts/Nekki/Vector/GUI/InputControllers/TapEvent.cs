using System;
using UnityEngine;
using UnityEngine.Events;

namespace Nekki.Vector.GUI.InputControllers
{
	[Serializable]
	public class TapEvent : UnityEvent<int, Vector2>
	{
	}
}
