using System;
using System.Collections;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using HarmonyLib;
using RogueLibsCore;
using UnityEngine;

namespace BunnyMod
{
	public class BunnyAbilities
	{
		#region Main
		public void Awake()
		{
			InitializeAbilities();

			BunnyHeader.MainInstance.PatchPostfix(typeof(AgentHitbox), "LandedOnLand", GetType(), "AgentHitbox_LandedOnLand", new Type[0] { });

			BunnyHeader.MainInstance.PatchPostfix(typeof(Bullet), "BulletHitEffect", GetType(), "Bullet_BulletHitEffect", new Type[1] { typeof(GameObject) });

			BunnyHeader.MainInstance.PatchPrefix(typeof(Explosion), "SetupExplosion", GetType(), "Explosion_SetupExplosion", new Type[0] { });

			BunnyHeader.MainInstance.PatchPostfix(typeof(LoadLevel), "SetupMore5_2", GetType(), "LoadLevel_SetupMore5_2", new Type[0] { });

			BunnyHeader.MainInstance.PatchPostfix(typeof(StatusEffects), "AddStatusEffectSpecial", GetType(), "StatusEffects_AddStatusEffectSpecial", new Type[4] { typeof(String), typeof(Agent), typeof(Agent), typeof(bool) });
			BunnyHeader.MainInstance.PatchPostfix(typeof(StatusEffects), "GiveSpecialAbility", GetType(), "StatusEffects_GiveSpecialAbility", new Type[1] { typeof(String) });
			BunnyHeader.MainInstance.PatchPrefix(typeof(StatusEffects), "Stomp", GetType(), "StatusEffects_Stomp", new Type[0] { });
		}
		public static void InitializeAbilities() // Main
		{
			#region Chronomancy

			Sprite spriteChronomancy = RogueUtilities.ConvertToSprite(Properties.Resources.Chronomancy);

			CustomAbility chronomancy = RogueLibs.CreateCustomAbility("Chronomancy", spriteChronomancy, true,
				new CustomNameInfo("Chronomancy"),
				new CustomNameInfo("You can slow down time for everyone but yourself. Sometimes you accidentally do the opposite. You can dodge bullets, but have a lot of trouble eating soup."),
				delegate (InvItem item)
				{
					item.cantDrop = true;
					item.Categories.Add("Usable");
					item.Categories.Add("NPCsCantPickup");
					item.dontAutomaticallySelect = true;
					item.dontSelectNPC = true;
					item.otherDamage = 0; // Bitwise variable field, see Extension method class below
					item.isWeapon = false;
					item.initCount = 100;
					item.rechargeAmountInverse = 100;
					item.maxAmmo = 100;
					item.itemType = "";
					item.rechargeAmountInverse = item.initCount;
					item.stackable = true;
					item.thiefCantSteal = true;
				});

			chronomancy.Available = true;
			chronomancy.AvailableInCharacterCreation = true;
			chronomancy.CostInCharacterCreation = 10;

			chronomancy.OnPressed = delegate (InvItem item, Agent agent)
			{
				if (ChronomancyIsWindingUp(agent) || ChronomancyIsMiscast(agent))
					ChronomancyDialogueCantDo(agent);
				else if (ChronomancyIsCast(agent))
					ChronomancyDecast(agent);
				else
				{
					if (ChronomancyRollForMiscast(agent, (float)(100 - item.invItemCount)/100))
						ChronomancyMiscast(agent, ChronomancyCalcTimescale(agent, true));
					else
						ChronomancyCast(agent, ChronomancyCalcTimescale(agent, false));
				}
			};
			chronomancy.Recharge = (item, agent) =>
			{
				if (ChronomancyIsCast(agent))
				{
					item.invItemCount -= ChronomancyCalcManaCost(agent);

					if (item.invItemCount < 0)
						ChronomancyMiscast(agent, ChronomancyCalcTimescale(agent, true));
				}
				else if (item.invItemCount < 100 && agent.statusEffects.CanRecharge())
				{
					item.invItemCount += Math.Min(100 - item.invItemCount, 5);

					if (item.invItemCount == 100)
						ChronomancyRecharge(agent);
				}
				else if (item.invItemCount == 100 && ChronomancyIsMiscast(agent) || ChronomancyIsWindingUp(agent))
					ChronomancyRecharge(agent);
			};
			chronomancy.RechargeInterval = (item, myAgent) =>
				item.invItemCount > 0 ? new WaitForSeconds(1f) : null;
			#endregion Chronomancy
			#region Cryomancy
			Sprite spriteCryomancy = RogueUtilities.ConvertToSprite(Properties.Resources.Cryomancy);

			CustomAbility cryomancy = RogueLibs.CreateCustomAbility("Cryomancy", spriteCryomancy, true,
				new CustomNameInfo("Cryomancy"),
				new CustomNameInfo("You can shoot a Freeze Ray from your hands. Your ice cream never melts."),
				delegate (InvItem item)
				{
					item.cantDrop = true;
					item.Categories.Add("Weapons");
					item.Categories.Add("NPCsCantPickUp");
					item.dontAutomaticallySelect = true;
					item.dontSelectNPC = true;
					item.gunKnockback = 0;
					item.isWeapon = true;
					item.rapidFire = false;
					item.initCount = 100;
					item.itemType = "WeaponProjectile";
					//item.LoadItemSprite("Fireball");
					item.rapidFire = false;
					item.rechargeAmountInverse = item.initCount;
					item.shadowOffset = 2;
					item.specialMeleeTexture = true;
					item.stackable = true;
					item.thiefCantSteal = true;
					item.weaponCode = weaponType.WeaponProjectile;
				});

			cryomancy.Available = true;
			cryomancy.AvailableInCharacterCreation = true;
			cryomancy.CostInCharacterCreation = 8;

			bool icedOut = false;

			cryomancy.OnPressed = delegate (InvItem item, Agent agent)
			{
				if (icedOut)
				{
					item.agent.gc.audioHandler.Play(item.agent, "CantDo");
				}

				if (CryomancyRollForMiscast(agent, 0))
				{
					CryomancyMiscast(agent, 20);
					icedOut = true;
				}
				else
				{
					CryomancyCast(agent);
					item.invItemCount -= CryomancyManaCost(agent);
				}
			};

			cryomancy.Recharge = (item, myAgent) =>
			{
				if (item.invItemCount < item.rechargeAmountInverse && myAgent.statusEffects.CanRecharge())
				{
					item.invItemCount++;

					if (item.invItemCount == 100)
					{
						if (icedOut)
							icedOut = false;

						myAgent.statusEffects.CreateBuffText("Recharged", myAgent.objectNetID);
						myAgent.gc.audioHandler.Play(myAgent, "Recharge");
						myAgent.inventory.buffDisplay.specialAbilitySlot.MakeUsable();
					}
				}
			};

			cryomancy.RechargeInterval = (item, myAgent) =>
				item.invItemCount > 0 ? new WaitForSeconds(0.2f) : null;

			#endregion
			#region Electromancy
			Sprite spriteElectromancy = RogueUtilities.ConvertToSprite(Properties.Resources.Electromancy);

			CustomAbility electromancy = RogueLibs.CreateCustomAbility("Electromancy", spriteElectromancy, true,
				new CustomNameInfo("Electromancy"),
				new CustomNameInfo("You can shoot a little bolt of lightning from your hands. Do not try to charge your phone with it."),
				delegate (InvItem item)
				{
					item.cantDrop = true;
					item.Categories.Add("Weapons");
					item.Categories.Add("NPCsCantPickUp");
					item.dontAutomaticallySelect = true;
					item.dontSelectNPC = true;
					item.gunKnockback = 0;
					item.isWeapon = true;
					item.rapidFire = false;
					item.initCount = 100;
					item.itemType = "WeaponProjectile";
					item.rapidFire = false;
					item.rechargeAmountInverse = item.initCount;
					item.shadowOffset = 2;
					item.specialMeleeTexture = true;
					item.stackable = true;
					item.thiefCantSteal = true;
					item.weaponCode = weaponType.WeaponProjectile;
				});

			electromancy.Available = true;
			electromancy.AvailableInCharacterCreation = true;
			electromancy.CostInCharacterCreation = 8;

			bool zappedOut = false; // Move to Bitfield variable

			electromancy.OnPressed = delegate (InvItem item, Agent agent)
			{
				if (zappedOut)
					ElectromancyDialogueCantDo(agent);
				else if (ElectromancyRollForMiscast(agent, 0))
					ElectromancyMiscast(agent, 20);
				else
				{
					ElectromancyCast(agent);
					item.invItemCount -= ElectromancyManaCost(agent);
				}
			};
			electromancy.Recharge = (item, myAgent) =>
			{
				if (item.invItemCount < item.rechargeAmountInverse && myAgent.statusEffects.CanRecharge())
				{
					item.invItemCount++;

					if (item.invItemCount == 100)
					{
						if (zappedOut)
							zappedOut = false;

						myAgent.statusEffects.CreateBuffText("Recharged", myAgent.objectNetID);
						myAgent.gc.audioHandler.Play(myAgent, "Recharge");
						myAgent.inventory.buffDisplay.specialAbilitySlot.MakeUsable();
					}
				}
			};

			electromancy.RechargeInterval = (item, myAgent) =>
				item.invItemCount > 0 ? new WaitForSeconds(0.2f) : null;
			#endregion
			#region Pyromancy

			Sprite spritePyromancy = RogueUtilities.ConvertToSprite(Properties.Resources.Pyromancy);

			CustomAbility pyromancy = RogueLibs.CreateCustomAbility("Pyromancy", spritePyromancy, true,
				new CustomNameInfo("Pyromancy"),
				new CustomNameInfo("You can throw fire from your hands. This tends to fix a lot of your problems, and create much worse ones."),
				delegate (InvItem item)
				{
					item.cantDrop = true;
					item.Categories.Add("Weapons");
					item.Categories.Add("NPCsCantPickUp");
					item.dontAutomaticallySelect = true;
					item.dontSelectNPC = true;
					item.gunKnockback = 0;
					item.isWeapon = true;
					item.rapidFire = true; //testing
					item.initCount = 100;
					item.itemType = "WeaponProjectile";
					item.LoadItemSprite("Fireball");
					item.rapidFire = true;
					item.rechargeAmountInverse = item.initCount;
					item.shadowOffset = 2;
					item.stackable = true;
					item.thiefCantSteal = true;
					item.weaponCode = weaponType.WeaponProjectile;
				});

			pyromancy.Available = true;
			pyromancy.AvailableInCharacterCreation = true;
			pyromancy.CostInCharacterCreation = 8;

			pyromancy.OnHeld = delegate (InvItem item, Agent agent, ref float unused)
			{
				if (!PyromancyIsBurnedOut(agent))
				{
					if (PyromancyRollForMiscast(agent, -75))
						PyromancyMiscast(agent, 20);
					else
					{
						PyromancyCast(agent);

						if (PyromancyManaCost(agent))
							item.invItemCount--;

						if (item.invItemCount <= 0)
							PyromancySetBurnedOut(agent, true);
					}
				}
			};
			pyromancy.Recharge = (item, myAgent) =>
			{
				if (item.invItemCount < item.rechargeAmountInverse && myAgent.statusEffects.CanRecharge())
				{
					item.invItemCount++;

					if (item.invItemCount == 100)
						PyromancyRecharge(myAgent);
				}
			};
			pyromancy.RechargeInterval = (item, myAgent) =>
				item.invItemCount > 0 ? new WaitForSeconds(0.2f) : null;
			#endregion
			#region Telemancy
			Sprite spriteTelemancy = RogueUtilities.ConvertToSprite(Properties.Resources.Telemancy);

			CustomAbility telemancy = RogueLibs.CreateCustomAbility("Telemancy", spriteTelemancy, true,
				new CustomNameInfo("Telemancy"),
				new CustomNameInfo("You can teleport sort of at will, but it's unpredictable and makes you feel sick. Maybe you can get better at this?"),
				delegate (InvItem item)
				{
					item.cantDrop = true;
					item.Categories.Add("Usable"); //
					item.Categories.Add("NPCsCantPickup");
					item.dontAutomaticallySelect = true;
					item.dontSelectNPC = true;
					item.isWeapon = false;
					item.initCount = 100;
					item.itemType = ""; //
					item.rechargeAmountInverse = item.initCount;
					item.stackable = true;
					item.thiefCantSteal = true;
				});

			telemancy.Available = true;
			telemancy.AvailableInCharacterCreation = true;
			telemancy.CostInCharacterCreation = 8;

			telemancy.OnPressed = delegate (InvItem item, Agent agent)
			{
				if (TelemancyRollForMiscast(agent, (float)(100 - item.invItemCount) / 100))
					TelemancyMiscast(agent);

				if (item.invItemCount <= 0)
				{
					item.agent.gc.audioHandler.Play(item.agent, "CantDo");
					agent.Say("I need to give it a rest or my head will explode. I've seen it happen.");
				}
				else
				{
					agent.SpawnParticleEffect("Spawn", agent.curPosition);

					Vector3 targetLocation = TelemancyCast(agent, false, false, true, true, true);

					agent.Teleport(targetLocation, false, true);
					agent.rb.velocity = Vector2.zero;

					agent.SpawnParticleEffect("Spawn", agent.tr.position, false);
					GameController.gameController.audioHandler.Play(agent, "Spawn");

					item.invItemCount -= TelemancyManaCost(agent);

					if (item.invItemCount <= 0)
						TelemancyMiscast(agent);
				}
			};
			telemancy.Recharge = (item, agent) =>
			{
				if (item.invItemCount < 100 && agent.statusEffects.CanRecharge())
				{
					item.invItemCount++;

					if (item.invItemCount == 100)
					{
						agent.statusEffects.CreateBuffText("Recharged", agent.objectNetID);
						agent.gc.audioHandler.Play(agent, "Recharge");
						agent.inventory.buffDisplay.specialAbilitySlot.MakeUsable();
					}
				}
			};

			telemancy.RechargeInterval = (item, myAgent) =>
				item.invItemCount > 0 ? new WaitForSeconds(0.2f) : null;
			#endregion
		}
		#endregion

		#region Magic
		public static Vector2 MouseIngamePosition()
		{
			Plane plane = new Plane(new Vector3(0, 0, 1), new Vector3(0, 0, 0));
			Ray ray = Camera.main.ScreenPointToRay(UnityEngine.Input.mousePosition);
			return plane.Raycast(ray, out float enter) ? (Vector2)ray.GetPoint(enter) : default;
		}
		#region Chronomancy

		public static float baseTimeScale;

		public static int ChronomancyCalcManaCost(Agent agent)
		{
			int increment = 5;

			if (agent.statusEffects.hasTrait("WildCasting"))
				increment += UnityEngine.Random.Range(-1, 1);
			else if (agent.statusEffects.hasTrait("WildCasting_2"))
				increment = UnityEngine.Random.Range(-3, 3);

			if (agent.statusEffects.hasTrait("FocusedCasting"))
				increment -= 1;
			else if (agent.statusEffects.hasTrait("FocusedCasting_2"))
				increment -= 2;

			return increment;
		}
		public static float ChronomancyCalcTimescale(Agent agent, bool MisCast)
		{
			float timescale = 0.0f;

			if (!MisCast)
			{
				timescale = 2.0f;

				if (agent.statusEffects.hasTrait("WildCasting"))
					timescale += 0.5f;
				else if (agent.statusEffects.hasTrait("WildCasting_2"))
					timescale += 1.0f;

				if (agent.statusEffects.hasTrait("MagicTraining"))
					timescale += 0.5f;
				else if (agent.statusEffects.hasTrait("MagicTraining_2"))
					timescale += 1.0f;
			}
			else if (MisCast)
			{
				timescale = 4.0f;

				if (agent.statusEffects.hasTrait("WildCasting"))
					timescale += 1.0f;
				else if (agent.statusEffects.hasTrait("WildCasting_2"))
					timescale += 2.0f;

				if (agent.statusEffects.hasTrait("FocusedCasting"))
					timescale -= 0.5f;
				else if (agent.statusEffects.hasTrait("FocusedCasting_2"))
					timescale -= 1.0f;
			}

			return timescale;
		}
		public static void ChronomancyCast(Agent agent, float speedupfactor)
		{
			agent.SpawnParticleEffect("ExplosionMindControl", agent.curPosition);
			GameController.gameController.audioHandler.Play(agent, "UseNecronomicon");

			ChronomancySetCast(agent, true);

			agent.gc.selectedTimeScale /= speedupfactor;
			agent.gc.mainTimeScale /= speedupfactor;
			agent.speedMax = agent.FindSpeed() * (int)speedupfactor;
		}
		public static async void ChronomancyDecast(Agent agent)
		{
			agent.SpawnParticleEffect("ExplosionWarp", agent.curPosition);
			GameController.gameController.audioHandler.Play(agent, "UseNecronomicon");

			//TODO: Eliminate redundancies between Recharge and DeCast

			agent.speedMax = agent.FindSpeed();

			if (!agent.underWater && !agent.jumped && !agent.melee.attackAnimPlaying && 
				(agent.statusEffects.hasTrait("HammerTime") || agent.statusEffects.hasTrait("HammerTime_2")))
			{
				agent.stomping = true;
				agent.Jump();

				agent.gc.selectedTimeScale /= 3f;
				agent.gc.mainTimeScale /= 3f;

				await Task.Delay((int)(1000 / agent.gc.mainTimeScale)); // Trying this out, as I think it uses real time rather than game time
			}

			agent.gc.selectedTimeScale = baseTimeScale;
			agent.gc.mainTimeScale = baseTimeScale;

			await Task.Delay(1000);

			ChronomancySetCast(agent, false);
			await ChronomancyStartWindingUp(agent);
		}
		public static void ChronomancyDialogueCantDo(Agent agent)
		{
			agent.gc.audioHandler.Play(agent, "CantDo");

			switch (UnityEngine.Random.Range(1, 2))
			{
				case 1:
					agent.Say("I need to take a \"time out!\" Get it? But seriously, my heart will stop.");
					break;
				case 2:
					agent.Say("I'm gonna take the blue pill for a sec.");
					break;
			}
		}
		public static void ChronomancyDialogueMiscast(Agent agent)
		{
			switch (UnityEngine.Random.Range(1, 4))
			{
				case 1:
					agent.Say("Iii ttthhhiiinnnkkk Iii mmmeeesssssseeeddd uuuppp...");
					break;
				case 2:
					agent.Say("Bullet Time? More like Bullshit Time!");
					break;
				case 3:
					agent.Say("(Slow Motion Noises)");
					break;
				case 4:
					agent.Say("Okay, maybe there is a spoon, I guess. Whatever.");
					break;
			}
		}
		public static bool ChronomancyIsCast(Agent agent) =>
			(agent.inventory.equippedSpecialAbility.otherDamage & 0b_0001) != 0;
		public static bool ChronomancyIsMiscast(Agent agent) =>
			(agent.inventory.equippedSpecialAbility.otherDamage & 0b_0010) != 0;
		public static bool ChronomancyIsWindingUp(Agent agent) =>
			(agent.inventory.equippedSpecialAbility.otherDamage & 0b_0100) != 0;
		public static void ChronomancyMiscast(Agent agent, float slowdownFactor)
		{
			agent.SpawnParticleEffect("ExplosionEMP", agent.curPosition);
			agent.gc.audioHandler.Play(agent, "ToiletTeleportIn");
			ChronomancyDialogueMiscast(agent);

			if (ChronomancyIsCast(agent))
				ChronomancySetCast(agent, false);
			ChronomancySetMiscast(agent, true);

			ChronomancyStartWindingUp(agent); // TODO: Ensure that this duration is equal to miscast duration

			agent.gc.selectedTimeScale *= slowdownFactor;
			agent.gc.mainTimeScale *= slowdownFactor;
			agent.speedMax = agent.FindSpeed() / (int)slowdownFactor;
			agent.inventory.buffDisplay.specialAbilitySlot.MakeNotUsable();
		}
		public static void ChronomancyRecharge(Agent agent)
		{
			agent.statusEffects.CreateBuffText("Recharged", agent.objectNetID);
			agent.gc.audioHandler.Play(agent, "Recharge");

			if (ChronomancyIsWindingUp(agent))
				ChronomancySetWindingUp(agent, false);

			if (ChronomancyIsMiscast(agent))
			{
				//TODO: Eliminate redundancies between Recharge and DeCast

				ChronomancySetMiscast(agent, false);

				agent.gc.selectedTimeScale = baseTimeScale;
				agent.gc.mainTimeScale = baseTimeScale;
				agent.speedMax = agent.FindSpeed();

				agent.inventory.buffDisplay.specialAbilitySlot.MakeUsable();
			}
		}
		public static bool ChronomancyRollForMiscast(Agent agent, float modifier)
		{
			float risk = 1.0f + modifier;

			if (agent.statusEffects.hasTrait("FocusedCasting"))
				risk -= 0.25f;
			else if (agent.statusEffects.hasTrait("FocusedCasting_2"))
				risk -= 0.50f;

			if (agent.statusEffects.hasTrait("WildCasting"))
				risk += 0.75f;
			else if (agent.statusEffects.hasTrait("WildCasting_2"))
				risk += 1.50f;

			if (agent.statusEffects.hasTrait("MagicTraining"))
				risk *= (4 / 5);
			else if (agent.statusEffects.hasTrait("MagicTraining_2"))
				risk *= (3 / 5);

			return (UnityEngine.Random.Range(0f, 100f) <= risk);
		}
		public static void ChronomancySetCast(Agent agent, bool value)
		{
			//BunnyHeader.Log("SetCast " + value);

			if (value) agent.inventory.equippedSpecialAbility.otherDamage |= 0b_0001;
			else agent.inventory.equippedSpecialAbility.otherDamage &= ~0b_0001;
		}
		public static void ChronomancySetMiscast(Agent agent, bool value)
		{
			//BunnyHeader.Log("SetSlowedDown " + value);

			if (value) agent.inventory.equippedSpecialAbility.otherDamage |= 0b_0010;
			else agent.inventory.equippedSpecialAbility.otherDamage &= ~0b_0010;
		}
		public static void ChronomancySetWindingUp(Agent agent, bool value)
		{
			//BunnyHeader.Log("SetCoolingDown " + value);

			if (value) agent.inventory.equippedSpecialAbility.otherDamage |= 0b_0100;
			else agent.inventory.equippedSpecialAbility.otherDamage &= ~0b_0100;
		}
		public static async Task ChronomancyStartWindingUp(Agent agent)
		{
			ChronomancySetWindingUp(agent, true);

			float duration = 4000f;

			if (agent.statusEffects.hasTrait("WildCasting"))
				duration -= 1000f;
			else if (agent.statusEffects.hasTrait("WildCasting_2"))
				duration -= 2000f;

			if (agent.statusEffects.hasTrait("MagicTraining"))
				duration -= 1000f;
			else if (agent.statusEffects.hasTrait("MagicTraining_2"))
				duration -= 2000f;

			await Task.Delay((int)duration);

			while (ChronomancyIsMiscast(agent))
				await Task.Delay(1000); 
			
			ChronomancyRecharge(agent);
		}
		#endregion
		#region Cryomancy
		public static void CryomancyCast(Agent agent)
		{
			agent.gun.HideGun();

			Bullet bullet = agent.gc.spawnerMain.SpawnBullet(agent.gun.tr.position, bulletStatus.FreezeRay, agent);

			if (agent.controllerType == "Keyboard" && !agent.gc.sessionDataBig.trackpadMode)
				bullet.movement.RotateToMouseTr(agent.agentCamera.actualCamera);
			else if (agent.target.AttackTowardTarget())
				bullet.tr.rotation = Quaternion.Euler(0f, 0f, agent.target.transform.eulerAngles.z);
			else
				bullet.tr.rotation = Quaternion.Euler(0f, 0f, agent.gun.FindWeaponAngleGamepad() - 90f);

			if (agent.gc.sessionDataBig.autoAim != "Off")
			{
				int myChance = 25; // Placeholder, find the real numbers later. For now, suck it, Auto-aimers B)
				if (agent.gc.percentChance(myChance))
					bullet.movement.AutoAim(agent, agent.movement.FindAimTarget(true), bullet);
			}
		}
		public static int CryomancyManaCost(Agent agent)
		{
			int minimum = 20;
			int maximum = 40;

			if (agent.statusEffects.hasTrait("MagicTraining_2"))
			{
				minimum -= 5;
				maximum -= 5;
			}
			else if (agent.statusEffects.hasTrait("MagicTraining"))
			{
				minimum -= 2;
				maximum -= 2;
			}

			if (agent.statusEffects.hasTrait("WildCasting"))
			{
				minimum -= UnityEngine.Random.Range(-3, 5);
				maximum -= UnityEngine.Random.Range(-3, 5);
			}
			else if (agent.statusEffects.hasTrait("WildCasting_2"))
			{
				minimum -= UnityEngine.Random.Range(-5, 10);
				maximum -= UnityEngine.Random.Range(-5, 10);
			}

			return UnityEngine.Random.Range(minimum, maximum);
		}
		public static void CryomancyMiscast(Agent agent, int degree)
		{
			agent.statusEffects.AddStatusEffect("Frozen", degree);
		}
		public static bool CryomancyRollForMiscast(Agent agent, int modifier)
		{
			int risk = 100 + modifier;

			if (agent.statusEffects.hasTrait("FocusedCasting_2"))
				risk -= 50;
			else if (agent.statusEffects.hasTrait("FocusedCasting"))
				risk -= 25;

			if (agent.statusEffects.hasTrait("WildCasting_2"))
				risk += 150;
			else if (agent.statusEffects.hasTrait("WildCasting"))
				risk += 75;

			if (agent.statusEffects.hasTrait("MagicTraining_2"))
				risk *= (3 / 5);
			else if (agent.statusEffects.hasTrait("MagicTraining"))
				risk *= (4 / 5);

			return (UnityEngine.Random.Range(0, 10000) <= risk);
		}
		#endregion
		#region Electromancy
		public static void ElectromancyCast(Agent agent)
		{
			agent.gun.HideGun();

			Bullet bullet = agent.gc.spawnerMain.SpawnBullet(agent.gun.tr.position, bulletStatus.Taser, agent);
			bullet.speed *= 3 / 2; //
			bullet.cameFromWeapon = "ChainLightning";
			//bullet.agent.SpawnParticleEffect("Electrocution", bullet.transform.position);

			if (agent.controllerType == "Keyboard" && !agent.gc.sessionDataBig.trackpadMode)
				bullet.movement.RotateToMouseTr(agent.agentCamera.actualCamera);
			else if (agent.target.AttackTowardTarget())
				bullet.tr.rotation = Quaternion.Euler(0f, 0f, agent.target.transform.eulerAngles.z);
			else
				bullet.tr.rotation = Quaternion.Euler(0f, 0f, agent.gun.FindWeaponAngleGamepad() - 90f);

			if (agent.gc.sessionDataBig.autoAim != "Off")
			{
				int myChance = 25; // Placeholder, find the real numbers later. For now, suck it, Auto-aimers B)
				if (agent.gc.percentChance(myChance))
					bullet.movement.AutoAim(agent, agent.movement.FindAimTarget(true), bullet);
			}
		}
		public static void ElectromancyDialogueCantDo(Agent agent)
		{

		}
		public static int ElectromancyManaCost(Agent agent)
		{
			int minimum = 20;
			int maximum = 40;

			if (agent.statusEffects.hasTrait("MagicTraining_2"))
			{
				minimum -= 5;
				maximum -= 5;
			}
			else if (agent.statusEffects.hasTrait("MagicTraining"))
			{
				minimum -= 2;
				maximum -= 2;
			}

			if (agent.statusEffects.hasTrait("WildCasting"))
			{
				minimum -= UnityEngine.Random.Range(-3, 5);
				maximum -= UnityEngine.Random.Range(-3, 5);
			}
			else if (agent.statusEffects.hasTrait("WildCasting_2"))
			{
				minimum -= UnityEngine.Random.Range(-5, 10);
				maximum -= UnityEngine.Random.Range(-5, 10);
			}

			return UnityEngine.Random.Range(minimum, maximum);
		}
		public static void ElectromancyMiscast(Agent agent, int degree)
		{
			agent.statusEffects.AddStatusEffect("Electrocuted", degree);
		}
		public static void ElectromancyImpact(GameObject hitObject, Bullet bullet)
		{

			int numberOfChains = 1;

			for (int i = 0; i <= numberOfChains; i++)
			{
				Bullet newBullet = bullet.agent.gc.spawnerMain.SpawnBullet(hitObject.transform.position, bulletStatus.Taser, bullet.agent);
				newBullet.speed *= 3 / 2;
				newBullet.agent = bullet.agent;

				if (ElectromancyRollForRebound(bullet.agent, 0))
					newBullet.cameFromWeapon = "ChainLightning"; // Only apply this if the skill successfully rolled for a rebound.

				Vector2 origin = hitObject.transform.position;
				Agent closestAgent = bullet.agent.gc.agentList.OrderBy(a => Vector2.Distance(origin, a.transform.position)).FirstOrDefault();

				Vector3 target;

				if (closestAgent != null)
					target = closestAgent.curPosition;
				else
					target = newBullet.curPosition + UnityEngine.Random.insideUnitCircle.normalized;

				newBullet.movement.RotateToPosition(target);
				//newBullet.movement.RotateToAgent(closest);
				//newBullet.movement.AutoAim(bullet.agent, closest, newBullet);
			}
		}
		public static bool ElectromancyRollForMiscast(Agent agent, int modifier)
		{
			int risk = 100 + modifier;

			if (agent.statusEffects.hasTrait("FocusedCasting_2"))
				risk -= 50;
			else if (agent.statusEffects.hasTrait("FocusedCasting"))
				risk -= 25;

			if (agent.statusEffects.hasTrait("WildCasting_2"))
				risk += 150;
			else if (agent.statusEffects.hasTrait("WildCasting"))
				risk += 75;

			if (agent.statusEffects.hasTrait("MagicTraining_2"))
				risk *= (3 / 5);
			else if (agent.statusEffects.hasTrait("MagicTraining"))
				risk *= (4 / 5);

			return (UnityEngine.Random.Range(0, 10000) <= risk);
		}
		public static bool ElectromancyRollForRebound(Agent agent, int modifier)
		{
			int chance = 50 + modifier;

			return (chance > UnityEngine.Random.Range(1, 100));
		}
		#endregion
		#region Hematomancy // Blood Magic
		#endregion
		#region Kinetomancy // Telekinesis
		#endregion
		#region Megaleiomancy //Charm Person
		#endregion
		#region Necromancy
		// 1 Summon hostile Zombies from corpses / Turn ghosts into small number of crystals
		// 2 Zombies are Neutral to you / Turn ghosts into medium number of crystals
		// 3 Zombies will join your party / Turn ghosts into large number of crystals
		// Miscast turns all of them hostile, or summons hostile ghosts
		// When close to a ghost, you can turn them into mana crystals
		#endregion
		#region Pyromancy
		public static void PyromancyBurnOut(Agent agent)
		{
			agent.gc.audioHandler.Play(agent, "MindControlEnd");

			PyromancySetBurnedOut(agent, true);
		}
		public static void PyromancyCast(Agent agent)
		{
			agent.gun.HideGun();
			//agent.gc.audioHandler.Play(agent, "FireConstant"); // Still won't stop. Not sure how to stop it.

			Bullet bullet = agent.gc.spawnerMain.SpawnBullet(agent.gun.tr.position, bulletStatus.Fire, agent);

			if (agent.controllerType == "Keyboard" && !agent.gc.sessionDataBig.trackpadMode)
				bullet.movement.RotateToMouseTr(agent.agentCamera.actualCamera);
			else if (agent.target.AttackTowardTarget())
				bullet.tr.rotation = Quaternion.Euler(0f, 0f, agent.target.transform.eulerAngles.z);
			else
				bullet.tr.rotation = Quaternion.Euler(0f, 0f, agent.gun.FindWeaponAngleGamepad() - 90f);

			if (agent.gc.sessionDataBig.autoAim != "Off")
			{
				int myChance = 25; // Placeholder, find the real numbers later. For now, suck it, Auto-aimers B)
				if (agent.gc.percentChance(myChance))
					bullet.movement.AutoAim(agent, agent.movement.FindAimTarget(true), bullet);
			}

			if (agent.statusEffects.hasTrait("WildCasting"))
				bullet.speed = 9;
			else if (agent.statusEffects.hasTrait("WildCasting_2"))
				bullet.speed = 12;
			else
				bullet.speed = 6;
		}
		public static void PyromancyDialogueCantDo(Agent agent)
		{
			agent.gc.audioHandler.Play(agent, "CantDo");

			switch (UnityEngine.Random.Range(1, 2))
			{
				case 1:
					agent.Say("I'm burned out.");
					break;
				case 2:
					agent.Say("Please wait. I don't feel like exploding right now.");
					break;
			}
		}
		public static void PyromancyDialogueMiscast(Agent agent)
		{
			switch (UnityEngine.Random.Range(1, 4))
			{
				case 1:
					agent.Say("Not very stoked right now.");
					break;
				case 2:
					agent.Say("Haha my skin is melting lol XDDD");
					break;
				case 3:
					agent.Say("Flame off! Flame off!");
					break;
				case 4:
					agent.Say("I shidded an farded an burst into flame.");
					break;
			}
		}
		public static bool PyromancyIsBurnedOut(Agent agent) =>
			(agent.inventory.equippedSpecialAbility.otherDamage & 0b_0001) != 0;
		public static bool PyromancyIsMiscast(Agent agent) =>
			(agent.inventory.equippedSpecialAbility.otherDamage & 0b_0010) != 0;
		public static bool PyromancyManaCost(Agent agent)
		{
			int chance = 100;

			if (agent.statusEffects.hasTrait("MagicTraining"))
				chance -= 10;
			else if (agent.statusEffects.hasTrait("MagicTraining_2"))
				chance -= 20;
			if (agent.statusEffects.hasTrait("WildCasting"))
				chance -= 15;
			else if (agent.statusEffects.hasTrait("WildCasting_2"))
				chance -= 30;

			return agent.gc.percentChance(chance);
		}
		public static void PyromancyMiscast(Agent agent, int degree)
		{
			agent.gc.spawnerMain.SpawnExplosion(agent, agent.curPosition, "FireBomb");
			agent.statusEffects.ChangeHealth(-degree);

			PyromancySetBurnedOut(agent, true);
		}
		public static void PyromancyRecharge(Agent agent) //TODO
		{
			agent.statusEffects.CreateBuffText("Recharged", agent.objectNetID);
			agent.gc.audioHandler.Play(agent, "Recharge");

			if (PyromancyIsBurnedOut(agent))
				PyromancySetBurnedOut(agent, false);

			if (PyromancyIsMiscast(agent))
				PyromancySetMiscast(agent, false);

			agent.inventory.buffDisplay.specialAbilitySlot.MakeUsable();
		}
		public static bool PyromancyRollForMiscast(Agent agent, int modifier)
		{
			int risk = 100 + modifier;

			if (agent.statusEffects.hasTrait("FocusedCasting_2"))
				risk -= 66;
			else if (agent.statusEffects.hasTrait("FocusedCasting"))
				risk -= 33;

			if (agent.statusEffects.hasTrait("WildCasting_2"))
				risk += 50;
			else if (agent.statusEffects.hasTrait("WildCasting"))
				risk += 25;

			if (agent.statusEffects.hasTrait("MagicTraining_2"))
				risk *= 3 / 5;
			else if (agent.statusEffects.hasTrait("MagicTraining"))
				risk *= 4 / 5;

			return UnityEngine.Random.Range(0, 10000) <= risk;
		}
		public static void PyromancySetBurnedOut(Agent agent, bool value)
		{
			BunnyHeader.Log("PyromancySetBurnedOut " + value);

			if (value) agent.inventory.equippedSpecialAbility.otherDamage |= 0b_0001;
			else agent.inventory.equippedSpecialAbility.otherDamage &= ~0b_0001;
		}
		public static void PyromancySetMiscast(Agent agent, bool value)
		{
			BunnyHeader.Log("PyromancySetBurnedOut " + value);

			if (value) agent.inventory.equippedSpecialAbility.otherDamage |= 0b_0010;
			else agent.inventory.equippedSpecialAbility.otherDamage &= ~0b_0010;
		}
		#endregion
		#region Telemancy
		public static Vector2 TelemancyCast(Agent agent, bool accountForObstacles, bool notInside, bool dontCareAboutDanger, bool teleporting, bool accountForWalls)
		{
			TileInfo tileInfo = agent.gc.tileInfo;
			Vector2 currentPosition = agent.curPosition;
			Vector2 targetPosition;
			float rangeNear = 2.5f;
			float rangeFar = 5.5f;

			if (agent.statusEffects.hasTrait("MagicTraining_2"))
			{
				rangeNear -= 2.5f;
				rangeFar -= 2f;
			}
			if (agent.statusEffects.hasTrait("MagicTraining"))
			{
				rangeNear -= 1.5f;
				rangeFar -= 1f;
			}
			if (agent.statusEffects.hasTrait("WildCasting"))
				rangeNear -= 1f;
			else if (agent.statusEffects.hasTrait("WildCasting_2"))
				rangeNear -= 2f;
			else if (agent.statusEffects.hasTrait("FocusedCasting"))
				rangeFar -= 1f;
			else if (agent.statusEffects.hasTrait("FocusedCasting_2"))
				rangeFar -= 2f;

			rangeNear = rangeNear < 0 ? 0 : rangeNear;
			rangeFar = rangeFar < 0 ? 0 : rangeFar;

			for (int i = 0; i < 50; i++)
			{
				float distance = UnityEngine.Random.Range(rangeNear, rangeFar);

				if (agent.statusEffects.hasTrait("MagicTraining"))
					targetPosition = MouseIngamePosition() + distance * UnityEngine.Random.insideUnitCircle.normalized;
				else
					targetPosition = currentPosition + distance * UnityEngine.Random.insideUnitCircle.normalized;

				TileData tileData = tileInfo.GetTileData(targetPosition);

				if (tileData.solidObject)
					continue;
				else if (tileData.dangerousToWalk && !dontCareAboutDanger && !tileData.spillOoze) // Consider allowing Ooze, for balance
					continue;
				else if (tileInfo.WallExist(tileData) && (accountForObstacles || accountForWalls))
					continue;
				else if (tileInfo.IsOverlapping(targetPosition, "Anything") && accountForObstacles) // Currently always false, but enable this if you're getting stuck on objects. Although that might be fun.
					continue;

				else if (!accountForObstacles)
					if (tileInfo.GetWallMaterial(targetPosition.x, targetPosition.y) == wallMaterialType.Border) // Removed Conveyor, Water, Hole
						continue;

					else if (teleporting && accountForObstacles && tileInfo.IsOverlapping(targetPosition, "Anything", 0.32f))
						continue;

				if (notInside && (tileInfo.IsIndoors(targetPosition) || tileData.owner == 55 || (tileData.floorMaterial == floorMaterialType.ClearFloor && tileData.owner != 0)))
					continue;

				return targetPosition;
			}
			return currentPosition;
		}
		public static int TelemancyManaCost(Agent agent)
		{
			int minimum = 20;
			int maximum = 40;

			if (agent.statusEffects.hasTrait("MagicTraining_2"))
			{
				minimum -= 10;
				maximum -= 10;
			}
			else if (agent.statusEffects.hasTrait("MagicTraining"))
			{
				minimum -= 5;
				maximum -= 5;
			}

			if (agent.statusEffects.hasTrait("WildCasting"))
			{
				minimum -= UnityEngine.Random.Range(-3, 5);
				maximum -= UnityEngine.Random.Range(-3, 5);
			}
			else if (agent.statusEffects.hasTrait("WildCasting_2"))
			{
				minimum -= UnityEngine.Random.Range(-5, 10);
				maximum -= UnityEngine.Random.Range(-5, 10);
			}

			return UnityEngine.Random.Range(minimum, maximum);
		}
		public static void TelemancyMiscast(Agent agent)
		{
			agent.gc.audioHandler.Play(agent, "ZombieSpitFire");
			switch (UnityEngine.Random.Range(1, 4))
			{
				case 1:
					agent.Say("I smell burning toast.");
					break;
				case 2:
					agent.Say("Blurgh. (Drool)");
					break;
				case 3:
					agent.Say("I pink I bust hab a stroke.");
					break;
				case 4:
					agent.Say("My head a splode.");
					break;
			}

			int degree = 20;

			if (agent.statusEffects.hasTrait("StrongGagReflex"))
				degree /= 2;
			else if (agent.statusEffects.hasTrait("StrongGagReflex_2"))
				degree /= 4;

			agent.statusEffects.ChangeHealth(-degree);
			agent.statusEffects.AddStatusEffect("Dizzy", degree / 4);
			agent.inventory.buffDisplay.specialAbilitySlot.MakeNotUsable();
		}
		public static bool TelemancyRollForMiscast(Agent agent, float modifier)
		{
			float risk = 1.0f + modifier;

			if (agent.statusEffects.hasTrait("FocusedCasting"))
				risk -= 0.25f;
			else if (agent.statusEffects.hasTrait("FocusedCasting_2"))
				risk -= 0.50f;

			if (agent.statusEffects.hasTrait("WildCasting"))
				risk += 0.75f;
			else if (agent.statusEffects.hasTrait("WildCasting_2"))
				risk += 1.50f;

			if (agent.statusEffects.hasTrait("MagicTraining"))
				risk *= (4 / 5);
			else if (agent.statusEffects.hasTrait("MagicTraining_2"))
				risk *= (3 / 5);

			return (UnityEngine.Random.Range(0f, 100f) <= risk);
		}
		#endregion
		#endregion

		#region AgentHitbox
		public static void AgentHitbox_LandedOnLand(AgentHitbox __instance) // Postfix
		{
			BunnyHeader.Log("AgentHitBox_LandedOnLand: IsCast" + ChronomancyIsCast(__instance.agent));

			if (ChronomancyIsCast(__instance.agent) && __instance.agent.stomping)
			{
				//__instance.agent.stomping = true; // possibly cause of always-stomp during chronomancy
				__instance.agent.statusEffects.Stomp();
				__instance.agent.stomping = false;
			}
		}
		#endregion
		#region Bullet
		public static void Bullet_BulletHitEffect(GameObject hitObject, Bullet __instance) // Postfix
		{
			if (__instance.cameFromWeapon == "ChainLightning" && __instance.agent.inventory.equippedSpecialAbility.invItemName == "Electromancy")
				ElectromancyImpact(hitObject, __instance);
		}
		#endregion
		#region Explosion
		public static bool Explosion_SetupExplosion(Explosion __instance) // Prefix
		{
			// see Explosion.immediateHit if these aren't doing damage.
			// Appears safe to leave it as always false. That's good luck, since the rest of this algorithm will assume it.
			// However, there's an "else" that doesn't seem reachable since I don't see any cases where immediateHit is null.

			if (__instance.explosionType == "HammerTime")
			{
				BunnyHeader.Log("Explosion_SetupExplosion detected HammerTime trait for Stomp");

				__instance.explosionType = "Stomp"; // Hopefully will enable damage
				// Okay, there's another trait: StompDamagesAgents, aka "Aftershocked". Enable that.
				__instance.gc.playerAgent.objectMult.SpawnExplosion(__instance);

				// ...

				__instance.StartCoroutine(__instance.SpawnNoiseLate());
				__instance.StartCoroutine(__instance.PlaySoundAfterTick());

				__instance.circleCollider2D.enabled = true;
				__instance.circleCollider2D.radius = 2.6f;

				if (__instance.agent != null && (__instance.agent.statusEffects.hasTrait("Fatass")))
					__instance.circleCollider2D.radius += 1.0f;
				if (__instance.agent != null && __instance.agent.statusEffects.hasTrait("HammerTime_2"))
					__instance.circleCollider2D.radius += 1.0f;

				__instance.gc.spawnerMain.SpawnParticleEffect("ExplosionStomp", __instance.tr.position, 0f).transform.localScale = new Vector3(2f, 2f, 2f);
				__instance.gc.FreezeFrames(1);

				return false;
			}
			else if (__instance.explosionType == "Stomp" && __instance.agent.statusEffects.hasTrait("Fatass"))
			{
				BunnyHeader.Log("Explosion_SetupExplosion detected Fatass trait for Stomp");

				__instance.gc.playerAgent.objectMult.SpawnExplosion(__instance);

				// ...

				__instance.StartCoroutine(__instance.SpawnNoiseLate());
				__instance.StartCoroutine(__instance.PlaySoundAfterTick());

				__instance.circleCollider2D.enabled = true;

				if (__instance.agent != null && (__instance.agent.statusEffects.hasTrait("BiggerStompRadius") || (__instance.agent.oma.superSpecialAbility && __instance.agent.agentName == "Bouncer")))
					__instance.circleCollider2D.radius = 4.4f;
				else
					__instance.circleCollider2D.radius = 3.6f;

				__instance.gc.spawnerMain.SpawnParticleEffect("ExplosionStomp", __instance.tr.position, 0f).transform.localScale = new Vector3(2f, 2f, 2f);
				__instance.gc.FreezeFrames(1);

				return false;
			}
			return true;
		}
		#endregion
		#region LoadLevel
		public static void LoadLevel_SetupMore5_2(LoadLevel __instance) // Postfix
		{
			baseTimeScale = GameController.gameController.selectedTimeScale;
		}
		#endregion
		#region StatusEffects
		public static void StatusEffects_AddStatusEffectSpecial(string effect, Agent damagerAgent, Agent causingAgent, bool shooting, StatusEffects __instance, ref string __result) // Postfix
		{
			// TODO
		}
		public static void StatusEffects_GiveSpecialAbility(string abilityName, StatusEffects __instance) // Postfix
		{
			if (abilityName == "Chronomancy" || abilityName == "Electromancy" || abilityName == "Pyromancy")
				__instance.agent.inventory.equippedSpecialAbility.otherDamage = 0;
		}
		public static bool StatusEffects_Stomp(StatusEffects __instance) // Replacement
		{
			__instance.agent.gc.spawnerMain.SpawnExplosion(__instance.agent, __instance.agent.tr.position, "Stomp");

			float num = 8f;

			if (__instance.agent.gc.challenges.Contains("LowHealth"))
				num = 4f;

			if (__instance.hasTrait("StompLessDamage") || __instance.hasTrait("HammerTime") || __instance.hasTrait("HammerTime_2") || (__instance.agent.agentName == "Bouncer" && __instance.agent.oma.superSpecialAbility))
				num *= 0.5f;

			if (__instance.hasTrait("HammerTime") || __instance.hasTrait("HammerTime_2"))
				num *= 0.0f;

			if (__instance.agent.health <= num)
				num = __instance.agent.health - 1f;

			__instance.agent.statusEffects.ChangeHealth(-num);

			MethodInfo stomp2 = AccessTools.DeclaredMethod(typeof(StatusEffects), "Stomp2");
			IEnumerator enumerator = (IEnumerator)stomp2.Invoke(__instance, new object[0]);
			__instance.StartCoroutine(enumerator);

			return false;
		}
		#endregion
	}
}
