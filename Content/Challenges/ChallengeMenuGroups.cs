using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RogueLibsCore;

namespace BunnyMod.Content.Challenges
{
	public static class ChallengeMenuGroups
	{
		[RLSetup]
		public static void Start()
		{
			// TODO: Everything in here
			// This makes Mutator Group entries that can be expanded/collapsed. All such groupings should probably be in this class.

			#region Buildings
			CustomMutator Buildings_Hide = RogueLibs.CreateCustomMutator(cChallenge.Buildings_Hide, true,
				new CustomNameInfo("Buildings [-]"),
				new CustomNameInfo("Click to hide group"));
			CustomMutator Buildings_Show = RogueLibs.CreateCustomMutator(cChallenge.Buildings_Show, true,
				new CustomNameInfo("Buildings [+]"),
				new CustomNameInfo("Click to show group"));

			CustomMutator[] buildingMutators = new CustomMutator[] { CityOfSteel, GreenLiving, Panoptikopolis, ShantyTown, SpelunkyDory };
			Buildings_Hide.Available = false;
			Buildings_Hide.ScrollingMenu_PushedButton = (menu, button) =>
			{
				GC.audioHandler.Play(GC.playerAgent, vAudioClip.ClickButton);

				Buildings_Hide.Available = false;
				Buildings_Show.Available = true;

				foreach (CustomMutator m in buildingMutators)
					m.Available = false;

				GC.mainGUI.scrollingMenuScript.OpenScrollingMenu();

				return false;
			};
			Buildings_Show.ScrollingMenu_PushedButton = (menu, button) =>
			{
				GC.audioHandler.Play(GC.playerAgent, vAudioClip.ClickButton);

				Buildings_Hide.Available = true;
				Buildings_Show.Available = false;

				foreach (CustomMutator m in buildingMutators)
					m.Available = true;

				GC.mainGUI.scrollingMenuScript.OpenScrollingMenu();

				return false;
			};
			#endregion
			#region Exteriors
			CustomMutator Exteriors_Hide = RogueLibs.CreateCustomMutator(cChallenge.Exteriors_Hide, true,
				new CustomNameInfo("Exteriors [-]"),
				new CustomNameInfo("Click to hide group\n\nYes, there's only one mutator in this category right now. Modding is hard :("));
			CustomMutator Exteriors_Show = RogueLibs.CreateCustomMutator(cChallenge.Exteriors_Show, true,
				new CustomNameInfo("Exteriors [+]"),
				new CustomNameInfo("Click to show group\n\nYes, there's only one mutator in this category right now. Modding is hard :("));

			CustomMutator[] ExteriorsMutators = new CustomMutator[] { ArcologyEcology };
			Exteriors_Hide.Available = false;
			Exteriors_Hide.ScrollingMenu_PushedButton = (menu, button) =>
			{
				GC.audioHandler.Play(GC.playerAgent, vAudioClip.ClickButton);

				Exteriors_Hide.Available = false;
				Exteriors_Show.Available = true;

				foreach (CustomMutator m in ExteriorsMutators)
					m.Available = false;

				GC.mainGUI.scrollingMenuScript.OpenScrollingMenu();

				return false;
			};
			Exteriors_Show.ScrollingMenu_PushedButton = (menu, button) =>
			{
				GC.audioHandler.Play(GC.playerAgent, vAudioClip.ClickButton);

				Exteriors_Hide.Available = true;
				Exteriors_Show.Available = false;

				foreach (CustomMutator m in ExteriorsMutators)
					m.Available = true;

				GC.mainGUI.scrollingMenuScript.OpenScrollingMenu();

				return false;
			};
			#endregion
			#region Fire
			CustomMutator Fire_Hide = RogueLibs.CreateCustomMutator(cChallenge.Fire_Hide, true,
				new CustomNameInfo("Fire [-]"),
				new CustomNameInfo("Click to hide group"));
			CustomMutator Fire_Show = RogueLibs.CreateCustomMutator(cChallenge.Fire_Show, true,
				new CustomNameInfo("Fire [+]"),
				new CustomNameInfo("Click to show group"));

			CustomMutator[] fireMutators = new CustomMutator[] { GasolineHumidity, NapalmSprings, Mildfire };
			Fire_Hide.Available = false;
			Fire_Hide.ScrollingMenu_PushedButton = (menu, button) =>
			{
				GC.audioHandler.Play(GC.playerAgent, vAudioClip.ClickButton);

				Fire_Hide.Available = false;
				Fire_Show.Available = true;

				foreach (CustomMutator m in fireMutators)
					m.Available = false;

				GC.mainGUI.scrollingMenuScript.OpenScrollingMenu();

				return false;
			};
			Fire_Show.ScrollingMenu_PushedButton = (menu, button) =>
			{
				GC.audioHandler.Play(GC.playerAgent, vAudioClip.ClickButton);

				Fire_Hide.Available = true;
				Fire_Show.Available = false;

				foreach (CustomMutator m in fireMutators)
					m.Available = true;

				GC.mainGUI.scrollingMenuScript.OpenScrollingMenu();

				return false;
			};
			#endregion
			#region Features
			CustomMutator Features_Hide = RogueLibs.CreateCustomMutator(cChallenge.Features_Hide, true,
	new CustomNameInfo("Features [-]"),
	new CustomNameInfo("Click to hide group"));
			CustomMutator Features_Show = RogueLibs.CreateCustomMutator(cChallenge.Features_Show, true,
				new CustomNameInfo("Features [+]"),
				new CustomNameInfo("Click to show group"));

			CustomMutator[] FeaturesMutators = new CustomMutator[]
			{
				BadNeighborhoods, BroughtBackFountain, CartOfTheDeal, FloralerFlora, LakeItOrLeaveIt, LitterallyTheWorst, PowerWhelming, SkywayDistrict,
				SurveillanceSociety, ThePollutionSolution
			};
			Features_Hide.Available = false;
			Features_Hide.ScrollingMenu_PushedButton = (menu, button) =>
			{
				GC.audioHandler.Play(GC.playerAgent, vAudioClip.ClickButton);

				Features_Hide.Available = false;
				Features_Show.Available = true;

				foreach (CustomMutator m in FeaturesMutators)
					m.Available = false;

				GC.mainGUI.scrollingMenuScript.OpenScrollingMenu();

				return false;
			};
			Features_Show.ScrollingMenu_PushedButton = (menu, button) =>
			{
				GC.audioHandler.Play(GC.playerAgent, vAudioClip.ClickButton);

				Features_Hide.Available = true;
				Features_Show.Available = false;

				foreach (CustomMutator m in FeaturesMutators)
					m.Available = true;

				GC.mainGUI.scrollingMenuScript.OpenScrollingMenu();

				return false;
			};
			#endregion
			#region Knockback
			CustomMutator Knockback_Hide = RogueLibs.CreateCustomMutator(cChallenge.Knockback_Hide, true,
	new CustomNameInfo("Knockback [-]"),
	new CustomNameInfo("Click to hide group"));
			CustomMutator Knockback_Show = RogueLibs.CreateCustomMutator(cChallenge.Knockback_Show, true,
				new CustomNameInfo("Knockback [+]"),
				new CustomNameInfo("Click to show group"));

			CustomMutator[] KnockbackMutators = new CustomMutator[] { BoringPhysics, SaveTheWalls, WallWallopWorld };
			Knockback_Hide.Available = false;
			Knockback_Hide.ScrollingMenu_PushedButton = (menu, button) =>
			{
				GC.audioHandler.Play(GC.playerAgent, vAudioClip.ClickButton);

				Knockback_Hide.Available = false;
				Knockback_Show.Available = true;

				foreach (CustomMutator m in KnockbackMutators)
					m.Available = false;

				GC.mainGUI.scrollingMenuScript.OpenScrollingMenu();

				return false;
			};
			Knockback_Show.ScrollingMenu_PushedButton = (menu, button) =>
			{
				GC.audioHandler.Play(GC.playerAgent, vAudioClip.ClickButton);

				Knockback_Hide.Available = true;
				Knockback_Show.Available = false;

				foreach (CustomMutator m in KnockbackMutators)
					m.Available = true;

				GC.mainGUI.scrollingMenuScript.OpenScrollingMenu();

				return false;
			};

			#endregion
			#region Map Size
			CustomMutator MapSize_Hide = RogueLibs.CreateCustomMutator(cChallenge.MapSize_Hide, true,
	new CustomNameInfo("MapSize [-]"),
	new CustomNameInfo("Click to hide group"));
			CustomMutator MapSize_Show = RogueLibs.CreateCustomMutator(cChallenge.MapSize_Show, true,
				new CustomNameInfo("MapSize [+]"),
				new CustomNameInfo("Click to show group"));

			CustomMutator[] MapSizeMutators = new CustomMutator[] { ACityForAnts, Claustropolis, Megalopolis, Ultrapolis };
			MapSize_Hide.Available = false;
			MapSize_Hide.ScrollingMenu_PushedButton = (menu, button) =>
			{
				GC.audioHandler.Play(GC.playerAgent, vAudioClip.ClickButton);

				MapSize_Hide.Available = false;
				MapSize_Show.Available = true;

				foreach (CustomMutator m in MapSizeMutators)
					m.Available = false;

				GC.mainGUI.scrollingMenuScript.OpenScrollingMenu();

				return false;
			};
			MapSize_Show.ScrollingMenu_PushedButton = (menu, button) =>
			{
				GC.audioHandler.Play(GC.playerAgent, vAudioClip.ClickButton);

				MapSize_Hide.Available = true;
				MapSize_Show.Available = false;

				foreach (CustomMutator m in MapSizeMutators)
					m.Available = true;

				GC.mainGUI.scrollingMenuScript.OpenScrollingMenu();

				return false;
			};
			#endregion
			#region Overhauls

			#endregion
			#region Population
			CustomMutator Population_Hide = RogueLibs.CreateCustomMutator(cChallenge.Population_Hide, true,
	new CustomNameInfo("Population [-]"),
	new CustomNameInfo("Click to hide group"));
			CustomMutator Population_Show = RogueLibs.CreateCustomMutator(cChallenge.Population_Show, true,
				new CustomNameInfo("Population [+]"),
				new CustomNameInfo("Click to show group"));

			CustomMutator[] PopulationMutators = new CustomMutator[] { GhostTown, HordeAlmighty, LetMeSeeThatThrong, SwarmWelcome };
			Population_Hide.Available = false;
			Population_Hide.ScrollingMenu_PushedButton = (menu, button) =>
			{
				GC.audioHandler.Play(GC.playerAgent, vAudioClip.ClickButton);

				Population_Hide.Available = false;
				Population_Show.Available = true;

				foreach (CustomMutator m in PopulationMutators)
					m.Available = false;

				GC.mainGUI.scrollingMenuScript.OpenScrollingMenu();

				return false;
			};
			Population_Show.ScrollingMenu_PushedButton = (menu, button) =>
			{
				GC.audioHandler.Play(GC.playerAgent, vAudioClip.ClickButton);

				Population_Hide.Available = true;
				Population_Show.Available = false;

				foreach (CustomMutator m in PopulationMutators)
					m.Available = true;

				GC.mainGUI.scrollingMenuScript.OpenScrollingMenu();

				return false;
			};
			#endregion
			#region Quest Count
			CustomMutator QuestCount_Hide = RogueLibs.CreateCustomMutator(cChallenge.QuestCount_Hide, true,
	new CustomNameInfo("QuestCount [-]"),
	new CustomNameInfo("Click to hide group"));
			CustomMutator QuestCount_Show = RogueLibs.CreateCustomMutator(cChallenge.QuestCount_Show, true,
				new CustomNameInfo("QuestCount [+]"),
				new CustomNameInfo("Click to show group"));

			CustomMutator[] QuestCountMutators = new CustomMutator[] { RushinRevolution, SingleMinded, Workhorse };
			QuestCount_Hide.Available = false;
			QuestCount_Hide.ScrollingMenu_PushedButton = (menu, button) =>
			{
				GC.audioHandler.Play(GC.playerAgent, vAudioClip.ClickButton);

				QuestCount_Hide.Available = false;
				QuestCount_Show.Available = true;

				foreach (CustomMutator m in QuestCountMutators)
					m.Available = false;

				GC.mainGUI.scrollingMenuScript.OpenScrollingMenu();

				return false;
			};
			QuestCount_Show.ScrollingMenu_PushedButton = (menu, button) =>
			{
				GC.audioHandler.Play(GC.playerAgent, vAudioClip.ClickButton);

				QuestCount_Hide.Available = true;
				QuestCount_Show.Available = false;

				foreach (CustomMutator m in QuestCountMutators)
					m.Available = true;

				GC.mainGUI.scrollingMenuScript.OpenScrollingMenu();

				return false;
			};
			#endregion
			#region Quest Rewards
			CustomMutator QuestRewards_Hide = RogueLibs.CreateCustomMutator(cChallenge.QuestRewards_Hide, true,
	new CustomNameInfo("QuestRewards [-]"),
	new CustomNameInfo("Click to hide group"));
			CustomMutator QuestRewards_Show = RogueLibs.CreateCustomMutator(cChallenge.QuestRewards_Show, true,
				new CustomNameInfo("QuestRewards [+]"),
				new CustomNameInfo("Click to show group"));

			CustomMutator[] QuestRewardsMutators = new CustomMutator[] { DoublePlyRewards, UnpaidInternship };
			QuestRewards_Hide.Available = false;
			QuestRewards_Hide.ScrollingMenu_PushedButton = (menu, button) =>
			{
				GC.audioHandler.Play(GC.playerAgent, vAudioClip.ClickButton);

				QuestRewards_Hide.Available = false;
				QuestRewards_Show.Available = true;

				foreach (CustomMutator m in QuestRewardsMutators)
					m.Available = false;

				GC.mainGUI.scrollingMenuScript.OpenScrollingMenu();

				return false;
			};
			QuestRewards_Show.ScrollingMenu_PushedButton = (menu, button) =>
			{
				GC.audioHandler.Play(GC.playerAgent, vAudioClip.ClickButton);

				QuestRewards_Hide.Available = true;
				QuestRewards_Show.Available = false;

				foreach (CustomMutator m in QuestRewardsMutators)
					m.Available = true;

				GC.mainGUI.scrollingMenuScript.OpenScrollingMenu();

				return false;
			};
			#endregion
			#region Roamers
			CustomMutator Roamers_Hide = RogueLibs.CreateCustomMutator(cChallenge.Roamers_Hide, true,
	new CustomNameInfo("Roamers [-]"),
	new CustomNameInfo("Click to hide group"));
			CustomMutator Roamers_Show = RogueLibs.CreateCustomMutator(cChallenge.Roamers_Show, true,
				new CustomNameInfo("Roamers [+]"),
				new CustomNameInfo("Click to show group"));

			CustomMutator[] RoamersMutators = new CustomMutator[] { HoodlumsWonderland, MobTown, YoungMenIntheNeighborhood };
			Roamers_Hide.Available = false;
			Roamers_Hide.ScrollingMenu_PushedButton = (menu, button) =>
			{
				GC.audioHandler.Play(GC.playerAgent, vAudioClip.ClickButton);

				Roamers_Hide.Available = false;
				Roamers_Show.Available = true;

				foreach (CustomMutator m in RoamersMutators)
					m.Available = false;

				GC.mainGUI.scrollingMenuScript.OpenScrollingMenu();

				return false;
			};
			Roamers_Show.ScrollingMenu_PushedButton = (menu, button) =>
			{
				GC.audioHandler.Play(GC.playerAgent, vAudioClip.ClickButton);

				Roamers_Hide.Available = true;
				Roamers_Show.Available = false;

				foreach (CustomMutator m in RoamersMutators)
					m.Available = true;

				GC.mainGUI.scrollingMenuScript.OpenScrollingMenu();

				return false;
			};
			#endregion
		}
	}
}
