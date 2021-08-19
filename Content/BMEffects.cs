using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RogueLibsCore;
using UnityEngine;

namespace BunnyMod.Content
{
	[EffectParameters(EffectLimitations.RemoveOnDeath | EffectLimitations.RemoveOnKnockOut | EffectLimitations.RemoveOnNextLevel)]
	public class ChronomanticDilation : CustomEffect
	{
		[RLSetup]
		public static void Setup()
		{
			RogueLibs.CreateCustomEffect<ChronomanticDilation>()
				.WithDescription(new CustomNameInfo("Time is slowed down for everyone but you. Still... treasure it. You'll never get it back."))
				.WithName(new CustomNameInfo("Chronomantic Dilation"));
		}
		public override int GetEffectTime() => 15;
		public override int GetEffectHate() => 0;
		public override void OnAdded()
		{
		}
		public override void OnRemoved()
		{
		}
		public override void OnUpdated(EffectUpdatedArgs e)
		{
			e.UpdateDelay = 1f;
			CurrentTime--;
		}
	}

	[EffectParameters(EffectLimitations.RemoveOnDeath | EffectLimitations.RemoveOnKnockOut | EffectLimitations.RemoveOnNextLevel)]
	public class MuscleSpasms : CustomEffect
	{
		[RLSetup]
		public static void Setup()
		{
			RogueLibs.CreateCustomEffect<MuscleSpasms>()
				.WithName(new CustomNameInfo("Muscle Spasms"))
				.WithDescription(new CustomNameInfo("Herky, jerky, and occasionally twerky."));
		}
		public override int GetEffectTime() => 30;
		public override int GetEffectHate() => 5;
		public override void OnAdded()
		{
			Owner.SetAccuracy(Owner.accuracyStatMod - 3);
		}
		public override void OnRemoved()
		{
			Owner.SetAccuracy(Owner.accuracyStatMod + 3);
		}
		public override void OnUpdated(EffectUpdatedArgs e)
		{
			e.UpdateDelay = 1.0f; // 1 update per second

			if (gc.percentChance(CurrentTime))
			{
				// Spasm here

				BMHeaderTools.SayDialogue(Owner, BMHeaderTools.RandomFromList(vocalTics), vNameType.Dialogue);
			}

			CurrentTime--;
		}

		List<string> vocalTics = new List<string>()
			{
				"MuscleSpasm_01",
				"MuscleSpasm_02",
				"MuscleSpasm_03",
				"MuscleSpasm_04",
				"MuscleSpasm_05",
				"MuscleSpasm_06",
				"MuscleSpasm_07",
				"MuscleSpasm_08",
				"MuscleSpasm_09",
				"MuscleSpasm_10",
				"MuscleSpasm_11",
				"MuscleSpasm_12",
			};

		public static void InitializeNames()
		{
			_ = RogueLibs.CreateCustomName("MuscleSpasm_01", "Dialogue", new CustomNameInfo("Gurk!"));
			_ = RogueLibs.CreateCustomName("MuscleSpasm_02", "Dialogue", new CustomNameInfo("Ack!"));
			_ = RogueLibs.CreateCustomName("MuscleSpasm_03", "Dialogue", new CustomNameInfo("Pfpfpfth!"));
			_ = RogueLibs.CreateCustomName("MuscleSpasm_04", "Dialogue", new CustomNameInfo("Egh!"));
			_ = RogueLibs.CreateCustomName("MuscleSpasm_05", "Dialogue", new CustomNameInfo("Ungh!"));
			_ = RogueLibs.CreateCustomName("MuscleSpasm_06", "Dialogue", new CustomNameInfo("*twitch*"));
			_ = RogueLibs.CreateCustomName("MuscleSpasm_07", "Dialogue", new CustomNameInfo("*jerk*"));
			_ = RogueLibs.CreateCustomName("MuscleSpasm_08", "Dialogue", new CustomNameInfo("*spasm*"));
			_ = RogueLibs.CreateCustomName("MuscleSpasm_09", "Dialogue", new CustomNameInfo("Graaah!"));
			_ = RogueLibs.CreateCustomName("MuscleSpasm_10", "Dialogue", new CustomNameInfo("Ick!"));
			_ = RogueLibs.CreateCustomName("MuscleSpasm_11", "Dialogue", new CustomNameInfo("Eh! Ooh! *twitch* Ow!"));
			_ = RogueLibs.CreateCustomName("MuscleSpasm_12", "Dialogue", new CustomNameInfo("Ag! *twitch*"));
		}
	}
}
