using RogueLibsCore;
using System;
using System.Collections.Generic;
using UnityEngine;
using BunnyMod;
using Random = UnityEngine.Random;
using Object = UnityEngine.Object;

namespace BunnyMod.Content.Challenges.C_Buildings
{
	public class SpelunkyDory
	{
		[RLSetup]
		static void Start()
		{
			UnlockBuilder unlockBuilder = RogueLibs.CreateCustomUnlock(new MutatorUnlock(cChallenge.SpelunkyDory, true))
				.WithDescription(new CustomNameInfo 
				{
					[LanguageCode.English] = "Buildings: Spelunky Dory",
				})
				.WithName(new CustomNameInfo
				{
					[LanguageCode.English] = "You and your fellow citizens live in a disgusting cave complex. As the mayor says, \"Don't be a CAN'Tibal, be a CANnibal!\"\n\nMan, fuck the Mayor.",
				});

			BMChallengesManager.RegisterChallenge<SpelunkyDory>(new BMChallengeInfo(cChallenge.SpelunkyDory, unlockBuilder)
				.WithConflictGroup(EChallengeConflictGroup.BuildingChallenges));
		}
	}
}
