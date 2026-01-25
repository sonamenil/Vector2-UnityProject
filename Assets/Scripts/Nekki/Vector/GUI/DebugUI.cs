using Nekki.Vector.GUI.Common;
using Nekki.Vector.GUI.Scenes.Run;

namespace Nekki.Vector.GUI
{
	public class DebugUI : ModuleHolder
	{
		public static ConsoleUI Console
		{
			get
			{
				return UIModule.GetModule<ConsoleUI>();
			}
		}

		public static FPSMeter FPSMeter
		{
			get
			{
				return UIModule.GetModule<FPSMeter>();
			}
		}

		public static RunFPSMeter RunFPSMeter
		{
			get
			{
				return UIModule.GetModule<RunFPSMeter>();
			}
		}

		public static DebugPanel RunDebugPanel
		{
			get
			{
				return UIModule.GetModule<DebugPanel>();
			}
		}
	}
}
