using System;
using CodeStage.AntiCheat.ObscuredTypes;
using Nekki.Vector.Core.User;
using UnityEngine;

namespace Nekki.Vector.Core.GameManagement
{
	public static class EnergyManager
	{
		public class EnergyManagerRender : MonoBehaviour
		{
			private const float _RefreshTime = 1f;

			private static EnergyManagerRender _Current;

			private float _Time;

			public static void Run()
			{
				if (_Current != null)
				{
					_Current.enabled = true;
				}
			}

			public static void Stop()
			{
				if (_Current != null)
				{
					_Current.enabled = false;
				}
			}

			private void Awake()
			{
				_Current = this;
			}

			private void OnDestroy()
			{
				_Current = null;
			}

			private void Update()
			{
				_Time += Time.deltaTime;
				if (_Time > 1f)
				{
					_Time = 0f;
					Render();
				}
			}
		}

		private static int _MaxLevel = int.MaxValue;

		private static int _CurrentLevel = 4;

		private static int _RechargeTime;

		private static ObscuredLong _TimeSpendEnergy;

		public static System.Action OnSpend;

		public static System.Action OnRecharge;

		public static int MaxLevel
		{
			get
			{
				return _MaxLevel;
			}
		}

		public static int CurrentLevel
		{
			get
			{
				return _CurrentLevel;
			}
		}

		public static bool IsEnergyFull
		{
			get
			{
				return _CurrentLevel >= _MaxLevel;
			}
		}

		public static bool IsEnergyEmpty
		{
			get
			{
				return _CurrentLevel == 0;
			}
		}

		public static int TimeToFullRecharge
		{
			get
			{
				Render();
				if (_CurrentLevel >= MaxLevel)
				{
					return -1;
				}
				int num = (_MaxLevel - _CurrentLevel) * _RechargeTime;
				return num - (int)((TimeManager.UTCTime - (long)_TimeSpendEnergy) / 1000);
			}
		}

		public static int TimeToOneCharge
		{
			get
			{
				return _RechargeTime - (int)((TimeManager.UTCTime - (long)_TimeSpendEnergy) / 1000);
			}
		}

		public static void Init()
		{
			if (_MaxLevel == int.MaxValue)
			{
				RefreshValuesByBalance();
				UserProperty userPropertyByName = DataLocal.Current.GetUserPropertyByName("Energy");
				if (userPropertyByName == null)
				{
					userPropertyByName = DataLocal.Current.GetOrCreateUserPropertyByName("Energy");
					userPropertyByName.AddAttribute("Level", "5");
					_CurrentLevel = 5;
					userPropertyByName.AddAttribute("SpendTime", "0");
				}
				else
				{
					_CurrentLevel = userPropertyByName.ValueInt("Level");
					int num = userPropertyByName.ValueInt("SpendTime");
					_TimeSpendEnergy = ((num != -1) ? TimeManager.ConvertIntToMs(num) : (-1));
				}
				Render();
			}
		}

		private static void SaveData()
		{
			UserProperty userProperty = DataLocal.Current.GetUserPropertyByName("Energy");
			if (userProperty == null)
			{
				userProperty = DataLocal.Current.GetOrCreateUserPropertyByName("Energy");
				userProperty.AddAttribute("Level", "5");
				userProperty.AddAttribute("SpendTime", "0");
			}
			userProperty.SetValue("Level", _CurrentLevel);
			userProperty.SetValue("SpendTime", ((long)_TimeSpendEnergy != -1) ? TimeManager.ConvertMsToInt(_TimeSpendEnergy) : (-1));
			DataLocal.Current.Save(false);
		}

		public static void RefreshValuesByBalance()
		{
			_MaxLevel = int.Parse(BalanceManager.Current.GetBalance("Energy", "MaxLevel"));
			_RechargeTime = int.Parse(BalanceManager.Current.GetBalance("Energy", "RechargeTime"));
		}

		public static void SpendEnergyLevel(int p_levels = 1)
		{
			_CurrentLevel -= p_levels;
			CallOnSpend();
			if (_CurrentLevel < _MaxLevel)
			{
				StartRechgarge();
			}
			SaveData();
		}

		public static void ChargeEnergyLevel(int p_levels = 1)
		{
			_CurrentLevel += p_levels;
			CallOnRecharge();
			SaveData();
		}

		public static void ForceChargeToFullLevel()
		{
			int p_levels = _MaxLevel - _CurrentLevel;
			_TimeSpendEnergy = -1L;
			ChargeEnergyLevel(p_levels);
		}

		public static void Render()
		{
			if (_MaxLevel <= _CurrentLevel)
			{
				_CurrentLevel = _MaxLevel;
				_TimeSpendEnergy = -1L;
				EnergyManagerRender.Stop();
				return;
			}
			int num = (int)((TimeManager.UTCTime - (long)_TimeSpendEnergy) / 1000);
			if (num >= _RechargeTime)
			{
				int num2 = num / _RechargeTime;
				if (_CurrentLevel + num2 >= _MaxLevel)
				{
					_TimeSpendEnergy = -1L;
					ChargeEnergyLevel(_MaxLevel - _CurrentLevel);
					EnergyManagerRender.Stop();
				}
				else
				{
					_TimeSpendEnergy = (long)_TimeSpendEnergy + _RechargeTime * num2 * 1000;
					ChargeEnergyLevel(num2);
				}
			}
		}

		private static void StartRechgarge()
		{
			if ((long)_TimeSpendEnergy == -1)
			{
				_TimeSpendEnergy = TimeManager.UTCTime;
			}
			EnergyManagerRender.Run();
		}

		private static void CallOnRecharge()
		{
			if (OnRecharge != null)
			{
				OnRecharge();
			}
		}

		private static void CallOnSpend()
		{
			if (OnSpend != null)
			{
				OnSpend();
			}
		}
	}
}
