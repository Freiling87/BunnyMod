using RogueLibsCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace BunnyMod.Content
{
	class BMAgents
	{
		public static GameController GC => GameController.gameController;
		public static bool Prefix(Type type, string methodName, Type patchType, string patchMethodName, Type[] types) => BMHeader.MainInstance.PatchPrefix(type, methodName, patchType, patchMethodName, types);
		public static bool Postfix(Type type, string methodName, Type patchType, string patchMethodName, Type[] types) => BMHeader.MainInstance.PatchPostfix(type, methodName, patchType, patchMethodName, types);
		public static void BMLog(string logMessage) => BMHeader.Log(logMessage);

		public void Awake()
		{
			// Agent
			Postfix(typeof(Agent), "SetupAgentStats", GetType(), "Agent_SetupAgentStats", new Type[1] { typeof(string) });

			// RandomAgentWeapons
			Postfix(typeof(RandomAgentWeapons), "fillAgentWeapons", GetType(), "RandomAgentWeapons_fillAgentWeapons", new Type[0] { });
		}

		#region Agent
		public static void Agent_SetupAgentStats(string transformationType, Agent __instance) // Postfix
		{
			if (__instance.agentName == "ResistanceAgent")
			{
				__instance.SetStrength(2);
				__instance.SetEndurance(1);
				__instance.SetAccuracy(3);
				__instance.SetSpeed(3);

				__instance.modMeleeSkill = 2;
				__instance.modGunSkill = 2;
				__instance.modToughness = 2;
				__instance.modVigilant = 1;

				__instance.statusEffects.AddTrait("RegenerateHealthWhenLow");

				__instance.AddJob("Recruit", 5);
				__instance.AddDesire("Supplies");
				__instance.AddDesire("Technology");
				__instance.AddDesire("Weapons");

				if (BMTraits.IsTraitActive("Reinforcements"))
				{
					__instance.inventory.startingHeadPiece = "HardHat";
					__instance.inventory.AddItemPlayerStart("Pistol", 0);
					__instance.inventory.AddItemPlayerStart("Knife", 100);
				}
				if (BMTraits.IsTraitActive("Reinforcements_2"))
				{
					__instance.inventory.startingHeadPiece = "SoldierHelmet";
					__instance.inventory.AddItemPlayerStart("MachineGun", 0);
					__instance.inventory.AddItemPlayerStart("Grenade", 5);
					__instance.inventory.AddItemPlayerStart("Knife", 100);
				}

				__instance.agentHitboxScript.legsColor = new Color32(66, 101, 61, byte.MaxValue);

				__instance.agentCategories.Clear();
				__instance.agentCategories.Add("Guns");
				__instance.agentCategories.Add("Melee");
				__instance.agentCategories.Add("Defense");
				__instance.setInitialCategories = true;
			}
		}
		#endregion
		#region RandomAgentWeapons
		public static void RandomAgentWeapons_fillAgentWeapons() // Postfix
		{
			int reinforcements = 0;

			if (BMTraits.IsTraitActive("Reinforcements"))
				reinforcements = 4;
			else if (BMTraits.IsTraitActive("Reinforcements_2"))
				reinforcements = 8;

			if (reinforcements == 0)
				return;

			Dictionary<RandomElement, int> elementList = new Dictionary<RandomElement, int>()
			{
				{new RandomElement() { rName = "Empty",         rChance = 8 - reinforcements},      8  - reinforcements},
				{new RandomElement() { rName = "Grenade",       rChance = reinforcements / 2 },     reinforcements / 2},
				{new RandomElement() { rName = "MachineGun",    rChance = reinforcements},          reinforcements},
				{new RandomElement() { rName = "Pistol",        rChance = 12 - reinforcements },    12 - reinforcements},
				{new RandomElement() { rName = "Revolver",      rChance = reinforcements / 2 },     reinforcements / 2},
			};

			for (int tier = 1; tier < 5; tier++)
				foreach (KeyValuePair<RandomElement, int> tuple in elementList)
					GC.rnd.randomListTableStatic["ResistanceLeaderWeapon" + tier].elementList.Add(tuple.Key);
		}
		#endregion
	}
}
