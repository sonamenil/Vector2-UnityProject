using Nekki.Vector.Core.Game;
using Nekki.Vector.Core.Generator;
using Nekki.Vector.GUI;

namespace Nekki.Vector.Core
{
	public class LinkEditor
	{
		public static bool TryLaunchEditorRoom()
		{
			int p_floor = Settings.PlayCommandFloor - 1;
			if (!GeneratorHelper.PrepareEditorCommand(p_floor))
			{
				return false;
			}
			Manager.Load(SceneKind.Run);
			return true;
		}

		public static bool IsFileNew()
		{
			return GeneratorHelper.IsEditorGenFileNew();
		}

		public static void RemoveTempFile()
		{
			GeneratorHelper.RemoveEditorGenFile();
		}
	}
}
