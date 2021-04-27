using RogueLibsCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Random = UnityEngine.Random;

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
			AgentInteractions_00();
			Relationships_00();
			StatusEffects_00();
		}

		#region Custom
		public static int ToolCost(Agent agent, int baseCost)
		{
			BMLog("ToolCost:");
			BMLog("\tagent = " + agent.name);
			BMLog("\tbaseCost = " + baseCost);

			if (agent.statusEffects.hasTrait(cTrait.TamperTantrum))
				return (baseCost / 2);

			if (agent.statusEffects.hasTrait(cTrait.TamperTantrum_2))
				return 0;

			return baseCost;
		}
		#endregion

		#region Agent
		public void Agent_00()
		{
			Prefix(typeof(Agent), "CanShakeDown", GetType(), "Agent_CanShakeDown", new Type[0] { });
			Postfix(typeof(Agent), "SetupAgentStats", GetType(), "Agent_SetupAgentStats", new Type[1] { typeof(string) });
			Postfix(typeof(Agent), "Start", GetType(), "Agent_Start", new Type[0] { });
		}
		public static bool Agent_CanShakeDown(ref bool __result) // Prefix
		{
			BMLog("Agent_CanShakeDown");

			if (BMTraits.IsPlayerTraitActive(cTrait.Warlord))
			{
				__result = true;
				return false;
			}

			return true;
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
		public static void Agent_Start(Agent __instance) // Postfix
		{
			Agent_Remora remora = new Agent_Remora();
			Agent_Variables[__instance] = remora;
			remora.agentHost = __instance;
		}
		public static Dictionary<Agent, Agent_Remora> Agent_Variables = new Dictionary<Agent, Agent_Remora>();
		#endregion
		#region AgentInteractions
		public void AgentInteractions_00()
		{
			Prefix(typeof(AgentInteractions), "AddButton", GetType(), "AgentInteractions_AddButton", new Type[3] { typeof(string), typeof(int), typeof(string) });
		}
		public static void AgentInteractions_AddButton(string buttonName, int moneyCost, string extraCost, AgentInteractions __instance, ref Agent ___mostRecentInteractingAgent) // Prefix
		{
			if (extraCost.EndsWith("-30"))
				extraCost.Replace("-30", "-" + ToolCost(___mostRecentInteractingAgent, 30));
			else if (extraCost.EndsWith("-20"))
				extraCost.Replace("-20", "-" + ToolCost(___mostRecentInteractingAgent, 20));
		}
		#endregion
		#region Relationships
		public void Relationships_00()
		{
			Prefix(typeof(Relationships), "OwnCheck", GetType(), "Relationships_OwnCheck", new Type[7] { typeof(Agent), typeof(GameObject), typeof(int), typeof(string), typeof(bool), typeof(int), typeof(Fire) });
			Postfix(typeof(Relationships), "SetupRelationshipOriginal", GetType(), "Relationships_SetupRelationshipOriginal", new Type[1] { typeof(Agent) });
		}
		public static bool Relationships_OwnCheck(Agent otherAgent, GameObject affectedGameObject, int tagType, string ownCheckType, bool extraSprite, int strikes, Fire fire) // Prefix
		{
			// TODO: PoliceState sets all Objects NoStrikesIfDestroyed to false
			return true;
		}
		public static void Relationships_SetupRelationshipOriginal(Agent otherAgent, Relationships __instance, ref Agent ___agent) // Postfix
		{
			// This method sets how ___agent feels about otherAgent
			// Don't delete this comment, it gets confusing-er-than-shit

			if (GC.levelType == vLevelType.HomeBase)
			{
				__instance.SetRelInitial(otherAgent, vRelationship.Aligned);
				otherAgent.relationships.SetRelInitial(___agent, vRelationship.Aligned);
			}
			else
			{
				if (otherAgent.agentName == vAgent.ResistanceLeader)
				{
					__instance.SetRelInitial(otherAgent, vRelationship.Aligned);
					otherAgent.relationships.SetRelInitial(___agent, vRelationship.Aligned);
				}

				if (BMTraits.isPlayerInitialRelationshipTraitActive && ___agent.isPlayer != 0)
				{
					BMLog("Relationships_SetupRelationshipOriginal: ");
					BMLog("\tAgent = " + ___agent.name);
					BMLog("\totherAgent = " + otherAgent.name);
					BMLog("\tRelationship = '" + __instance.GetRel(otherAgent) + "'");

					if (__instance.GetRel(otherAgent) == vRelationship.Neutral)
					{
						int roll = Random.Range(0, 100);
						string newRel = vRelationship.Neutral;

						if ((___agent.statusEffects.hasTrait(cTrait.GenerallyUnpleasant) && roll <= 20) ||
							___agent.statusEffects.hasTrait(cTrait.GenerallyUnpleasant_2))
							newRel = vRelationship.Annoyed;
						else if (___agent.statusEffects.hasTrait(cTrait.Polarizing))
						{
							if (roll <= 50)
								newRel = vRelationship.Annoyed;
							else
								newRel = vRelationship.Friendly;
						}
						else if (___agent.statusEffects.hasTrait(cTrait.Polarizing_2))
						{
							if (roll <= 25)
								newRel = vRelationship.Hateful;
							else if (roll <= 50)
								newRel = vRelationship.Annoyed;
							else if (roll <= 67)
								newRel = vRelationship.Friendly;
							else if (roll <= 88)
								newRel = vRelationship.Loyal;
							else if (roll <= 100)
								newRel = vRelationship.Aligned;
						}

						roll = Random.Range(0, 100);

						if (___agent.statusEffects.hasTrait(cTrait.Domineering))
						{
							if (roll <= 5)
								newRel = vRelationship.Submissive;
						}
						else if (___agent.statusEffects.hasTrait(cTrait.Domineering_2))
						{
							if (roll <= 10)
								newRel = vRelationship.Submissive;
						}


						if (newRel != vRelationship.Neutral)
						{
							__instance.SetRelInitial(otherAgent, newRel);
							otherAgent.relationships.SetRelInitial(___agent, newRel);

							if (newRel == vRelationship.Annoyed)
								otherAgent.relationships.SetStrikes(___agent, 2);
						}
					}
				}
			}
		}
		#endregion
		#region StatusEffects
		public void StatusEffects_00()
		{
			Postfix(typeof(StatusEffects), "AddTrait", GetType(), "StatusEffects_AddTrait", new Type[3] { typeof(string), typeof(bool), typeof(bool) });
			Postfix(typeof(StatusEffects), "BecomeHidden", GetType(), "StatusEffects_BecomeHidden", new Type[1] { typeof(ObjectReal) });
			Postfix(typeof(StatusEffects), "BecomeNotHidden", GetType(), "StatusEffects_BecomeNotHidden", new Type[0]);
			Postfix(typeof(StatusEffects), "RemoveTrait", GetType(), "StatusEffects_RemoveTrait", new Type[2] { typeof(string), typeof(bool) });
		}
		public static void StatusEffects_AddTrait(string traitName, bool isStarting, bool justRefresh, StatusEffects __instance) // Postfix
		{
			Agent agent = __instance.agent;

			if (traitName == cTrait.Fatass)
			{
				agent.SetEndurance(agent.enduranceStatMod + 1);
				agent.SetSpeed(agent.speedStatMod - 1);
			}
		}
		public static void StatusEffects_BecomeHidden(ObjectReal hiddenInObject, StatusEffects __instance) // Postfix
		{
			BMLog("StatusEffects_BecomeHidden");
			try { BMLog("\tObjectReal: " + hiddenInObject.name); }
			catch { BMLog("\tObjectReal: null"); }
			BMLog("\tAgent: " + __instance.agent.agentName);

			Agent agent = __instance.agent;

			if (!(hiddenInObject is null))
				if (BMTraits.IsPlayerTraitActive(cTrait.UnderdarkCitizen) && agent.isPlayer == 0 && hiddenInObject.objectName == vObject.Manhole)
					agent.statusEffects.BecomeNotHidden();

			if (hiddenInObject is Bathtub || hiddenInObject is Plant || hiddenInObject is PoolTable || hiddenInObject is TableBig)
				__instance.agent.agentCollider.enabled = false;
		}
		public static void StatusEffects_BecomeNotHidden(StatusEffects __instance)
		{
			__instance.agent.agentCollider.enabled = true;
		}
		public static void StatusEffects_RemoveTrait(string traitName, bool onlyLocal, StatusEffects __instance) // Postfix
		{
			Agent agent = __instance.agent;
			if (traitName == cTrait.Fatass)
			{
				//TODO: CharacterCreation.CreatePointTallyText() for stat mods
				agent.SetEndurance(agent.enduranceStatMod - 1);
				agent.SetSpeed(agent.speedStatMod + 1);
			}
		}
		#endregion
	}
	public class Agent_Remora
	{
		public static GameController GC => GameController.gameController;

		public Agent agentHost;

		public bool hasElevatorPass = false;
	}
}
