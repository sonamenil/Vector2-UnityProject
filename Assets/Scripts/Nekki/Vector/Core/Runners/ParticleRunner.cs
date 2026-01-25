using System;
using System.Xml;
using Nekki.Vector.Core.Effects;
using Nekki.Vector.Core.Scripts.Effects;
using Nekki.Vector.Core.Utilites;
using UnityEngine;

namespace Nekki.Vector.Core.Runners
{
	public class ParticleRunner : MatrixSupport
	{
		private XmlNode _PropertyNode;

		private ParticleBehaviour _ParticleBehaviour;

		private ParticleSystem _ParticleSystem;

		private ParticleSystemRenderer _ParticleRenderer;

		private float _Factor;

		private float? _Duration;

		private bool? _Looping;

		private float _PlayingTime;

		private string _Layer;

		public override bool IsEnabled
		{
			get
			{
				return _IsEnabled;
			}
		}

		public override Element ParentElements
		{
			get
			{
				return base.ParentElements;
			}
		}

		public ParticleRunner(string name, float p_x, float p_y, float factor, Element p_elements, XmlNode p_node)
			: base(p_x, p_y, p_elements, p_node)
		{
			_TypeClass = TypeRunner.Particle;
			_Name = name;
			_Factor = factor;
			_Layer = XmlUtils.ParseString(p_node.Attributes["Layer"], string.Empty);
			_PropertyNode = p_node["Properties"];
			RunMainController.OnPause += Pause;
			RunMainController.OnSimulate += Simulate;
			RunnerRender.AddRunner(this);
		}

		protected override void GenerateObject()
		{
			base.GenerateObject();
			_CachedTransform.localPosition = _DefautPosition;
			_ParticleSystem = EffectManager.Instantiate(_Name, base.UnityObject.transform);
			_ParticleBehaviour = _UnityObject.AddComponent<ParticleBehaviour>();
			_ParticleBehaviour.SetFactor(_Factor, _Factor);
			_ParticleBehaviour.SetParticleSystem(_ParticleSystem);
			_ParticleBehaviour.SetParent(_ParentElements.Parent.UnityObject);
			_ParticleRenderer = _ParticleSystem.gameObject.GetComponent<ParticleSystemRenderer>();
			Initialize();
			UpdateLayer();
			Transform();
		}

		public override void SetEnabled(bool p_enabled, bool restore = false, bool fromHierarchy = false)
		{
			base.SetEnabled(p_enabled, restore, fromHierarchy);
			if (_ParticleSystem != null)
			{
				if (p_enabled)
				{
					PlayAnimation();
				}
				else
				{
					StopAnimation();
				}
			}
		}

		private void Initialize()
		{
			SetEnabled(_ParticleSystem.playOnAwake);
			ReadProperties();
			_ParticleSystem.gravityModifier *= -1f;
			_ParticleSystem.transform.localPosition = Vector3.zero;
			_ParticleSystem.transform.localEulerAngles += new Vector3(180f, 0f, 0f);
			if (_Looping.HasValue)
			{
				_ParticleSystem.loop = _Looping.Value;
			}
			if (_Duration.HasValue)
			{
				_ParticleSystem.loop = true;
			}
			if (IsEnabled)
			{
				PlayAnimation();
			}
			else
			{
				StopAnimation();
			}
		}

		private void UpdateLayer()
		{
			_ParticleRenderer.sortingLayerName = _Layer;
			_ParticleRenderer.sortingOrder = 0;
		}

		private void ReadProperties()
		{
			if (_PropertyNode == null)
			{
				return;
			}
			XmlNode xmlNode = _PropertyNode["Static"];
			if (xmlNode == null)
			{
				return;
			}
			foreach (XmlNode childNode in xmlNode.ChildNodes)
			{
				switch (childNode.Name)
				{
				case "Enable":
					SetEnabled(XmlUtils.ParseInt(childNode.Attributes["Value"], _IsEnabled ? 1 : 0) != 0);
					break;
				case "Duration":
					_Duration = XmlUtils.ParseFloat(childNode.Attributes["Value"], _ParticleSystem.duration);
					break;
				case "Looping":
					_Looping = XmlUtils.ParseInt(childNode.Attributes["Value"], _ParticleSystem.loop ? 1 : 0) != 0;
					break;
				case "StartDelay":
					_ParticleSystem.startDelay = XmlUtils.ParseFloat(childNode.Attributes["Value"], _ParticleSystem.startDelay);
					break;
				case "StartLifetime":
					_ParticleSystem.startLifetime = XmlUtils.ParseFloat(childNode.Attributes["Value"], _ParticleSystem.startLifetime);
					break;
				case "StartSpeed":
					_ParticleSystem.startSpeed = XmlUtils.ParseFloat(childNode.Attributes["Value"], _ParticleSystem.startSpeed);
					break;
				case "StartSize":
					_ParticleSystem.startSize = XmlUtils.ParseFloat(childNode.Attributes["Value"], _ParticleSystem.startSize);
					break;
				case "StartRotation":
					_ParticleSystem.startRotation = XmlUtils.ParseFloat(childNode.Attributes["Value"], _ParticleSystem.startRotation);
					break;
				case "StartColor":
					_ParticleSystem.startColor = ColorUtils.FromHex(childNode.Attributes["Value"].Value);
					break;
				case "GravityMultiplier":
					_ParticleSystem.gravityModifier = XmlUtils.ParseFloat(childNode.Attributes["Value"], _ParticleSystem.gravityModifier);
					break;
				case "SimulationSpace":
					_ParticleSystem.simulationSpace = (ParticleSystemSimulationSpace)(int)Enum.Parse(typeof(ParticleSystemSimulationSpace), childNode.Attributes["Value"].Value);
					break;
				case "MaxParticles":
					_ParticleSystem.maxParticles = XmlUtils.ParseInt(childNode.Attributes["Value"], _ParticleSystem.maxParticles);
					break;
				case "EmissionRate":
				{
					ParticleSystem.EmissionModule emission = _ParticleSystem.emission;
					emission.rate = new ParticleSystem.MinMaxCurve(XmlUtils.ParseFloat(childNode.Attributes["Value"], emission.rate.constantMax));
					break;
				}
				case "RandomSeed":
					_ParticleSystem.randomSeed = XmlUtils.ParseUint(childNode.Attributes["Value"], _ParticleSystem.randomSeed);
					break;
				case "PlaybackSpeed":
					_ParticleSystem.playbackSpeed = XmlUtils.ParseFloat(childNode.Attributes["Value"], _ParticleSystem.playbackSpeed);
					break;
				}
			}
			_PropertyNode = null;
		}

		public override bool Render()
		{
			if (IsEnabled && HasDurationTimeEnded())
			{
				SetEnabled(IsEnabled);
			}
			_PlayingTime += Time.deltaTime;
			return false;
		}

		private bool HasDurationTimeEnded()
		{
			return _ParticleSystem != null && _ParticleSystem.isPlaying && (!_Looping.HasValue || !_Looping.Value) && _Duration.HasValue && _Duration.Value < _PlayingTime;
		}

		public void PlayAnimation()
		{
			_PlayingTime = 0f;
			_ParticleSystem.Play();
		}

		public void StopAnimation()
		{
			_ParticleSystem.Stop();
		}

		public void Pause(bool p_pause)
		{
			if (!(_ParticleSystem == null) && !_ParticleSystem.isStopped)
			{
				if (p_pause)
				{
					_ParticleSystem.Pause();
				}
				else
				{
					_ParticleSystem.Play();
				}
			}
		}

		public void Simulate(float p_time)
		{
			if (!(_ParticleSystem == null) && !_ParticleSystem.isStopped)
			{
				_ParticleSystem.Simulate(p_time, true, false);
			}
		}

		public override void End()
		{
			base.End();
			RunMainController.OnPause -= Pause;
			RunMainController.OnSimulate -= Simulate;
		}
	}
}
