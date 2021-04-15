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

			// SpawnerMain
			Prefix(typeof(SpawnerMain), "SpawnBullet", GetType(), "SpawnerMain_SpawnBullet", new Type[4] { typeof(Vector3), typeof(bulletStatus), typeof(PlayfieldObject), typeof(int) });
		}
		public static void InitializeMutators()
		{
			#region Buildings

			CustomMutator CityOfSteel = RogueLibs.CreateCustomMutator(cMutators.CityOfSteel, true,
				new CustomNameInfo("City Of Steel"),
				new CustomNameInfo("A gleaming city of steel! The world of the future, today. Mankind's dream in... Wow, it *really* smells like steel cleaner. Like, it fucking stinks. This is pungent."));
			CityOfSteel.Available = true;
			CityOfSteel.Conflicting.AddRange(new string[] { cMutators.ShantyTown });
			CityOfSteel.IsActive = false;

			CustomMutator ShantyTown = RogueLibs.CreateCustomMutator(cMutators.ShantyTown, true,
				new CustomNameInfo("Shanty Town"),
				new CustomNameInfo("Hard mode for Firefighters, easy mode for arsonists. Fun mode for psychopaths."));
			ShantyTown.Available = true;
			ShantyTown.Conflicting.AddRange(new string[] { cMutators.CityOfSteel });
			ShantyTown.IsActive = false;

			#endregion
			#region Map Size

			CustomMutator ACityForAnts = RogueLibs.CreateCustomMutator(cMutators.ACityForAnts, true,
				new CustomNameInfo("A City for Ants"),
				new CustomNameInfo("Yes, that is indeed what it is, figuratively speaking."));
			ACityForAnts.Available = true;
			ACityForAnts.Conflicting.AddRange(new string[] { cMutators.Claustrophobia, cMutators.Megalopolis, cMutators.Ultrapolis });
			ACityForAnts.IsActive = false;

			CustomMutator Claustrophobia = RogueLibs.CreateCustomMutator(cMutators.Claustrophobia, true,
				new CustomNameInfo(cMutators.Claustrophobia),
				new CustomNameInfo("Damn, this city is cramped! Who's Claus, anyway?"));
			Claustrophobia.Available = true;
			Claustrophobia.Conflicting.AddRange(new string[] { cMutators.ACityForAnts, cMutators.Megalopolis, cMutators.Ultrapolis});
			Claustrophobia.IsActive = false;

			CustomMutator Megalopolis = RogueLibs.CreateCustomMutator(cMutators.Megalopolis, true,
				new CustomNameInfo(cMutators.Megalopolis),
				new CustomNameInfo("Damn, this town has gotten big. You remember when it was just a small Mega-Arcology. Now it's a Mega-Mega-Arcology."));
			Megalopolis.Available = true;
			Megalopolis.Conflicting.AddRange(new string[] { cMutators.ACityForAnts, cMutators.Claustrophobia, cMutators.Ultrapolis });
			Megalopolis.IsActive = false;

			CustomMutator Ultrapolis = RogueLibs.CreateCustomMutator(cMutators.Ultrapolis, true,
				new CustomNameInfo(cMutators.Ultrapolis),
				new CustomNameInfo("You get vertigo when you look up. This city is MASSIVE."));
			Ultrapolis.Available = true;
			Ultrapolis.Conflicting.AddRange(new string[] { cMutators.ACityForAnts, cMutators.Claustrophobia, cMutators.Megalopolis });
			Ultrapolis.IsActive = false;

			#endregion
			#region Mechanics

			CustomMutator ScaryGuns = RogueLibs.CreateCustomMutator(cMutators.ScaryGuns, true,
				new CustomNameInfo("Scary Guns"),
				new CustomNameInfo("Guns are deadlier, but adds Cover mechanics. Get good, or get dead."));
			ScaryGuns.Available = true;
			ScaryGuns.Conflicting.AddRange(new string[] { });
			ScaryGuns.IsActive = false;

			#endregion
			#region Quests

			CustomMutator SingleMinded = RogueLibs.CreateCustomMutator(cMutators.SingleMinded, true,
				new CustomNameInfo("Single-minded"),
				new CustomNameInfo("Your Resistance HR profile says \"Not a good multi-tasker.\" They only give you one job per Floor."));
			SingleMinded.Available = true;
			SingleMinded.Conflicting.AddRange(new string[] { cMutators.Workhorse, cMutators.ZeroQuests });
			SingleMinded.IsActive = false;

			CustomMutator Workhorse = RogueLibs.CreateCustomMutator(cMutators.Workhorse, true,
				new CustomNameInfo("Workhorse"),
				new CustomNameInfo("You made the mistake of being reliable. Now the Resistance sends you all the work. You're never short on jobs.."));
			Workhorse.Available = true;
			Workhorse.Conflicting.AddRange(new string[] { cMutators.SingleMinded, cMutators.ZeroQuests });
			Workhorse.IsActive = false;

			//CustomMutator ZeroQuests = RogueLibs.CreateCustomMutator(cMutators.ZeroQuests, true,
			//	new CustomNameInfo("Zero Quests"),
			//	new CustomNameInfo("The Resistance has sort of given up on asking you to do things. Just... go to the elevator. That's fine."));
			//ZeroQuests.Available = true;
			//ZeroQuests.Conflicting.AddRange(new string[] { cMutators.FourQuests, cMutators.OneQuest });
			//ZeroQuests.IsActive = false;
			// This one needs an additional check. The elevator to exit the level is not activated.

			#endregion
		}

		#region Bullet
		public static void Bullet_SetupBullet(Bullet __instance) // Postfix
		{
			if (GC.challenges.Contains(cMutators.ScaryGuns))
			{
				__instance.damage = Mathf.Max(1, (int)(__instance.damage * UnityEngine.Random.Range(0.25f, 5f)));
				__instance.speed = Mathf.Min(65, __instance.speed * 3);
			}
		}
		#endregion

		#region SpawnerMain
		public static bool SpawnerMain_SpawnBullet(Vector3 bulletPos, bulletStatus bulletType, PlayfieldObject myPlayfieldObject, int bulletNetID, SpawnerMain __instance, ref Bullet __result) // Prefix
		{
			if (!GC.challenges.Contains(cMutators.ScaryGuns)
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
