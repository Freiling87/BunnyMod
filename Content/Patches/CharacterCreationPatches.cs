using HarmonyLib;

namespace BunnyMod.Patches
{
	[HarmonyPatch(declaringType: typeof(CharacterCreation))]
	public static class CharacterCreationPatches
	{
		/*[HarmonyPostfix, HarmonyPatch(methodName: "Awake")]
		private static void Awake_Postfix(CharacterCreation __instance)
		{
			__instance.facialHairTypes.Add("TestFacialHair");
			__instance.facialHairTypes.Add("TestFacialHair");
		}*/
	}
}