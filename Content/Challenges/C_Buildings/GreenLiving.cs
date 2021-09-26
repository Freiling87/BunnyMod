using RogueLibsCore;
using System;
using System.Collections.Generic;
using UnityEngine;
using BunnyMod;
using Random = UnityEngine.Random;
using Object = UnityEngine.Object;

namespace BunnyMod.Content.Challenges.C_Buildings
{
	public class GreenLiving
	{
		[RLSetup]
		static void Start()
		{
			UnlockBuilder unlockBuilder = RogueLibs.CreateCustomUnlock(new MutatorUnlock(cChallenge.GreenLiving, true))
				.WithDescription(new CustomNameInfo 
				{
					[LanguageCode.English] = "Buildings: Green Living",
				})
				.WithName(new CustomNameInfo
				{
					[LanguageCode.English] = "The Mayor has retrofitted most buildings to eco-friendly plant-based construction. The air is mighty fresh... except near the compost-burning stoves.",
				});

			BMChallengesManager.RegisterChallenge<GreenLiving>(new BMChallengeInfo(cChallenge.GreenLiving, unlockBuilder)
				.WithConflictGroup(EChallengeConflictGroup.BuildingChallenges));
		}
	}
}
