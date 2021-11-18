using BunnyMod.Traits.T_Social;
using HarmonyLib;

namespace BunnyMod.Patches
{
	[HarmonyPatch(declaringType: typeof(ObjectMult))]
	public static class ObjectMultPatches
	{
		[HarmonyPrefix, HarmonyPatch(methodName: nameof(ObjectMult.RemoveShakedownPerson), argumentTypes: new[] { typeof(Agent), typeof(int) })]
		public static bool RemoveShakedownPerson_Prefix(Agent shookDownAgent, int shakedownAmount, ObjectMult __instance)
		{
			return Warlord.ObjectMult_RemoveShakedownPerson_Prefix(__instance);
		}
	}
}