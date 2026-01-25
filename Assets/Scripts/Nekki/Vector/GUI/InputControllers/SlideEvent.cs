using System;
using UnityEngine;
using UnityEngine.Events;

namespace Nekki.Vector.GUI.InputControllers
{
	[Serializable]
	public class SlideEvent : UnityEvent<int, Vector2, Vector2>
	{
	}
}
