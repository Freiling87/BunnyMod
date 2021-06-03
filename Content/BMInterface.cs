using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

using BepInEx;
using HarmonyLib;
using UnityEngine;
using RogueLibsCore;

using Random = UnityEngine.Random;
using UnityEngine.Networking;
using System.Linq;
using UnityEngine.UI;

namespace BunnyMod.Content
{
	public class BMInterface
	{
		public static GameController GC => GameController.gameController;
		public static bool Prefix(Type type, string methodName, Type patchType, string patchMethodName, Type[] types) => BMHeader.MainInstance.PatchPrefix(type, methodName, patchType, patchMethodName, types);
		public static bool Postfix(Type type, string methodName, Type patchType, string patchMethodName, Type[] types) => BMHeader.MainInstance.PatchPostfix(type, methodName, patchType, patchMethodName, types);
		public static void BMLog(string logMessage) => BMHeader.Log(logMessage);

		#region Main
		public void Awake()
		{
			Agent_00();
			Item_00();
			InvInterface_00();
			ObjectReal_00();
			PlayerControl_00();
			StatusEffects_00();
		}
		#endregion

		#region Custom
		public static float GetZoomLevel(Agent playerAgent)
		{
			if (playerAgent.statusEffects.hasTrait(cTrait.EagleEyes))
				return 0.75f;
			else if (playerAgent.statusEffects.hasTrait(cTrait.EagleEyes_2))
				return 0.50f;
			else if (playerAgent.statusEffects.hasTrait(cTrait.Myopic))
				return 1.50f;
			else if (playerAgent.statusEffects.hasTrait(cTrait.Myopic2))
				return 2.00f;
			else
				return 1f;
		}
		public static float GetZoomLevel()
		{
			float result = 1.0f;

			if (GC.splitScreen)
				result = 0.8f;
			if (GC.fourPlayerMode)
				result = 0.6f;

			if (BMTraits.IsPlayerTraitActive(cTrait.EagleEyes))
				result *= 0.70f;
			else if (BMTraits.IsPlayerTraitActive(cTrait.EagleEyes_2))
				result *= 0.40f;
			else if (BMTraits.IsPlayerTraitActive(cTrait.Myopic))
				result *= 1.50f;
			else if (BMTraits.IsPlayerTraitActive(cTrait.Myopic2))
				result *= 2.00f;

			return result;
		}
		#endregion

		#region Agent
		public void Agent_00()
		{
			Type t = typeof(Agent);
			Type g = GetType();

			Prefix(t, "AgentOnCamera", g, "Agent_AgentOnCamera", new Type[0] { });
		}
		public static bool Agent_AgentOnCamera(Agent __instance, ref bool __result) // Replacement
		{
			if (__instance.isPlayer != 0)
			{
				__instance.onCamera = true;

				__result = true; 
				return false;
			}

			if (GC.serverPlayer)
			{
				if ((!__instance.brain.active && !__instance.oma._dead && !__instance.frozen && !__instance.wasPossessed2 && GC.serverPlayer && GC.loadCompleteReally && !GC.loadLevel.recentlyStartedLevel && !__instance.oma.mindControlled) || __instance.objectAgent)
				{
					__instance.onCamera = false;

					__result = false; 
					return false;
				}
			}
			else if ((!__instance.brain.active && !__instance.dead && !__instance.frozen && !__instance.wasPossessed2 && GC.serverPlayer && GC.loadCompleteReally && !GC.loadLevel.recentlyStartedLevel) || __instance.objectAgent)
			{
				__instance.onCamera = false;

				__result = false; 
				return false;
			}

			Vector2 v = __instance.tr.position;
			Vector2 vector = GC.playerAgent.agentCamera.originalCamera.WorldToViewportPoint(v);
			float x = vector.x / GetZoomLevel();
			float y = vector.y / GetZoomLevel();

			if (x > -0.1f && x < 1.1f && 
				y > -0.1f && y < 1.1f)
			{
				__instance.onCamera = true;

				__result = true; 
				return false;
			}

			if (GC.coopMode || GC.fourPlayerMode)
			{
				vector = GC.playerAgent2.agentCamera.originalCamera.WorldToViewportPoint(v);
				x = vector.x / GetZoomLevel();
				y = vector.y / GetZoomLevel();

				if (x > -0.1f && x < 1.1f && 
					y > -0.1f && y < 1.1f)
				{
					__instance.onCamera = true;

					__result = true; 
					return false;
				}

				if (GC.fourPlayerMode)
				{
					vector = GC.playerAgent3.agentCamera.originalCamera.WorldToViewportPoint(v);
					x = vector.x / GetZoomLevel();
					y = vector.y / GetZoomLevel();

					if (x > -0.1f && x < 1.1f && 
						y > -0.1f && y < 1.1f)
					{
						__instance.onCamera = true;

						__result = true; 
						return false;
					}
					if (!GC.sessionDataBig.threePlayer)
					{
						vector = GC.playerAgent4.agentCamera.originalCamera.WorldToViewportPoint(v);
						x = vector.x / GetZoomLevel();
						y = vector.y / GetZoomLevel();

						if (x > -0.1f && x < 1.1f && 
							y > -0.1f && y < 1.1f)
						{
							__instance.onCamera = true;

							__result = true; 
							return false;
						}
					}
				}
			}

			__instance.onCamera = false;

			__result = false; 
			return false;
		}
		#endregion
		#region InvInterface
		public void InvInterface_00()
		{
			Type t = typeof(InvInterface);
			Type g = GetType();

			Prefix(t, "ShowCursorText", g, "InvInterface_ShowCursorText", new Type[4] { typeof(string), typeof(string), typeof(PlayfieldObject), typeof(int) });
		}
		public static bool InvInterface_ShowCursorText(string myText, string myText2, PlayfieldObject myPlayfieldObject, int specificColor, InvInterface __instance) // Replacement
		{
			// "Guilty" cursor text and others

			bool player2 = false;
			bool player3 = false;
			bool player4 = false;

			if (GC.levelEnded || __instance.noCursorText)
				return false;
			
			if (__instance.cursorTextAgent != null)
			{
				__instance.cursorTextAgent.showingCursorText = false;
				__instance.cursorTextAgent = null;
			}
			
			if (GC.coopMode || GC.fourPlayerMode)
			{
				if (GC.mainGUI2.openedQuestSheet && GC.mainGUI2.questSheetScript.openedByPlayer == GC.playerAgent2)
					player2 = true;
				
				if (GC.fourPlayerMode)
				{
					if (GC.mainGUI3.openedQuestSheet && GC.mainGUI3.questSheetScript.openedByPlayer == GC.playerAgent3)
						player3 = true;
				
					if (GC.mainGUI4.openedQuestSheet && GC.mainGUI4.questSheetScript.openedByPlayer == GC.playerAgent4)
						player4 = true;
				}
			}

			if (Cursor.visible || __instance.mainGUI.agent.controllerType != "Keyboard" || player2 || player3 || player4)
			{
				__instance.cursorTextCanvas.enabled = true;
				__instance.cursorTextString.text = myText;

				#region Colors & Health Bar
				if (myPlayfieldObject != null)
				{
					if (myPlayfieldObject.playfieldObjectType == "Agent")
					{
						Agent agent = (Agent)myPlayfieldObject;
						__instance.cursorTextAgent = agent;
						__instance.cursorTextAgent.showingCursorText = true;

						if (GC.levelType == "Normal")
						{
							try
							{
								if (agent.health != agent.healthMax && agent.health != 0f && !agent.localPlayer)
									agent.agentHealthBar.ChangeHealth(true, false);
							}
							catch
							{	Debug.LogError("Error in AgentHealthBar ChangeHealth: " + agent);	}
						}

						if (agent.ownerID > 0)
							__instance.cursorTextString.color = new Color32(byte.MaxValue, 225, 0, byte.MaxValue);
						else
							__instance.cursorTextString.color = Color.white;
					}
					else if (myPlayfieldObject.playfieldObjectType == "ObjectReal")
					{
						if (((ObjectReal)myPlayfieldObject).owner > 0)
							__instance.cursorTextString.color = new Color32(byte.MaxValue, 225, 0, byte.MaxValue);
						else
							__instance.cursorTextString.color = Color.white;
					}
					else if (myPlayfieldObject.playfieldObjectType == "Item")
					{
						Item item = (Item)myPlayfieldObject;

						if (item.owner != null)
						{
							if (item.owner.isPlayer == 0)
								__instance.cursorTextString.color = new Color32(byte.MaxValue, 225, 0, byte.MaxValue);
							else
								__instance.cursorTextString.color = Color.white;
						}
						else
							__instance.cursorTextString.color = Color.white;
					}
				}
				else
					__instance.cursorTextString.color = Color.white;

				if (myText2 != "" && myText2 != "\n()")
				{
					if (myText2.Contains("Hateful"))
					{
						__instance.cursorTextString2.color = Color.red;
						__instance.cursorTextString2.text = "\n(" + InvInterface.hatefulName + ")";
					}
					else if (myText2.Contains("Annoyed"))
					{
						__instance.cursorTextString2.color = new Color32(byte.MaxValue, 155, 0, byte.MaxValue);
						__instance.cursorTextString2.text = "\n(" + InvInterface.annoyedName + ")";
					}
					else if (myText2.Contains("Friendly"))
					{
						__instance.cursorTextString2.color = Color.cyan;
						__instance.cursorTextString2.text = "\n(" + InvInterface.friendlyName + ")";
					}
					else if (myText2.Contains("Loyal"))
					{
						__instance.cursorTextString2.color = Color.cyan;
						__instance.cursorTextString2.text = "\n(" + InvInterface.loyalName + ")";
					}
					else if (myText2.Contains("Aligned"))
					{
						__instance.cursorTextString2.color = Color.green;
						__instance.cursorTextString2.text = "\n(" + InvInterface.alignedName + ")";
					}
					else if (myText2.Contains("Submissive"))
					{
						__instance.cursorTextString2.color = Color.green;
						__instance.cursorTextString2.text = "\n(" + InvInterface.submissiveName + ")";
					}
					else if (myText2.Contains("Teleport"))
					{
						__instance.cursorTextString2.color = Color.red;
						__instance.cursorTextString2.text = "\n" + GC.nameDB.GetName(myText2, "Interface");
					}
					else if (myText2.Contains("Requirement"))
					{
						__instance.cursorTextString2.color = Color.red;
						__instance.cursorTextString2.text = "\n(" + GC.nameDB.GetName(myText2, "Unlock") + ")";
					}
					else if (myText2.Contains("Steam"))
					{
						__instance.cursorTextString2.color = Color.red;
						__instance.cursorTextString2.text = "\n(" + GC.nameDB.GetName(myText2, "Interface") + ")";
					}
					else if (myText2.Contains("RightClickToChangeCharacter"))
					{
						__instance.cursorTextString2.color = Color.yellow;
						__instance.cursorTextString2.text = "\n(" + GC.nameDB.GetName(myText2, "Interface") + ")";
					}
					else if (myText2.Contains("OnlyPasswordProtected"))
					{
						__instance.cursorTextString2.color = Color.red;
						__instance.cursorTextString2.text = "\n(" + GC.nameDB.GetName(myText2, "Interface") + ")";
					}
					else if (myText2 != "" && myText2 != null)
					{
						__instance.cursorTextString2.color = Color.white;
						__instance.cursorTextString2.text = myText2;
					}

					if (__instance.cursorTextString2.text.Contains("E_"))
					{
						__instance.cursorTextString2.color = Color.white;
						__instance.cursorTextString2.text = myText2;
					}
					
					if (!myText2.Contains("Neutral"))
						__instance.cursorTextCanvas2.enabled = true;
					else
						__instance.cursorTextCanvas2.enabled = false;
				}
				else
					__instance.cursorTextCanvas2.enabled = false;
				#endregion

				__instance.cursorTextCanvas4.enabled = false;
				__instance.cursorTextCanvas5.enabled = false;
				bool flag4 = false;

				if (myPlayfieldObject != null && myPlayfieldObject.playfieldObjectType == "Agent")
				{
					Agent agent2 = (Agent)myPlayfieldObject;

					if (agent2.statusEffects.StatusEffectList.Count > 0 && (agent2.isPlayer == 0 || !agent2.localPlayer))
					{
						__instance.cursorTextCanvas4.enabled = true;
						__instance.cursorTextString4.color = Color.magenta;

						if (__instance.cursorTextCanvas2.enabled)
							__instance.cursorTextString4.text = "\n\n(";
						else
							__instance.cursorTextString4.text = "\n(";
						
						if (agent2.statusEffects.StatusEffectList.Count > 1)
						{
							Text text = __instance.cursorTextString4;
							text.text = text.text + GC.nameDB.GetName(agent2.statusEffects.StatusEffectList[0].statusEffectName, "StatusEffect") + "...)";
						}
						else
						{
							Text text2 = __instance.cursorTextString4;
							text2.text = text2.text + GC.nameDB.GetName(agent2.statusEffects.StatusEffectList[0].statusEffectName, "StatusEffect") + ")";
						}
					}

					bool flag5 = false;

					if ((__instance.mainGUI.agent.enforcer || __instance.mainGUI.agent.bigQuest == "Cop" || __instance.mainGUI.agent.statusEffects.hasTrait(cTrait.VeryHardOnYourself)) && !agent2.statusEffects.IsInnocent(__instance.mainGUI.agent) && !agent2.dead)
					{
						flag5 = true;

						if (__instance.cursorTextCanvas4.enabled)
						{
							__instance.cursorTextCanvas5.enabled = true;
							__instance.cursorTextString5.color = new Color32(byte.MaxValue, 155, 0, byte.MaxValue);

							if (__instance.cursorTextCanvas2.enabled)
								__instance.cursorTextString5.text = "\n\n\n(";
							else
								__instance.cursorTextString5.text = "\n\n(";
							
							Text text3 = __instance.cursorTextString5;
							text3.text = text3.text + InvInterface.guiltyName + ")";
						}
						else
						{
							__instance.cursorTextCanvas4.enabled = true;
							__instance.cursorTextString4.color = new Color32(byte.MaxValue, 155, 0, byte.MaxValue);

							if (__instance.cursorTextCanvas2.enabled)
								__instance.cursorTextString4.text = "\n\n(";
							else
								__instance.cursorTextString4.text = "\n(";
							
							Text text4 = __instance.cursorTextString4;
							text4.text = text4.text + InvInterface.guiltyName + ")";
						}
					}

					if (__instance.mainGUI.agent.statusEffects.hasSpecialAbility("StealingGlove") && agent2.pickpocketedTimes > 0 && agent2.isPlayer == 0 && agent2.objectMultAgent.emptyInventoryPickpocket)
					{
						if (flag5)
						{
							if (__instance.cursorTextCanvas5.enabled)
							{
								if (__instance.cursorTextCanvas2.enabled)
								{
									Text text5 = __instance.cursorTextString5;
									text5.text = text5.text + " <color=white>" + InvInterface.emptyName + "</color>";
								}
								else
								{
									flag4 = true;
									Text text6 = __instance.cursorTextString5;
									text6.text = text6.text + "\n<color=white>" + InvInterface.emptyName + "</color>";
								}
							}
							else if (__instance.cursorTextCanvas4.enabled)
							{
								__instance.cursorTextCanvas5.enabled = true;
								__instance.cursorTextString5.color = Color.white;

								if (__instance.cursorTextCanvas2.enabled)
									__instance.cursorTextString5.text = "\n\n\n";
								else
									__instance.cursorTextString5.text = "\n\n";
								
								Text text7 = __instance.cursorTextString5;
								text7.text += InvInterface.emptyName;
							}
						}
						else if (__instance.cursorTextCanvas4.enabled)
						{
							__instance.cursorTextCanvas5.enabled = true;
							__instance.cursorTextString5.color = Color.white;

							if (__instance.cursorTextCanvas2.enabled)
								__instance.cursorTextString5.text = "\n\n\n";
							else
								__instance.cursorTextString5.text = "\n\n";
							
							Text text8 = __instance.cursorTextString5;
							text8.text += InvInterface.emptyName;
						}
						else
						{
							__instance.cursorTextCanvas4.enabled = true;
							__instance.cursorTextString4.color = Color.white;

							if (__instance.cursorTextCanvas2.enabled)
								__instance.cursorTextString4.text = "\n\n";
							else
								__instance.cursorTextString4.text = "\n";
							
							Text text9 = __instance.cursorTextString4;
							text9.text += InvInterface.emptyName;
						}
					}
				}

				if (specificColor != 0)
					__instance.cursorTextString.color = new Color32(byte.MaxValue, 225, 0, byte.MaxValue);
				
				if (__instance.cursorTextString.text != "")
					__instance.cursorTextBackgroundCanvas.enabled = true;
				
				LayoutRebuilder.ForceRebuildLayoutImmediate(__instance.cursorTextRect);
				__instance.cursorTextBackgroundRect.localPosition = new Vector2(__instance.cursorTextRect.localPosition.x - 17f, 8f);
				float width = __instance.cursorTextRect.rect.width;
				float num = 54.5f;
				
				if (flag4)
					num += 44f;
				
				if (__instance.cursorTextString.text.Contains("\n"))
					num += 44f;
				
				if (__instance.cursorTextCanvas2.enabled)
				{
					LayoutRebuilder.ForceRebuildLayoutImmediate(__instance.cursorTextRect2);
				
					if (__instance.cursorTextRect2.rect.width > width)
						width = __instance.cursorTextRect2.rect.width;
					
					num += 44f;
				}

				if (__instance.cursorTextCanvas4.enabled)
				{
					LayoutRebuilder.ForceRebuildLayoutImmediate(__instance.cursorTextRect4);
				
					if (__instance.cursorTextRect4.rect.width > width)
						width = __instance.cursorTextRect4.rect.width;
					
					num += 44f;
				}

				if (__instance.cursorTextCanvas5.enabled)
				{
					LayoutRebuilder.ForceRebuildLayoutImmediate(__instance.cursorTextRect5);
					
					if (__instance.cursorTextRect5.rect.width > width)
						width = __instance.cursorTextRect5.rect.width;
					
					num += 44f;
				}

				__instance.cursorTextBackgroundRect.sizeDelta = new Vector2(width * 0.5f + 30f, num);
			}

			return false;
		}
		#endregion
		#region Item
		public void Item_00()
		{
			Type t = typeof(Item);
			Type g = GetType();
		}
		#endregion
		#region ObjectReal
		public void ObjectReal_00()
		{
			Type t = typeof(ObjectReal);
			Type g = GetType();

			Prefix(t, "ObjectRealOnCamera", g, "ObjectReal_ObjectRealOnCamera", new Type[0]);
		}
		public static bool ObjectReal_ObjectRealOnCamera(ObjectReal __instance, ref bool __result) // Replacement
		{
			// Eagle Eye activation range

			// BMLog("ObjectReal_ObjectRealOnCamera"); // Drags framerate

			if (!__instance.activeObject || __instance.notRealObject)
			{
				__instance.onCamera = false;

				__result = false;
				return false;
			}

			if ((!GC.coopMode && !GC.fourPlayerMode) || GC.splitScreen)
			{
				float cameraWidth = 13f / GetZoomLevel();
				float cameraHeight = 8f / GetZoomLevel();
				Vector2 curPosition = GC.playerAgent.agentCamera.curPosition;

				if (curPosition.x > __instance.curPosition.x - cameraWidth && 
					curPosition.x < __instance.curPosition.x + cameraWidth && 
					curPosition.y > __instance.curPosition.y - cameraHeight && 
					curPosition.y < __instance.curPosition.y + cameraHeight)
				{
					__instance.onCamera = true;

					__result = true;
					return false;
				}

				if (GC.coopMode || GC.fourPlayerMode)
				{
					curPosition = GC.playerAgent2.agentCamera.curPosition;

					if (curPosition.x > __instance.curPosition.x - cameraWidth && 
						curPosition.x < __instance.curPosition.x + cameraWidth && 
						curPosition.y > __instance.curPosition.y - cameraHeight && 
						curPosition.y < __instance.curPosition.y + cameraHeight)
					{
						__instance.onCamera = true;

						__result = true;
						return false;
					}

					if (GC.fourPlayerMode)
					{
						curPosition = GC.playerAgent3.agentCamera.curPosition;

						if (curPosition.x > __instance.curPosition.x - cameraWidth && 
							curPosition.x < __instance.curPosition.x + cameraWidth && 
							curPosition.y > __instance.curPosition.y - cameraHeight && 
							curPosition.y < __instance.curPosition.y + cameraHeight)
						{
							__instance.onCamera = true;

							__result = true;
							return false;
						}

						if (!GC.sessionDataBig.threePlayer)
						{
							curPosition = GC.playerAgent4.agentCamera.curPosition;

							if (curPosition.x > __instance.curPosition.x - cameraWidth && 
								curPosition.x < __instance.curPosition.x + cameraWidth && 
								curPosition.y > __instance.curPosition.y - cameraHeight && 
								curPosition.y < __instance.curPosition.y + cameraHeight)
							{
								__instance.onCamera = true;

								__result = true;
								return false;
							}
						}
					}
				}
			}
			else
			{
				Vector2 vector = GC.playerAgent.agentCamera.originalCamera.WorldToViewportPoint(__instance.curPosition);

				if (vector.x > -0.1f && 
					vector.x < 1.1f && 
					vector.y > -0.1f && 
					vector.y < 1.1f)
				{
					__instance.onCamera = true;

					__result = true;
					return false;
				}

				if (GC.coopMode || GC.fourPlayerMode)
				{
					vector = GC.playerAgent2.agentCamera.originalCamera.WorldToViewportPoint(__instance.curPosition);
					if (vector.x > -0.1f && 
						vector.x < 1.1f && 
						vector.y > -0.1f && 
						vector.y < 1.1f)
					{
						__instance.onCamera = true;

						__result = true;
						return false;
					}

					if (GC.fourPlayerMode)
					{
						vector = GC.playerAgent3.agentCamera.originalCamera.WorldToViewportPoint(__instance.curPosition);
						if (vector.x > -0.1f && 
							vector.x < 1.1f && 
							vector.y > -0.1f && 
							vector.y < 1.1f)
						{
							__instance.onCamera = true;

							__result = true;
							return false;
						}

						if (!GC.sessionDataBig.threePlayer)
						{
							vector = GC.playerAgent4.agentCamera.originalCamera.WorldToViewportPoint(__instance.curPosition);
							if (vector.x > -0.1f && 
								vector.x < 1.1f && 
								vector.y > -0.1f && 
								vector.y < 1.1f)
							{
								__instance.onCamera = true;

								__result = true;
								return false;
							}
						}
					}
				}
			}

			__instance.onCamera = false;

			__result = false;
			return false;
		}
		#endregion
		#region PlayerControl
		public void PlayerControl_00()
		{
			Postfix(typeof(PlayerControl), "Update", GetType(), "PlayerControl_Update", new Type[0] { });
		}
		public static void PlayerControl_Update(PlayerControl __instance) // Postfix
		{
			GC.cameraScript.zoomLevel = GetZoomLevel();
			__instance.myCamera.zoomLevel = GetZoomLevel();
		}
		#endregion
		#region StatusEffects
		public void StatusEffects_00()
		{
			Type t = typeof(StatusEffects);
			Type g = GetType();

			Prefix(t, "SpecialAbilityInterfaceCheck2", g, "StatusEffects_SpecialAbilityInterfaceCheck2", new Type[0] { });
		}
		public static bool StatusEffects_SpecialAbilityInterfaceCheck2(StatusEffects __instance, ref IEnumerator __result) // Replacement
		{
			__result = StatusEffects_SpecialAbilityInterfaceCheck2_Replacement(__instance);

			return false;
		}
		public static IEnumerator StatusEffects_SpecialAbilityInterfaceCheck2_Replacement(StatusEffects __instance) // Non-Patch
		{
			// Sniper Headshot indicators
			#region Vanilla
			__instance.startedSpecialAbilityInterfaceCheck = true;

			if (__instance.startedSpecialAbilityInterfaceCheck)
			{
				do
				{
					if (GC.loadComplete && 
						__instance.agent.specialAbilityIndicator != null && 
						!__instance.agent.disappearedArcade && 
						__instance.agent.inventory.buffDisplay.specialAbilitySlot != null && 
						!__instance.agent.ghost)
					{
						string specialAbility = __instance.agent.specialAbility;

						if (specialAbility == vSpecialAbility.Bite)
						{
							bool flag = false;

							if ((__instance.agent.health < __instance.agent.healthMax || __instance.agent.oma.superSpecialAbility || __instance.hasTrait("BiteFullHealth")) && 
								__instance.agent.bitingAgent == null && 
								__instance.CanShowSpecialAbilityIndicator())
							{
								Agent agent = (Agent)__instance.FindSpecialAbilityObject();

								if (agent != null)
								{
									__instance.agent.specialAbilityIndicator.ShowIndicator(agent, "Bite");
									flag = true;
								}
							}
						
							if (!flag)
								__instance.agent.specialAbilityIndicator.Revert();
						}
						else if (specialAbility == vSpecialAbility.ChloroformHankie)
						{
							bool targetAvailable = false;

							if (__instance.agent.inventory.equippedSpecialAbility.invItemCount == 0 && __instance.CanShowSpecialAbilityIndicator())
							{
								Agent targetAgent = __instance.CanSpecialAttack(false);

								if (targetAgent != null)
								{
									__instance.agent.specialAbilityIndicator.ShowIndicator(targetAgent, "Chloroform");
									targetAvailable = true;
								}

								__instance.agent.inventory.buffDisplay.specialAbilitySlot.MakeUsable();
							}

							if (!targetAvailable)
								__instance.agent.specialAbilityIndicator.Revert();
						}
						else if (specialAbility == vSpecialAbility.Handcuffs)
						{
							bool flag3 = false;

							if (__instance.agent.arrestingAgent == null && __instance.CanShowSpecialAbilityIndicator())
							{
								Agent agent3 = (Agent)__instance.FindSpecialAbilityObject();
							
								if (agent3 != null)
								{
									if (agent3.statusEffects.IsInnocent(__instance.agent) && !__instance.agent.oma.superSpecialAbility)
										__instance.agent.specialAbilityIndicator.ShowIndicator(agent3, "Handcuffs", "Innocent");
									else
										__instance.agent.specialAbilityIndicator.ShowIndicator(agent3, "Handcuffs", "Guilty");
								
									flag3 = true;
								}
							}

							if (!flag3)
								__instance.agent.specialAbilityIndicator.Revert();
						}
						else if (specialAbility == vSpecialAbility.Enslave)
						{
							bool flag4 = false;

							if (__instance.agent.arrestingAgent == null && __instance.CanShowSpecialAbilityIndicator())
							{
								Agent agent4 = (Agent)__instance.FindSpecialAbilityObject();
							
								if (agent4 != null)
								{
									__instance.agent.specialAbilityIndicator.ShowIndicator(agent4, "Enslave");
									flag4 = true;
								}
							}

							if (!flag4)
								__instance.agent.specialAbilityIndicator.Revert();
						}
						else if (specialAbility == vSpecialAbility.Toss)
						{
							bool flag5 = false;

							if (__instance.agent.hoistingObject == null && __instance.CanShowSpecialAbilityIndicator())
							{
								ObjectReal objectReal = (ObjectReal)__instance.FindSpecialAbilityObject();
							
								if (objectReal != null)
								{
									flag5 = true;
									__instance.agent.specialAbilityIndicator.ShowIndicator(objectReal, "Toss");
								}
							}

							if (!flag5)
								__instance.agent.specialAbilityIndicator.Revert();
						}
						else if (specialAbility == vSpecialAbility.Cannibalize)
						{
							bool flag6 = false;

							if (__instance.agent.bitingAgent == null && __instance.CanShowSpecialAbilityIndicator())
							{
								Agent agent5 = (Agent)__instance.FindSpecialAbilityObject();
							
								if (agent5 != null && (__instance.agent.health < __instance.agent.healthMax || (agent5.agentName == __instance.agent.oma.bigQuestTarget1 && __instance.agent.oma.bigQuestObjectCount == 0)))
								{
									__instance.agent.specialAbilityIndicator.ShowIndicator(agent5, "Cannibalize");
									flag6 = true;
								}
							}

							if (!flag6)
								__instance.agent.specialAbilityIndicator.Revert();
						}
						else if (specialAbility == vSpecialAbility.Possess)
						{
							bool flag7 = false;

							if (!__instance.agent.possessing && __instance.CanShowSpecialAbilityIndicator())
							{
								Agent agent6 = (Agent)__instance.FindSpecialAbilityObject();
							
								if (agent6 != null)
								{
									flag7 = true;
									__instance.agent.specialAbilityIndicator.ShowIndicator(agent6, "Possess");
								}
							}

							if (!flag7)
								__instance.agent.specialAbilityIndicator.Revert();
						}
						else if (specialAbility == vSpecialAbility.MechTransformation)
						{
							bool flag8 = false;

							if (__instance.agent.arrestingAgent == null && __instance.agent.inventory.equippedSpecialAbility.invItemCount == 0 && __instance.CanShowSpecialAbilityIndicator())
							{
								Agent agent7 = (Agent)__instance.FindSpecialAbilityObject();
							
								if (agent7 != null)
								{
									__instance.agent.specialAbilityIndicator.ShowIndicator(agent7, "MechTransform");
									flag8 = true;
								}
							}

							if (!flag8)
								__instance.agent.specialAbilityIndicator.Revert();
						}
						else if (specialAbility == "InvisibleLimitedItem")
						{
							if (__instance.agent.invisible)
								__instance.agent.inventory.buffDisplay.specialAbilitySlot.MakeNotUsable();
							else if (__instance.agent.inventory.equippedSpecialAbility.invItemCount > 0)
								__instance.agent.inventory.buffDisplay.specialAbilitySlot.MakeNotUsable();
							else if (__instance.CanShowSpecialAbilityIndicator())
							{
								bool flag9 = true;
							
								for (int i = 0; i < GC.activeBrainAgentList.Count; i++)
								{
									Agent agent8 = GC.activeBrainAgentList[i];
								
									if (agent8 != __instance.agent && !agent8.dead)
									{
										Relationship relationship = agent8.relationships.GetRelationship(__instance.agent);
									
										if (relationship.HasLOS("") && relationship.relType != "Aligned" && relationship.relType != "Loyal" && agent8.employer != __instance.agent && !agent8.oma.hidden)
										{
											__instance.agent.inventory.buffDisplay.specialAbilitySlot.MakeNotUsable();
											flag9 = false;
										
											break;
										}
									}
								}

								if (flag9)
								{
									for (int j = 0; j < GC.objectRealList.Count; j++)
									{
										if (GC.objectRealList[j].objectName == "SecurityCam")
										{
											SecurityCam securityCam = (SecurityCam)GC.objectRealList[j];

											if (securityCam.agentsInView.Contains(__instance.agent) && !securityCam.destroyed && securityCam.functional)
											{
												__instance.agent.inventory.buffDisplay.specialAbilitySlot.MakeNotUsable();
												flag9 = false;

												break;
											}
										}
									}
								}

								if (flag9)
									__instance.agent.inventory.buffDisplay.specialAbilitySlot.MakeUsable();
							}
						}
					}

					yield return new WaitForSeconds(0.1f);
				}
				while (__instance.startedSpecialAbilityInterfaceCheck);
			}
			#endregion
			if ((__instance.agent.statusEffects.hasTrait(cTrait.Sniper) || __instance.agent.statusEffects.hasTrait(cTrait.Sniper_2)) &&
				__instance.agent.agentInvDatabase.equippedWeapon.invItemName == vItem.Revolver)
			{
				// Test for reduced range if hidden

				foreach (Agent targetAgent in GC.agentList)
				{
					if (!targetAgent.dead && 
						!targetAgent.ghost &&
						__instance.agent.movement.HasLOSAgent360(targetAgent) &&
						__instance.agent.movement.GetDistance(__instance.agent.gameObject, targetAgent.gameObject) > 6f)
					{
						__instance.agent.specialAbilityIndicator.ShowIndicator(targetAgent, "Chloroform"); // Placeholder for new sprite
					}
				}
			}

			yield break;
		}
		#endregion
	}
}
