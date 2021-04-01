using RogueLibsCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

namespace BunnyMod
{
	public class BunnyAbilities
	{
		public void Awake()
		{
			InitializeAbilities();

			BunnyHeader.MainInstance.PatchPostfix(typeof(Agent), "FindOperatingTime", GetType(), "Agent_FindOperatingTime", new Type[2] { typeof(PlayfieldObject), typeof(float) });

			BunnyHeader.MainInstance.PatchPostfix(typeof(Bullet), "BulletHitEffect", GetType(), "Bullet_BulletHitEffect", new Type[1] { typeof(GameObject) });

			BunnyHeader.MainInstance.PatchPostfix(typeof(LoadLevel), "SetupMore5_2", GetType(), "LoadLevel_SetupMore5_2", new Type[0] { });

			BunnyHeader.MainInstance.PatchPostfix(typeof(StatusEffects), "GiveSpecialAbility", GetType(), "StatusEffects_GiveSpecialAbility", new Type[1] { typeof(String) });
		}
		public static void InitializeAbilities()
		{
			Chronomancy_Initialize();
			//Cryomancy_Initialize();
			//Electromancy_Initialize();
			Pyromancy_Initialize();
			Telemancy_Initialize();
		}

		#region Magic General
		public static int CalcMaxMana(Agent agent)
		{
			if (agent.statusEffects.hasTrait("ManaBattery"))
				return 150;
			else if (agent.statusEffects.hasTrait("ManaBattery_2"))
				return 200;
			else if (agent.statusEffects.hasTrait("Archmage"))
				return 10000;
			else
				return 100;
		}
		public static Vector2 MouseIngamePosition()
		{
			Plane plane = new Plane(new Vector3(0, 0, 1), new Vector3(0, 0, 0));
			Ray ray = Camera.main.ScreenPointToRay(UnityEngine.Input.mousePosition);
			return plane.Raycast(ray, out float enter) ? (Vector2)ray.GetPoint(enter) : default;
		}
		#endregion
		#region Chronomancy
		#region Chronomancy - Bits
		public static bool ChronomancyIsCast(Agent agent)
		{
			try
			{
				return (agent.inventory.equippedSpecialAbility.otherDamage & 0b_0001) != 0;
			}
			catch
			{
				return false;
			}
		}
		public static bool ChronomancyIsMiscast(Agent agent) =>
			(agent.inventory.equippedSpecialAbility.otherDamage & 0b_0010) != 0;
		public static bool ChronomancyIsWindingUp(Agent agent) =>
			(agent.inventory.equippedSpecialAbility.otherDamage & 0b_0100) != 0;
		public static void ChronomancyLogVariables(Agent agent)
		{
			BunnyHeader.Log("ChronomancyIsCast: " + ChronomancyIsCast(agent));
			BunnyHeader.Log("ChronomancyIsMiscast: " + ChronomancyIsMiscast(agent));
			BunnyHeader.Log("ChronomancyIsWindindUp: " + ChronomancyIsWindingUp(agent));
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
		#endregion
		#region Chronomancy - Dialogue
		public static void ChronomancyDialogueCantDo(Agent agent)
		{
			agent.gc.audioHandler.Play(agent, "CantDo");

			string[] dialogue =
			{
				"I need to take a \"time out!\" Get it? But seriously, my heart will stop.",
				"I'm gonna take the blue pill for a sec."
			};

			agent.Say(dialogue[UnityEngine.Random.Range(0, dialogue.Count() - 1)]);
		}
		public static void ChronomancyDialogueCast(Agent agent) // TODO
		{
			string[] dialogue =
			{
				""
			};

			agent.Say(dialogue[UnityEngine.Random.Range(0, dialogue.Count() - 1)]);
		}
		public static void ChronomancyDialogueMiscast(Agent agent)
		{
			string[] dialogue =
			{
				"Iii ttthhhiiinnnkkk Iii mmmeeesssssseeeddd uuuppp...",
				"Bullet Time? More like Bullshit Time!",
				"(Slow Motion Noises)",
				"Okay, maybe there is a spoon, I guess. Whatever."
			};

			agent.Say(dialogue[UnityEngine.Random.Range(0, dialogue.Count() - 1)]);
		}
		public static void ChronomancyDialogueRecharge(Agent agent)
		{
			string[] dialogue =
			{
				"It's Slowing-down-time... Time!"
			};

			agent.Say(dialogue[UnityEngine.Random.Range(0, dialogue.Count() - 1)]);
		}
		#endregion
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
				else if (ChronomancyRollMiscast(agent, (float)((CalcMaxMana(agent) - item.invItemCount) / 100f)))
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
				else if (item.invItemCount < CalcMaxMana(agent) && agent.statusEffects.CanRecharge())
				{
					item.invItemCount += Math.Min(CalcMaxMana(agent) - item.invItemCount, 5);

					if (item.invItemCount == CalcMaxMana(agent) && !ChronomancyIsMiscast(agent))
						ChronomancyStartRecharge(agent, true);
				}
			};
			chronomancy.RechargeInterval = (item, myAgent) =>
				item.invItemCount > 0 ? new WaitForSeconds(1f) : null;
		}
		public static void ChronomancyDisplayTimescale(Agent agent)
		{

		}
		public static int ChronomancyRollManaCost(Agent agent)
		{
			int manaCost = 15;

			if (agent.statusEffects.hasTrait("FocusedCasting"))
				manaCost -= 2;
			else if (agent.statusEffects.hasTrait("FocusedCasting_2"))
				manaCost -= 4;
			else if (agent.statusEffects.hasTrait("WildCasting"))
				manaCost += UnityEngine.Random.Range(-2, 2);
			else if (agent.statusEffects.hasTrait("WildCasting_2"))
				manaCost = UnityEngine.Random.Range(-5, 5);
			else if (agent.statusEffects.hasTrait("Archmage"))
				manaCost = 0;

			return manaCost;
		}
		public static bool ChronomancyRollMiscast(Agent agent, float percentMod)
		{
			float risk = 1.0f + percentMod;

			if (agent.statusEffects.hasTrait("Archmage"))
				return false;
			else if (agent.statusEffects.hasTrait("FocusedCasting"))
				risk -= 0.25f;
			else if (agent.statusEffects.hasTrait("FocusedCasting_2"))
				risk -= 0.50f;
			else if (agent.statusEffects.hasTrait("WildCasting"))
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
			float timescale = 1.000f;

			if (agent.statusEffects.hasTrait("Archmage"))
				return 4.000f;

			if (!MisCast)
			{
				timescale = 2.000f;

				if (agent.statusEffects.hasTrait("FocusedCasting"))
					timescale += 0.250f;
				else if (agent.statusEffects.hasTrait("FocusedCasting_2"))
					timescale += 0.500f;
				else if (agent.statusEffects.hasTrait("WildCasting"))
					timescale += 0.750f;
				else if (agent.statusEffects.hasTrait("WildCasting_2"))
					timescale += 1.500f;

				if (agent.statusEffects.hasTrait("MagicTraining"))
					timescale += 0.500f;
				else if (agent.statusEffects.hasTrait("MagicTraining_2"))
					timescale += 1.000f;
			}
			else if (MisCast)
			{
				timescale = 0.250f;

				if (agent.statusEffects.hasTrait("FocusedCasting"))
					timescale += 0.375f;
				else if (agent.statusEffects.hasTrait("FocusedCasting_2"))
					timescale += 0.500f;
				else if (agent.statusEffects.hasTrait("WildCasting"))
					timescale -= 0.063f;
				else if (agent.statusEffects.hasTrait("WildCasting_2"))
					timescale -= 0.188f;

				if (agent.statusEffects.hasTrait("MagicTraining"))
					timescale += 0.125f;
				else if (agent.statusEffects.hasTrait("MagicTraining_2"))
					timescale += 0.250f;
			}

			BunnyHeader.Log("ChronomancyRollTimescale: " + timescale);

			return timescale;
		}
		public static void ChronomancyStartCast(Agent agent, float speedupfactor)
		{
			agent.SpawnParticleEffect("ExplosionMindControl", agent.curPosition);
			GameController.gameController.audioHandler.Play(agent, "MakeOffering");

			ChronomancySetCast(agent, true);

			agent.gc.selectedTimeScale = baseTimeScale / speedupfactor;
			agent.gc.mainTimeScale = baseTimeScale / speedupfactor;
			agent.speedMax = agent.FindSpeed() * (int)speedupfactor;

			BunnyHeader.Log("Timescale: " + agent.gc.mainTimeScale.ToString());
		}
		public static async void ChronomancyStartDecast(Agent agent)
		{
			GameController.gameController.audioHandler.Play(agent, "MakeOffering");

			agent.speedMax = agent.FindSpeed();

			ChronomancySetCast(agent, false); // Needs to occur before delays or Overcast occurs erroneously

			agent.gc.selectedTimeScale = baseTimeScale;
			agent.gc.mainTimeScale = baseTimeScale;

			await Task.Delay(1000);

			await ChronomancyStartWindingUp(agent);

			BunnyHeader.Log("Timescale: " + agent.gc.mainTimeScale.ToString());
		}
		public static async void ChronomancyStartMiscast(Agent agent, float slowdownFactor)
		{
			BunnyHeader.Log("ChronomancyStartMiscast: " + slowdownFactor);

			agent.SpawnParticleEffect("ExplosionEMP", agent.curPosition);
			agent.gc.audioHandler.Play(agent, "ToiletTeleportIn");
			ChronomancyDialogueMiscast(agent);

			if (ChronomancyIsCast(agent))
				ChronomancySetCast(agent, false);

			ChronomancySetMiscast(agent, true);

			ChronomancyStartWindingUp(agent); // TODO: Ensure that this duration is equal to miscast duration

			agent.gc.selectedTimeScale = baseTimeScale / slowdownFactor;
			agent.gc.mainTimeScale = baseTimeScale / slowdownFactor;
			agent.speedMax = (int)((float)agent.FindSpeed() * slowdownFactor);

			agent.inventory.buffDisplay.specialAbilitySlot.MakeNotUsable();

			BunnyHeader.Log("Timescale: " + agent.gc.mainTimeScale.ToString());

			await Task.Delay(5000);

			ChronomancySetMiscast(agent, false);
			ChronomancyStartDecast(agent);
		}
		public static void ChronomancyStartRecharge(Agent agent, bool routine)
		{
			BunnyHeader.Log("ChronomancyStartRecharge");

			if (!routine)
			{
				agent.statusEffects.CreateBuffText("Recharged", agent.objectNetID);
				agent.gc.audioHandler.Play(agent, "Recharge");

				ChronomancyDialogueRecharge(agent);
			}

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

			BunnyHeader.Log("Timescale: " + agent.gc.mainTimeScale.ToString());
		}
		public static async Task ChronomancyStartWindingUp(Agent agent)
		{
			ChronomancySetWindingUp(agent, true);

			float duration = 4000f;

			if (agent.statusEffects.hasTrait("Archmage"))
				duration = 1000f;
			else if (agent.statusEffects.hasTrait("WildCasting"))
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
			
			ChronomancyStartRecharge(agent, true);
		}
		#endregion
		#region Cryomancy
		#region Cryomancy - Bits
		#endregion
		#region Cryomancy - Dialogue
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
		#endregion
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
					item.rechargeAmountInverse = 100;
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

			cryomancy.Recharge = (item, agent) =>
			{
				if (item.invItemCount < CalcMaxMana(agent) && agent.statusEffects.CanRecharge())
				{
					item.invItemCount++;

					if (item.invItemCount == CalcMaxMana(agent))
					{
						if (icedOut)
							icedOut = false;

						agent.statusEffects.CreateBuffText("Recharged", agent.objectNetID);
						agent.gc.audioHandler.Play(agent, "Recharge");
						agent.inventory.buffDisplay.specialAbilitySlot.MakeUsable();
					}
				}
			};

			cryomancy.RechargeInterval = (item, myAgent) =>
				item.invItemCount > 0 ? new WaitForSeconds(0.2f) : null;
		}
		public static void CryomancyStartCast(Agent agent)
		{
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
		#region Electromancy - Bits
		public static bool ElectromancyIs_VARIABLE1(Agent agent) =>
			(agent.inventory.equippedSpecialAbility.otherDamage & 0b_0001) != 0;
		public static bool ElectromancyIs_VARIABLE2(Agent agent) =>
			(agent.inventory.equippedSpecialAbility.otherDamage & 0b_0010) != 0;
		public static bool ElectromancyIs_VARIABLE3(Agent agent) =>
			(agent.inventory.equippedSpecialAbility.otherDamage & 0b_0100) != 0;
		public static bool ElectromancyIs_VARIABLE4(Agent agent) =>
			(agent.inventory.equippedSpecialAbility.otherDamage & 0b_1000) != 0;
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
		#endregion
		#region Electromancy - Dialogue
		public static void ElectromancyDialogueCast(Agent agent)
		{
			string[] dialogue =
			{
				"UNLIMITED... POWER!!!"
			};

			agent.Say(dialogue[UnityEngine.Random.Range(0, dialogue.Count() - 1)]);
		}
		public static void ElectromancyDialogueCantDo(Agent agent)
		{
			agent.gc.audioHandler.Play(agent, "CantDo");

			string[] dialogue =
			{
				"Ion wanna do that right now!" ,
				"Let me ground myself for a second."
			};

			agent.Say(dialogue[UnityEngine.Random.Range(0, dialogue.Count() - 1)]);
		}
		public static void ElectromancyDialogueMiscast(Agent agent)
		{
			agent.gc.audioHandler.Play(agent, "CantDo");

			string[] dialogue =
			{
				"I'm not ex-static about this." ,
				"This kinda hertz!",
				"Watt just happened??"
			};

			agent.Say(dialogue[UnityEngine.Random.Range(0, dialogue.Count() - 1)]);
		}
		public static void ElectromancyDialogueRecharge(Agent agent)
		{
			agent.gc.audioHandler.Play(agent, "Recharge");

			string[] dialogue =
			{
				"Resistance is futile!" ,
				"Don't forget to... *keep current*. Hah."
			};

			agent.Say(dialogue[UnityEngine.Random.Range(0, dialogue.Count() - 1)]);
		}
		#endregion
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
					item.rechargeAmountInverse = 100;
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
			electromancy.Recharge = (item, agent) =>
			{
				if (item.invItemCount < CalcMaxMana(agent) && agent.statusEffects.CanRecharge())
				{
					item.invItemCount++;

					if (item.invItemCount == CalcMaxMana(agent))
					{
						if (zappedOut)
							zappedOut = false;

						agent.statusEffects.CreateBuffText("Recharged", agent.objectNetID);
						agent.gc.audioHandler.Play(agent, "Recharge");
						agent.inventory.buffDisplay.specialAbilitySlot.MakeUsable();
					}
				}
			};

			electromancy.RechargeInterval = (item, myAgent) =>
				item.invItemCount > 0 ? new WaitForSeconds(0.2f) : null;
		}
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
		public static void ElectromancyStartCast(Agent agent)
		{
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
		public static void ElectromancyStartRecharge(Agent agent, bool routine)
		{

		}
		#endregion
		#region Pyromancy
		#region Pyromancy - Bits
		public static bool PyromancyIsBurnedOut(Agent agent)
		{
			BunnyHeader.Log("PyromancyIsBurnedOut: " + ((agent.inventory.equippedSpecialAbility.otherDamage & 0b_0001) != 0));

			return (agent.inventory.equippedSpecialAbility.otherDamage & 0b_0001) != 0;
		}
		public static bool PyromancyIsCoolingDown(Agent agent)
		{
			BunnyHeader.Log("PyromancyIsCoolingDown: " + ((agent.inventory.equippedSpecialAbility.otherDamage & 0b_0010) != 0));

			return (agent.inventory.equippedSpecialAbility.otherDamage & 0b_0010) != 0;
		}
		public static bool PyromancyIsMiscast(Agent agent)
		{
			BunnyHeader.Log("PyromancyIsMiscast: " + ((agent.inventory.equippedSpecialAbility.otherDamage & 0b_0100) != 0));

			return (agent.inventory.equippedSpecialAbility.otherDamage & 0b_0100) != 0;
		}
		public static void PyromancyLogBooleans(Agent agent)
		{
			BunnyHeader.Log("PyromancyIsBurnedOut: " + PyromancyIsBurnedOut(agent));
			BunnyHeader.Log("PyromancyIsCoolingDown: " + PyromancyIsCoolingDown(agent));
			BunnyHeader.Log("PyromancyIsMiscast: " + PyromancyIsMiscast(agent));
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
			BunnyHeader.Log("PyromancySetMiscast " + value);

			if (value) agent.inventory.equippedSpecialAbility.otherDamage |= 0b_0100;
			else agent.inventory.equippedSpecialAbility.otherDamage &= ~0b_0100;
		}
		#endregion
		#region Pyromancy - Dialogue
		public static void PyromancyDialogueCantDo(Agent agent)
		{
			agent.gc.audioHandler.Play(agent, "CantDo");

			string[] dialogue =
			{
				"I'm burned out.",
				"Hang on. I don't feel like exploding right now."
			};

			agent.Say(dialogue[UnityEngine.Random.Range(0, dialogue.Count() - 1)]);
		}
		public static void PyromancyDialogueCast(Agent agent) // Not used yet
		{
			agent.gc.audioHandler.Play(agent, "AgentLaugh");

			string[] dialogue =
			{
				"Die! Burn! Die! Die!",
				"Burn, baby, burn!",
				"BURN-ie 2024!",
				"Yer fired! Get it?"
			};

			agent.Say(dialogue[UnityEngine.Random.Range(0, dialogue.Count() - 1)]);
		}
		public static void PyromancyDialogueMiscast(Agent agent)
		{
			string[] dialogue =
			{
				"Not very stoked right now.",
				"Haha my skin is melting lol XDDD",
				"Flame off! Flame off!",
				"I shidded an farded an bursteded into flames."
			};

			agent.Say(dialogue[UnityEngine.Random.Range(0, dialogue.Count() - 1)]);
		}
		public static void PyromancyDialogueRecharge(Agent agent)
		{
			string[] dialogue =
			{
				"Ready to burn!",
				"I'm here to burn things and chew bubblegum. I'm not out of gum, but I'm still gonna do both.",
				"(Laughs maniacally)",
				"Why are the innocent so fun to burn?"
			};

			agent.Say(dialogue[UnityEngine.Random.Range(0, dialogue.Count() - 1)]);
		}
		#endregion
		public static void Pyromancy_Initialize()
		{
			Sprite spritePyromancy = RogueUtilities.ConvertToSprite(Properties.Resources.Pyromancy);

			CustomAbility pyromancy = RogueLibs.CreateCustomAbility("Pyromancy", spritePyromancy, true,
				new CustomNameInfo("Pyromancy"),
				new CustomNameInfo("You can throw fire from your hands. This tends to fix a lot of your problems, and create much worse ones."),
				delegate (InvItem item)
				{
					item.cantDrop = true;
					item.Categories.Add("NPCsCantPickUp");
					item.dontAutomaticallySelect = true;
					item.dontSelectNPC = true;
					item.gunKnockback = 0;
					item.rapidFire = true;
					item.initCount = 100;
					item.itemType = "WeaponProjectile";
					item.rapidFire = true;
					item.rechargeAmountInverse = 100;
					item.shadowOffset = 2;
					item.stackable = true;
					item.thiefCantSteal = true;
				});

			pyromancy.Available = true;
			pyromancy.AvailableInCharacterCreation = true;
			pyromancy.CostInCharacterCreation = 8;

			int pyromancyTimeHeld = 0;

			pyromancy.OnHeld = delegate (InvItem item, Agent agent, ref float time)
			{
				pyromancyTimeHeld++;

				if (pyromancyTimeHeld >= PyromancyRollFireRate(agent) && !PyromancyIsBurnedOut(agent) && !PyromancyIsCoolingDown(agent) && !PyromancyIsMiscast(agent))
				{
					pyromancyTimeHeld = 0;

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
				if (pyromancyTimeHeld > 0 && !PyromancyIsBurnedOut(agent) && !PyromancyIsCoolingDown(agent) && !PyromancyIsMiscast(agent))
					PyromancyStartCoolingDown(agent);
			};

			pyromancy.Recharge = (item, agent) =>
			{
				if (item.invItemCount < CalcMaxMana(agent) && agent.statusEffects.CanRecharge())
				{
					item.invItemCount++;

					if (item.invItemCount == CalcMaxMana(agent))
						PyromancyCompleteRecharge(agent, true);
				}
			};

			pyromancy.RechargeInterval = (item, myAgent) =>
				item.invItemCount > 0 ? new WaitForSeconds(0.1f) : null;
		}
		public static float PyromancyRollFireRate(Agent agent)
		{
			float divisor = 5.000f;

			StatusEffects se = agent.statusEffects;

			if (se.hasTrait("FocusedCasting"))
				divisor -= 0.250f;
			else if (se.hasTrait("FocusedCasting_2"))
				divisor -= 0.500f;
			else if (se.hasTrait("WildCasting"))
				divisor -= 0.750f;
			else if (se.hasTrait("WildCasting_2"))
				divisor -= 1.500f;

			if (se.hasTrait("MagicTraining"))
				divisor -= 0.250f;
			else if (se.hasTrait("MagicTraining_2"))
				divisor -= 0.500f;

			return divisor;
		}
		public static bool PyromancyRollManaCost(Agent agent)
		{
			int chance = 100;

			if (agent.statusEffects.hasTrait("Archmage"))
				return false;

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

			if (agent.statusEffects.hasTrait("Archmage"))
				return false;
			else if (agent.statusEffects.hasTrait("FocusedCasting"))
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
		public static void PyromancyStartBurnout(Agent agent)
		{
			if (!agent.statusEffects.hasTrait("WildCasting") && !agent.statusEffects.hasTrait("WildCasting_2"))
			{
				agent.gc.audioHandler.Play(agent, "MindControlEnd");
				PyromancySetBurnedOut(agent, true);
			}

			Task.Delay(5000);

			PyromancySetBurnedOut(agent, false); // 202103301952
		}
		public static void PyromancyStartCast(Agent agent)
		{
			BunnyHeader.Log("PyromancyStartCast");

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

			bullet.speed = 6;

			if (agent.statusEffects.hasTrait("MagicTraining"))
				bullet.speed += 1;
			else if (agent.statusEffects.hasTrait("MagicTraining_2"))
				bullet.speed += 2;

			if (agent.statusEffects.hasTrait("WildCasting"))
				bullet.speed += 3;
			else if (agent.statusEffects.hasTrait("WildCasting_2") || agent.statusEffects.hasTrait("Archmage"))
				bullet.speed += 6;
		}
		public static async Task PyromancyStartCoolingDown(Agent agent)
		{
			BunnyHeader.Log("PyromancyStartCoolingDown");

			if (PyromancyIsCoolingDown(agent) == false)
			{
				PyromancySetCoolingDown(agent, true);

				float duration = 2000f;

				if (agent.statusEffects.hasTrait("MagicTraining"))
					duration -= 375f;
				else if (agent.statusEffects.hasTrait("MagicTraining_2"))
					duration -= 750f;

				if (agent.statusEffects.hasTrait("Archmage"))
					duration = 0f;
				if (agent.statusEffects.hasTrait("WildCasting"))
					duration -= 625f;
				else if (agent.statusEffects.hasTrait("WildCasting_2"))
					duration -= 1250f;

				duration = Mathf.Max(0f, duration);

				await Task.Delay((int)duration);

				PyromancySetCoolingDown(agent, false);
			}
		}
		public static void PyromancyStartMiscast(Agent agent, int degree)
		{
			BunnyHeader.Log("PyromancyStartMiscast");

			PyromancyDialogueMiscast(agent);

			agent.gc.spawnerMain.SpawnExplosion(agent, agent.curPosition, "FireBomb");

			PyromancyStartBurnout(agent);
		}
		public static void PyromancyCompleteRecharge(Agent agent, bool routine)
		{
			BunnyHeader.Log("PyromancyStartRecharge");

			if (!routine)
			{
				agent.statusEffects.CreateBuffText("Recharged", agent.objectNetID);
				agent.gc.audioHandler.Play(agent, "Recharge");
			}

			if (PyromancyIsBurnedOut(agent))
				PyromancySetBurnedOut(agent, false);

			if (PyromancyIsMiscast(agent))
				PyromancySetMiscast(agent, false);

			agent.inventory.buffDisplay.specialAbilitySlot.MakeUsable();
		}
		#endregion
		#region Telemancy
		#region Telemancy - Bits
		public static bool TelemancyIsReturning(Agent agent)
		{
			//BunnyHeader.Log("TelemancyIsReturning: " + ((agent.inventory.equippedSpecialAbility.otherDamage & 0b_0001) != 0));

			return (agent.inventory.equippedSpecialAbility.otherDamage & 0b_0001) != 0;
		}
		public static bool TelemancyIsMiscast(Agent agent)
		{
			//BunnyHeader.Log("TelemancyIsMiscast: " + ((agent.inventory.equippedSpecialAbility.otherDamage & 0b_0010) != 0));

			return (agent.inventory.equippedSpecialAbility.otherDamage & 0b_0010) != 0;
		}
		public static bool TelemancyIs_VARIABLE3(Agent agent) =>
			(agent.inventory.equippedSpecialAbility.otherDamage & 0b_0100) != 0;
		public static bool TelemancyIs_VARIABLE4(Agent agent) =>
			(agent.inventory.equippedSpecialAbility.otherDamage & 0b_1000) != 0;
		public static void TelemancyLogBits(Agent agent)
		{
			BunnyHeader.Log("TelemancyIsMiscast: " + TelemancyIsMiscast(agent));
			BunnyHeader.Log("TelemancyIsReturning: " + TelemancyIsReturning(agent));
		}
		public static void TelemancySetReturning(Agent agent, bool value)
		{
			BunnyHeader.Log("TelemancySetReturning: " + value);

			if (value) agent.inventory.equippedSpecialAbility.otherDamage |= 0b_0001;
			else agent.inventory.equippedSpecialAbility.otherDamage &= ~0b_0001;
		}
		public static void TelemancySetMiscast(Agent agent, bool value)
		{
			BunnyHeader.Log("TelemancySetMiscast: " + value);

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
		#endregion
		#region Telemancy - Dialogue
		public static void TelemancyDialogueCantDo(Agent agent)
		{
			agent.gc.audioHandler.Play(agent, "CantDo");

			if (agent.gc.percentChance(75))
				return;

			string[] dialogue =
			{
				"I need to give it a rest or my head will explode. I've seen it happen.",
				"Slow down! Haven't you seen The Fly?"
			};

			agent.Say(dialogue[UnityEngine.Random.Range(0, dialogue.Count() - 1)]);
		}
		public static void TelemancyDialogueCast(Agent agent)
		{
			if (agent.gc.percentChance(75))
				return;

			agent.SpawnParticleEffect("Spawn", agent.curPosition);
			GameController.gameController.audioHandler.Play(agent, "Spawn");

			string[] dialogue =
			{
				"Vwip!",
				"Nothing personal, kid."
			};

			agent.Say(dialogue[UnityEngine.Random.Range(0, dialogue.Count() - 1)]);
		}
		public static void TelemancyDialogueFullyCharged(Agent agent) // TODO
		{
			agent.SpawnParticleEffect("ExplosionEMP", agent.curPosition);
			agent.gc.audioHandler.Play(agent, "Hypnotize");
		}
		public static void TelemancyDialogueMiscast(Agent agent)
		{
			agent.gc.audioHandler.Play(agent, "ZombieSpitFire");

			string[] dialogue =
			{
				"I smell burning toast.",
				"Blurgh. (Drool)",
				"I pink I bust hab a stwoke.",
				"My head a splode."
			};

			agent.Say(dialogue[UnityEngine.Random.Range(0, dialogue.Count() - 1)]);
		}
		public static void TelemancyDialogueRecharge(Agent agent)
		{
			string[] dialogue =
			{
				"Who needs Scotty? I'll beam my damn self up."
			};

			agent.Say(dialogue[UnityEngine.Random.Range(0, dialogue.Count() - 1)]);
		}
		#endregion
		public static void Telemancy_Initialize()
		{
			Sprite spriteTelemancy = RogueUtilities.ConvertToSprite(Properties.Resources.Telemancy);

			CustomAbility telemancy = RogueLibs.CreateCustomAbility("Telemancy", spriteTelemancy, true,
				new CustomNameInfo("Telemancy"),
				new CustomNameInfo("Hold to charge, release to teleport to your targeting reticle. May cause stroke."),
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

			int telemancyHeldCounter = 0; // Seconds ability held to charge
			int telemancyNetCharge = 0; // Net total of post-ability charge level

			telemancy.OnPressed = delegate (InvItem item, Agent agent)
			{
				if (!TelemancyIsMiscast(agent) && !TelemancyIsReturning(agent) && item.invItemCount > 0)
					agent.SpawnParticleEffect("ExplosionMindControl", agent.curPosition);
				else
					TelemancyDialogueCantDo(agent);
			};

			telemancy.OnHeld = delegate (InvItem item, Agent agent, ref float timeHeld)
			{
				if (!TelemancyIsMiscast(agent) && !TelemancyIsReturning(agent) && item.invItemCount > 0 && telemancyNetCharge < 100)
				{
					if (timeHeld * TelemancyRollChargeRate(agent) >= telemancyHeldCounter)
					{
						telemancyHeldCounter++;

						BunnyHeader.Log("Telemancy OnHeld: HeldCounter = " + telemancyHeldCounter + "; timeHeld = " + timeHeld);

						int manaCost = Mathf.Min(TelemancyRollManaCost(agent), 100 - telemancyNetCharge);

						item.invItemCount -= manaCost;
						telemancyNetCharge += manaCost;

						if (telemancyNetCharge == 100)
							TelemancyDialogueFullyCharged(agent);
					}
				}
			};

			telemancy.OnReleased = delegate (InvItem item, Agent agent)
			{
				if (telemancyNetCharge > 0 && !TelemancyIsReturning(agent) && !TelemancyTryMiscast(agent, TelemancyRollMiscast(agent, telemancyHeldCounter)))
				{
					TelemancyStartCast(agent, Math.Max(100, telemancyNetCharge));
					TelemancyStartReturn(agent, TelemancyRollReturnDuration(agent));
					telemancyHeldCounter = 0;
					telemancyNetCharge = 0;
				}
			};

			telemancy.Recharge = (item, agent) =>
			{
				if (telemancyNetCharge == 0 && item.invItemCount < CalcMaxMana(agent) && agent.statusEffects.CanRecharge())
				{
					item.invItemCount = Math.Min(CalcMaxMana(agent), item.invItemCount + TelemancyRollRechargeRate(agent));

					if (item.invItemCount == CalcMaxMana(agent))
						TelemancyStartRecharge(agent, true);
				}
			};

			telemancy.RechargeInterval = (item, myAgent) =>
				item.invItemCount > 0 ? new WaitForSeconds(1f) : null;
		}
		public static int TelemancyRollChargeRate(Agent agent)
		{
			float rate = 1.000f;

			if (agent.statusEffects.hasTrait("Archmage"))
				return 10;
			else if (agent.statusEffects.hasTrait("MagicTraining"))
				rate *= 1.250f;
			else if (agent.statusEffects.hasTrait("MagicTraining_2"))
				rate *= 1.500f;

			if (agent.statusEffects.hasTrait("FocusedCasting"))
				rate *= 1.250f;
			else if (agent.statusEffects.hasTrait("FocusedCasting_2"))
				rate *= 1.500f;
			else if (agent.statusEffects.hasTrait("WildCasting"))
				rate *= 2.000f;
			else if (agent.statusEffects.hasTrait("WildCasting_2"))
				rate *= 3.000f;

			return (int)rate;
		}
		public static Vector2 TelemancyRollDestination(Agent agent, float maxError)
		{
			TileInfo tileInfo = agent.gc.tileInfo;
			Vector2 currentPosition = agent.curPosition;
			Vector2 targetPosition = agent.curPosition;
			float[] range = { 0f, maxError };

			for (int i = 0; i < 50; i++)
			{
				float distance = UnityEngine.Random.Range(range[0], range[1]);

				targetPosition = MouseIngamePosition() + distance * UnityEngine.Random.insideUnitCircle.normalized;

				TileData tileData = tileInfo.GetTileData(targetPosition);

				if ((tileData.solidObject) ||
					(tileInfo.WallExist(tileData)) ||
					(tileInfo.GetWallMaterial(targetPosition.x, targetPosition.y) == wallMaterialType.Border)) // Removed Conveyor, Water, Hole
					continue;

				return targetPosition;
			}
			return currentPosition;
		}
		public static int TelemancyRollManaCost(Agent agent)
		{
			BunnyHeader.Log("TelemancyRollManaCost");

			float min = 25.000f;
			float max = 33.000f;

			if (agent.statusEffects.hasTrait("Archmage"))
				return 0;
			else if (agent.statusEffects.hasTrait("MagicTraining"))
			{
				min *= 0.875f;
				max *= 0.875f;
			}
			else if (agent.statusEffects.hasTrait("MagicTraining_2"))
			{
				min *= 0.750f;
				max *= 0.750f;
			}

			if (agent.statusEffects.hasTrait("FocusedCasting"))
			{
				min *= 0.750f;
				max *= 0.750f;
			}
			else if (agent.statusEffects.hasTrait("FocusedCasting_2"))
			{
				min *= 0.500f;
				max *= 0.500f;
			}
			else if (agent.statusEffects.hasTrait("WildCasting"))
			{
				min *= 0.500f;
				max *= 1.250f;
			}
			else if (agent.statusEffects.hasTrait("WildCasting_2"))
			{
				min *= 0.250f;
				max *= 1.500f;
			}

			return (int)UnityEngine.Random.Range(min, max);
		}
		public static int TelemancyRollMiscast(Agent agent, float secondsHeld)
		{
			BunnyHeader.Log("TelemancyRollMiscast");

			if (agent.statusEffects.hasTrait("Archmage"))
				return 0;
			else if (agent.statusEffects.hasTrait("MagicTraining"))
				secondsHeld *= 0.750f;
			else if (agent.statusEffects.hasTrait("MagicTraining_2"))
				secondsHeld *= 0.500f;

			if (agent.statusEffects.hasTrait("FocusedCasting"))
				secondsHeld *= 0.625f;
			else if (agent.statusEffects.hasTrait("FocusedCasting_2"))
				secondsHeld *= 0.375f;
			else if (agent.statusEffects.hasTrait("WildCasting"))
				secondsHeld *= 3.000f;
			else if (agent.statusEffects.hasTrait("WildCasting_2"))
				secondsHeld *= 6.000f;

			float margin = secondsHeld - UnityEngine.Random.Range(0, 100);

			return (int)margin;
		}
		public static int TelemancyRollRechargeRate(Agent agent)
		{
			float min = 5.00f;
			float max = 10.00f;

			if (agent.statusEffects.hasTrait("Archmage"))
				return 1000;
			if (agent.statusEffects.hasTrait("MagicTraining"))
			{
				min *= 1.500f;
				max *= 1.500f;
			}
			else if (agent.statusEffects.hasTrait("MagicTraining_2"))
			{
				min *= 2.250f;
				max *= 2.250f;
			}

			if (agent.statusEffects.hasTrait("FocusedCasting"))
			{
				min *= 2.000f;
				max *= 1.250f;
			}
			else if (agent.statusEffects.hasTrait("FocusedCasting_2"))
			{
				min *= 3.000f;
				max *= 1.750f;
			}
			else if (agent.statusEffects.hasTrait("WildCasting"))
			{
				min *= 0.500f;
				max *= 3.000f;
			}
			else if (agent.statusEffects.hasTrait("WildCasting_2"))
			{
				min *= 0.250f;
				max *= 4.000f;
			}

			return (int)UnityEngine.Random.Range(min, max);
		}
		public static int TelemancyRollReturnDuration(Agent agent)
		{
			float duration = 4000f;

			if (agent.statusEffects.hasTrait("Archmage"))
				return 0;
			else if (agent.statusEffects.hasTrait("MagicTraining"))
				duration *= 0.750f;
			else if (agent.statusEffects.hasTrait("MagicTraining_2"))
				duration *= 0.500f;

			if (agent.statusEffects.hasTrait("FocusedCasting"))
				duration *= 0.875f;
			else if (agent.statusEffects.hasTrait("FocusedCasting_2"))
				duration *= 0.750f;
			else if (agent.statusEffects.hasTrait("WildCasting"))
				duration *= 0.500f;
			else if (agent.statusEffects.hasTrait("WildCasting_2"))
				duration *= 0.250f;

			return (int)duration;
		}
		public static void TelemancyStartCast(Agent agent, float charge)
		{
			float maxError = 100.000f;

			if (agent.statusEffects.hasTrait("FocusedCasting"))
				maxError -= 12.500f;
			else if (agent.statusEffects.hasTrait("FocusedCasting_2"))
				maxError -= 25.000f;
			else if (agent.statusEffects.hasTrait("WildCasting"))
				maxError += 12.500f;
			else if (agent.statusEffects.hasTrait("WildCasting_2"))
				maxError += 25.000f;

			maxError = (maxError - charge) / 20f;

			agent.Teleport(TelemancyRollDestination(agent, maxError), false, true);
			agent.rb.velocity = Vector2.zero;

			TelemancyDialogueCast(agent);
		}
		public static bool TelemancyTryMiscast(Agent agent, int severity)
		{
			bool failTeleport = false;
			bool lightAndSound = false;
			bool randomTeleport = false;
			bool loseItem = false;
			bool stroke = false;

			if (severity == 0)
				return false;
			else if (severity == 1)
			{
				lightAndSound = true;
				stroke = true;
			}
			else if (severity == 2)
			{
				failTeleport = true;
				randomTeleport = true;
				stroke = true;
			}
			else if (severity == 3)
			{
				loseItem = true;
				stroke = true;
			}
			else if (severity >= 4)
			{
				failTeleport = true;
				loseItem = true;
				lightAndSound = true;
				randomTeleport = true;
				stroke = true;
			}

			if (randomTeleport)
			{
				BunnyHeader.Log("TelemancyTryMiscast: randomTeleport");

				// TODO
			}
			if (lightAndSound)
			{
				BunnyHeader.Log("TelemancyTryMiscast: lightAndSound");

				// TODO
			}
			if (stroke)
			{
				BunnyHeader.Log("TelemancyTryMiscast: stroke");

				int degree = severity * 5;

				agent.statusEffects.ChangeHealth(-degree);
				agent.statusEffects.AddStatusEffect("Dizzy", degree / 4);

				TelemancyDialogueMiscast(agent);
			}
			if (loseItem)
			{
				BunnyHeader.Log("TelemancyTryMiscast: LoseItem");

				// TODO
				// A Shortcut might be telling the game that the item has fallen into a hole. It would have the AV effects already set up. But might not be that simple.
			}

			agent.inventory.buffDisplay.specialAbilitySlot.MakeNotUsable();

			if (!failTeleport)
				return false;

			return true;	
		}
		public static void TelemancyStartRecharge(Agent agent, bool routine)
		{
			if (!routine)
			{
				agent.statusEffects.CreateBuffText("Recharged", agent.objectNetID);
				agent.gc.audioHandler.Play(agent, "Recharge");
			}
			agent.inventory.buffDisplay.specialAbilitySlot.MakeUsable();
		}
		public static async void TelemancyStartReturn(Agent agent, int mSecs)
		{
			TelemancySetReturning(agent, true);

			await Task.Delay(mSecs);

			TelemancySetReturning(agent, false);
		}
		#endregion

		#region Agent
		public static void Agent_FindOperatingTime(PlayfieldObject operatingOnObject, float timeToUnlock, Agent __instance, ref float __result) // Postfix
		{
			__result *= __instance.gc.selectedTimeScale;
		}
		#endregion
		#region Bullet
		public static void Bullet_BulletHitEffect(GameObject hitObject, Bullet __instance) // Postfix
		{
			if (__instance.cameFromWeapon == "ChainLightning" && __instance.agent.inventory.equippedSpecialAbility.invItemName == "Electromancy")
			{
				if (hitObject.CompareTag("AgentSprite"))
				{
					try
					{
						hitObject = hitObject.GetComponent<AgentColliderBox>().objectSprite.go;
					}
					catch
					{
						hitObject = hitObject.transform.Find("AgentHitboxColliders").transform.GetChild(0).GetComponent<AgentColliderBox>().objectSprite.go;
					}

					ElectromancyOnImpact(hitObject, __instance);
				}
			}
		}
		#endregion
		#region LoadLevel
		public static void LoadLevel_SetupMore5_2(LoadLevel __instance) // Postfix
		{
			baseTimeScale = GameController.gameController.selectedTimeScale;
		}
		#endregion
		#region StatusEffects
		public static void StatusEffects_GiveSpecialAbility(string abilityName, StatusEffects __instance) // Postfix
		{
			BunnyHeader.Log("StatusEffects_GiveSpecialAbility: " + abilityName);

			if (__instance.agent.inventory.equippedSpecialAbility != null)
			{
				InvItem ability = __instance.agent.inventory.equippedSpecialAbility;
				Agent agent = __instance.agent;

				string[] magicAbilities = 
				{ 
					"Chronomancy", 
					"Cryomancy",
					"Electromancy", 
					"Pyromancy", 
					"Telemancy" 
				};

				if (magicAbilities.Contains(abilityName))
				{
					ability.otherDamage = 0; // Bitwise variables

					ability.initCount = CalcMaxMana(agent);
					ability.maxAmmo = CalcMaxMana(agent);
					ability.rechargeAmountInverse = CalcMaxMana(agent);
				}
			}
		}
		#endregion
	}
}
