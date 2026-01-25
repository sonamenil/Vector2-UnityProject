using System;
using System.Collections.Generic;
using System.Xml;
using Nekki.Vector.Core.User;

namespace Nekki.Vector.Core.GameManagement
{
	public static class ZoneManager
	{
		private class ZoneResourceInfo
		{
			private string _Id;

			private Dictionary<Zone, string> _Paths = new Dictionary<Zone, string>();

			private string _LastReturnedPath;

			public ZoneResourceInfo(string p_id)
			{
				_Id = p_id;
			}

			public void AddZonePath(Zone p_zone, XmlNode p_node)
			{
				_Paths.Add(p_zone, XmlUtils.ParseString(p_node.Attributes["Path"], string.Empty));
			}

			public bool IsNeedReload(Zone p_zone)
			{
				string pathInternal = GetPathInternal(p_zone);
				return _LastReturnedPath != pathInternal;
			}

			public string GetPath(Zone p_zone)
			{
				_LastReturnedPath = GetPathInternal(p_zone);
				return _LastReturnedPath;
			}

			private string GetPathInternal(Zone p_zone)
			{
				string value = null;
				if (!_Paths.TryGetValue(p_zone, out value))
				{
					value = _Paths[Zone.Default];
				}
				return value;
			}
		}

		private const string _FileName = "zones.xml";

		private static Dictionary<string, ZoneResourceInfo> _ZoneResources = new Dictionary<string, ZoneResourceInfo>();

		private static Zone _CurrentZone = Zone.Zone1;

		private static HashSet<Zone> _AvailableZones = new HashSet<Zone>();

		private static bool _IsInited = false;

		public static Zone CurrentZone
		{
			get
			{
				return _CurrentZone;
			}
			set
			{
				if (_CurrentZone != value)
				{
					_CurrentZone = value;
					StarterPacksManager.UnequipAllStarterPacks();
					SaveSettings();
					if (ZoneManager.OnCurrentZoneChanged != null)
					{
						ZoneManager.OnCurrentZoneChanged();
					}
				}
			}
		}

		public static HashSet<Zone> AvailableZones
		{
			get
			{
				return _AvailableZones;
			}
		}

		public static event System.Action OnCurrentZoneChanged;

		public static bool IsZoneAvailiable(Zone p_zone)
		{
			return _AvailableZones.Contains(p_zone);
		}

		public static void SetZoneAvailable(Zone p_zone, bool p_available)
		{
			if (p_available && !_AvailableZones.Contains(p_zone))
			{
				_AvailableZones.Add(p_zone);
			}
			else
			{
				if (p_available || !_AvailableZones.Contains(p_zone))
				{
					return;
				}
				_AvailableZones.Remove(p_zone);
			}
			SaveSettings();
		}

		public static string GetResourceFilePath(string p_resourceId, string p_def = null)
		{
			ZoneResourceInfo zoneResourceInfo = GetZoneResourceInfo(p_resourceId);
			if (zoneResourceInfo == null)
			{
				return p_def;
			}
			return zoneResourceInfo.GetPath(CurrentZone);
		}

		public static bool IsResourceNeedReload(string p_resourceId, bool p_def = false)
		{
			ZoneResourceInfo zoneResourceInfo = GetZoneResourceInfo(p_resourceId);
			if (zoneResourceInfo == null)
			{
				return p_def;
			}
			return zoneResourceInfo.IsNeedReload(CurrentZone);
		}

		private static ZoneResourceInfo GetZoneResourceInfo(string p_fileId)
		{
			ZoneResourceInfo value = null;
			_ZoneResources.TryGetValue(p_fileId, out value);
			return value;
		}

		public static void Init()
		{
			if (!_IsInited)
			{
				_IsInited = true;
				ParseZones();
				LoadDefaultSettings();
			}
		}

		private static void ParseZones()
		{
			XmlDocument xmlDocument = XmlUtils.OpenXMLDocument(VectorPaths.GeneratorData, "zones.xml");
			foreach (XmlNode childNode in xmlDocument["Zones"].ChildNodes)
			{
				ParseZoneResources(childNode);
			}
		}

		private static void ParseZoneResources(XmlNode p_node)
		{
			Zone p_zone = StringUtils.ParseEnum(p_node.Name, Zone.Default);
			foreach (XmlNode childNode in p_node.ChildNodes)
			{
				ZoneResourceInfo value;
				if (!_ZoneResources.TryGetValue(childNode.Name, out value))
				{
					value = new ZoneResourceInfo(childNode.Name);
					_ZoneResources.Add(childNode.Name, value);
				}
				value.AddZonePath(p_zone, childNode);
			}
		}

		private static void LoadDefaultSettings()
		{
			_CurrentZone = Zone.Zone1;
			_AvailableZones.Clear();
			_AvailableZones.Add(_CurrentZone);
		}

		public static void LoadSettings()
		{
			UserProperty userPropertyByName = DataLocal.Current.GetUserPropertyByName("Zones");
			if (userPropertyByName == null)
			{
				LoadDefaultSettings();
				SaveSettings();
				return;
			}
			_CurrentZone = StringUtils.ParseEnum(userPropertyByName.ValueString("Current"), Zone.Zone1);
			_AvailableZones.Clear();
			foreach (int value in Enum.GetValues(typeof(Zone)))
			{
				if (value != 0 && (int)userPropertyByName.ValueInt(((Zone)value).ToString()) == 1)
				{
					_AvailableZones.Add((Zone)value);
				}
			}
			GameManager.ReloadZoneSpecieficPresets();
		}

		private static void SaveSettings()
		{
			UserProperty orCreateUserPropertyByName = DataLocal.Current.GetOrCreateUserPropertyByName("Zones");
			orCreateUserPropertyByName.AddAttributeOrSetValue("Current", _CurrentZone.ToString());
			foreach (int value in Enum.GetValues(typeof(Zone)))
			{
				if (value != 0)
				{
					orCreateUserPropertyByName.AddAttributeOrSetValue(((Zone)value).ToString(), (!IsZoneAvailiable((Zone)value)) ? "0" : "1");
				}
			}
			DataLocal.Current.Save(false);
		}
	}
}
