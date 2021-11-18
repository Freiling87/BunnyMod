using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using BepInEx.Logging;
using BTHarmonyUtils.TranspilerUtils;
using BunnyMod.Logging;
using HarmonyLib;

namespace BunnyMod.Patches
{
	[HarmonyPatch(declaringType: typeof(ScrollingMenu))]
	public static class ScrollingMenuPatches
	{
		private static readonly ManualLogSource logger = BMLogger.GetLogger();

		[HarmonyTranspiler, HarmonyPatch(methodName: nameof(ScrollingMenu.GetTraitsRemoveTrait))]
		private static IEnumerable<CodeInstruction> GetTraitsRemoveTrait_Transpiler(IEnumerable<CodeInstruction> instructionsEnumerable)
		{
			// Removed gate that prevents traits with less than -5 value being removed.
			
			FieldInfo unlock_cost3 = AccessTools.Field(typeof(Unlock), nameof(Unlock.cost3));
			FieldInfo scrollingMenu_gc = AccessTools.Field(typeof(ScrollingMenu), nameof(ScrollingMenu.gc));
			FieldInfo gameController_challenges = AccessTools.Field(typeof(GameController), nameof(GameController.challenges));

			// TODO change to marker-method
			// since we want to remove the `(unlock.cost3 > -5 || this.gc.challenges.Contains("NoLimits"))` condition, we'll look for exactly that
			CodeReplacementPatch patch = new CodeReplacementPatch(
					expectedMatches: 1,
					targetInstructionSequence: new List<CodeInstruction>
					{
							new CodeInstruction(OpCodes.Ldloc_3), // get the `Unlock` local variable
							new CodeInstruction(OpCodes.Ldfld, unlock_cost3), // access the cost3 field on that variable
							new CodeInstruction(OpCodes.Ldc_I4_S, -5), // prepare int value -5
							new CodeInstruction(OpCodes.Bgt_S), // if cost3 > -5 move on.
							new CodeInstruction(OpCodes.Ldarg_0), // get `this`
							new CodeInstruction(OpCodes.Ldfld, scrollingMenu_gc), // get this.gc
							new CodeInstruction(OpCodes.Ldfld, gameController_challenges), // get this.gc.challenges
							new CodeInstruction(OpCodes.Ldstr, "NoLimits"), // prepare string "NoLimits"
							new CodeInstruction(OpCodes.Callvirt), // call this.gc.challenges.Contains("NoLimits")
							new CodeInstruction(OpCodes.Brfalse_S) // go somewhere else if it does not contain "NoLimits"
					});

			List<CodeInstruction> instructions = instructionsEnumerable.ToList();
			patch.ApplySafe(instructions, logger);
			return instructions;
		}
	}
}