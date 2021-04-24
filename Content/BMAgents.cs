using RogueLibsCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace BunnyMod.Content
{
	public class BMAgents
	{
		public static GameController GC => GameController.gameController;
		public static bool Prefix(Type type, string methodName, Type patchType, string patchMethodName, Type[] types) => BMHeader.MainInstance.PatchPrefix(type, methodName, patchType, patchMethodName, types);
		public static bool Postfix(Type type, string methodName, Type patchType, string patchMethodName, Type[] types) => BMHeader.MainInstance.PatchPostfix(type, methodName, patchType, patchMethodName, types);
		public static void BMLog(string logMessage) => BMHeader.Log(logMessage);

		public void Awake()
		{
			Agent_00();
			Relationships_00();
			StatusEffects_00();
		}

		#region Agent
		public void Agent_00()
		{
			Postfix(typeof(Agent), "SetupAgentStats", GetType(), "Agent_SetupAgentStats", new Type[1] { typeof(string) });
		}
		public static void Agent_SetupAgentStats(string transformationType, Agent __instance) // Postfix
		{
			if (__instance.agentName == vAgent.ResistanceLeader)
			{
				if (BMTraits.IsPlayerTraitActive(cTrait.Reinforcements))
				{
					__instance.SetStrength(3);
					__instance.SetEndurance(2);
					__instance.inventory.startingHeadPiece = vArmorHead.HardHat;
				}
				else if (BMTraits.IsPlayerTraitActive(cTrait.Reinforcements_2))
				{
					__instance.SetStrength(3);
					__instance.SetEndurance(3);
					__instance.inventory.startingHeadPiece = vArmorHead.SoldierHelmet;
				}
			}
			else if (__instance.agentName == cAgent.ResistanceSoldier)
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

				__instance.AddJob("Recruit", 0);
				__instance.AddDesire("Supplies");
				__instance.AddDesire("Technology");
				__instance.AddDesire("Weapons");

				if (BMTraits.IsPlayerTraitActive(cTrait.Reinforcements))
				{
					__instance.inventory.startingHeadPiece = "HardHat";
					__instance.inventory.AddItemPlayerStart("Pistol", 0);
					__instance.inventory.AddItemPlayerStart("Knife", 100);
				}
				if (BMTraits.IsPlayerTraitActive(cTrait.Reinforcements_2))
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
		#region Relationships
		public void Relationships_00()
		{
			Prefix(typeof(Relationships), "OwnCheck", GetType(), "Relationships_OwnCheck", new Type[7] { typeof(Agent), typeof(GameObject), typeof(int), typeof(string), typeof(bool), typeof(int), typeof(Fire) });
		}
		public static bool Relationships_OwnCheck(Agent otherAgent, GameObject affectedGameObject, int tagType, string ownCheckType, bool extraSprite, int strikes, Fire fire) // Prefix
		{
			// TODO: PoliceState sets all Objects NoStrikesIfDestroyed to false
			return true;
		}
		#endregion
		#region StatusEffects
		public void StatusEffects_00()
		{
			Postfix(typeof(StatusEffects), "BecomeHidden", GetType(), "StatusEffects_BecomeHidden", new Type[1] { typeof(ObjectReal) });
			Postfix(typeof(StatusEffects), "BecomeNotHidden", GetType(), "StatusEffects_BecomeNotHidden", new Type[0]);
		}
		public static void StatusEffects_BecomeHidden(ObjectReal hiddenInObject, StatusEffects __instance)
		{
			if (hiddenInObject is Bathtub || hiddenInObject is Plant || hiddenInObject is PoolTable || hiddenInObject is TableBig)
				__instance.agent.agentCollider.enabled = false;
			return;
		}
		public static void StatusEffects_BecomeNotHidden(StatusEffects __instance)
		{
			__instance.agent.agentCollider.enabled = true;
		}
		#endregion
	}
}
