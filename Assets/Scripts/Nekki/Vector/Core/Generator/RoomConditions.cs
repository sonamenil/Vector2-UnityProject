using System.Collections.Generic;
using Nekki.Vector.Core.Counter;
using Nekki.Vector.Core.GameManagement;
using Nekki.Vector.Core.Generator.Test;
using Nekki.Yaml;

namespace Nekki.Vector.Core.Generator
{
	public class RoomConditions : ZoneResource<RoomConditions>
	{
		private List<Preset> _Presets = new List<Preset>();

		protected override string ResourceId
		{
			get
			{
				return "Generator";
			}
		}

		protected override void Parse()
		{
			YamlDocumentNekki yamlDocumentNekki = YamlUtils.OpenYamlFile(VectorPaths.GeneratorData, base.FilePath);
			foreach (Mapping item in yamlDocumentNekki.GetRoot(0))
			{
				_Presets.Add(Preset.Create(item));
			}
		}

		public RoomConditionList GetConditionList()
		{
			List<CounterCondition> list = new List<CounterCondition>();
			for (int i = 0; i < _Presets.Count; i++)
			{
				_Presets[i].GetCounterConditions(list);
			}
			return new RoomConditionList(list);
		}

		public RoomConditionListTest GetConditionListTest()
		{
			List<CounterCondition> list = new List<CounterCondition>();
			for (int i = 0; i < _Presets.Count; i++)
			{
				_Presets[i].GetCounterConditions(list);
			}
			return new RoomConditionListTest(list);
		}
	}
}
