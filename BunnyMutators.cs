using RogueLibsCore;
using System;
using UnityEngine;

namespace BunnyMod
{
	public class BunnyMutators
	{
		public void Awake()
		{
			InitializeMutators();

			BunnyHeader.MainInstance.PatchPostfix(typeof(Bullet), "SetupBullet", GetType(), "Bullet_SetupBullet", new Type[0] { });

			BunnyHeader.MainInstance.PatchPrefix(typeof(SpawnerMain), "SpawnBullet", GetType(), "SpawnerMain_SpawnBullet", new Type[4] { typeof(Vector3), typeof(bulletStatus), typeof(PlayfieldObject), typeof(int) });

			BunnyHeader.MainInstance.PatchPrefix(typeof(PoolsScene), "SpawnWall", GetType(), "PoolsScene_SpawnWall", new Type[7] { typeof(bool), typeof(wallMaterialType), typeof(Vector3), typeof(TileData), typeof(int), typeof(int), typeof(bool)});


		}
		public static void InitializeMutators()
		{
			#region Map Modification
			CustomMutator shantyTown = RogueLibs.CreateCustomMutator("Shantytown", true,
				new CustomNameInfo("Shantytown"),
				new CustomNameInfo("Hard mode for Firefighters, easy mode for arsonists. Fun mode for psychopaths."));
			shantyTown.Available = true;
			shantyTown.Conflicting.AddRange(new string[] { "SteelCity" });
			shantyTown.IsActive = true;

			CustomMutator cityOfSteel = RogueLibs.CreateCustomMutator("CityOfSteel", true,
				new CustomNameInfo("CityOfSteel"),
				new CustomNameInfo("A gleaming city of steel! The world of the future, today. Mankind's dream in... Wow, it *really* smells like steel cleaner. Like, everywhere. This is pungent."));
			cityOfSteel.Available = true;
			cityOfSteel.Conflicting.AddRange(new string[] { "Shantytown" });
			cityOfSteel.IsActive = true;
			#endregion

			CustomMutator scaryGuns = RogueLibs.CreateCustomMutator("Scary Guns", true,
				new CustomNameInfo("Scary Guns"),
				new CustomNameInfo("Bullets act a little more realistically. Get good, or get dead."));
			scaryGuns.Available = true;
			scaryGuns.IsActive = true;
		}

		#region Bullet
		public static void Bullet_SetupBullet(Bullet __instance) // Postfix
		{
			if (BunnyHeader.gc.challenges.Contains("ScaryGuns"))
			{
				__instance.damage *= 2;
				__instance.speed *= 2;
			}
		}
		#endregion
		#region PoolsScene
		public static bool PoolsScene_SpawnWall(bool isFront, wallMaterialType myWallMaterial, Vector3 myPos, TileData myTileData, int streamingSize, int streamingOffset, bool buildingStreamingChunk) // Prefix
		{
			BunnyHeader.Log("PoolsScene_SpawnWall: ");

			if (BunnyHeader.gc.levelType != "HomeBase")
			{
				if (BunnyHeader.gc.challenges.Contains("ShantyTown"))
				{
					if (myWallMaterial == wallMaterialType.Normal || myWallMaterial == wallMaterialType.Steel)
						myWallMaterial = wallMaterialType.Wood;
					else if (myWallMaterial == wallMaterialType.Bars)
						myWallMaterial = wallMaterialType.BarbedWire;
				}
				else if (BunnyHeader.gc.challenges.Contains("CityOfSteel"))
				{
					if (myWallMaterial == wallMaterialType.Normal || myWallMaterial == wallMaterialType.Wood)
						myWallMaterial = wallMaterialType.Steel;
					else if (myWallMaterial == wallMaterialType.BarbedWire)
						myWallMaterial = wallMaterialType.Bars;
				}
			}

			return true;
		}
		#endregion
		#region SpawnerMain
		public static bool SpawnerMain_SpawnBullet(Vector3 bulletPos, bulletStatus bulletType, PlayfieldObject myPlayfieldObject, int bulletNetID, SpawnerMain __instance, ref Bullet __result) // Prefix
		{
			if (!BunnyHeader.gc.challenges.Contains("ScaryGuns") 
				|| !(bulletType == bulletStatus.Normal || bulletType == bulletStatus.Shotgun || bulletType == bulletStatus.Revolver))
				return true;

			Agent agent = null;
			Item item = null;
			ObjectReal objectReal = null;
			bool isFromAgent = false;
			bool isFromItem = false;
			float bulletScale = 0.33333f;

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
