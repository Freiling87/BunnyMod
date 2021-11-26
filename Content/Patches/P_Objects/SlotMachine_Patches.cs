using BunnyMod.ObjectBehaviour.Controllers;
using HarmonyLib;

namespace BunnyMod.Patches.Objects
{
	[HarmonyPatch(declaringType: typeof(SlotMachine))]
	public static class SlotMachine_Patches
	{
		[HarmonyPrefix, HarmonyPatch(methodName: nameof(SlotMachine.Interact), argumentTypes: new[] { typeof(Agent) })]
		private static bool Interact_Prefix(SlotMachine __instance, Agent agent)
		{
			return SlotMachineController.HandleInteract_Prefix(__instance, agent);
		}

		[HarmonyPrefix, HarmonyPatch(methodName: nameof(SlotMachine.InteractFar), argumentTypes: new[] { typeof(Agent) })]
		private static bool InteractFar_Prefix(SlotMachine __instance, Agent agent)
		{
			return SlotMachineController.HandleInteractFar_Prefix(__instance, agent);
		}

		[HarmonyPrefix, HarmonyPatch(methodName: nameof(SlotMachine.Gamble), argumentTypes: new[] { typeof(int) })]
		private static bool Gamble_Prefix(SlotMachine __instance, int gambleAmt)
		{
			return !SlotMachineController.HandleGamble(__instance, gambleAmt);
		}
	}
}