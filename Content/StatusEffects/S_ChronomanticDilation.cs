using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RogueLibsCore;
using UnityEngine;

namespace BunnyMod.Content.StatusEffects
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
}
