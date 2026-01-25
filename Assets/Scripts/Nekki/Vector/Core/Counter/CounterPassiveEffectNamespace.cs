using System.Xml;
using CodeStage.AntiCheat.ObscuredTypes;
using Nekki.Vector.Core.PassiveEffects;

namespace Nekki.Vector.Core.Counter
{
	public class CounterPassiveEffectNamespace : CounterNamespace
	{
		public CounterPassiveEffectNamespace(string p_name)
			: base(p_name)
		{
		}

		public CounterPassiveEffectNamespace(CounterNamespace p_copy)
			: base(p_copy)
		{
		}

		public CounterPassiveEffectNamespace(string p_name, XmlNode p_node)
			: base(p_name, p_node)
		{
		}

		public override CounterNamespace Copy()
		{
			return new CounterPassiveEffectNamespace(this);
		}

		public override ObscuredInt GetCounter(string p_counterName)
		{
			return (int)base.GetCounter(p_counterName) + ControllerPassiveEffects.CounterAppendValue(p_counterName);
		}
	}
}
