namespace Nekki.Vector.Core.Variables.Functions
{
	internal class VF_GetTimerFrames : VariableFunction
	{
		public override int ValueInt
		{
			get
			{
				return (RunMainController.Location.Timer == null) ? (-1) : RunMainController.Location.Timer.FrameCount;
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

		public override string ValueForSave
		{
			get
			{
				return ((!_IsPointer) ? "?" : "?_") + "getTimerFrames[]";
			}
		}

		protected internal VF_GetTimerFrames(string p_value, string p_name, IVariableParent p_parent)
			: base(p_value, p_name, p_parent)
		{
		}
	}
}
