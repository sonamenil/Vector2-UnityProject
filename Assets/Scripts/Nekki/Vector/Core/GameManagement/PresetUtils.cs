namespace Nekki.Vector.Core.GameManagement
{
	public static class PresetUtils
	{
		private const string _CreateCardPreset = "CreateCard";

		public static CardsGroupAttribute CreateCard(string p_cardName)
		{
			Preset presetByName = PresetsManager.GetPresetByName("CreateCard");
			StringBuffer.AddString("Arg0", p_cardName);
			PresetResult presetResult = presetByName.RunPreset();
			return CardsGroupAttribute.Create(presetResult.Item.Groups[0]);
		}
	}
}
