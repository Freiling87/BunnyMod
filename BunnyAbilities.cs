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

			BunnyHeader.MainInstance.PatchPrefix(typeof(AgentHitbox), "LandedOnLand", GetType(), "AgentHitbox_LandedOnLand", new Type[0] { });

			BunnyHeader.MainInstance.PatchPostfix(typeof(Bullet), "BulletHitEffect", GetType(), "Bullet_BulletHitEffect", new Type[1] { typeof(GameObject) });

			BunnyHeader.MainInstance.PatchPrefix(typeof(Explosion), "SetupExplosion", GetType(), "Explosion_SetupExplosion", new Type[0] { });

			BunnyHeader.MainInstance.PatchPostfix(typeof(LoadLevel), "SetupMore5_2", GetType(), "LoadLevel_SetupMore5_2", new Type[0] { });

			BunnyHeader.MainInstance.PatchPostfix(typeof(StatusEffects), "AddStatusEffectSpecial", GetType(), "StatusEffects_AddStatusEffectSpecial", new Type[4] { typeof(String), typeof(Agent), typeof(Agent), typeof(bool) });
			BunnyHeader.MainInstance.PatchPostfix(typeof(StatusEffects), "GiveSpecialAbility", GetType(), "StatusEffects_GiveSpecialAbility", new Type[1] { typeof(String) });
			BunnyHeader.MainInstance.PatchPrefix(typeof(StatusEffects), "Stomp", GetType(), "StatusEffects_Stomp", new Type[0] { });
		}
		public static void InitializeAbilities()
		{
			Chronomancy_Initialize();
			//Cryomancy_Initialize();
			//Electromancy_Initialize();
			Pyromancy_Initialize();
			Telemancy_Initialize();
		}
		#endregion

		#region Magic General
		public static Vector2 MouseIngamePosition()
		{
			Plane plane = new Plane(new Vector3(0, 0, 1), new Vector3(0, 0, 0));
			Ray ray = Camera.main.ScreenPointToRay(UnityEngine.Input.mousePosition);
			return plane.Raycast(ray, out float enter) ? (Vector2)ray.GetPoint(enter) : default;
		}
		#endregion
		#region Chronomancy
		public static float baseTimeScale;
		public static void Chronomancy_Initialize()
		{
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
					ChronomancyStartDecast(agent);
				else if (ChronomancyRollMiscast(agent, (float)(100 - item.invItemCount) / 100))
					ChronomancyStartMiscast(agent, ChronomancyRollTimescale(agent, true));
				else
					ChronomancyStartCast(agent, ChronomancyRollTimescale(agent, false));
			};

			chronomancy.Recharge = (item, agent) =>
			{
				if (ChronomancyIsCast(agent))
				{
					item.invItemCount -= ChronomancyRollManaCost(agent);

					if (item.invItemCount < 0)
						ChronomancyStartMiscast(agent, ChronomancyRollTimescale(agent, true));
				}
				else if (item.invItemCount < 100 && agent.statusEffects.CanRecharge())
				{
					item.invItemCount += Math.Min(100 - item.invItemCount, 5);

					if (item.invItemCount == 100)
						ChronomancyStartRecharge(agent);
				}
				else if (item.invItemCount == 100 && ChronomancyIsMiscast(agent) || ChronomancyIsWindingUp(agent))
					ChronomancyStartRecharge(agent);
			};
			chronomancy.RechargeInterval = (item, myAgent) =>
				item.invItemCount > 0 ? new WaitForSeconds(1f) : null;
		}
		public static void ChronomancyDialogueCantDo(Agent agent)
		{
			agent.gc.audioHandler.Play(agent, "CantDo");

			string[] dialogue = new string[] {
				"I need to take a \"time out!\" Get it? But seriously, my heart will stop.",
				"I'm gonna take the blue pill for a sec."
			};

			agent.Say(dialogue[UnityEngine.Random.Range(0, dialogue.Count() - 1)]);
		}
		public static void ChronomancyDialogueCast(Agent agent) // Not yet implemented
		{
			string[] dialogue = new string[] {
				""
			};

			agent.Say(dialogue[UnityEngine.Random.Range(0, dialogue.Count() - 1)]);
		}
		public static void ChronomancyDialogueMiscast(Agent agent)
		{
			string[] dialogue = new string[] {
				"Iii ttthhhiiinnnkkk Iii mmmeeesssssseeeddd uuuppp...",
				"Bullet Time? More like Bullshit Time!",
				"(Slow Motion Noises)",
				"Okay, maybe there is a spoon, I guess. Whatever."
			};

			agent.Say(dialogue[UnityEngine.Random.Range(0, dialogue.Count() - 1)]);
		}
		public static void ChronomancyDialogueRecharge(Agent agent)
		{
			string[] dialogue = new string[] {
				"It's Slowing-down-time... Time!"
			};

			agent.Say(dialogue[UnityEngine.Random.Range(0, dialogue.Count() - 1)]);
		}
		public static bool ChronomancyIsCast(Agent agent) =>
			(agent.inventory.equippedSpecialAbility.otherDamage & 0b_0001) != 0;
		public static bool ChronomancyIsMiscast(Agent agent) =>
			(agent.inventory.equippedSpecialAbility.otherDamage & 0b_0010) != 0;
		public static bool ChronomancyIsWindingUp(Agent agent) =>
			(agent.inventory.equippedSpecialAbility.otherDamage & 0b_0100) != 0;
		public static int ChronomancyRollManaCost(Agent agent)
		{
			int increment = 10;

			if (agent.statusEffects.hasTrait("WildCasting"))
				increment += UnityEngine.Random.Range(-2, 2);
			else if (agent.statusEffects.hasTrait("WildCasting_2"))
				increment = UnityEngine.Random.Range(-5, 5);

			if (agent.statusEffects.hasTrait("FocusedCasting"))
				increment -= 2;
			else if (agent.statusEffects.hasTrait("FocusedCasting_2"))
				increment -= 4;

			return increment;
		}
		public static bool ChronomancyRollMiscast(Agent agent, float modifier)
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
		public static float ChronomancyRollTimescale(Agent agent, bool MisCast)
		{
			float timescale = 0.00f;

			if (!MisCast)
			{
				timescale = 2.00f;

				if (agent.statusEffects.hasTrait("FocusedCasting"))
					timescale += 0.25f;
				else if (agent.statusEffects.hasTrait("FocusedCasting_2"))
					timescale += 0.50f;
				else if (agent.statusEffects.hasTrait("WildCasting"))
					timescale += 0.75f;
				else if (agent.statusEffects.hasTrait("WildCasting_2"))
					timescale += 1.50f;

				if (agent.statusEffects.hasTrait("MagicTraining"))
					timescale += 0.5f;
				else if (agent.statusEffects.hasTrait("MagicTraining_2"))
					timescale += 1.0f;
			}
			else if (MisCast)
			{
				timescale = 4.00f;

				if (agent.statusEffects.hasTrait("FocusedCasting"))
					timescale -= 0.75f;
				else if (agent.statusEffects.hasTrait("FocusedCasting_2"))
					timescale -= 1.50f;
				else if (agent.statusEffects.hasTrait("WildCasting"))
					timescale += 0.50f;
				else if (agent.statusEffects.hasTrait("WildCasting_2"))
					timescale += 1.00f;

				if (agent.statusEffects.hasTrait("MagicTraining"))
					timescale -= 0.25f;
				else if (agent.statusEffects.hasTrait("MagicTraining_2"))
					timescale -= 0.50f;
			}

			return timescale;
		}
		public static void ChronomancySetCast(Agent agent, bool value)
		{
			//BunnyHeader.Log("SetCast " + value);

			if (value) agent.inventory.equippedSpecialAbility.otherDamage |= 0b_0001;
			else agent.inventory.equippedSpecialAbility.otherDamage &= ~0b_0001;
		}
		public static void ChronomancySetMiscast(Agent agent, bool value)
		{
			//BunnyHeader.Log("SetMiscast " + value);

			if (value) agent.inventory.equippedSpecialAbility.otherDamage |= 0b_0010;
			else agent.inventory.equippedSpecialAbility.otherDamage &= ~0b_0010;
		}
		public static void ChronomancySetWindingUp(Agent agent, bool value)
		{
			//BunnyHeader.Log("SetWindingUp" + value);

			if (value) agent.inventory.equippedSpecialAbility.otherDamage |= 0b_0100;
			else agent.inventory.equippedSpecialAbility.otherDamage &= ~0b_0100;
		}
		public static void ChronomancyStartCast(Agent agent, float speedupfactor)
		{
			agent.SpawnParticleEffect("ExplosionMindControl", agent.curPosition);
			GameController.gameController.audioHandler.Play(agent, "UseNecronomicon");

			ChronomancySetCast(agent, true);

			agent.gc.selectedTimeScale /= speedupfactor;
			agent.gc.mainTimeScale /= speedupfactor;
			agent.speedMax = agent.FindSpeed() * (int)speedupfactor;
		}
		public static async void ChronomancyStartDecast(Agent agent)
		{
			GameController.gameController.audioHandler.Play(agent, "UseNecronomicon");

			agent.speedMax = agent.FindSpeed();

			ChronomancySetCast(agent, false); // Needs to occur before delays or Overcast occurs erroneously

			if (!agent.underWater && !agent.jumped && !agent.melee.attackAnimPlaying && agent.statusEffects.hasTrait("HammerTime"))
			{
				agent.stomping = true;
				agent.Jump();

				agent.gc.selectedTimeScale /= 3f;
				agent.gc.mainTimeScale /= 3f;

				await Task.Delay((int)(500 / agent.gc.mainTimeScale)); // May need to do a base amount divided by timescale, but first attempt didn't 
			}

			agent.gc.selectedTimeScale = baseTimeScale;
			agent.gc.mainTimeScale = baseTimeScale;

			await Task.Delay(1000);

			await ChronomancyStartWindingUp(agent);
		}
		public static void ChronomancyStartMiscast(Agent agent, float slowdownFactor)
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
		public static void ChronomancyStartRecharge(Agent agent)
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

			ChronomancyDialogueRecharge(agent);
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
			
			ChronomancyStartRecharge(agent);
		}
		#endregion
		#region Cryomancy
		public static void Cryomancy_Initialize()
		{
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

				if (CryomancyRollMiscast(agent, 0))
				{
					CryomancyStartMiscast(agent, 20);
					icedOut = true;
				}
				else
				{
					CryomancyStartCast(agent);
					item.invItemCount -= CryomancyRollManaCost(agent);
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
		}
		public static void CryomancyDialogueCast(Agent agent)
		{

		}
		public static void CryomancyDialogueCantDo(Agent agent)
		{

		}
		public static void CryomancyDialogueMiscast(Agent agent)
		{

		}
		public static void CryomancyDialogueRecharge(Agent agent)
		{

		}
		public static void CryomancyStartCast(Agent agent)
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
		public static void CryomancyStartMiscast(Agent agent, int degree)
		{
			agent.statusEffects.AddStatusEffect("Frozen", degree);
		}
		public static int CryomancyRollManaCost(Agent agent)
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
		public static bool CryomancyRollMiscast(Agent agent, int modifier)
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
		public static void Electromancy_Initialize()
		{
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
				else if (ElectromancyRollMiscast(agent, 0))
					ElectromancyStartMiscast(agent, 20);
				else
				{
					ElectromancyStartCast(agent);
					item.invItemCount -= ElectromancyRollManaCost(agent);
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
		}
		public static void ElectromancyDialogueCast(Agent agent)
		{

		}
		public static void ElectromancyDialogueCantDo(Agent agent)
		{
			agent.gc.audioHandler.Play(agent, "CantDo");

			string[] dialogue = new string[] {
				"Ion wanna do that right now!" ,
				"Let me ground myself for a second.",
				"Watt just happened??"
			};

			agent.Say(dialogue[UnityEngine.Random.Range(0, dialogue.Count() - 1)]);
		}
		public static void ElectromancyDialogueMiscast(Agent agent)
		{
			agent.gc.audioHandler.Play(agent, "CantDo");

			string[] dialogue = new string[] {
				"I'm not ex-static about this." ,
				"This kinda hertz!",
				"Watt just happened??"
			};

			agent.Say(dialogue[UnityEngine.Random.Range(0, dialogue.Count() - 1)]);
		}
		public static void ElectromancyDialogueRecharge(Agent agent)
		{
			agent.gc.audioHandler.Play(agent, "Recharge");

			string[] dialogue = new string[] {
				"Resistance is futile!" ,
				"Don't forget to... *keep current*. Hah."
			};

			agent.Say(dialogue[UnityEngine.Random.Range(0, dialogue.Count() - 1)]);
		}
		public static bool ElectromancyIs_VARIABLE1(Agent agent) =>
			(agent.inventory.equippedSpecialAbility.otherDamage & 0b_0001) != 0;
		public static bool ElectromancyIs_VARIABLE2(Agent agent) =>
			(agent.inventory.equippedSpecialAbility.otherDamage & 0b_0010) != 0;
		public static bool ElectromancyIs_VARIABLE3(Agent agent) =>
			(agent.inventory.equippedSpecialAbility.otherDamage & 0b_0100) != 0;
		public static bool ElectromancyIs_VARIABLE4(Agent agent) =>
			(agent.inventory.equippedSpecialAbility.otherDamage & 0b_1000) != 0;
		public static void ElectromancyOnImpact(GameObject hitObject, Bullet bullet)
		{
			int numberOfChains = 1;

			for (int i = 0; i <= numberOfChains; i++)
			{
				Bullet newBullet = bullet.agent.gc.spawnerMain.SpawnBullet(hitObject.transform.position, bulletStatus.Taser, bullet.agent);
				newBullet.speed *= 3 / 2;
				newBullet.agent = bullet.agent;

				if (ElectromancyRollRebound(bullet.agent, 0))
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
		public static int ElectromancyRollManaCost(Agent agent)
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
		public static bool ElectromancyRollMiscast(Agent agent, int modifier)
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
		public static bool ElectromancyRollRebound(Agent agent, int modifier)
		{
			int chance = 50 + modifier;

			return (chance > UnityEngine.Random.Range(1, 100));
		}
		public static void ElectromancySet_VARIABLE1(Agent agent, bool value)
		{
			//BunnyHeader.Log("Set___" + value);

			if (value) agent.inventory.equippedSpecialAbility.otherDamage |= 0b_0001;
			else agent.inventory.equippedSpecialAbility.otherDamage &= ~0b_0001;
		}
		public static void ElectromancySet_VARIABLE2(Agent agent, bool value)
		{
			//BunnyHeader.Log("Set___" + value);

			if (value) agent.inventory.equippedSpecialAbility.otherDamage |= 0b_0010;
			else agent.inventory.equippedSpecialAbility.otherDamage &= ~0b_0010;
		}
		public static void ElectromancySet_VARIABLE3(Agent agent, bool value)
		{
			//BunnyHeader.Log("Set___" + value);

			if (value) agent.inventory.equippedSpecialAbility.otherDamage |= 0b_0100;
			else agent.inventory.equippedSpecialAbility.otherDamage &= ~0b_0100;
		}
		public static void ElectromancySet_VARIABLE4(Agent agent, bool value)
		{
			//BunnyHeader.Log("Set___" + value);

			if (value) agent.inventory.equippedSpecialAbility.otherDamage |= 0b_1000;
			else agent.inventory.equippedSpecialAbility.otherDamage &= ~0b_1000;
		}
		public static void ElectromancyStartCast(Agent agent)
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
		public static void ElectromancyStartMiscast(Agent agent, int degree)
		{
			agent.statusEffects.AddStatusEffect("Electrocuted", degree);
		}
		public static void ElectromancyStartRecharge(Agent agent)
		{

		}
		#endregion
		#region Pyromancy
		public static void Pyromancy_Initialize()
		{
			Sprite spritePyromancy = RogueUtilities.ConvertToSprite(Properties.Resources.Pyromancy);

			CustomAbility pyromancy = RogueLibs.CreateCustomAbility("Pyromancy", spritePyromancy, true,
				new CustomNameInfo("Pyromancy"),
				new CustomNameInfo("You can throw fire from your hands. This tends to fix a lot of your problems, and create much worse ones."),
				delegate (InvItem item)
				{
					item.cantDrop = true;
					item.Categories.Add("Weapons"); // Might this set off sensors?
					item.Categories.Add("NPCsCantPickUp");
					item.dontAutomaticallySelect = true;
					item.dontSelectNPC = true;
					item.gunKnockback = 0;
					item.isWeapon = true;
					item.rapidFire = true;
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
				if (!PyromancyIsBurnedOut(agent) && !PyromancyIsCoolingDown(agent) && !PyromancyIsMiscast(agent))
				{
					if (PyromancyRollMiscast(agent, 0))
						PyromancyStartMiscast(agent, 20);
					else
					{
						PyromancyStartCast(agent);

						if (PyromancyRollManaCost(agent))
							item.invItemCount--;

						if (item.invItemCount <= 0)
							PyromancyStartBurnout(agent);
					}
				}
			};

			pyromancy.OnReleased = delegate (InvItem item, Agent agent)
			{
				if (!PyromancyIsBurnedOut(agent) && !PyromancyIsCoolingDown(agent) && !PyromancyIsMiscast(agent))
					PyromancyStartCoolingDown(agent);
			};

			pyromancy.Recharge = (item, myAgent) =>
			{
				if (item.invItemCount < item.rechargeAmountInverse && myAgent.statusEffects.CanRecharge())
				{
					item.invItemCount++;

					if (item.invItemCount == 100)
						PyromancyStartRecharge(myAgent);
				}
			};

			pyromancy.RechargeInterval = (item, myAgent) =>
				item.invItemCount > 0 ? new WaitForSeconds(0.1f) : null;
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
		public static void PyromancyDialogueCast(Agent agent) // Not used yet
		{
			agent.gc.audioHandler.Play(agent, "AgentLaugh");

			string[] dialogue = new string[] {
				"Die! Burn! Die! Die!",
				"Burn, baby, burn!"
			};

			agent.Say(dialogue[UnityEngine.Random.Range(0, dialogue.Count() - 1)]);
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
					agent.Say("I shidded an farded an bursteded into flames.");
					break;
			}
		}
		public static void PyromancyDialogueRecharge(Agent agent)
		{

		}
		public static bool PyromancyIsBurnedOut(Agent agent) =>
			(agent.inventory.equippedSpecialAbility.otherDamage & 0b_0001) != 0;
		public static bool PyromancyIsCoolingDown(Agent agent) =>
			(agent.inventory.equippedSpecialAbility.otherDamage & 0b_0010) != 0;
		public static bool PyromancyIsMiscast(Agent agent) =>
			(agent.inventory.equippedSpecialAbility.otherDamage & 0b_0100) != 0;
		public static bool PyromancyRollManaCost(Agent agent)
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
		public static bool PyromancyRollMiscast(Agent agent, float modifier)
		{
			float risk = 1.000f + modifier;

			if (agent.statusEffects.hasTrait("FocusedCasting"))
				risk -= 0.500f;
			else if (agent.statusEffects.hasTrait("FocusedCasting_2"))
				risk -= 0.750f;
			else if (agent.statusEffects.hasTrait("WildCasting"))
				risk += 0.250f;
			else if (agent.statusEffects.hasTrait("WildCasting_2"))
				risk += 0.500f;

			if (agent.statusEffects.hasTrait("MagicTraining"))
				risk *= 0.75f;
			else if (agent.statusEffects.hasTrait("MagicTraining_2"))
				risk *= 0.50f;

			return risk >= UnityEngine.Random.Range(0f, 100f);
		}
		public static void PyromancySetBurnedOut(Agent agent, bool value)
		{
			BunnyHeader.Log("PyromancySetBurnedOut " + value);

			if (value) agent.inventory.equippedSpecialAbility.otherDamage |= 0b_0001;
			else agent.inventory.equippedSpecialAbility.otherDamage &= ~0b_0001;
		}
		public static void PyromancySetCoolingDown(Agent agent, bool value)
		{
			BunnyHeader.Log("PyromancySetCoolingDown " + value);

			if (value) agent.inventory.equippedSpecialAbility.otherDamage |= 0b_0010;
			else agent.inventory.equippedSpecialAbility.otherDamage &= ~0b_0010;
		}
		public static void PyromancySetMiscast(Agent agent, bool value)
		{
			BunnyHeader.Log("PyromancySetBurnedOut " + value);

			if (value) agent.inventory.equippedSpecialAbility.otherDamage |= 0b_0100;
			else agent.inventory.equippedSpecialAbility.otherDamage &= ~0b_0100;
		}
		public static void PyromancyStartBurnout(Agent agent)
		{
			agent.gc.audioHandler.Play(agent, "MindControlEnd");

			PyromancySetBurnedOut(agent, true);
		}
		public static void PyromancyStartCast(Agent agent)
		{
			Bullet bullet = agent.gc.spawnerMain.SpawnBullet(agent.gun.tr.position, bulletStatus.Fire, agent);
			bullet.gc.audioHandler.Play(bullet, "fireConstant");

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
		public static async Task PyromancyStartCoolingDown(Agent agent)
		{
			PyromancySetCoolingDown(agent, true);

			float duration = 2000f;

			if (agent.statusEffects.hasTrait("WildCasting"))
				duration -= 625f;
			else if (agent.statusEffects.hasTrait("WildCasting_2"))
				duration -= 1250f;

			if (agent.statusEffects.hasTrait("MagicTraining"))
				duration -= 375f;
			else if (agent.statusEffects.hasTrait("MagicTraining_2"))
				duration -= 750f;

			duration = Mathf.Max(0f, duration);

			await Task.Delay((int)duration);

			PyromancySetCoolingDown(agent, false);
		}
		public static void PyromancyStartMiscast(Agent agent, int degree)
		{
			agent.gc.spawnerMain.SpawnExplosion(agent, agent.curPosition, "FireBomb");

			PyromancyDialogueMiscast(agent);

			PyromancyStartBurnout(agent);
		}
		public static void PyromancyStartRecharge(Agent agent) //TODO
		{
			agent.statusEffects.CreateBuffText("Recharged", agent.objectNetID);
			agent.gc.audioHandler.Play(agent, "Recharge");

			if (PyromancyIsBurnedOut(agent))
				PyromancySetBurnedOut(agent, false);

			if (PyromancyIsMiscast(agent))
				PyromancySetMiscast(agent, false);

			agent.inventory.buffDisplay.specialAbilitySlot.MakeUsable();
		}
		#endregion
		#region Telemancy
		public static void Telemancy_Initialize()
		{
			Sprite spriteTelemancy = RogueUtilities.ConvertToSprite(Properties.Resources.Telemancy);

			CustomAbility telemancy = RogueLibs.CreateCustomAbility("Telemancy", spriteTelemancy, true,
				new CustomNameInfo("Telemancy"),
				new CustomNameInfo("You can teleport sort of at will, but it's unpredictable and makes you feel sick. Maybe you can get better at this?"),
				delegate (InvItem item)
				{
					item.cantDrop = true;
					item.Categories.Add("NPCsCantPickup");
					item.dontAutomaticallySelect = true;
					item.dontSelectNPC = true;
					item.isWeapon = false;
					item.initCount = 100;
					item.rechargeAmountInverse = 100;
					item.maxAmmo = 100;
					item.stackable = true;
					item.thiefCantSteal = true;
				});

			telemancy.Available = true;
			telemancy.AvailableInCharacterCreation = true;
			telemancy.CostInCharacterCreation = 8;

			int telemancyCharge = 0;

			telemancy.OnPressed = delegate (InvItem item, Agent agent)
			{
				if (item.invItemCount <= 0 || TelemancyIsReturning(agent) || TelemancyIsMiscast(agent))
					TelemancyDialogueCantDo(agent);
			};

			telemancy.OnHeld = delegate (InvItem item, Agent agent, ref float time)
			{
				if (!TelemancyIsReturning(agent) && telemancyCharge < 100 && item.invItemCount >= 1)
				{
					int curCost = TelemancyRollManaCost(agent);

					if (TelemancyRollMiscast(agent, curCost))
						TelemancyStartMiscast(agent);
					else 
					{
						item.invItemCount -= Mathf.Min(item.invItemCount, curCost);
						telemancyCharge += curCost;
					}
				}
				
				if (telemancyCharge >= 100 || item.invItemCount <= 0)
					telemancy.OnReleased(item, agent);
			};

			telemancy.OnReleased = delegate (InvItem item, Agent agent)
			{
				TelemancyStartCast(agent, telemancyCharge);
			};

			telemancy.Recharge = (item, agent) =>
			{
				if (item.invItemCount < 100 && agent.statusEffects.CanRecharge())
				{
					item.invItemCount = Math.Min(100, item.invItemCount + TelemancyRollCharge(agent));

					if (item.invItemCount == 100)
						TelemancyStartRecharge(agent);
				}
			};

			telemancy.RechargeInterval = (item, myAgent) =>
				item.invItemCount > 0 ? new WaitForSeconds(0.1f) : null;
		}
		public static void TelemancyDialogueCantDo(Agent agent)
		{
			agent.gc.audioHandler.Play(agent, "CantDo");
			agent.Say("I need to give it a rest or my head will explode. I've seen it happen.");
		}
		public static void TelemancyDialogueCast(Agent agent)
		{

		}
		public static void TelemancyDialogueMiscast(Agent agent)
		{

		}
		public static void TelemancyDialogueRecharge(Agent agent)
		{

		}
		public static bool TelemancyIsReturning(Agent agent) =>
			(agent.inventory.equippedSpecialAbility.otherDamage & 0b_0001) != 0;
		public static bool TelemancyIsMiscast(Agent agent) =>
			(agent.inventory.equippedSpecialAbility.otherDamage & 0b_0010) != 0;
		public static bool TelemancyIs_VARIABLE3(Agent agent) =>
			(agent.inventory.equippedSpecialAbility.otherDamage & 0b_0100) != 0;
		public static bool TelemancyIs_VARIABLE4(Agent agent) =>
			(agent.inventory.equippedSpecialAbility.otherDamage & 0b_1000) != 0;
		public static int TelemancyRollManaCost(Agent agent)
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
		public static float[] TelemancyRollAccuracy(Agent agent)
		{
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

			return new float[] { rangeNear, rangeFar };
		}
		public static int TelemancyRollCharge(Agent agent)
		{
			int charge = 0;

			// Charge rate can be increased

			return charge;
		}
		public static Vector2 TelemancyRollDestination(Agent agent, bool accountForObstacles, bool notInside, bool dontCareAboutDanger, bool teleporting, bool accountForWalls)
		{
			TileInfo tileInfo = agent.gc.tileInfo;
			Vector2 currentPosition = agent.curPosition;
			Vector2 targetPosition = agent.curPosition;
			float[] range = TelemancyRollAccuracy(agent);

			for (int i = 0; i < 50; i++)
			{
				float distance = UnityEngine.Random.Range(range[0], range[1]);

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
		public static bool TelemancyRollMiscast(Agent agent, float modifier)
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
		public static void TelemancySetReturning(Agent agent, bool value)
		{
			//BunnyHeader.Log("Set___" + value);

			if (value) agent.inventory.equippedSpecialAbility.otherDamage |= 0b_0001;
			else agent.inventory.equippedSpecialAbility.otherDamage &= ~0b_0001;
		}
		public static void TelemancySetMiscast(Agent agent, bool value)
		{
			//BunnyHeader.Log("Set___" + value);

			if (value) agent.inventory.equippedSpecialAbility.otherDamage |= 0b_0010;
			else agent.inventory.equippedSpecialAbility.otherDamage &= ~0b_0010;
		}
		public static void TelemancySet_VARIABLE3(Agent agent, bool value)
		{
			//BunnyHeader.Log("Set___" + value);

			if (value) agent.inventory.equippedSpecialAbility.otherDamage |= 0b_0100;
			else agent.inventory.equippedSpecialAbility.otherDamage &= ~0b_0100;
		}
		public static void TelemancySet_VARIABLE4(Agent agent, bool value)
		{
			//BunnyHeader.Log("Set___" + value);

			if (value) agent.inventory.equippedSpecialAbility.otherDamage |= 0b_1000;
			else agent.inventory.equippedSpecialAbility.otherDamage &= ~0b_1000;
		}
		public static void TelemancyStartCast(Agent agent, float charge)
		{
			agent.SpawnParticleEffect("Spawn", agent.curPosition);

			Vector2 targetLocation = TelemancyRollDestination(agent, false, false, true, true, true);

			agent.Teleport(targetLocation, false, true);

			agent.rb.velocity = Vector2.zero;

			agent.SpawnParticleEffect("Spawn", agent.tr.position, false);
			GameController.gameController.audioHandler.Play(agent, "Spawn");
		}
		public static void TelemancyStartMiscast(Agent agent)
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
		public static void TelemancyStartRecharge(Agent agent)
		{
			agent.statusEffects.CreateBuffText("Recharged", agent.objectNetID);
			agent.gc.audioHandler.Play(agent, "Recharge");
			agent.inventory.buffDisplay.specialAbilitySlot.MakeUsable();
		}
		public static void TelemancyStartReturn(Agent agent)
		{

		}
		#endregion

		#region AgentHitbox
		public static bool AgentHitbox_LandedOnLand(AgentHitbox __instance) // Prefix
		{
			Agent agent = __instance.agent;

			if (ChronomancyIsCast(agent) && agent.stomping)
			{
				if (agent.isPlayer > 0 && agent.localPlayer)
					__instance.gc.playerControl.SetCantPressGameplayButtons("Jump", 0, agent.isPlayer - 1);

				if (!__instance.gc.serverPlayer && agent.localPlayer)
					agent.objectMult.CallCmdLandOnLand();

				agent.pathfindingAI.rePath = true;
				agent.jumped = false;
				agent.justGotUp = true;
				agent.agentSpriteAnimator.Play("AgentSpriteNoAnim");

				MethodInfo setAgentSpritePosLate = AccessTools.DeclaredMethod(typeof(AgentHitbox), "SetAgentSpritePosLate");
				IEnumerator setAgentSpritePosLate_Enumerator = (IEnumerator)setAgentSpritePosLate.Invoke(__instance, new object[0]);
				__instance.StartCoroutine(setAgentSpritePosLate_Enumerator);

				agent.justLanded = true;
				agent.clientRecentlyLandedOnLand = false;
				agent.clientRecentlyLandedOnWater = false;

				MethodInfo justLandedFrame = AccessTools.DeclaredMethod(typeof(AgentHitbox), "JustLandedFrame");
				IEnumerator justLandedFrame_Enumerator = (IEnumerator)justLandedFrame.Invoke(__instance, new object[0]);
				__instance.StartCoroutine(justLandedFrame_Enumerator);

				agent.SetOverHole(false);
				agent.movement.SetPhysics(agent.movement.curPhysicsType);

				if (agent.localPlayer)
					agent.statusEffects.Stomp();

				agent.stomping = false;

				return false;
			}
			return true;
		}
		#endregion
		#region Bullet
		public static void Bullet_BulletHitEffect(GameObject hitObject, Bullet __instance) // Postfix
		{
			if (__instance.cameFromWeapon == "ChainLightning" && __instance.agent.inventory.equippedSpecialAbility.invItemName == "Electromancy")
				ElectromancyOnImpact(hitObject, __instance);
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

				//__instance.explosionType = "Stomp"; // Hopefully will enable damage... but on test it no longer stuns enemies??
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
			if (abilityName == "Chronomancy" || abilityName == "Electromancy" || abilityName == "Pyromancy" && __instance.agent.inventory.equippedSpecialAbility != null)
				__instance.agent.inventory.equippedSpecialAbility.otherDamage = 0;

			// TODO: If you need a Remora, put it here.
		}
		public static bool StatusEffects_Stomp(StatusEffects __instance) // Replacement
		{
			__instance.agent.gc.spawnerMain.SpawnExplosion(__instance.agent, __instance.agent.tr.position, "Stomp");

			float num = 8f;

			if (__instance.agent.gc.challenges.Contains("LowHealth"))
				num = 4f;

			if (__instance.hasTrait("StompLessDamage") || __instance.hasTrait("HammerTime") || (__instance.agent.agentName == "Bouncer" && __instance.agent.oma.superSpecialAbility))
				num *= 0.5f;

			if (__instance.hasTrait("HammerTime"))
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
