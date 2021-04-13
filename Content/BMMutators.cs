using RogueLibsCore;
using System;
using System.Collections.Generic;
using UnityEngine;
using BunnyMod;

using Random = UnityEngine.Random;
using Object = UnityEngine.Object;

namespace BunnyMod.Content
{
	public class BMMutators
	{
		public static GameController GC => GameController.gameController;
		public static bool Prefix(Type type, string methodName, Type patchType, string patchMethodName, Type[] types) => BMHeader.MainInstance.PatchPrefix(type, methodName, patchType, patchMethodName, types);
		public static bool Postfix(Type type, string methodName, Type patchType, string patchMethodName, Type[] types) => BMHeader.MainInstance.PatchPostfix(type, methodName, patchType, patchMethodName, types);
		public static void BMLog(string logMessage) => BMHeader.Log(logMessage);

		public void Awake()
		{
			InitializeMutators();

			// Bullet
			Postfix(typeof(Bullet), "SetupBullet", GetType(), "Bullet_SetupBullet", new Type[0] { });

			// LoadLevel

			// RandomWalls
			Prefix(typeof(RandomWalls), "fillWalls", GetType(), "RandomWalls_fillWalls", new Type[0] { });

			// SpawnerMain
			Prefix(typeof(SpawnerMain), "SpawnBullet", GetType(), "SpawnerMain_SpawnBullet", new Type[4] { typeof(Vector3), typeof(bulletStatus), typeof(PlayfieldObject), typeof(int) });
		}
		public static void InitializeMutators()
		{
			#region City Life
			//CustomMutator alwaysSpawnArsonists = RogueLibs.CreateCustomMutator("AlwaysSpawnArsonists", true,
			//	new CustomNameInfo("Always Spawn Arsonists"),
			//	new CustomNameInfo("You're not the only one in town, you know."));
			//alwaysSpawnArsonists.Available = true;
			//alwaysSpawnArsonists.IsActive = false;

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
			#region Map Modification
			CustomMutator cityOfSteel = RogueLibs.CreateCustomMutator("CityOfSteel", true,
				new CustomNameInfo("City Of Steel"),
				new CustomNameInfo("A gleaming city of steel! The world of the future, today. Mankind's dream in... Wow, it *really* smells like steel cleaner. Like, it fucking stinks. This is pungent."));
			cityOfSteel.Available = true;
			cityOfSteel.Conflicting.AddRange(new string[] { "Shantytown" });
			cityOfSteel.IsActive = false;

			CustomMutator citySprawl = RogueLibs.CreateCustomMutator("CitySprawl", true,
				new CustomNameInfo("City Sprawl"),
				new CustomNameInfo("Damn, this town has gotten big. You remember when it was just a small Mega-Arcology. Now it's a Mega-Mega-Arcology."));
			citySprawl.Available = true;
			citySprawl.Conflicting.AddRange(new string[] { "CloseQuarters" });
			citySprawl.IsActive = false;

			CustomMutator closeQuarters = RogueLibs.CreateCustomMutator("CloseQuarters", true,
				new CustomNameInfo("Close Quarters"),
				new CustomNameInfo("This city felt so big when you were a little kid. Now it feels cramped. Guess people weren't meant to live in a bunker!"));
			closeQuarters.Available = true;
			closeQuarters.Conflicting.AddRange(new string[] { "CitySprawl" });
			closeQuarters.IsActive = false;

			CustomMutator shantyTown = RogueLibs.CreateCustomMutator("ShantyTown", true,
				new CustomNameInfo("Shanty Town"),
				new CustomNameInfo("Hard mode for Firefighters, easy mode for arsonists. Fun mode for psychopaths."));
			shantyTown.Available = true;
			shantyTown.Conflicting.AddRange(new string[] { "SteelCity" });
			shantyTown.IsActive = false;
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
			scaryGuns.Conflicting.AddRange(new string[] { });
			scaryGuns.IsActive = false;

			//CustomMutator wagTheDog = RogueLibs.CreateCustomMutator("WagTheDog", true,
			//	new CustomNameInfo("Wag The Dog"),
			//	new CustomNameInfo("Electability now relies on a few new factors, and isn't so limited to particular playstyles."));
			//wagTheDog.Available = true;
			//wagTheDog.IsActive = true;
			#endregion
			#region Miscellaneous
			CustomMutator fourQuests = RogueLibs.CreateCustomMutator("FourQuests", true,
				new CustomNameInfo("Four Quests"),
				new CustomNameInfo("Four quests per level. Boom."));
			fourQuests.Available = true;
			fourQuests.Conflicting.AddRange(new string[] { });
			fourQuests.IsActive = false;
			#endregion
		}



		#region Bullet
		public static void Bullet_SetupBullet(Bullet __instance) // Postfix
		{
			if (GC.challenges.Contains("ScaryGuns"))
			{
				__instance.damage = Mathf.Max(1, (int)(__instance.damage * UnityEngine.Random.Range(0.25f, 5f)));
				__instance.speed = Mathf.Min(65, __instance.speed * 3);
			}
		}
		#endregion
		#region RandomWalls
		public static bool RandomWalls_fillWalls() // Replacement
		{
			string wallType;

			if (GC.challenges.Contains("ShantyTown"))
				wallType = "Wood";
			else if (GC.challenges.Contains("CityOfSteel"))
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
			if (!GC.challenges.Contains("ScaryGuns")
				|| bulletType != bulletStatus.Normal || bulletType != bulletStatus.Shotgun || bulletType != bulletStatus.Revolver)
				return true;

			BMLog("SpawnerMain_SpawnBullet: bulletType = " + bulletType);

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

			if (GC.challenges.Contains("RogueVision"))
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
