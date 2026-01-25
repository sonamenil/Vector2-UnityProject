namespace Nekki.Vector.Core.Controllers
{
	public class KeyVariables
	{
		private Key _Key;

		public Key Key
		{
			get
			{
				return _Key;
			}
		}

		public KeyVariables(string p_key)
		{
			_Key = Parse(p_key);
		}

		public static Key Parse(string p_key)
		{
			switch (p_key)
			{
			case "Up":
				return Key.Up;
			case "Left":
				return Key.Left;
			case "Down":
				return Key.Down;
			case "Right":
				return Key.Right;
			default:
				return Key.None;
			}
		}

		public bool IsEqual(KeyVariables p_keysVariables)
		{
			return _Key == p_keysVariables.Key;
		}

		public int ToInteger()
		{
			return (int)_Key;
		}

		public override string ToString()
		{
			switch (_Key)
			{
			case Key.Up:
				return "Up";
			case Key.Left:
				return "Left";
			case Key.Down:
				return "Down";
			case Key.Right:
				return "Right";
			default:
				return "None";
			}
		}
	}
}
