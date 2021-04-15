﻿using RogueLibsCore;
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
			#region Construction

			CustomMutator CityOfSteel = RogueLibs.CreateCustomMutator(cChallenge.CityOfSteel, true,
				new CustomNameInfo("Construction: City Of Steel"),
				new CustomNameInfo("A gleaming city of steel! The world of the future, today. Mankind's dream in... Wow, it *really* smells like steel cleaner. Like, it fucking stinks. This is pungent."));
			CityOfSteel.Available = true;
			CityOfSteel.Conflicting.AddRange(new string[] { cChallenge.GreenLiving, cChallenge.Panoptikopolis, cChallenge.ShantyTown, cChallenge.SpelunkyDory });
			CityOfSteel.IsActive = false;

			CustomMutator GreenLiving = RogueLibs.CreateCustomMutator(cChallenge.GreenLiving, true,
				new CustomNameInfo("Construction: Green Living"),
				new CustomNameInfo("The Mayor has retrofitted most buildings to eco-friendly plant-based construction. Caterpillars crawl on your face while you sleep, but the air is mighty fresh!"));
			GreenLiving.Available = true;
			GreenLiving.Conflicting.AddRange(new string[] { cChallenge.CityOfSteel, cChallenge.Panoptikopolis, cChallenge.ShantyTown, cChallenge.SpelunkyDory });
			GreenLiving.IsActive = false;

			CustomMutator Panoptikopolis = RogueLibs.CreateCustomMutator(cChallenge.Panoptikopolis, true,
				new CustomNameInfo("Construction: Panoptikopolis"),
				new CustomNameInfo("Locals just call it The Pan for short. Authoritarian surveillance measures mandate that most buildings have to be built with glass walls. If you have nothing to hide, what are you worried about, citizen?"));
			Panoptikopolis.Available = true;
			Panoptikopolis.Conflicting.AddRange(new string[] { cChallenge.GreenLiving, cChallenge.CityOfSteel, cChallenge.ShantyTown, cChallenge.SpelunkyDory });

			CustomMutator ShantyTown = RogueLibs.CreateCustomMutator(cChallenge.ShantyTown, true,
				new CustomNameInfo("Construction: Shanty Town"),
				new CustomNameInfo("Hard mode for Firefighters, easy mode for arsonists. Fun mode for psychopaths."));
			ShantyTown.Available = true;
			ShantyTown.Conflicting.AddRange(new string[] { cChallenge.GreenLiving, cChallenge.CityOfSteel, cChallenge.Panoptikopolis, cChallenge.SpelunkyDory });
			ShantyTown.IsActive = false;

			CustomMutator SpelunkyDory = RogueLibs.CreateCustomMutator(cChallenge.SpelunkyDory, true,
				new CustomNameInfo("Construction: Spelunky Dory"),
				new CustomNameInfo("You and your fellow citizens live in a disgusting cave complex, but you keep a stiff upper lip about it!"));
			SpelunkyDory.Available = true;
			SpelunkyDory.Conflicting.AddRange(new string[] { cChallenge.GreenLiving, cChallenge.CityOfSteel, cChallenge.Panoptikopolis, cChallenge.ShantyTown });
			SpelunkyDory.IsActive = false;

			#endregion
			#region Floors & Features

			CustomMutator ArcologyEcology = RogueLibs.CreateCustomMutator(cChallenge.ArcologyEcology, true,
				new CustomNameInfo("FloorTiles: Arcology Ecology"),
				new CustomNameInfo("Sustainable Eco-homes! Trees! Less pollution! What's not to love?\n\n(Answer: It's still miserable.)"));
			ArcologyEcology.Available = true;
			ArcologyEcology.Conflicting.AddRange(new string[] { cChallenge.SunkenCity, cChallenge.TransitExperiment });
			ArcologyEcology.IsActive = false;

			CustomMutator SunkenCity = RogueLibs.CreateCustomMutator(cChallenge.SunkenCity, true,
				new CustomNameInfo("FloorTiles: Sunken City"),
				new CustomNameInfo("More like \"Stinkin' Shitty!\" No, but seriously, that's all sewage."));
			SunkenCity.Available = true;
			SunkenCity.Conflicting.AddRange(new string[] { cChallenge.ArcologyEcology, cChallenge.TransitExperiment });
			SunkenCity.IsActive = false;

			CustomMutator TransitExperiment = RogueLibs.CreateCustomMutator(cChallenge.TransitExperiment, true,
				new CustomNameInfo("FloorTiles: Transit Experiment"),
				new CustomNameInfo("The City's authorities considered making all the public streets into conveyor belts, but that was too hard to mod into the game... er, I mean construct. Yeah."));
			TransitExperiment.Available = true;
			TransitExperiment.Conflicting.AddRange(new string[] { cChallenge.ArcologyEcology, cChallenge.SunkenCity });
			TransitExperiment.IsActive = false;

			#endregion
			#region Map Size

			CustomMutator ACityForAnts = RogueLibs.CreateCustomMutator(cChallenge.ACityForAnts, true,
				new CustomNameInfo("MapSize: A City for Ants?!"),
				new CustomNameInfo("Yes, that is indeed what it is, figuratively speaking."));
			ACityForAnts.Available = true;
			ACityForAnts.Conflicting.AddRange(new string[] { cChallenge.Claustrophobia, cChallenge.Megalopolis, cChallenge.Ultrapolis });
			ACityForAnts.IsActive = false;

			CustomMutator Claustrophobia = RogueLibs.CreateCustomMutator(cChallenge.Claustrophobia, true,
				new CustomNameInfo("MapSize: Claustrophobia"),
				new CustomNameInfo("Damn, this city is cramped! Who's Claus, anyway?"));
			Claustrophobia.Available = true;
			Claustrophobia.Conflicting.AddRange(new string[] { cChallenge.ACityForAnts, cChallenge.Megalopolis, cChallenge.Ultrapolis});
			Claustrophobia.IsActive = false;

			CustomMutator Megalopolis = RogueLibs.CreateCustomMutator(cChallenge.Megalopolis, true,
				new CustomNameInfo("MapSize: Megalopolis"),
				new CustomNameInfo("Damn, this town has gotten big. You remember when it was just a small Mega-Arcology. Now it's a Mega-Mega-Arcology."));
			Megalopolis.Available = true;
			Megalopolis.Conflicting.AddRange(new string[] { cChallenge.ACityForAnts, cChallenge.Claustrophobia, cChallenge.Ultrapolis });
			Megalopolis.IsActive = false;

			CustomMutator Ultrapolis = RogueLibs.CreateCustomMutator(cChallenge.Ultrapolis, true,
				new CustomNameInfo("MapSize: Ultrapolis"),
				new CustomNameInfo("You get vertigo when you look up. This city is MASSIVE."));
			Ultrapolis.Available = true;
			Ultrapolis.Conflicting.AddRange(new string[] { cChallenge.ACityForAnts, cChallenge.Claustrophobia, cChallenge.Megalopolis });
			Ultrapolis.IsActive = false;

			#endregion
			#region Population

			CustomMutator GhostTown = RogueLibs.CreateCustomMutator(cChallenge.GhostTown, true,
				new CustomNameInfo("Population: Ghost Town"),
				new CustomNameInfo("No one walks the streets in this city. Don't act all innocent, I know what you do to people in this game!"));
			GhostTown.Available = true;
			GhostTown.Conflicting.AddRange(new string[] { cChallenge.LetMeSeeThatThrong, cChallenge.SwarmWelcome });
			GhostTown.IsActive = false;

			CustomMutator LetMeSeeThatThrong = RogueLibs.CreateCustomMutator(cChallenge.LetMeSeeThatThrong, true,
				new CustomNameInfo("Population: Let Me See That Throng"),
				new CustomNameInfo("The City's HR department is still working out an effective Eugenics program. For now, people are breeding out of control."));
			LetMeSeeThatThrong.Available = true;
			LetMeSeeThatThrong.Conflicting.AddRange(new string[] { cChallenge.GhostTown, cChallenge.SwarmWelcome });
			LetMeSeeThatThrong.IsActive = false;

			CustomMutator SwarmWelcome = RogueLibs.CreateCustomMutator(cChallenge.SwarmWelcome, true,
				new CustomNameInfo("Population: Swarm Welcome"),
				new CustomNameInfo("This whole city feels like a crowded subway. Pickpocketing is bad enough, but the frottage is out of control!"));
			SwarmWelcome.Available = true;
			SwarmWelcome.Conflicting.AddRange(new string[] { cChallenge.GhostTown, cChallenge.LetMeSeeThatThrong });
			SwarmWelcome.IsActive = false;

			#endregion
			#region Roamers

			CustomMutator YoungMenIntheNeighborhood = RogueLibs.CreateCustomMutator(cChallenge.YoungMenInTheNeighborhood, true,
				new CustomNameInfo("Young Men in the Neighborhood"),
				new CustomNameInfo("Beause the young gentlemen in the hood are always polite; If you start acting rude, we'll set you right!\nYour friendly local Gangbangers now roam every district."));
			YoungMenIntheNeighborhood.Available = true;
			YoungMenIntheNeighborhood.Conflicting.AddRange(new string[] { });
			YoungMenIntheNeighborhood.IsActive = false;

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

			#endregion
			#region QuestCount

			CustomMutator SingleMinded = RogueLibs.CreateCustomMutator(cChallenge.SingleMinded, true,
				new CustomNameInfo("QuestCount: Single-minded"),
				new CustomNameInfo("Your Resistance HR profile says \"Not a good multi-tasker.\" They only give you one job per Floor."));
			SingleMinded.Available = true;
			SingleMinded.Conflicting.AddRange(new string[] { cChallenge.Workhorse, cChallenge.ZeroQuests });
			SingleMinded.IsActive = false;

			CustomMutator Workhorse = RogueLibs.CreateCustomMutator(cChallenge.Workhorse, true,
				new CustomNameInfo("QuestCount: Workhorse"),
				new CustomNameInfo("You made the mistake of being reliable. Now the Resistance sends you all the work. You're never short on jobs.."));
			Workhorse.Available = true;
			Workhorse.Conflicting.AddRange(new string[] { cChallenge.SingleMinded, cChallenge.ZeroQuests });
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
			if (GC.challenges.Contains(cChallenge.ScaryGuns))
			{
				__instance.damage = Mathf.Max(1, (int)(__instance.damage * UnityEngine.Random.Range(0.25f, 5f)));
				__instance.speed = Mathf.Min(65, __instance.speed * 3);
			}
		}
		#endregion
		#region SpawnerMain
		public static bool SpawnerMain_SpawnBullet(Vector3 bulletPos, bulletStatus bulletType, PlayfieldObject myPlayfieldObject, int bulletNetID, SpawnerMain __instance, ref Bullet __result) // Prefix
		{
			if (!GC.challenges.Contains(cChallenge.ScaryGuns)
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
