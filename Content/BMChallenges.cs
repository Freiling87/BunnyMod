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
		public static GameController GC => GameController.gameController;
		public static bool Prefix(Type type, string methodName, Type patchType, string patchMethodName, Type[] types) => BMHeader.MainInstance.PatchPrefix(type, methodName, patchType, patchMethodName, types);
		public static bool Postfix(Type type, string methodName, Type patchType, string patchMethodName, Type[] types) => BMHeader.MainInstance.PatchPostfix(type, methodName, patchType, patchMethodName, types);
		public static void BMLog(string logMessage) => BMHeader.Log(logMessage);

		public void Awake()
		{
			InitializeChallenges();
		}
		public static void InitializeChallenges()
		{
			#region Map Generation Overhauls

			CustomMutator AnCapistan = RogueLibs.CreateCustomMutator(cChallenge.AnCapistan, true,
				new CustomNameInfo("000: Overhaul: AnCapistan"),
				new CustomNameInfo("Freedom, at last! Freedom to starve in the gutter and watch your children wallow in the poverty you could never escape. Keep on dreaming and you'll make it someday!"));
			AnCapistan.Available = false;
			AnCapistan.Conflicting.AddRange(vChallenge.AddsLawEnforcement);
			AnCapistan.Conflicting.AddRange(cChallenge.Overhauls);
			AnCapistan.Conflicting.AddRange(new string[] { vChallenge.MixedUpLevels, vChallenge.NoGuns, cChallenge.ArcologyEcology });
			AnCapistan.IsActive = false;

			CustomMutator MACITS = RogueLibs.CreateCustomMutator(cChallenge.MACITS, true,
				new CustomNameInfo("000: Overhaul: Mostly Automated Comfortable Inclusive Terrestrial Socialism"),
				new CustomNameInfo("The Revolution is complete! No more living in privation to feed the fat capitalist pigs! Now all that's left is to have another Revolution and fuck it all up!"));
			MACITS.Available = false;
			MACITS.Conflicting.AddRange(cChallenge.AddCriminals);
			MACITS.Conflicting.AddRange(cChallenge.Overhauls);
			MACITS.Conflicting.AddRange(vChallenge.RemovesLawEnforcement);
			MACITS.Conflicting.Add(vChallenge.MixedUpLevels);
			MACITS.IsActive = false;

			CustomMutator PoliceState = RogueLibs.CreateCustomMutator(cChallenge.PoliceState, true,
				new CustomNameInfo("000: Overhaul: Police State"),
				new CustomNameInfo("The Mayor has decided to get tough on crime. What's crime? Whatever we want it to be."));
			PoliceState.Available = false;
			PoliceState.Conflicting.AddRange(cChallenge.AddCriminals);
			PoliceState.Conflicting.AddRange(vChallenge.AddsNonhumans);
			PoliceState.Conflicting.AddRange(cChallenge.Overhauls);
			PoliceState.Conflicting.AddRange(vChallenge.RemovesLawEnforcement);
			PoliceState.Conflicting.AddRange(vChallenge.Zombies);
			PoliceState.Conflicting.Add(vChallenge.MixedUpLevels);
			PoliceState.IsActive = false;

			#endregion
			#region Floors & Features

			CustomMutator ArcologyEcology = RogueLibs.CreateCustomMutator(cChallenge.ArcologyEcology, true,
				new CustomNameInfo("000: FloorTiles: Arcology Ecology"),
				new CustomNameInfo("Sustainable Eco-homes! Trees! Less pollution! What's not to love?\n\n(Answer: It's still miserable.)"));
			ArcologyEcology.Available = true;
			ArcologyEcology.Conflicting.AddRange(cChallenge.FloorsAndFeatures);
			ArcologyEcology.IsActive = false;

			CustomMutator SkywayDistrict = RogueLibs.CreateCustomMutator(cChallenge.SkywayDistrict, true,
				new CustomNameInfo("000: FloorTiles: Skyway District"),
				new CustomNameInfo(""));
			SkywayDistrict.Available = true;
			SkywayDistrict.IsActive = false;

			CustomMutator SunkenCity = RogueLibs.CreateCustomMutator(cChallenge.SunkenCity, true,
				new CustomNameInfo("000: FloorTiles: Sunken City"),
				new CustomNameInfo("More like \"Stinkin' Shitty!\" No, but seriously, that's all sewage."));
			SunkenCity.Available = true;
			SunkenCity.Conflicting.AddRange(cChallenge.FloorsAndFeatures);
			SunkenCity.IsActive = false;

			CustomMutator TransitExperiment = RogueLibs.CreateCustomMutator(cChallenge.TransitExperiment, true,
				new CustomNameInfo("000: FloorTiles: Transit Experiment"),
				new CustomNameInfo("The City's authorities considered making all the public streets into conveyor belts, but that was too hard to mod into the game... er, I mean construct. Yeah."));
			TransitExperiment.Available = true;
			TransitExperiment.Conflicting.AddRange(cChallenge.FloorsAndFeatures);
			TransitExperiment.IsActive = false;

			#endregion
			#region Map Size

			CustomMutator ACityForAnts = RogueLibs.CreateCustomMutator(cChallenge.ACityForAnts, true,
				new CustomNameInfo("MapSize: A City for Ants?!"),
				new CustomNameInfo("Yes, that is indeed what it is, figuratively speaking."));
			ACityForAnts.Available = true;
			ACityForAnts.Conflicting.AddRange(cChallenge.MapSize);
			ACityForAnts.IsActive = false;

			CustomMutator Claustropolis = RogueLibs.CreateCustomMutator(cChallenge.Claustropolis, true,
				new CustomNameInfo("MapSize: Claustrophobia"),
				new CustomNameInfo("Damn, this city is cramped! Who's Claus, anyway?"));
			Claustropolis.Available = true;
			Claustropolis.Conflicting.AddRange(cChallenge.MapSize);
			Claustropolis.IsActive = false;

			CustomMutator Megalopolis = RogueLibs.CreateCustomMutator(cChallenge.Megalopolis, true,
				new CustomNameInfo("MapSize: Megalopolis"),
				new CustomNameInfo("Damn, this town has gotten big. You remember when it was just a small Mega-Arcology. Now it's a Mega-Mega-Arcology."));
			Megalopolis.Available = true;
			Megalopolis.Conflicting.AddRange(cChallenge.MapSize);
			Megalopolis.IsActive = false;

			CustomMutator Ultrapolis = RogueLibs.CreateCustomMutator(cChallenge.Ultrapolis, true,
				new CustomNameInfo("MapSize: Ultrapolis"),
				new CustomNameInfo("You get vertigo when you look up. This city is MASSIVE."));
			Ultrapolis.Available = true;
			Ultrapolis.Conflicting.AddRange(cChallenge.MapSize);
			Ultrapolis.IsActive = false;

			#endregion
			#region Population

			CustomMutator GhostTown = RogueLibs.CreateCustomMutator(cChallenge.GhostTown, true,
				new CustomNameInfo("Population: Ghost Town"),
				new CustomNameInfo("No one walks the streets in this city. Don't act all innocent, I know what you do to people in this game!"));
			GhostTown.Available = true;
			GhostTown.Conflicting.AddRange(cChallenge.Population);
			GhostTown.IsActive = false;

			CustomMutator LetMeSeeThatThrong = RogueLibs.CreateCustomMutator(cChallenge.LetMeSeeThatThrong, true,
				new CustomNameInfo("Population: Let Me See That Throng"),
				new CustomNameInfo("The City's HR department is still working out an effective Eugenics program. For now, people are breeding out of control."));
			LetMeSeeThatThrong.Available = true;
			LetMeSeeThatThrong.Conflicting.AddRange(cChallenge.Population);
			LetMeSeeThatThrong.IsActive = false;

			CustomMutator SwarmWelcome = RogueLibs.CreateCustomMutator(cChallenge.SwarmWelcome, true,
				new CustomNameInfo("Population: Swarm Welcome"),
				new CustomNameInfo("This whole city feels like a crowded subway. Pickpocketing is bad enough, but the frottage is out of control!"));
			SwarmWelcome.Available = true;
			SwarmWelcome.Conflicting.AddRange(cChallenge.Population);
			SwarmWelcome.IsActive = false;

			#endregion
			#region Quest Count

			CustomMutator RushinRevolution = RogueLibs.CreateCustomMutator(cChallenge.RushinRevolution, true,
				new CustomNameInfo("QuestCount: Rushin' Revolution"),
				new CustomNameInfo("There are decades where nothing happens; and there are weeks where decades happen. And then there are days where you just don't have time for this shit.\n\nNo quests. Bum rush the Mayor. Take a long weekend."));
			RushinRevolution.Available = true;
			RushinRevolution.Conflicting.AddRange(cChallenge.QuestCount);
			RushinRevolution.IsActive = false;

			CustomMutator SingleMinded = RogueLibs.CreateCustomMutator(cChallenge.SingleMinded, true,
				new CustomNameInfo("QuestCount: Single-minded"),
				new CustomNameInfo("Your Resistance HR profile says \"Not a good multi-tasker.\" They only give you one job per Floor."));
			SingleMinded.Available = true;
			SingleMinded.Conflicting.AddRange(cChallenge.QuestCount);
			SingleMinded.IsActive = false;

			CustomMutator Workhorse = RogueLibs.CreateCustomMutator(cChallenge.Workhorse, true,
				new CustomNameInfo("QuestCount: Workhorse"),
				new CustomNameInfo("You made the mistake of being reliable. Now the Resistance sends you all the work. You're never short on jobs.."));
			Workhorse.Available = true;
			Workhorse.Conflicting.AddRange(cChallenge.QuestCount);
			Workhorse.IsActive = false;

			#endregion
			#region Quest Rewards

			CustomMutator DoublePlyRewards = RogueLibs.CreateCustomMutator(cChallenge.DoublePlyRewards, true,
				new CustomNameInfo("Rewards: Double-Ply Rewards"),
				new CustomNameInfo("The Resistance is running low on... everything. But please, accept this stack of coupons! They are totally really actually valid.\n\nThe smell? That's *value*!"));
			DoublePlyRewards.Available = true;
			DoublePlyRewards.Conflicting.AddRange(new string[] { cChallenge.UnpaidInternship });
			DoublePlyRewards.IsActive = false;

			CustomMutator UnpaidInternship = RogueLibs.CreateCustomMutator(cChallenge.UnpaidInternship, true,
				new CustomNameInfo("Rewards: Unpaid Internship"),
				new CustomNameInfo("The double-experience you're getting working for the Resistance is worth more than any reward, they say. But so far, you're mainly learning one thing: Work for people who pay you. At least they provide lunch, though?"));
			UnpaidInternship.Available = true;
			UnpaidInternship.Conflicting.AddRange(new string[] { cChallenge.DoublePlyRewards });
			UnpaidInternship.IsActive = false;

			#endregion
			#region Roamers

			CustomMutator HoodlumsWonderland = RogueLibs.CreateCustomMutator(cChallenge.HoodlumsWonderland, true,
				new CustomNameInfo("Hoodlum's Wonderland"),
				new CustomNameInfo("The annual charity drive for the Blahds and Crepes happened to overlap this year. They're in tough competition to sell the most cookies! Roaming gang spawns are increased. By a lot."));
			HoodlumsWonderland.Available = true;
			HoodlumsWonderland.Conflicting.AddRange(new string[] { });
			HoodlumsWonderland.IsActive = false;

			CustomMutator MobTown = RogueLibs.CreateCustomMutator(cChallenge.MobTown, true,
				new CustomNameInfo("Mob Town"),
				new CustomNameInfo("Turns out the Union Membership is high here for all the wrong reasons. The Mob is in every district."));
			MobTown.Available = true;
			MobTown.Conflicting.AddRange(new string[] { });
			MobTown.IsActive = false;

			CustomMutator YoungMenIntheNeighborhood = RogueLibs.CreateCustomMutator(cChallenge.YoungMenInTheNeighborhood, true,
				new CustomNameInfo("Young Men in the Neighborhood"),
				new CustomNameInfo("Beause the young gentlemen in the hood are always polite; If you start acting rude, we'll set you right!\nYour friendly local Gangbangers now roam every district."));
			YoungMenIntheNeighborhood.Available = true;
			YoungMenIntheNeighborhood.Conflicting.AddRange(new string[] { });
			YoungMenIntheNeighborhood.IsActive = false;

			#endregion
			#region Walls

			CustomMutator CityOfSteel = RogueLibs.CreateCustomMutator(cChallenge.CityOfSteel, true,
				new CustomNameInfo("Construction: City Of Steel"),
				new CustomNameInfo("A gleaming city of steel! The world of the future, today. Mankind's dream in... Wow, it *really* smells like steel cleaner. Like, it fucking stinks. This is pungent."));
			CityOfSteel.Available = true;
			CityOfSteel.Conflicting.AddRange(cChallenge.WallsAndFloors);
			CityOfSteel.IsActive = false;

			CustomMutator GreenLiving = RogueLibs.CreateCustomMutator(cChallenge.GreenLiving, true,
				new CustomNameInfo("Construction: Green Living"),
				new CustomNameInfo("The Mayor has retrofitted most buildings to eco-friendly plant-based construction. Caterpillars crawl on your face while you sleep, but the air is mighty fresh!"));
			GreenLiving.Available = true;
			GreenLiving.Conflicting.AddRange(cChallenge.WallsAndFloors);
			GreenLiving.IsActive = false;

			CustomMutator Panoptikopolis = RogueLibs.CreateCustomMutator(cChallenge.Panoptikopolis, true,
				new CustomNameInfo("Construction: Panoptikopolis"),
				new CustomNameInfo("Locals just call it The Pan for short. Authoritarian surveillance measures mandate that most buildings have to be built with glass walls. If you have nothing to hide, what are you worried about, citizen?"));
			Panoptikopolis.Available = true;
			Panoptikopolis.Conflicting.AddRange(cChallenge.WallsAndFloors);
			Panoptikopolis.IsActive = false;

			CustomMutator ShantyTown = RogueLibs.CreateCustomMutator(cChallenge.ShantyTown, true,
				new CustomNameInfo("Construction: Shanty Town"),
				new CustomNameInfo("Looks like we're short on Steel.\n\nHard mode for Firefighters, easy mode for arsonists. Fun mode for psychopaths."));
			ShantyTown.Available = true;
			ShantyTown.Conflicting.AddRange(cChallenge.WallsAndFloors);
			ShantyTown.IsActive = false;

			CustomMutator SpelunkyDory = RogueLibs.CreateCustomMutator(cChallenge.SpelunkyDory, true,
				new CustomNameInfo("000: Construction: Spelunky Dory"),
				new CustomNameInfo("You and your fellow citizens live in a disgusting cave complex, but you keep a stiff upper lip about it!"));
			SpelunkyDory.Available = true;
			SpelunkyDory.Conflicting.AddRange(cChallenge.WallsAndFloors);
			SpelunkyDory.IsActive = false;

			#endregion
		}

		#region Custom
		public static string GetActiveChallengeFromList(List<string> challengeList)
		{
			foreach (string mutator in challengeList)
				if (GC.challenges.Contains(mutator))
					return mutator;

			return "";
		}
		public static bool IsChallengeFromListActive(List<string> challengeList)
		{
			foreach (string mutator in challengeList)
				if (GC.challenges.Contains(mutator))
					return true;

			return false;
		}
		#endregion
	}
}
