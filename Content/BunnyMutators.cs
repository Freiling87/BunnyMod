using RogueLibsCore;
using System;
using System.Collections.Generic;
using UnityEngine;
using BunnyMod;

namespace BunnyMod.Content
{
	public class BunnyMutators
	{
		public void Awake()
		{
			InitializeMutators();

			// Bullet
			BunnyHeader.MainInstance.PatchPostfix(typeof(Bullet), "SetupBullet", GetType(), "Bullet_SetupBullet", new Type[0] { });

			// Quests
			BunnyHeader.MainInstance.PatchPostfix(typeof(Quests), "CheckIfBigQuestObject", GetType(), "Quests_CheckIfBigQuestObject", new Type[1] { typeof(PlayfieldObject) });

			// RandomWalls
			BunnyHeader.MainInstance.PatchPrefix(typeof(RandomWalls), "fillWalls", GetType(), "RandomWalls_fillWalls", new Type[0] { });

			// SpawnerMain
			BunnyHeader.MainInstance.PatchPrefix(typeof(SpawnerMain), "SpawnBullet", GetType(), "SpawnerMain_SpawnBullet", new Type[4] { typeof(Vector3), typeof(bulletStatus), typeof(PlayfieldObject), typeof(int) });
		}
		public static void InitializeMutators()
		{
			#region City Life
			CustomMutator alwaysSpawnArsonists = RogueLibs.CreateCustomMutator("AlwaysSpawnArsonists", true,
				new CustomNameInfo("Always Spawn Arsonists"),
				new CustomNameInfo("You're not the only one in town, you know."));
			alwaysSpawnArsonists.Available = true;
			alwaysSpawnArsonists.IsActive = false;

			//CustomMutator badAirQuality = RogueLibs.CreateCustomMutator("BadAirQuality", true,
			//	new CustomNameInfo("Bad Air Quality"),
			//	new CustomNameInfo("No one's out and about."));
			//badAirQuality.Available = true;
			//badAirQuality.IsActive = true;

			//CustomMutator gangLand = RogueLibs.CreateCustomMutator("GangLand", true,
			//	new CustomNameInfo("Gang Land"),
			//	new CustomNameInfo(""));
			//gangLand.Available = true;
			//gangLand.IsActive = true;
			// Distinct from the Gang War disaster. These are just roamers who will fight on sight, not like WarZone.

			//CustomMutator heyYou = RogueLibs.CreateCustomMutator("HeyYou", true,
			//	new CustomNameInfo("Hey, You!"),
			//	new CustomNameInfo("NPCs will occasionally approach you: Slum Dwellers begging, Gangbangers mugging, Cops asking to frisk you, Scientists offering paid medical experiments, Jocks doing pranks. Maybe more!"));
			//heyYou.Available = true;
			//heyYou.IsActive = true;

			//CustomMutator letMeSeeThatThrong = RogueLibs.CreateCustomMutator("LetMeSeeThatThrong", true,
			//	new CustomNameInfo("Let Me See That Throng"),
			//	new CustomNameInfo("Don't get too excited. Throng as in crowd. There are about 4x as many roaming NPCs."));
			//letMeSeeThatThrong.Available = true;
			//letMeSeeThatThrong.IsActive = true;

			//CustomMutator returnToShrunke = RogueLibs.CreateCustomMutator("ReturnToShrunke", true,
			//	new CustomNameInfo("Return to Shrunke"),
			//	new CustomNameInfo("Everyone's shrunken."));
			//returnToShrunke.Available = true;
			//returnToShrunke.IsActive = true;
			#endregion

			#region Interface
			#endregion

			#region Map Modification
			CustomMutator shantyTown = RogueLibs.CreateCustomMutator("ShantyTown", true,
				new CustomNameInfo("Shanty Town"),
				new CustomNameInfo("Hard mode for Firefighters, easy mode for arsonists. Fun mode for psychopaths."));
			shantyTown.Available = true;
			shantyTown.Conflicting.AddRange(new string[] { "SteelCity" });
			shantyTown.IsActive = true; // TEST

			CustomMutator cityOfSteel = RogueLibs.CreateCustomMutator("CityOfSteel", true,
				new CustomNameInfo("City Of Steel"),
				new CustomNameInfo("A gleaming city of steel! The world of the future, today. Mankind's dream in... Wow, it *really* smells like steel cleaner. Like, it fucking stinks. This is pungent."));
			cityOfSteel.Available = true;
			cityOfSteel.Conflicting.AddRange(new string[] { "Shantytown" });
			cityOfSteel.IsActive = false;
			#endregion

			#region Overhauls
			//CustomMutator cyberWarfare = RogueLibs.CreateCustomMutator("CyberWarfare", true,
			//	new CustomNameInfo("Cyber Warfare"),
			//	new CustomNameInfo("Hacking is more complex, and carries greater risks & rewards. Hack the planet!"));
			//cyberWarfare.Available = true;
			//cyberWarfare.IsActive = true;

			//CustomMutator physicalSecurity = RogueLibs.CreateCustomMutator("physicalSecurity", true,
			//	new CustomNameInfo("Physical Security"),
			//	new CustomNameInfo("Doors, Windows and Safes might be hooked up to alarms. You'll need a new set of skills to deal with those."));
			//physicalSecurity.Available = true;
			//physicalSecurity.IsActive = true;

			CustomMutator scaryGuns = RogueLibs.CreateCustomMutator("ScaryGuns", true,
				new CustomNameInfo("Scary Guns"),
				new CustomNameInfo("Guns are deadlier, but adds Cover mechanics. Get good, or get dead."));
			scaryGuns.Available = true;
			scaryGuns.IsActive = true;

			//CustomMutator wagTheDog = RogueLibs.CreateCustomMutator("WagTheDog", true,
			//	new CustomNameInfo("Wag The Dog"),
			//	new CustomNameInfo("Electability now relies on a few new factors, and isn't so limited to particular playstyles."));
			//wagTheDog.Available = true;
			//wagTheDog.IsActive = true;
			#endregion

		}

		#region Bullet
		public static void Bullet_SetupBullet(Bullet __instance) // Postfix
		{
			if (BunnyHeader.gc.challenges.Contains("ScaryGuns"))
			{
				__instance.damage = Mathf.Max(1, (int)(__instance.damage * UnityEngine.Random.Range(0.25f, 5f)));
				__instance.speed = Mathf.Min(65, __instance.speed * 3);

				BunnyHeader.Log("               New: damage = " + __instance.damage + "; speed = " + __instance.speed);
			}
		}
		#endregion
		#region Quests
		public static void Quests_CheckIfBigQuestObject(PlayfieldObject myObject, Quests __instance) // Postfix
		{
			if (BunnyHeader.gc.levelFeeling == "Riot" || BunnyHeader.gc.percentChance(2) || BunnyHeader.gc.challenges.Contains("AlwaysSpawnArsonists"))
			{
				for (int i = 0; i < BunnyHeader.gc.playerAgentList.Count; i++)
				{
					Agent agent_i = BunnyHeader.gc.playerAgentList[i];

					if ((agent_i.localPlayer || BunnyHeader.gc.serverPlayer) && __instance.CanHaveBigQuest(agent_i))
					{
						if (BunnyHeader.gc.serverPlayer && !BunnyHeader.gc.loadLevel.setArsonist && !BunnyHeader.gc.loadLevel.LevelContainsMayor())
						{
							for (int j = 0; j < BunnyHeader.gc.agentList.Count; j++)
							{
								Agent agent_j = BunnyHeader.gc.agentList[j];

								if (agent_j.isPlayer == 0 && !agent_j.dead && !agent_j.objectAgent && !agent_j.oma.rioter && !agent_j.ghost && !agent_j.inhuman && !agent_j.beast && !agent_j.zombified && !agent_j.oma.hidden && !agent_j.arsonist && !agent_j.oma.secretWerewolf && agent_j.ownerID == 0 && agent_j.startingChunk == 0 && !agent_j.enforcer && !agent_j.upperCrusty && agent_j.agentName != "Assassin" && agent_j.agentName != "Custom")
									if (!agent_j.QuestInvolvementFull())
										continue;
									else if (agent_j.arsonist)
									{
										if (!__instance.bigQuestObjectList.Contains(agent_j))
											__instance.bigQuestObjectList.Add(agent_j);
										if (!agent_i.localPlayer)
											agent_j.noBigQuestMarker = true;

										agent_j.isBigQuestObject = true;
										agent_j.bigQuestType = agent_i.bigQuest;
										agent_j.noBigQuestMarker = true;
									}
							}

							if (!BunnyHeader.gc.loadLevel.LevelContainsMayor())
							{
								UnityEngine.Random.InitState(BunnyHeader.gc.loadLevel.randomSeedNum + BunnyHeader.gc.sessionDataBig.curLevelEndless + agent_i.isPlayer);
								agent_i.needArsonist = 1;
								agent_i.arsonistAppearance = (float)UnityEngine.Random.Range(20, 75);
							}

							BunnyHeader.gc.loadLevel.setArsonist = true;
						}

						if (myObject.isAgent && ((Agent)myObject).arsonist)
							return; // true;

						if (agent_i.oma.bigQuestTarget1 == "")
						{
							agent_i.oma.NetworkbigQuestTarget1 = "x";
							return; // false
						}
					}
				}
				return;
			}
		}
		#endregion
		#region RandomWalls
		public static bool RandomWalls_fillWalls() // Replacement
		{
			string wallType;

			if (BunnyHeader.gc.challenges.Contains("ShantyTown"))
				wallType = "Wood";
			else if (BunnyHeader.gc.challenges.Contains("CityOfSteel"))
				wallType = "Steel";
			else
				return true;

			RandomSelection component = GameObject.Find("ScriptObject").GetComponent<RandomSelection>();

			RandomList rList;

			rList = component.CreateRandomList("WallsNormal", "Walls", "Wall");
			component.CreateRandomElement(rList, wallType, 3);

			rList = component.CreateRandomList("WallsWeak", "Walls", "Wall");
			component.CreateRandomElement(rList, wallType, 3);

			rList = component.CreateRandomList("WallsStrong", "Walls", "Wall");
			component.CreateRandomElement(rList, wallType, 3);

			rList = component.CreateRandomList("WallsHideout", "Walls", "Wall");
			component.CreateRandomElement(rList, wallType, 3);

			return false;
		}
		#endregion
		#region SpawnerMain
		public static bool SpawnerMain_SpawnBullet(Vector3 bulletPos, bulletStatus bulletType, PlayfieldObject myPlayfieldObject, int bulletNetID, SpawnerMain __instance, ref Bullet __result) // Prefix
		{
			if (!BunnyHeader.gc.challenges.Contains("ScaryGuns")
				|| bulletType != bulletStatus.Normal || bulletType != bulletStatus.Shotgun || bulletType != bulletStatus.Revolver)
				return true;

			BunnyHeader.Log("SpawnerMain_SpawnBullet: bulletType = " + bulletType);

			Agent agent = null;
			Item item = null;
			ObjectReal objectReal = null;
			bool isFromAgent = false;
			bool isFromItem = false;
			float bulletScale = 0.333333f;

			if (myPlayfieldObject != null)
			{
				if (myPlayfieldObject.playfieldObjectType == "Agent")
				{
					isFromAgent = true;
					agent = myPlayfieldObject.playfieldObjectAgent;
				}

				if (myPlayfieldObject.playfieldObjectType == "Item")
				{
					isFromItem = true;
					item = myPlayfieldObject.playfieldObjectItem;
				}
				else
					objectReal = myPlayfieldObject.playfieldObjectReal;
			}

			switch (bulletType)
			{
				case bulletStatus.Normal:
					__result = __instance.bulletNormalPrefab_S.Spawn(bulletPos);
					break;
				case bulletStatus.Shotgun:
					__result = __instance.bulletShotgunPrefab_S.Spawn(bulletPos);
					break;
				case bulletStatus.Revolver:
					__result = __instance.bulletRevolverPrefab_S.Spawn(bulletPos);
					break;
				default:
					__result = __instance.bulletPrefab_S.Spawn(bulletPos);
					break;
			}

			__result.streamBullet = false;

			__result.DoEnable();
			__result.bulletType = bulletType;
			__result.spr = __result.bulletSprite;
			__result.bulletNetID = bulletNetID;

			if (isFromAgent)
			{
				__result.agent = agent;
				__result.cameFromCollider = agent.agentColliderNormal;

				__result.cameFromWeapon = agent.inventory.equippedWeapon.invItemName;
			}
			else if (isFromItem)
			{
				__result.item = item;
				__result.explosionOwner = item.owner;
				__result.cameFromCollider = item.itemHitboxCollider;
				__result.cameFromWeapon = myPlayfieldObject.objectName;
			}
			else
			{
				__result.objectReal = objectReal;
				__result.cameFromCollider = objectReal.objectHitbox;
				__result.cameFromWeapon = myPlayfieldObject.objectName;

				if (objectReal.hasObjectAgent)
					__result.agent = objectReal.objectAgent;
			}

			__result.SetupBullet();

			GameObject gameObject9 = null;

			try
			{
				gameObject9 = __result.tr.Find("P_BulletTrail").gameObject;
				gameObject9.transform.localPosition = new Vector3(0f, -0.32f, 0f);
				gameObject9.transform.localScale = new Vector3(bulletScale, bulletScale, bulletScale);
			}
			catch
			{
				try
				{
					if (__result.particles == null)
					{
						string str = "BulletTrail";
						gameObject9 = __result.tr.Find("P_" + str).gameObject;
					}
					else
						gameObject9 = __result.particles;

					gameObject9.transform.SetParent(__result.tr);
					gameObject9.transform.localPosition = new Vector3(0f, -0.32f, 0f);
					gameObject9.transform.localEulerAngles = Vector3.zero;
					gameObject9.transform.localScale = new Vector3(bulletScale, bulletScale, bulletScale);
				}
				catch
				{
					string effectType = "BulletTrail";
					gameObject9 = __instance.SpawnParticleEffect(effectType, bulletPos, __result.tr.eulerAngles.z);
					gameObject9.transform.SetParent(__result.tr);
					gameObject9.transform.localPosition = new Vector3(0f, -0.32f, 0f);
					gameObject9.transform.localEulerAngles = Vector3.zero;
					gameObject9.transform.localScale = new Vector3(bulletScale, bulletScale, bulletScale);
				}
			}

			__result.particles = gameObject9;
			ParticleEffect component8 = __result.particles.GetComponent<ParticleEffect>();
			component8.hasAttachedToObject = true;
			component8.attachedToObject = __result;
			component8.attachedToObjectTr = __result.tr;
			component8.ps.Play();
			__result.particleEffectGetsDeattached = true;

			try
			{
				__result.lightTemp.fancyLightRenderer.enabled = true;
				__result.lightTemp.fancyLight.bulletChanges = true;
			}
			catch
			{
				Debug.LogError("Bullet has no LightTemp: " + __result);
			}

			__result.particles = gameObject9;

			MeshRenderer component9 = __result.spr.GetComponent<MeshRenderer>();

			if (__instance.gc.challenges.Contains("RogueVision"))
			{
				if (__result.agent != null)
				{
					if (!__result.agent.rogueVisionInvisible)
					{
						component9.enabled = true;

						if (__result.lightTemp != null)
							__result.lightTemp.tr.Find("LightFancy").GetComponent<MeshRenderer>().enabled = true;

						if (__result.lightReal != null)
							__result.lightReal.gameObject.SetActive(true);

						if (__result.particles != null)
							__result.particles.gameObject.SetActive(true);
					}
					else
					{
						component9.enabled = false;

						if (__result.lightTemp != null)
							__result.lightTemp.tr.Find("LightFancy").GetComponent<MeshRenderer>().enabled = false;

						if (__result.lightReal != null)
							__result.lightReal.gameObject.SetActive(false);

						if (__result.particles != null)
							__result.particles.gameObject.SetActive(false);
					}
				}
			}
			else if (!component9.enabled)
			{
				component9.enabled = true;
				if (__result.lightTemp != null)
					__result.lightTemp.tr.Find("LightFancy").GetComponent<MeshRenderer>().enabled = true;

				if (__result.lightReal != null)
					__result.lightReal.gameObject.SetActive(true);

				if (__result.particles != null)
					__result.particles.gameObject.SetActive(true);
			}
			return false;
		}
		#endregion
	}
}
