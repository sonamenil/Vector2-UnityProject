using System.Collections.Generic;
using System.Xml;
using Nekki.Vector.Core.GameManagement;
using Nekki.Vector.Core.Runners;

namespace Nekki.Vector.Core.Controllers
{
	public class ControllerSwarm
	{
		private string _Filename = "swarms.xml";

		private List<Swarm> _Swarms = new List<Swarm>();

		private List<SensorRunner> _Sensors = new List<SensorRunner>();

		private bool _IsRender;

		public List<Swarm> Swarms
		{
			get
			{
				return _Swarms;
			}
		}

		public List<SensorRunner> Sensors
		{
			get
			{
				return _Sensors;
			}
		}

		public ControllerSwarm(Location p_location, int p_countSwarms)
		{
			XmlNode xmlNode = XmlUtils.OpenXMLDocument(VectorPaths.RunDataLibs, _Filename);
			foreach (XmlNode item in xmlNode["Root"]["Objects"])
			{
				if (!(item.Name != "Swarm") && IsSwarmAvailableForCurrentZone(XmlUtils.ParseString(item.Attributes["AvailableZones"])))
				{
					Swarm swarm = new Swarm(this);
					swarm.Parse(item, null);
					swarm.Init();
					FindSensors(swarm);
					_Swarms.Add(swarm);
				}
			}
		}

		private bool IsSwarmAvailableForCurrentZone(string zones)
		{
			if (string.IsNullOrEmpty(zones))
			{
				return true;
			}
			string[] array = zones.Split('|');
			Zone currentZone = ZoneManager.CurrentZone;
			int i = 0;
			for (int num = array.Length; i < num; i++)
			{
				if (StringUtils.ParseEnum(array[i], Zone.Zone1) == currentZone)
				{
					return true;
				}
			}
			return false;
		}

		private void FindSensors(ObjectRunner p_object)
		{
			if (p_object.Element.Sensors.Count != 0)
			{
				_Sensors.AddRange(p_object.Element.Sensors);
			}
			foreach (ObjectRunner child in p_object.Childs)
			{
				FindSensors(child);
			}
		}

		public void TurnOn()
		{
			_IsRender = true;
		}

		public void AddQuads(List<QuadRunner> p_list)
		{
			for (int i = 0; i < _Swarms.Count; i++)
			{
				if (_Swarms[i].IsActive)
				{
					p_list.AddRange(_Swarms[i].Quads);
				}
			}
		}

		public void Render()
		{
			if (!_IsRender)
			{
				return;
			}
			_IsRender = false;
			for (int i = 0; i < _Swarms.Count; i++)
			{
				if (_Swarms[i].IsActive)
				{
					_IsRender = true;
					_Swarms[i].Render();
				}
			}
		}

		public Swarm GetSwarmByName(string p_name)
		{
			for (int i = 0; i < _Swarms.Count; i++)
			{
				if (_Swarms[i].Name == p_name)
				{
					return _Swarms[i];
				}
			}
			return null;
		}

		public void End()
		{
			for (int i = 0; i < _Swarms.Count; i++)
			{
				_Swarms[i].End();
			}
		}
	}
}
