using RogueLibsCore;
using System;
using System.Collections.Generic;
using UnityEngine;
using BunnyMod;
using Random = UnityEngine.Random;
using Object = UnityEngine.Object;

namespace BunnyMod.Content
{
	public class BMChallenges
	{
		public static void InitializeChallenges()
		{
			#region Physics/Gameplay
			//CustomMutator GasolineHumidity = RogueLibs.CreateCustomMutator(cChallenge.GasolineHumidity, true,
			//	new CustomNameInfo("Fire: Gasoline Humidity"),
			//	new CustomNameInfo("The weather for today is... all fucked up.\n\nFires spread faster."));
			//GasolineHumidity.Available = false;
			//GasolineHumidity.Conflicting.AddRange(new string[] { cChallenge.Mildfire });
			//GasolineHumidity.IsActive = false;

			//CustomMutator Mildfire = RogueLibs.CreateCustomMutator(cChallenge.Mildfire, true,
			//	new CustomNameInfo("Fire: Mildfire"),
			//	new CustomNameInfo("Fire spreads slower and doesn't last as long."));
			//Mildfire.Available = false;
			//Mildfire.Conflicting.AddRange(new string[] { cChallenge.GasolineHumidity, cChallenge.NapalmSprings });
			//Mildfire.IsActive = false;

			//CustomMutator NapalmSprings = RogueLibs.CreateCustomMutator(cChallenge.NapalmSprings, true,
			//	new CustomNameInfo("Fire: Napalm Springs"),
			//	new CustomNameInfo("Fire lasts longer and spreads further."));
			//NapalmSprings.Available = false;
			//NapalmSprings.Conflicting.AddRange(new string[] { cChallenge.Mildfire });
			//NapalmSprings.IsActive = false;




			//CustomMutator BoringPhysics = RogueLibs.CreateCustomMutator(cChallenge.BoringPhysics, true,
			//	new CustomNameInfo("Knockback: Boring Physics"),
			//	new CustomNameInfo("Knockback is reduced almost to zero. Nothing is gonna make this game realistic, ya know.\n\n- Knockback set to 10%"));
			//BoringPhysics.Available = false;
			//BoringPhysics.Conflicting.AddRange(new string[] { cChallenge.SaveTheWalls, vChallenge.BigKnockback, cChallenge.WallWallopWorld });
			//BoringPhysics.IsActive = false;

			//CustomMutator SaveTheWalls = RogueLibs.CreateCustomMutator(cChallenge.SaveTheWalls, true,
			//	new CustomNameInfo("Knockback: Save The Walls"),
			//	new CustomNameInfo(
			//		"There isn't much nature to protect in the City, so the Green party is a little confused. But due to the lobbying, there's now a city ordinance against punching people through walls, which is probably a good thing.\n\n- Knockback set to 50%."));
			//SaveTheWalls.Available = false;
			//SaveTheWalls.Conflicting.AddRange(new string[] { cChallenge.BoringPhysics, vChallenge.BigKnockback, cChallenge.WallWallopWorld });
			//SaveTheWalls.IsActive = false;

			//CustomMutator WallWallopWorld = RogueLibs.CreateCustomMutator(cChallenge.WallWallopWorld, true,
			//	new CustomNameInfo("Knockback: Wall Wallop World"),
			//	new CustomNameInfo(
			//		"Someone - not naming names here, but you know who you are - put a bunch of Muscly Pills onto the Fud Factory conveyor belt. And now we all have to suffer for it. Thanks a lot.\n\n- Knockback set to 500%"));
			//WallWallopWorld.Available = false;
			//WallWallopWorld.Conflicting.AddRange(new string[] { cChallenge.BoringPhysics, cChallenge.SaveTheWalls, vChallenge.BigKnockback });
			//WallWallopWorld.IsActive = false;




			#endregion
			#region Resistance Quartermaster
			//CustomMutator RushinRevolution = RogueLibs.CreateCustomMutator(cChallenge.RushinRevolution, true,
			//	new CustomNameInfo("QuestCount: Rushin' Revolution"),
			//	new CustomNameInfo(
			//		"There are decades where nothing happens; and there are weeks where decades happen. And then there are days where you just don't have time for this shit.\n\nNo quests. Bum rush the Mayor. Take a long weekend."));
			//RushinRevolution.Available = false;
			//RushinRevolution.Conflicting.AddRange(cChallenge.QuestCount);
			//RushinRevolution.IsActive = false;

			//CustomMutator SingleMinded = RogueLibs.CreateCustomMutator(cChallenge.SingleMinded, true,
			//	new CustomNameInfo("QuestCount: Single-minded"),
			//	new CustomNameInfo("Your Resistance HR profile says \"Not a good multi-tasker.\" They only give you one job per Floor."));
			//SingleMinded.Available = false;
			//SingleMinded.Conflicting.AddRange(cChallenge.QuestCount);
			//SingleMinded.IsActive = false;

			//CustomMutator Workhorse = RogueLibs.CreateCustomMutator(cChallenge.Workhorse, true,
			//	new CustomNameInfo("QuestCount: Workhorse"),
			//	new CustomNameInfo(
			//		"You made the mistake of being reliable. Now the Resistance sends you all the work. You don't mind, because the long hours are an excuse to avoid your family."));
			//Workhorse.Available = false;
			//Workhorse.Conflicting.AddRange(cChallenge.QuestCount);
			//Workhorse.IsActive = false;



			//CustomMutator DoublePlyRewards = RogueLibs.CreateCustomMutator(cChallenge.DoublePlyRewards, true,
			//	new CustomNameInfo("Rewards: Double-Ply Rewards"),
			//	new CustomNameInfo(
			//		"The Resistance is running low on... everything. But please, accept this stack of coupons! They are totally really actually valid.\n\nThe smell? That's *value*!"));
			//DoublePlyRewards.Available = false;
			//DoublePlyRewards.Conflicting.Add(vChallenge.MoneyRewards);
			//DoublePlyRewards.Conflicting.AddRange(new string[] { cChallenge.UnpaidInternship });
			//DoublePlyRewards.IsActive = false;

			//CustomMutator UnpaidInternship = RogueLibs.CreateCustomMutator(cChallenge.UnpaidInternship, true,
			//	new CustomNameInfo("Rewards: Unpaid Internship"),
			//	new CustomNameInfo(
			//		"The double-experience you're getting working for the Resistance is worth more than any reward, they say. But so far, you're mainly learning one thing: Work for people who pay you. At least they provide lunch, though?"));
			//UnpaidInternship.Available = false;
			//UnpaidInternship.Conflicting.Add(vChallenge.MoneyRewards);
			//UnpaidInternship.Conflicting.AddRange(new string[] { cChallenge.DoublePlyRewards });
			//UnpaidInternship.IsActive = false;
			#endregion
		}
	}
}