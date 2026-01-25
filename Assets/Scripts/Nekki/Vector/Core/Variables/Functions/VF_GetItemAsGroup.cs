using Nekki.Vector.Core.User;

namespace Nekki.Vector.Core.Variables.Functions
{
	public class VF_GetItemAsGroup : VariableFunction
	{
		private Variable _GroupNameVar;

		private VariableItem _Itemvar;

		private Variable _AttributeName;

		public override int ValueInt
		{
			get
			{
				return _Itemvar.Item.GetIntValueForTrigger(_AttributeName.ValueString, _GroupNameVar.ValueString);
			}
		}

		public override string ValueString
		{
			get
			{
				return _Itemvar.Item.GetStringValueForTrigger(_AttributeName.ValueString, _GroupNameVar.ValueString);
			}
		}

		public override float ValueFloat
		{
			get
			{
				return _Itemvar.Item.GetIntValueForTrigger(_AttributeName.ValueString, _GroupNameVar.ValueString);
			}
		}

		public override string DebugStringValue
		{
			get
			{
				if (_Itemvar.Item == null)
				{
					return "Item:null";
				}
				return ValueInt.ToString();
			}
		}

		protected internal VF_GetItemAsGroup(string p_value, string p_name, IVariableParent p_parent)
			: base(p_value, p_name, p_parent)
		{
			p_value = VariableFunction.TrimSpaces(p_value);
			string[] array = p_value.Split('.');
			VariableFunction.InitFuncVar(p_parent, ref _AttributeName, array[1]);
			int num = array[0].IndexOf("[") + 1;
			int num2 = array[0].LastIndexOf("]");
			string text = array[0].Substring(num, num2 - num);
			string[] array2 = text.Split(',');
			VariableFunction.InitFuncVar(p_parent, ref _GroupNameVar, array2[0]);
			Variable p_var = null;
			VariableFunction.InitFuncVar(p_parent, ref p_var, array2[1]);
			_Itemvar = p_var as VariableItem;
			if (_Itemvar != null)
			{
			}
		}

		public override void SetValue(int p_value)
		{
			_Itemvar.Item.SetValue(p_value, _AttributeName.ValueString, _GroupNameVar.ValueString);
			DataLocal.Current.Save(false);
		}

		public override void SetValue(float p_value)
		{
			_Itemvar.Item.SetValue(p_value, _AttributeName.ValueString, _GroupNameVar.ValueString);
			DataLocal.Current.Save(false);
		}

		public override void SetValue(string p_value)
		{
			_Itemvar.Item.SetValue(p_value, _AttributeName.ValueString, _GroupNameVar.ValueString);
			DataLocal.Current.Save(false);
		}

		public override void AppendValue(int p_value)
		{
			_Itemvar.Item.AppendValue(p_value, _AttributeName.ValueString, _GroupNameVar.ValueString);
			DataLocal.Current.Save(false);
		}

		public override void AppendValue(float p_value)
		{
			_Itemvar.Item.AppendValue(p_value, _AttributeName.ValueString, _GroupNameVar.ValueString);
			DataLocal.Current.Save(false);
		}

		public override void AppendValue(string p_value)
		{
			_Itemvar.Item.AppendValue(p_value, _AttributeName.ValueString, _GroupNameVar.ValueString);
			DataLocal.Current.Save(false);
		}

		public void setVariableToItem(Variable p_var)
		{
			_Itemvar.Item.ChangeVar(p_var, _AttributeName.ValueString, _GroupNameVar.ValueString);
		}

		public override void Log(string name)
		{
			base.Log(name);
			VectorLog.Tab(1);
			VectorLog.RunLog("GroupName", _GroupNameVar);
			VectorLog.RunLog("Item", _Itemvar);
			VectorLog.Untab(1);
		}

		public override void SimplifyArguments()
		{
			SimplifyArgument(ref _GroupNameVar);
			SimplifyArgument(ref _AttributeName);
		}
	}
}
