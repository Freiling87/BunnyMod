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
		}
		public static void InitializeAbilities()
		{
			#region Chronomancy
			bool spedUp = false;
			bool slowedDown = false;
			bool coolDown = false;

			Sprite spriteChronomancy = RogueUtilities.ConvertToSprite(Properties.Resources.Chronomancy);

			CustomAbility chronomancy = RogueLibs.CreateCustomAbility("Chronomancy", spriteChronomancy, true,
				new CustomNameInfo("Chronomancy"),
				new CustomNameInfo("You can slow down time for everyone but yourself. Sometimes you accidentally do the opposite. Your cardiologist is very concerned."),
				delegate (InvItem item)
				{
					item.cantDrop = true;
					item.Categories.Add("Usable"); //
					item.Categories.Add("NPCsCantPickup");
					item.dontAutomaticallySelect = true;
					item.dontSelectNPC = true;
					item.isWeapon = false;
					item.initCount = 0;
					item.itemType = ""; //
					item.rechargeAmountInverse = item.initCount;
					item.stackable = true;
					item.thiefCantSteal = true;
				});

			chronomancy.Available = true;
			chronomancy.AvailableInCharacterCreation = true;
			chronomancy.CostInCharacterCreation = 8;

			chronomancy.OnPressed = delegate (InvItem item, Agent agent)
			{
				if (coolDown)
					item.agent.gc.audioHandler.Play(item.agent, "CantDo");
				else
				{
					coolDown = true;

					if (spedUp)
					{
						spedUp = false;
						ChronomancyDecast(agent);
					}
					else if (slowedDown)
					{
						item.agent.gc.audioHandler.Play(item.agent, "CantDo");
						agent.Say("I need to take a \"time out!\" Get it? But seriously, my heart will stop.");
					}
					else
					{
						if (TelemancyRollForMiscast(agent, item.invItemCount))
						{
							slowedDown = true;
							ChronomancyMiscast(agent, item.invItemCount);
						}
						else
						{
							spedUp = true;
							item.invItemCount += ChronomancyManaCost(agent);

							if (item.invItemCount >= 20)
							{
								spedUp = false;
								slowedDown = true;
								ChronomancyDecast(agent);
								ChronomancyMiscast(agent, item.invItemCount);
							}
							else
								spedUp = true;
							ChronomancyCast(agent);
						}
					}
				}
			};
			chronomancy.Recharge = (item, agent) =>
			{
				coolDown = false;

				if (spedUp)
				{
					item.invItemCount += ChronomancyManaCost(agent);

					if (item.invItemCount >= 20)
					{
						spedUp = false;
						slowedDown = true;
						ChronomancyMiscast(agent, item.invItemCount);
					}
				}
				else // Inactive
				{
					if (item.invItemCount > 0 && agent.statusEffects.CanRecharge())
					{
						item.invItemCount--;

						if (item.invItemCount == 0)
						{
							agent.statusEffects.CreateBuffText("Recharged", agent.objectNetID);
							agent.gc.audioHandler.Play(agent, "Recharge");

							slowedDown = false;
							ChronomancyDecast(agent);
							agent.inventory.buffDisplay.specialAbilitySlot.MakeUsable();
						}
					}
				}
			};

			chronomancy.RechargeInterval = (item, myAgent) =>
				item.invItemCount > 0 ? new WaitForSeconds(1f) : null;

			#endregion
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

			bool zappedOut = false;

			electromancy.OnPressed = delegate (InvItem item, Agent agent)
			{
				if (zappedOut)
				{
					item.agent.gc.audioHandler.Play(item.agent, "CantDo");
				}

				if (ElectromancyRollForMiscast(agent, 0))
				{
					ElectromancyMiscast(agent, 20);
					zappedOut = true;
				}
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
					item.specialMeleeTexture = true;
					item.stackable = true;
					item.thiefCantSteal = true;
					item.weaponCode = weaponType.WeaponProjectile;
				});

			pyromancy.Available = true;
			pyromancy.AvailableInCharacterCreation = true;
			pyromancy.CostInCharacterCreation = 8;

			bool burntOut = false;

			pyromancy.OnHeld = delegate (InvItem item, Agent agent, ref float unused)
			{

				if (item.invItemCount == 0)
				{
					item.agent.gc.audioHandler.Play(item.agent, "MindControlEnd");
					burntOut = true;
				}
				else
				{
					if (TelemancyRollForMiscast(agent, 0))
					{
						PyromancyMiscast(agent, 20);
						burntOut = true;
					}
					else
					{
						PyromancyCast(agent);
						if (PyromancyManaCost(agent))
							item.invItemCount--;
					}
				}
			};

			pyromancy.Recharge = (item, myAgent) =>
			{
				if (item.invItemCount < item.rechargeAmountInverse && myAgent.statusEffects.CanRecharge())
				{
					item.invItemCount++;

					if (item.invItemCount == 100)
					{
						if (burntOut)
							burntOut = false;

						myAgent.statusEffects.CreateBuffText("Recharged", myAgent.objectNetID);
						myAgent.gc.audioHandler.Play(myAgent, "Recharge");
						myAgent.inventory.buffDisplay.specialAbilitySlot.MakeUsable();
					}
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
				if (TelemancyRollForMiscast(agent, item.invItemCount))
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

		#region Magic General
		public static Vector2 MouseIngamePosition()
		{
			Plane plane = new Plane(new Vector3(0, 0, 1), new Vector3(0, 0, 0));
			Ray ray = Camera.main.ScreenPointToRay(UnityEngine.Input.mousePosition);
			return plane.Raycast(ray, out float enter) ? (Vector2)ray.GetPoint(enter) : default;
		}
		#endregion
		#region Aimatomancy // Blood Magic
		// Harm self or destroy corpse for boosts
		#endregion
		#region Chronomancy
		public static void ChronomancyCast(Agent agent)
		{
			agent.SpawnParticleEffect("Spawn", agent.curPosition);
			GameController.gameController.audioHandler.Play(agent, "Spawn");

			agent.gc.mainTimeScale = 0.5f;
			agent.statusEffects.RemoveStatusEffect("Fast");
			agent.statusEffects.RemoveStatusEffect("Slow");
			agent.speedMax = agent.FindSpeed() * 2;
		}
		public static void ChronomancyDecast(Agent agent)
		{
			agent.SpawnParticleEffect("Spawn", agent.curPosition);
			GameController.gameController.audioHandler.Play(agent, "Spawn");

			agent.gc.mainTimeScale = 1f;
			agent.statusEffects.RemoveStatusEffect("Fast");
			agent.statusEffects.RemoveStatusEffect("Slow");
			agent.FindSpeed();
		}
		public static int ChronomancyManaCost(Agent agent)
		{
			int increment;

			if (agent.statusEffects.hasTrait("WildCasting"))
				increment = UnityEngine.Random.Range(1, 3);
			else if (agent.statusEffects.hasTrait("WildCasting_2"))
				increment = UnityEngine.Random.Range(0, 4);
			else
				increment = 2;

			return increment;
		}
		public static void ChronomancyMiscast(Agent agent, int degree)
		{
			agent.SpawnParticleEffect("Spawn", agent.curPosition);
			agent.gc.audioHandler.Play(agent, "ToiletTeleportIn");

			switch (UnityEngine.Random.Range(1, 3))
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
			}

			agent.gc.mainTimeScale = 2f;
			agent.statusEffects.RemoveStatusEffect("Fast");
			agent.statusEffects.RemoveStatusEffect("Slow");
			agent.speedMax = agent.FindSpeed() / 2;
			agent.inventory.buffDisplay.specialAbilitySlot.MakeNotUsable();
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
		#endregion
		#region Kinetomancy // Telekinesis

		#endregion
		#region Megaleiomancy //Charm Person

		#endregion
		#region Necromancy
		// Summon Zombies from corpses, automatically alleid
		// Miscast turns all of them hostile, or summons hostile ghosts
		// When close to a ghost, you can turn them into mana crystals
		#endregion
		#region Pyromancy
		public static void PyromancyCast(Agent agent)
		{
			agent.gun.HideGun();

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
				bullet.speed = 15;
			else if (agent.statusEffects.hasTrait("WildCasting_2"))
				bullet.speed = 20;
			else
				bullet.speed = 7;
		}
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
				risk *= (3 / 5);
			else if (agent.statusEffects.hasTrait("MagicTraining"))
				risk *= (4 / 5);

			return UnityEngine.Random.Range(0, 10000) <= risk;
		}
		#endregion
		#region Telemancy
		public static Vector2 TelemancyCast(Agent agent, bool accountForObstacles, bool notInside, bool dontCareAboutDanger, bool teleporting, bool accountForWalls)
		{
			TileInfo tileInfo = agent.gc.tileInfo;
			Vector2 currentPosition = agent.curPosition;
			Vector2 targetPosition;
			float rangeNear = 3f; //
			float rangeFar = 6f; //

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

			agent.statusEffects.ChangeHealth(- degree);
			agent.statusEffects.AddStatusEffect("Dizzy", degree / 4);
			agent.inventory.buffDisplay.specialAbilitySlot.MakeNotUsable();
		}
		public static bool TelemancyRollForMiscast(Agent agent, int modifier)
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
	}
}
