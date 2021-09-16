using HarmonyLib;

namespace BunnyMod.Content.Patches
{
	[HarmonyPatch(declaringType: typeof(CharacterSelect))]
	public static class CharacterSelectPatches
	{
		/*[HarmonyPostfix, HarmonyPatch(methodName: nameof(CharacterSelect.FakeStart))]
		private static void FakeStart_Postfix(CharacterSelect __instance)
		{
			__instance.facialHairTypes.Add("TestFacialHair");
		}*/
	}
}