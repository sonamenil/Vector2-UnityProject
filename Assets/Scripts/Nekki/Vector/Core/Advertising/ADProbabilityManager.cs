using System.Collections.Generic;
using System.Xml;
using Nekki.Vector.Core.Counter;
using Nekki.Vector.Core.Payment;
using UnityEngine;

namespace Nekki.Vector.Core.Advertising
{
	public class ADProbabilityManager
	{
		private class Probability
		{
			private float _Value;

			private float _Value_paid;

			public int BeforeRun;

			public float Value
			{
				get
				{
					return (!PaymentController.IsUserMakePayments) ? _Value : _Value_paid;
				}
			}

			public Probability()
			{
				_Value = 0.33f;
				_Value_paid = 0f;
			}

			public Probability(XmlNode p_node)
			{
				_Value = XmlUtils.ParseFloat(p_node.Attributes[(!DeviceInformation.IsiOS) ? "Android" : "iOS"], 0.33f);
				_Value_paid = XmlUtils.ParseFloat(p_node.Attributes[(!DeviceInformation.IsiOS) ? "Android_paid" : "iOS_paid"]);
				BeforeRun = XmlUtils.ParseInt(p_node.Attributes["BeforeRun"]);
			}
		}

		private const float _DefInterstitialAdProbability = 0.33f;

		private const float _DefInterstitialAdProbabilityPaid = 0f;

		private Probability _DefaultProbability;

		private List<Probability> _Probabilitys = new List<Probability>();

		private static ADProbabilityManager _Current;

		public static ADProbabilityManager Current
		{
			get
			{
				if (_Current == null)
				{
					_Current = new ADProbabilityManager();
				}
				return _Current;
			}
		}

		public int RunAfterTutorial
		{
			get
			{
				return (int)CounterController.Current.CounterRunCounter - (int)CounterController.Current.CounterTutorialEndRun;
			}
		}

		public bool IsProbabilityIsZero
		{
			get
			{
				return InterstitialAdProbability == 0f;
			}
		}

		public bool CanShowInterstitialAd
		{
			get
			{
				float num = Random.Range(0f, 1f);
				return num < InterstitialAdProbability;
			}
		}

		public float InterstitialAdProbability
		{
			get
			{
				int runAfterTutorial = RunAfterTutorial;
				if (_Probabilitys.Count == 0 || _Probabilitys[_Probabilitys.Count - 1].BeforeRun < runAfterTutorial)
				{
					return _DefaultProbability.Value;
				}
				for (int i = 0; i < _Probabilitys.Count; i++)
				{
					if (_Probabilitys[i].BeforeRun > runAfterTutorial)
					{
						return _Probabilitys[i].Value;
					}
				}
				return _DefaultProbability.Value;
			}
		}

		private ADProbabilityManager()
		{
			_DefaultProbability = new Probability();
		}

		public static void Init(XmlNode p_node)
		{
			_Current = new ADProbabilityManager();
			_Current.Parse(p_node);
		}

		private void Parse(XmlNode p_node)
		{
			if (p_node == null)
			{
				return;
			}
			XmlNode xmlNode = p_node["Interstitial"];
			for (int i = 0; i < xmlNode.ChildNodes.Count; i++)
			{
				XmlNode xmlNode2 = xmlNode.ChildNodes.Item(i);
				switch (xmlNode2.Name)
				{
				case "Probability":
					_Probabilitys.Add(new Probability(xmlNode2));
					break;
				case "ProbabilityDefault":
					_DefaultProbability = new Probability(xmlNode2);
					break;
				}
			}
		}
	}
}
