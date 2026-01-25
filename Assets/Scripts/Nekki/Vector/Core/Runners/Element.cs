using System.Collections.Generic;
using System.Xml;
using Nekki.Vector.Core.Runners.Animation;

namespace Nekki.Vector.Core.Runners
{
	public class Element
	{
		private ObjectRunner _Parent;

		private List<VisualRunner> _Visuals = new List<VisualRunner>();

		private List<PlatformRunner> _Platforms = new List<PlatformRunner>();

		private List<TriggerRunner> _Triggers = new List<TriggerRunner>();

		private List<AreaRunner> _Areas = new List<AreaRunner>();

		private List<Nekki.Vector.Core.Runners.Animation.Animation> _Animations = new List<Nekki.Vector.Core.Runners.Animation.Animation>();

		private List<CustomAnimation> _CustomAnimations = new List<CustomAnimation>();

		private List<ParticleRunner> _Particles = new List<ParticleRunner>();

		private List<UnityModelRunner> _UnityModels = new List<UnityModelRunner>();

		private List<TrapezoidRunner> _Trapezoids = new List<TrapezoidRunner>();

		private List<SpawnRunner> _Spawns = new List<SpawnRunner>();

		private List<CameraRunner> _Cameras = new List<CameraRunner>();

		private List<Runner> _Elements = new List<Runner>();

		private List<SoundSourceRunner> _SoundSources = new List<SoundSourceRunner>();

		private List<Placeholder> _Placeholders = new List<Placeholder>();

		private List<SensorRunner> _Sensors = new List<SensorRunner>();

		private List<LightningRunner> _Lightning = new List<LightningRunner>();

		private List<GateRunner> _Ins = new List<GateRunner>();

		private List<GateRunner> _Outs = new List<GateRunner>();

		private List<WaypointRunner> _Waypoints = new List<WaypointRunner>();

		private List<QuadRunner> _QuadsAll = new List<QuadRunner>();

		public ObjectRunner Parent
		{
			get
			{
				return _Parent;
			}
		}

		public List<VisualRunner> Visuals
		{
			get
			{
				return _Visuals;
			}
		}

		public List<PlatformRunner> Platforms
		{
			get
			{
				return _Platforms;
			}
		}

		public List<TriggerRunner> Triggers
		{
			get
			{
				return _Triggers;
			}
		}

		public List<AreaRunner> Areas
		{
			get
			{
				return _Areas;
			}
		}

		public List<Nekki.Vector.Core.Runners.Animation.Animation> Animations
		{
			get
			{
				return _Animations;
			}
		}

		public List<CustomAnimation> CustomAnimations
		{
			get
			{
				return _CustomAnimations;
			}
		}

		public List<ParticleRunner> Particles
		{
			get
			{
				return _Particles;
			}
		}

		public List<UnityModelRunner> UnityModels
		{
			get
			{
				return _UnityModels;
			}
		}

		public List<TrapezoidRunner> Trapezoids
		{
			get
			{
				return _Trapezoids;
			}
		}

		public List<SpawnRunner> Spawns
		{
			get
			{
				return _Spawns;
			}
		}

		public List<CameraRunner> Cameras
		{
			get
			{
				return _Cameras;
			}
		}

		public List<Runner> Elements
		{
			get
			{
				return _Elements;
			}
		}

		public List<SoundSourceRunner> SoundSources
		{
			get
			{
				return _SoundSources;
			}
		}

		public List<Placeholder> Placeholders
		{
			get
			{
				return _Placeholders;
			}
		}

		public List<SensorRunner> Sensors
		{
			get
			{
				return _Sensors;
			}
		}

		public List<LightningRunner> Lightning
		{
			get
			{
				return _Lightning;
			}
		}

		public List<GateRunner> Ins
		{
			get
			{
				return _Ins;
			}
		}

		public List<GateRunner> Outs
		{
			get
			{
				return _Outs;
			}
		}

		public List<WaypointRunner> Waypoints
		{
			get
			{
				return _Waypoints;
			}
		}

		public List<QuadRunner> QuadsAll
		{
			get
			{
				return _QuadsAll;
			}
		}

		public Element(ObjectRunner p_parent)
		{
			_Parent = p_parent;
		}

		public static bool CheckSelection(XmlNode p_staticProperty, Dictionary<string, string> p_choices)
		{
			if (p_staticProperty == null)
			{
				return true;
			}
			bool result = true;
			foreach (XmlNode childNode in p_staticProperty.ChildNodes)
			{
				if (childNode.Name == "Selection")
				{
					string value = childNode.Attributes["Choice"].Value;
					if (p_choices.ContainsKey(value) && p_choices[value] == childNode.Attributes["Variant"].Value)
					{
						return true;
					}
					result = false;
				}
			}
			return result;
		}

		public void Parse(XmlNode p_mainNode, Dictionary<string, string> p_choices)
		{
			if (p_mainNode == null)
			{
				return;
			}
			int count = _Elements.Count;
			int count2 = _Placeholders.Count;
			foreach (XmlNode childNode in p_mainNode.ChildNodes)
			{
				if ((childNode["Properties"] != null && !CheckSelection(childNode["Properties"]["Static"], p_choices)) || (childNode.Attributes["Phantom"] != null && childNode.Attributes["Phantom"].Value == "1"))
				{
					continue;
				}
				switch (childNode.Name)
				{
				case "Object":
				case "ObjectReference":
					_Parent.CreateChild(childNode, p_choices);
					break;
				case "Image":
				{
					VisualRunner visualRunner = CreateVisual(childNode);
					if (visualRunner != null)
					{
						_Visuals.Add(visualRunner);
						_Elements.Add(visualRunner);
					}
					break;
				}
				case "Platform":
				{
					PlatformRunner platformRunner = CreatePlatform(childNode);
					if (platformRunner != null)
					{
						_Platforms.Add(platformRunner);
						_QuadsAll.Add(platformRunner);
						_Elements.Add(platformRunner);
					}
					break;
				}
				case "Trigger":
				{
					TriggerRunner triggerRunner = CreateTrigger(childNode);
					if (triggerRunner != null)
					{
						_Triggers.Add(triggerRunner);
						_QuadsAll.Add(triggerRunner);
						_Elements.Add(triggerRunner);
					}
					break;
				}
				case "Area":
				{
					AreaRunner areaRunner = CreateArea(childNode);
					if (areaRunner != null)
					{
						_Areas.Add(areaRunner);
						_QuadsAll.Add(areaRunner);
						_Elements.Add(areaRunner);
					}
					break;
				}
				case "Trapezoid":
				{
					TrapezoidRunner trapezoidRunner = CreateTrapezoid(childNode);
					if (trapezoidRunner != null)
					{
						_Trapezoids.Add(trapezoidRunner);
						_QuadsAll.Add(trapezoidRunner);
						_Elements.Add(trapezoidRunner);
					}
					break;
				}
				case "Particle":
				{
					ParticleRunner particleRunner = CreateParticle(childNode);
					if (particleRunner != null)
					{
						_Particles.Add(particleRunner);
						_Elements.Add(particleRunner);
					}
					break;
				}
				case "UnityModel":
				{
					UnityModelRunner unityModelRunner = CreateUnityModel(childNode);
					if (unityModelRunner != null)
					{
						_UnityModels.Add(unityModelRunner);
						_Elements.Add(unityModelRunner);
					}
					break;
				}
				case "Animation":
				{
					Nekki.Vector.Core.Runners.Animation.Animation animation = CreateAnimation(childNode);
					if (animation != null)
					{
						_Animations.Add(animation);
						_Elements.Add(animation);
					}
					break;
				}
				case "Camera":
				{
					CameraRunner cameraRunner = CreateCamera(childNode);
					if (cameraRunner != null)
					{
						_Cameras.Add(cameraRunner);
						_Elements.Add(cameraRunner);
					}
					break;
				}
				case "Spawn":
				{
					SpawnRunner spawnRunner = CreateSpawn(childNode);
					if (spawnRunner != null)
					{
						_Spawns.Add(spawnRunner);
						_Elements.Add(spawnRunner);
					}
					break;
				}
				case "SoundSource":
				{
					SoundSourceRunner soundSourceRunner = CreateSoundsource(childNode);
					if (soundSourceRunner != null)
					{
						_SoundSources.Add(soundSourceRunner);
						_Elements.Add(soundSourceRunner);
					}
					break;
				}
				case "Item":
				case "Placeholder":
				{
					Placeholder placeholder = CreatePlaceholder(childNode);
					if (placeholder != null)
					{
						_Placeholders.Add(placeholder);
					}
					break;
				}
				case "Sensor":
				{
					SensorRunner sensorRunner = CreateSensor(childNode);
					if (sensorRunner != null)
					{
						_Sensors.Add(sensorRunner);
						_Elements.Add(sensorRunner);
					}
					break;
				}
				case "In":
				case "Out":
				{
					GateRunner gateRunner = CreateGate(childNode);
					if (gateRunner != null)
					{
						if (gateRunner.IsIn)
						{
							_Ins.Add(gateRunner);
						}
						else
						{
							_Outs.Add(gateRunner);
						}
						_Elements.Add(gateRunner);
					}
					break;
				}
				case "Lightning":
				{
					LightningRunner lightningRunner = CreateLightning(childNode);
					if (lightningRunner != null)
					{
						_Lightning.Add(lightningRunner);
						_Elements.Add(lightningRunner);
					}
					break;
				}
				case "Waypoint":
				{
					WaypointRunner waypointRunner = CreateWaypoint(childNode);
					if (waypointRunner != null)
					{
						_Waypoints.Add(waypointRunner);
						_Elements.Add(waypointRunner);
					}
					break;
				}
				case "CustomAnimation":
				{
					CustomAnimation customAnimation = CreateCustomAnimation(childNode);
					if (customAnimation != null)
					{
						_CustomAnimations.Add(customAnimation);
						_Elements.Add(customAnimation);
					}
					break;
				}
				}
			}
			for (int i = count; i < _Elements.Count; i++)
			{
				_Elements[i].Generate();
			}
			for (int j = count2; j < _Placeholders.Count; j++)
			{
				_Placeholders[j].Generate();
			}
		}

		private SensorRunner CreateSensor(XmlNode p_node)
		{
			float p_X = XmlUtils.ParseFloat(p_node.Attributes["X"]);
			float p_Y = XmlUtils.ParseFloat(p_node.Attributes["Y"]);
			int p_AI = XmlUtils.ParseInt(p_node.Attributes["AI"], -1);
			SensorRunner sensorRunner = new SensorRunner(p_X, p_Y, p_AI, this);
			sensorRunner.ParseProperty(p_node);
			return sensorRunner;
		}

		private GateRunner CreateGate(XmlNode p_node)
		{
			float p_x = XmlUtils.ParseFloat(p_node.Attributes["X"]);
			float p_y = XmlUtils.ParseFloat(p_node.Attributes["Y"]);
			bool p_isIn = p_node.Name == "In";
			string p_name = XmlUtils.ParseString(p_node.Attributes["Name"], string.Empty);
			GateRunner gateRunner = new GateRunner(p_x, p_y, p_name, p_isIn, this);
			gateRunner.ParseProperty(p_node);
			return gateRunner;
		}

		private Placeholder CreatePlaceholder(XmlNode p_node)
		{
			if (p_node == null)
			{
				return null;
			}
			float p_x = XmlUtils.ParseFloat(p_node.Attributes["X"]);
			float p_y = XmlUtils.ParseFloat(p_node.Attributes["Y"]);
			return new Placeholder(p_x, p_y, this, p_node);
		}

		private SoundSourceRunner CreateSoundsource(XmlNode p_node)
		{
			if (p_node == null)
			{
				return null;
			}
			float p_x = XmlUtils.ParseFloat(p_node.Attributes["X"]);
			float p_y = XmlUtils.ParseFloat(p_node.Attributes["Y"]);
			SoundSourceRunner soundSourceRunner = new SoundSourceRunner(p_x, p_y, this, p_node);
			soundSourceRunner.ParseProperty(p_node);
			return soundSourceRunner;
		}

		private TriggerRunner CreateTrigger(XmlNode p_node)
		{
			if (p_node == null)
			{
				return null;
			}
			float p_x = XmlUtils.ParseFloat(p_node.Attributes["X"]);
			float p_y = XmlUtils.ParseFloat(p_node.Attributes["Y"]);
			float p_width = XmlUtils.ParseFloat(p_node.Attributes["Width"]);
			float p_height = XmlUtils.ParseFloat(p_node.Attributes["Height"]);
			TriggerRunner triggerRunner = new TriggerRunner(p_x, p_y, p_width, p_height, this, p_node);
			triggerRunner.ParseProperty(p_node);
			return triggerRunner;
		}

		private AreaRunner CreateArea(XmlNode p_node)
		{
			if (p_node == null)
			{
				return null;
			}
			float p_x = XmlUtils.ParseFloat(p_node.Attributes["X"]);
			float p_y = XmlUtils.ParseFloat(p_node.Attributes["Y"]);
			float p_width = XmlUtils.ParseFloat(p_node.Attributes["Width"]);
			float p_height = XmlUtils.ParseFloat(p_node.Attributes["Height"]);
			AreaRunner areaRunner = new AreaRunner(p_x, p_y, p_width, p_height, this, p_node);
			areaRunner.ParseProperty(p_node);
			return areaRunner;
		}

		private VisualRunner CreateVisual(XmlNode p_node)
		{
			string p_name = XmlUtils.ParseString(p_node.Attributes["ClassName"]);
			float p_x = XmlUtils.ParseFloat(p_node.Attributes["X"]);
			float p_y = XmlUtils.ParseFloat(p_node.Attributes["Y"]);
			float p_width = XmlUtils.ParseFloat(p_node.Attributes["Width"]);
			float p_height = XmlUtils.ParseFloat(p_node.Attributes["Height"]);
			VisualRunner visualRunner = new VisualRunner(p_name, p_x, p_y, p_width, p_height, this, p_node);
			visualRunner.ParseProperty(p_node);
			return visualRunner;
		}

		private PlatformRunner CreatePlatform(XmlNode p_node)
		{
			string p_name = XmlUtils.ParseString(p_node.Attributes["Name"]);
			float p_x = XmlUtils.ParseFloat(p_node.Attributes["X"]);
			float p_y = XmlUtils.ParseFloat(p_node.Attributes["Y"]);
			float p_width = XmlUtils.ParseFloat(p_node.Attributes["Width"]);
			float p_height = XmlUtils.ParseFloat(p_node.Attributes["Height"]);
			bool p_stikly = XmlUtils.ParseBool(p_node.Attributes["Sticky"], true);
			PlatformRunner platformRunner = new PlatformRunner(p_name, p_x, p_y, p_width, p_height, p_stikly, this);
			platformRunner.ParseProperty(p_node);
			return platformRunner;
		}

		private TrapezoidRunner CreateTrapezoid(XmlNode p_node)
		{
			string p_name = XmlUtils.ParseString(p_node.Attributes["Name"]);
			int num = XmlUtils.ParseInt(p_node.Attributes["Type"], 1);
			float p_x = XmlUtils.ParseFloat(p_node.Attributes["X"]);
			float num2 = XmlUtils.ParseFloat(p_node.Attributes["Y"]);
			float num3 = XmlUtils.ParseFloat(p_node.Attributes["Width"]);
			float num4 = XmlUtils.ParseFloat(p_node.Attributes["Height"]);
			bool p_Stikly = XmlUtils.ParseBool(p_node.Attributes["Sticky"], true);
			float num5 = ((num != 1) ? num4 : (num4 - num3 / 2f));
			float num6 = ((num != 1) ? (num4 - num3 / 2f) : num4);
			if (num == 1)
			{
				num2 += num6 - num5;
			}
			TrapezoidRunner trapezoidRunner = new TrapezoidRunner(p_name, num, p_x, num2, num3, num5, num6, p_Stikly, this);
			trapezoidRunner.ParseProperty(p_node);
			return trapezoidRunner;
		}

		private ParticleRunner CreateParticle(XmlNode p_node)
		{
			string name = XmlUtils.ParseString(p_node.Attributes["Name"], "Test");
			float p_x = XmlUtils.ParseFloat(p_node.Attributes["X"]);
			float p_y = XmlUtils.ParseFloat(p_node.Attributes["Y"]);
			float factor = XmlUtils.ParseFloat(p_node.Attributes["Factor"]);
			ParticleRunner particleRunner = new ParticleRunner(name, p_x, p_y, factor, this, p_node);
			particleRunner.ParseProperty(p_node);
			return particleRunner;
		}

		private UnityModelRunner CreateUnityModel(XmlNode p_node)
		{
			string name = XmlUtils.ParseString(p_node.Attributes["Name"], "Test");
			float p_x = XmlUtils.ParseFloat(p_node.Attributes["X"]);
			float p_y = XmlUtils.ParseFloat(p_node.Attributes["Y"]);
			float factor = XmlUtils.ParseFloat(p_node.Attributes["Factor"]);
			UnityModelRunner unityModelRunner = new UnityModelRunner(name, p_x, p_y, factor, this, p_node);
			unityModelRunner.ParseProperty(p_node);
			return unityModelRunner;
		}

		private Nekki.Vector.Core.Runners.Animation.Animation CreateAnimation(XmlNode p_node)
		{
			string p_name = XmlUtils.ParseString(p_node.Attributes["ClassName"]);
			float p_x = XmlUtils.ParseFloat(p_node.Attributes["X"]);
			float p_y = XmlUtils.ParseFloat(p_node.Attributes["Y"]);
			float p_width = XmlUtils.ParseFloat(p_node.Attributes["Width"]);
			float p_height = XmlUtils.ParseFloat(p_node.Attributes["Height"]);
			Nekki.Vector.Core.Runners.Animation.Animation animation = new Nekki.Vector.Core.Runners.Animation.Animation(p_name, p_x, p_y, p_width, p_height, this, p_node);
			animation.ParseProperty(p_node);
			return animation;
		}

		private CustomAnimation CreateCustomAnimation(XmlNode p_node)
		{
			string p_name = XmlUtils.ParseString(p_node.Attributes["ClassName"]);
			float p_x = XmlUtils.ParseFloat(p_node.Attributes["X"]);
			float p_y = XmlUtils.ParseFloat(p_node.Attributes["Y"]);
			float p_width = XmlUtils.ParseFloat(p_node.Attributes["Width"]);
			float p_height = XmlUtils.ParseFloat(p_node.Attributes["Height"]);
			CustomAnimation customAnimation = new CustomAnimation(p_name, p_x, p_y, p_width, p_height, this, p_node);
			customAnimation.ParseProperty(p_node);
			return customAnimation;
		}

		private CameraRunner CreateCamera(XmlNode p_node)
		{
			CameraRunner cameraRunner = new CameraRunner(this, p_node);
			cameraRunner.ParseProperty(p_node);
			return cameraRunner;
		}

		private SpawnRunner CreateSpawn(XmlNode p_node)
		{
			SpawnRunner spawnRunner = new SpawnRunner(this, p_node);
			spawnRunner.ParseProperty(p_node);
			return spawnRunner;
		}

		private LightningRunner CreateLightning(XmlNode p_node)
		{
			string p_name = XmlUtils.ParseString(p_node.Attributes["Name"]);
			float p_x = XmlUtils.ParseFloat(p_node.Attributes["X"]);
			float p_y = XmlUtils.ParseFloat(p_node.Attributes["Y"]);
			LightningRunner lightningRunner = new LightningRunner(p_name, p_x, p_y, this, p_node);
			lightningRunner.ParseProperty(p_node);
			return lightningRunner;
		}

		private WaypointRunner CreateWaypoint(XmlNode p_node)
		{
			string p_name = XmlUtils.ParseString(p_node.Attributes["Name"]);
			float p_x = XmlUtils.ParseFloat(p_node.Attributes["X"]);
			float p_y = XmlUtils.ParseFloat(p_node.Attributes["Y"]);
			WaypointRunner waypointRunner = new WaypointRunner(p_x, p_y, p_name, p_node, this);
			waypointRunner.ParseProperty(p_node);
			return waypointRunner;
		}

		public void Reset()
		{
			_Visuals.Clear();
			_Platforms.Clear();
			_Triggers.Clear();
			_Areas.Clear();
			_Animations.Clear();
			_Particles.Clear();
			_UnityModels.Clear();
			_Trapezoids.Clear();
			_Spawns.Clear();
			_Cameras.Clear();
			_Elements.Clear();
			_Placeholders.Clear();
			_Sensors.Clear();
			_Ins.Clear();
			_Outs.Clear();
			_Lightning.Clear();
			_CustomAnimations.Clear();
		}
	}
}
