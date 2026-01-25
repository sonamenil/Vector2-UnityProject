using System.Text;
using Nekki.Vector.Core.CameraEffects;
using Nekki.Vector.Core.Utilites;

namespace Nekki.Vector.Core.Variables.Functions
{
	public class VF_GetCameraEffect : VariableFunction
	{
		private delegate int ParamGetter(CameraEffectBase p_effect);

		private delegate void ParamSetter(CameraEffectBase p_effect, int p_value);

		private Variable _EffectName;

		private string _ParamName;

		private ParamGetter _Getter;

		private ParamSetter _Setter;

		public override int ValueInt
		{
			get
			{
				CameraEffectBase effect = GetEffect();
				return _Getter(effect);
			}
		}

		public override float ValueFloat
		{
			get
			{
				return ValueInt;
			}
		}

		public override string ValueString
		{
			get
			{
				return ValueInt.ToString();
			}
		}

		public override string DebugStringValue
		{
			get
			{
				StringBuilder stringBuilder = new StringBuilder();
				CameraEffectBase effect = GetEffect();
				if (effect != null)
				{
					stringBuilder.Append("Effect=" + effect.GetType().Name);
				}
				else
				{
					stringBuilder.Append("Effect=null");
				}
				stringBuilder.Append(" Param=" + _ParamName);
				return stringBuilder.ToString();
			}
		}

		public override string ValueForSave
		{
			get
			{
				string text = ((!_IsPointer) ? "?" : "?_");
				string text2 = text;
				return text2 + "getCameraEffect[" + _EffectName.ValueForSave + "]." + _ParamName;
			}
		}

		protected internal VF_GetCameraEffect(string p_value, string p_name, IVariableParent p_parent)
			: base(p_value, p_name, p_parent)
		{
			p_value = VariableFunction.TrimSpaces(p_value);
			string[] array = p_value.Split('.');
			int num = array[0].IndexOf("[") + 1;
			int num2 = array[0].LastIndexOf("]");
			string p_name2 = array[0].Substring(num, num2 - num);
			VariableFunction.InitFuncVar(p_parent, ref _EffectName, p_name2);
			_ParamName = array[1];
			if (_ParamName == "enabled")
			{
				_Getter = GetEnabled;
				_Setter = SetEnabled;
			}
			else if (_ParamName == "alpha")
			{
				_Getter = GetAlpha;
				_Setter = SetAlpha;
			}
			else if (_ParamName == "radius")
			{
				_Getter = GetRadius;
				_Setter = SetRadius;
			}
			if (_Getter == null || _Setter == null)
			{
				DebugUtils.Dialog("VF_GetCameraEffect init error by param name = " + _ParamName, true);
			}
		}

		private CameraEffectBase GetEffect()
		{
			return CameraEffectManager.GetEffect(EnumUtils.Parse(_EffectName.ValueString, CameraEffectType.Unknown));
		}

		public int GetEnabled(CameraEffectBase p_effect)
		{
			return p_effect.enabled ? 1 : 0;
		}

		public int GetAlpha(CameraEffectBase p_effect)
		{
			return (p_effect as DarknessEffect).Alpha;
		}

		public int GetRadius(CameraEffectBase p_effect)
		{
			return (p_effect as DarknessEffect).Range;
		}

		public void SetEnabled(CameraEffectBase p_effect, int p_value)
		{
			p_effect.enabled = p_value != 0;
		}

		public void SetAlpha(CameraEffectBase p_effect, int p_value)
		{
			(p_effect as DarknessEffect).Alpha = p_value;
		}

		public void SetRadius(CameraEffectBase p_effect, int p_value)
		{
			(p_effect as DarknessEffect).Range = p_value;
		}

		public override void SetValue(int p_value)
		{
			CameraEffectBase effect = GetEffect();
			_Setter(effect, p_value);
		}

		public override void SetValue(float p_value)
		{
			SetValue((int)p_value);
		}

		public override void AppendValue(int p_value)
		{
			CameraEffectBase effect = GetEffect();
			_Setter(effect, _Getter(effect) + p_value);
		}

		public override void AppendValue(float p_value)
		{
			AppendValue((int)p_value);
		}

		public override void Log(string name)
		{
			base.Log(name);
			VectorLog.Tab(1);
			VectorLog.Log("Effect: " + _EffectName);
			VectorLog.Log("Param: " + _ParamName);
			VectorLog.Untab(1);
		}
	}
}
