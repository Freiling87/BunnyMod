using BunnyMod.Content.Traits;
using HarmonyLib;

namespace BunnyMod.Content.Patches
{
	[HarmonyPatch(declaringType: typeof(ObjectMult))]
	public static class P_ObjectMult
	{
		[HarmonyPrefix, HarmonyPatch(methodName: nameof(ObjectMult.RemoveShakedownPerson), argumentTypes: new[] { typeof(Agent), typeof(int) })]
		public static bool RemoveShakedownPerson_Prefix(Agent shookDownAgent, int shakedownAmount, ObjectMult __instance)
		{
			return Warlord.ObjectMult_RemoveShakedownPerson_Prefix(__instance);
		}
	}
}