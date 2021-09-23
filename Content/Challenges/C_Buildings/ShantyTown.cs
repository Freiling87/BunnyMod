using RogueLibsCore;
using System;
using System.Collections.Generic;
using UnityEngine;
using BunnyMod;
using Random = UnityEngine.Random;
using Object = UnityEngine.Object;

namespace BunnyMod.Content.Challenges.C_Buildings
{
	public class ShantyTown
	{
		[RLSetup]
		static void Start()
		{
			UnlockBuilder unlockBuilder = RogueLibs.CreateCustomUnlock(new MutatorUnlock(cChallenge.ShantyTown, true))
				.WithDescription(new CustomNameInfo 
				{
					[LanguageCode.English] = "Buildings: Shanty Town",
				})
				.WithName(new CustomNameInfo
				{
					[LanguageCode.English] = "More wooden construction: A tinderbox on cinder blocks.\n\nHard mode for Firefighters, easy mode for arsonists. Fun mode for psychopaths.",
				});

			BMChallengesManager.RegisterChallenge<ShantyTown>(new BMChallengeInfo(cChallenge.ShantyTown, unlockBuilder)
				.WithConflictGroup(EChallengeConflictGroup.BuildingChallenges));
		}
	}
}
