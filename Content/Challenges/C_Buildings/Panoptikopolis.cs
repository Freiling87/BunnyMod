using RogueLibsCore;
using System;
using System.Collections.Generic;
using UnityEngine;
using BunnyMod;
using Random = UnityEngine.Random;
using Object = UnityEngine.Object;

namespace BunnyMod.Content.Challenges.C_Buildings
{
	public class Panoptikopolis
	{
		[RLSetup]
		static void Start()
		{
			UnlockBuilder unlockBuilder = RogueLibs.CreateCustomUnlock(new MutatorUnlock(cChallenge.Panoptikopolis, true))
				.WithDescription(new CustomNameInfo 
				{
					[LanguageCode.English] = "Buildings: Panoptikopolis",
				})
				.WithName(new CustomNameInfo
				{
					[LanguageCode.English] = "Authoritarian surveillance measures mandate that most buildings have to be built with glass walls. If you have nothing to hide, what are you worried about, citizen?",
				});

			BMChallengesManager.RegisterChallenge<Panoptikopolis>(new BMChallengeInfo(cChallenge.Panoptikopolis, unlockBuilder)
				.WithConflictGroup(EChallengeConflictGroup.BuildingChallenges));
		}
	}
}
