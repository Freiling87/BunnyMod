using BepInEx.Logging;
using BunnyMod.Content.Logging;
using BunnyMod.Content.Traits;
using HarmonyLib;
using UnityEngine;
using RogueLibsCore;
using Random = UnityEngine.Random;
using UnityEngine.UI;

namespace BunnyMod.Content.Patches
{
	[HarmonyPatch(declaringType: typeof(InvInterface))]
	public static class P_InvInterface
	{
		private static readonly ManualLogSource logger = BMLogger.GetLogger();
		public static GameController GC => GameController.gameController;

		[HarmonyPrefix, HarmonyPatch(methodName:nameof(InvInterface.ShowCursorText), argumentTypes:new[] { typeof(string), typeof(string), typeof(PlayfieldObject), typeof(int) })]
		public static bool InvInterface_ShowCursorText(string myText, string myText2, PlayfieldObject myPlayfieldObject, int specificColor,	InvInterface __instance) 
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
							{
								Debug.LogError("Error in AgentHealthBar ChangeHealth: " + agent);
							}
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

					if ((__instance.mainGUI.agent.enforcer || __instance.mainGUI.agent.bigQuest == "Cop" ||
							__instance.mainGUI.agent.HasTrait<VeryHardOnYourself>()) &&
						!agent2.statusEffects.IsInnocent(__instance.mainGUI.agent) && !agent2.dead)
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

					if (__instance.mainGUI.agent.statusEffects.hasSpecialAbility("StealingGlove") && agent2.pickpocketedTimes > 0 && agent2.isPlayer == 0 &&
						agent2.objectMultAgent.emptyInventoryPickpocket)
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
	}
}
