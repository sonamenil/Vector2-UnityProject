using System.Collections.Generic;
using Nekki.Vector.Core.Animation;
using Nekki.Vector.Core.Game;

namespace Nekki.Vector.Core.User
{
	public class Data
	{
		private List<AnimationInfo> _AvailableAnimation = new List<AnimationInfo>();

		private string _Name;

		private List<string> _Skins = new List<string>();

		private bool _IsPlayer;

		private string _BirthSpawn;

		private int _AI;

		private float _StartTime;

		private float _LiveTime;

		public string Name
		{
			get
			{
				return _Name;
			}
			set
			{
				_Name = value;
			}
		}

		public List<string> Skins
		{
			get
			{
				return _Skins;
			}
			set
			{
				_Skins = value;
			}
		}

		public bool IsPlayer
		{
			get
			{
				return _IsPlayer;
			}
			set
			{
				_IsPlayer = value;
			}
		}

		public bool IsBot
		{
			get
			{
				return !_IsPlayer;
			}
		}

		public string BirthSpawn
		{
			get
			{
				return _BirthSpawn;
			}
			set
			{
				_BirthSpawn = value;
			}
		}

		public int AI
		{
			get
			{
				return _AI;
			}
			set
			{
				_AI = value;
			}
		}

		public float StartTime
		{
			get
			{
				return _StartTime;
			}
			set
			{
				_StartTime = value;
			}
		}

		public float LiveTime
		{
			get
			{
				return _LiveTime;
			}
			set
			{
				_LiveTime = value * 60f;
			}
		}

		public void Init()
		{
			SetModeles();
			SetAnimation();
		}

		private void SetModeles()
		{
			_Skins.Insert(0, "0");
			List<string> list = new List<string>();
			foreach (KeyValuePair<string, Settings.GadgetSlot> gadgetSlot in Settings.GadgetSlots)
			{
				list.Add(gadgetSlot.Value.Name);
			}
			foreach (UserItem item in DataLocal.Current.Equipped)
			{
				if (item.ContainsGroup("ST_Model"))
				{
					_Skins.Add(item.GetStrValueAttribute("ST_File", "ST_Model"));
					string strValueAttribute = item.GetStrValueAttribute("ST_Slot", "ST_MyGadgets");
					if (list.Contains(strValueAttribute))
					{
						list.Remove(strValueAttribute);
					}
				}
			}
			if (list.Contains("Head"))
			{
				_Skins.Add("hair");
			}
			for (int i = 0; i < Skins.Count; i++)
			{
				Skins[i] += ".xml";
			}
			_Skins = Skins;
		}

		public void SetAnimation()
		{
			_AvailableAnimation.Clear();
			List<AnimationInfo> list = Animations.ToList();
			for (int i = 0; i < list.Count; i++)
			{
				_AvailableAnimation.Add(list[i]);
			}
		}

		public AnimationInfo Animation(string Name)
		{
			for (int i = 0; i < _AvailableAnimation.Count; i++)
			{
				if (_AvailableAnimation[i].Name == Name)
				{
					return _AvailableAnimation[i];
				}
			}
			return null;
		}
	}
}
