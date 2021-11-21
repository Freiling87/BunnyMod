using RogueLibsCore;
using System.Collections.Generic;
using BepInEx.Logging;
using BunnyMod.Content.Logging;
using UnityEngine;
using Random = UnityEngine.Random;
using BunnyMod.Content.Traits;
using HarmonyLib;
using BunnyMod.Content.Challenges;
using System.Reflection;
using BTHarmonyUtils.TranspilerUtils;
using System.Reflection.Emit;
using System.Linq;

namespace BunnyMod.Content.Custom
{
	public static class C_LevelGen
	{
		private static readonly ManualLogSource logger = BMLogger.GetLogger();
		private static GameController GC => GameController.gameController;

		public static void SetQuestTriesTotal()
		{
			string active = BMChallengesManager.GetActiveChallengeFromList(cChallenge.QuestCount);

			if (active == cChallenge.RushinRevolution)
				GC.quests.questTriesTotal = 0;
			else if (active == cChallenge.SingleMinded)
				GC.quests.questTriesTotal = 1;
			else if (active == cChallenge.Workhorse)
				GC.quests.questTriesTotal = 4;
		}

		[HarmonyTranspiler, HarmonyPatch(methodName: "CreateInitialMap")]
		private static IEnumerable<CodeInstruction> CreateInitialMap_Transpiler_QuestCount(IEnumerable<CodeInstruction> instructionsEnumerable, ILGenerator generator)
		{
			List<CodeInstruction> instructions = instructionsEnumerable.ToList();

			FieldInfo loadLevel_squareMap = AccessTools.Field(typeof(LoadLevel), nameof(LoadLevel.squareMap));
			MethodInfo levelGenTools_SetQuestTriesTotal = AccessTools.Method(typeof(C_LevelGen), nameof(C_LevelGen.SetQuestTriesTotal));

			CodeReplacementPatch patch = new CodeReplacementPatch(
				expectedMatches: 1,
				prefixInstructionSequence: new List<CodeInstruction>
				{
					// Line 391
					new CodeInstruction(OpCodes.Ldarg_0),
					new CodeInstruction(OpCodes.Ldfld, loadLevel_squareMap),
					new CodeInstruction(OpCodes.Brfalse),
					new CodeInstruction(OpCodes.Ldarg_0),
				},
				insertInstructionSequence: new List<CodeInstruction>
				{
					// LevelGenTools.SetQuestTriesTotal();
					new CodeInstruction(OpCodes.Call, levelGenTools_SetQuestTriesTotal), // Clear
				});

			patch.ApplySafe(instructions, logger);
			return instructions;
		}
	}
}
