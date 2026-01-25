using System.Collections.Generic;
using System.Text;
using Nekki.Vector.Core.Counter;
using Nekki.Vector.Core.User;
using Nekki.Vector.Core.Variables;
using Nekki.Yaml;

namespace Nekki.Vector.Core.GameManagement
{
	public class CardsManager
	{
		private const string _FileName = "cards.yaml";

		private const string _BoostCardTimerPrefix = "BoostCardTimer_";

		private YamlDocumentNekki _YamlDocument;

		private static CardsManager _Current;

		public static CardsManager Current
		{
			get
			{
				if (_Current == null)
				{
					_Current = new CardsManager();
				}
				return _Current;
			}
		}

		public static float BoostTime
		{
			get
			{
				return float.Parse(BalanceManager.Current.GetBalance("Cards", "BoostTime"));
			}
		}

		public static string BoostCardTimerPrefix
		{
			get
			{
				return "BoostCardTimer_";
			}
		}

		private CardsManager()
		{
			OpenYamlDocument();
			TimersManager.OnTimerExpired += OnBoostCardTimerOver;
		}

		public static void Init()
		{
			if (_Current == null)
			{
				_Current = new CardsManager();
			}
		}

		public static void Reset()
		{
			_Current = null;
		}

		private void OpenYamlDocument()
		{
			_YamlDocument = YamlUtils.OpenYamlFile(VectorPaths.GeneratorDataDefault, "cards.yaml");
		}

		public bool HasCardElement(List<Variable> p_params, Variable p_element)
		{
			Mapping cardMapping = GetCardMapping(p_params);
			Nekki.Yaml.Node nodeFast = cardMapping.GetNodeFast(p_element.ValueString);
			return nodeFast != null;
		}

		public string GetCardInfo(List<Variable> p_params)
		{
			Mapping cardMapping = GetCardMapping(p_params, false);
			Nekki.Yaml.Node nodeFast = cardMapping.GetNodeFast(p_params[p_params.Count - 1].ValueString);
			if (nodeFast == null)
			{
				nodeFast = cardMapping.GetNodeFast("Default");
				if (nodeFast == null)
				{
					StringBuilder stringBuilder = new StringBuilder("Cards.yaml section not found: ");
					for (int i = 0; i < p_params.Count; i++)
					{
						stringBuilder.Append(p_params[i].ValueString);
						stringBuilder.Append(".");
					}
					DebugUtils.Dialog(stringBuilder.ToString(), false);
					return string.Empty;
				}
			}
			return Variable.CreateVariable(nodeFast.value.ToString(), string.Empty).ValueString;
		}

		public Mapping GetCardMapping(List<Variable> p_params, bool p_fullSearch = true)
		{
			Mapping mapping = _YamlDocument.GetRoot(0);
			int i = 0;
			for (int num = ((!p_fullSearch) ? (p_params.Count - 1) : p_params.Count); i < num; i++)
			{
				Mapping mappingFast = mapping.GetMappingFast(p_params[i].ValueString);
				if (mappingFast == null)
				{
					mapping = mapping.GetMappingFast("Default");
					if (mapping == null)
					{
						StringBuilder stringBuilder = new StringBuilder("Cards.yaml section not found: ");
						for (int j = 0; j <= i; j++)
						{
							stringBuilder.Append(p_params[j].ValueString);
							stringBuilder.Append(".");
						}
						DebugUtils.Dialog(stringBuilder.ToString(), false);
						return null;
					}
				}
				else
				{
					mapping = mappingFast;
				}
			}
			return mapping;
		}

		public Mapping GetCardMapping(bool p_notFindReturnNull, params string[] p_params)
		{
			Mapping mapping = _YamlDocument.GetRoot(0);
			int i = 0;
			for (int num = p_params.Length; i < num; i++)
			{
				Mapping mappingFast = mapping.GetMappingFast(p_params[i]);
				if (mappingFast == null)
				{
					mapping = mapping.GetMappingFast("Default");
					if (mapping != null)
					{
						continue;
					}
					if (!p_notFindReturnNull)
					{
						StringBuilder stringBuilder = new StringBuilder("Cards.yaml section not found: ");
						for (int j = 0; j <= i; j++)
						{
							stringBuilder.Append(p_params[j]);
							stringBuilder.Append(".");
						}
						DebugUtils.Dialog(stringBuilder.ToString(), false);
					}
					return null;
				}
				mapping = mappingFast;
			}
			return mapping;
		}

		public string GetCardInfo(params string[] p_params)
		{
			return GetCardInfo(false, p_params);
		}

		public string GetCardInfo(bool p_notFindReturnNull, params string[] p_params)
		{
			Mapping mapping = _YamlDocument.GetRoot(0);
			for (int i = 0; i < p_params.Length; i++)
			{
				if (i != p_params.Length - 1)
				{
					Mapping mappingFast = mapping.GetMappingFast(p_params[i]);
					if (mappingFast == null)
					{
						mapping = mapping.GetMappingFast("Default");
						if (mapping == null)
						{
							if (p_notFindReturnNull)
							{
								return null;
							}
							StringBuilder stringBuilder = new StringBuilder("Cards.yaml section not found: ");
							for (int j = 0; j <= i; j++)
							{
								stringBuilder.Append(p_params[j]);
								stringBuilder.Append(".");
							}
							DebugUtils.Dialog(stringBuilder.ToString(), false);
							return string.Empty;
						}
					}
					else
					{
						mapping = mappingFast;
					}
					continue;
				}
				Nekki.Yaml.Node nodeFast = mapping.GetNodeFast(p_params[i]);
				if (nodeFast == null)
				{
					nodeFast = mapping.GetNodeFast("Default");
					if (nodeFast == null)
					{
						if (p_notFindReturnNull)
						{
							return null;
						}
						StringBuilder stringBuilder2 = new StringBuilder("Cards.yaml section not found: ");
						for (int k = 0; k <= i; k++)
						{
							stringBuilder2.Append(p_params[k]);
							stringBuilder2.Append(".");
						}
						DebugUtils.Dialog(stringBuilder2.ToString(), false);
						return string.Empty;
					}
				}
				return Variable.CreateVariable(nodeFast.value.ToString(), string.Empty).ValueString;
			}
			return string.Empty;
		}

		public void BoostCard(CardsGroupAttribute p_card)
		{
			p_card.UserCardBoostLevel++;
			CounterController.Current.CreateCounterOrSetValue(p_card.CardName, 1, "OverweightedUpgrades");
			TimersManager.CreateTimer("BoostCardTimer_" + p_card.CardName, BoostTime);
		}

		public void OnBoostCardTimerOver(string p_timerName)
		{
			if (p_timerName.IndexOf("BoostCardTimer_") != -1)
			{
				string p_name = p_timerName.Replace("BoostCardTimer_", string.Empty);
				CardsGroupAttribute card = DataLocalHelper.GetCard(p_name);
				card.UserCardBoostLevel = 0;
			}
		}
	}
}
