using HarmonyLib;

namespace BunnyMod.Content.Patches
{
	[HarmonyPatch(declaringType: typeof(CharacterCreation))]
	public static class P_CharacterCreation
	{
		/*[HarmonyPostfix, HarmonyPatch(methodName: "Awake")]
		private static void Awake_Postfix(CharacterCreation __instance)
		{
			__instance.facialHairTypes.Add("TestFacialHair");
			__instance.facialHairTypes.Add("TestFacialHair");
		}*/
	}
}