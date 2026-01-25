using System.Collections.Generic;
using System.Text;

namespace Nekki.Vector.Core.Generator
{
	public class GeneratorLabelsRange
	{
		private Dictionary<string, Point> _Ranges = new Dictionary<string, Point>();

		private Dictionary<string, bool> _RangesFlag = new Dictionary<string, bool>();

		public bool IsContentRange(string p_name)
		{
			return _Ranges.ContainsKey(p_name);
		}

		public Point GetRange(string p_name)
		{
			return _Ranges[p_name];
		}

		public void Collect(List<Variant> p_variants)
		{
			if (p_variants != null)
			{
				for (int i = 0; i < p_variants.Count; i++)
				{
					Collect(p_variants[i], i == 0);
				}
			}
		}

		private void Collect(Variant p_variant, bool p_isFirstVariant)
		{
			ResetRangesFlag();
			if (!p_variant.IsContainsChoice)
			{
				Collect(p_variant.GeneratorLabels, false, !p_isFirstVariant);
				SetNotSetedRangeMinZerro();
				return;
			}
			if (p_variant.GeneratorLabels == null && p_variant.ChildChoice.Count == 1)
			{
				p_variant.ChildChoice[0].CollectRanges();
				Collect(p_variant.ChildChoice[0].Ranges, false);
				SetNotSetedRangeMinZerro();
				return;
			}
			GeneratorLabelsRange generatorLabelsRange = new GeneratorLabelsRange();
			generatorLabelsRange.Collect(p_variant.ChildChoice, true);
			generatorLabelsRange.Collect(p_variant.GeneratorLabels, true, !p_isFirstVariant);
			generatorLabelsRange.SetNotSetedRangeMinZerro();
			Collect(generatorLabelsRange, false);
			SetNotSetedRangeMinZerro();
		}

		public void Collect(List<Choice> p_Choices, bool p_isSum)
		{
			if (p_Choices != null)
			{
				for (int i = 0; i < p_Choices.Count; i++)
				{
					p_Choices[i].CollectRanges();
					Collect(p_Choices[i].Ranges, p_isSum);
				}
			}
		}

		public void Collect(List<GeneratorLabel> p_labels, bool p_isSum, bool p_setMinZero)
		{
			if (p_labels != null)
			{
				for (int i = 0; i < p_labels.Count; i++)
				{
					Collect(p_labels[i].Name, p_labels[i].Value, p_isSum, p_setMinZero);
				}
			}
		}

		private void Collect(GeneratorLabelsRange p_range, bool p_isSumm)
		{
			foreach (KeyValuePair<string, Point> range in p_range._Ranges)
			{
				Collect(range.Key, range.Value, p_isSumm);
			}
		}

		private void Collect(string p_name, int p_value, bool p_isSum, bool p_setMinZero)
		{
			Point point = null;
			if (_Ranges.ContainsKey(p_name))
			{
				point = _Ranges[p_name];
				_RangesFlag[p_name] = true;
				if (p_isSum)
				{
					point.X += p_value;
					point.Y += p_value;
					return;
				}
				if (point.X > (float)p_value)
				{
					point.X = p_value;
				}
				if (point.Y < (float)p_value)
				{
					point.Y = p_value;
				}
			}
			else
			{
				point = new Point((!p_setMinZero) ? p_value : 0, p_value);
				_Ranges.Add(p_name, point);
				_RangesFlag.Add(p_name, true);
			}
		}

		private void Collect(string p_name, Point p_value, bool p_isSum)
		{
			Point point = null;
			if (_Ranges.ContainsKey(p_name))
			{
				point = _Ranges[p_name];
				_RangesFlag[p_name] = true;
				if (p_isSum)
				{
					point.Add(p_value);
					return;
				}
				if (point.X > p_value.X)
				{
					point.X = p_value.X;
				}
				if (point.Y < p_value.Y)
				{
					point.Y = p_value.Y;
				}
			}
			else
			{
				point = new Point(p_value);
				_Ranges.Add(p_name, point);
				_RangesFlag.Add(p_name, true);
			}
		}

		private void ResetRangesFlag()
		{
			List<string> list = new List<string>(_RangesFlag.Keys);
			for (int i = 0; i < list.Count; i++)
			{
				_RangesFlag[list[i]] = false;
			}
		}

		private void SetNotSetedRangeMinZerro()
		{
			foreach (KeyValuePair<string, bool> item in _RangesFlag)
			{
				if (!item.Value)
				{
					_Ranges[item.Key].X = 0f;
				}
			}
		}

		public void Log(StringBuilder p_sb)
		{
			foreach (KeyValuePair<string, Point> range in _Ranges)
			{
				p_sb.Append("  " + range.Key + " " + range.Value.X + " " + range.Value.Y + "\n");
			}
		}
	}
}
