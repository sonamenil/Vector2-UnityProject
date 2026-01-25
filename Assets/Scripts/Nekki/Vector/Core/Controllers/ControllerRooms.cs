using System.Collections.Generic;
using Nekki.Vector.Core.Generator;

namespace Nekki.Vector.Core.Controllers
{
	public class ControllerRooms
	{
		private Location _Location;

		private List<Room> _RoomsOnTrack;

		private int _LastRoomIndex = -1;

		public ControllerRooms(Location p_location)
		{
			_Location = p_location;
		}

		public void Init()
		{
			_RoomsOnTrack = _Location.Sets.RoomsOnTrack;
		}

		public void Render()
		{
			int currentRoomIndex = _Location.CurrentRoomIndex;
			if (currentRoomIndex != -1 && currentRoomIndex != _LastRoomIndex)
			{
				EnableRoom(currentRoomIndex + 1);
				DisableRoom(currentRoomIndex - 2);
				_LastRoomIndex = currentRoomIndex;
			}
		}

		private void EnableRoom(int p_index)
		{
			if (p_index < _RoomsOnTrack.Count && p_index >= 0)
			{
				_RoomsOnTrack[p_index].Object.IsEnableUnityGO = true;
			}
		}

		private void DisableRoom(int p_index)
		{
			if (p_index < _RoomsOnTrack.Count && p_index >= 0)
			{
				_RoomsOnTrack[p_index].Object.IsEnableUnityGO = false;
			}
		}
	}
}
