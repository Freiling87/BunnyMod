using HarmonyLib;
using RogueLibsCore;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Random = UnityEngine.Random;

namespace BunnyMod.Content
{
	public class BMQuests
	{
		public static GameController GC => GameController.gameController;
		public static bool Prefix(Type type, string methodName, Type patchType, string patchMethodName, Type[] types) => BMHeader.MainInstance.PatchPrefix(type, methodName, patchType, patchMethodName, types);
		public static bool Postfix(Type type, string methodName, Type patchType, string patchMethodName, Type[] types) => BMHeader.MainInstance.PatchPostfix(type, methodName, patchType, patchMethodName, types);
		public static void BMLog(string logMessage) => BMHeader.Log(logMessage);

		public void Awake()
		{
            ExitPoint_00();
            Quests_00();
            SkillPoints_00();
		}

		#region ExitPoint
        public void ExitPoint_00()
        {
            Prefix(typeof(ExitPoint), "DetermineIfCanExit", GetType(), "ExitPoint_DetermineIfCanExit", new Type[0] { });
        }
        public static bool ExitPoint_DetermineIfCanExit(ref bool __result, ExitPoint __instance) // Prefix
		{
			if (GC.challenges.Contains(cChallenge.RushinRevolution))
			{
				__result = true;
				return false;
			}

			return true;
		}
        #endregion
        #region Quests
        public void Quests_00()
        {
			Type t = typeof(Quests);
			Type g = GetType();

            Prefix(t, "GetLevelExitChallengeItem", g, "Quests_GetLevelExitChallengeItem", new Type[0] { });
			Prefix(t, "setupQuests", g, "Quests_setupQuests", new Type[0] { });
        }
        public static bool Quests_GetLevelExitChallengeItem(Quests __instance, ref InvItem __result) // Prefix
		{
            bool flag = false;
            string item = "";

            if (GC.challenges.Contains(cChallenge.UnpaidInternship))
            {
                item = GC.Choose<string> (vItem.Banana,vItem.HamSandwich);
                flag = true;
            }
            else if (GC.challenges.Contains(cChallenge.DoublePlyRewards))
			{
                item = GC.Choose<string>(vItem.FreeItemVoucher, vItem.HiringVoucher);
                flag = true;
            }

            if (flag)
			{
                InvItem invItem3 = new InvItem();
                invItem3.invItemName = item;
                invItem3.SetupDetails(false);
                invItem3.invItemCount = 0;
                __instance.questItemsGiven.Add(item);

                __result = invItem3;
                return false;
            }

            return true;
        }
        public static bool Quests_setupQuests(Quests __instance, ref Chunk ___chosenChunk) // Replacement
		{
			__instance.settingUpQuests = true;
			__instance.questItemsGiven.Remove("Money");
			__instance.questItemsGiven.Remove("TwitchMystery");
			__instance.questItemsGiven.Remove("Nugget");

			if (GC.loadLevel.LevelContainsMayor())
				__instance.oneQuest = "MayorQuest";
			
			if (__instance.oneQuest != "" || GC.levelType == "Tutorial" || GC.streamingWorld)
				__instance.totalQuests = 1;
			
			GC.challenges.Contains("Sandbox"); // sic
			
			if (GC.streamingWorld)
			{
				Random.InitState(GC.loadLevel.randomSeedNum);

				if (__instance.mainQuestListLight.Count > 0)
				{
					Debug.LogError("ALREADY HAVE QUESTS");
				
					for (int i = 0; i < __instance.mainQuestListLight.Count; i++)
					{
						Agent agent = null;
						Agent agent2 = null;
						ObjectReal objectReal = null;
						ObjectReal objectReal2 = null;
						ObjectReal objectReal3 = null;
						QuestLight questLight = __instance.mainQuestListLight[i];
					
						if (questLight.questGiverID != 0)
						{
							Agent agent3 = null;
						
							for (int j = 0; j < GC.agentList.Count; j++)
							{
								Agent agent4 = GC.agentList[j];
							
								if (agent4.streamingChunkObjectID == questLight.questGiverID)
									agent3 = agent4;
							}

							if (agent3 == null)
							{
								agent3 = GC.spawnerMain.SpawnAgent(new Vector2(questLight.questGiverPosX, questLight.questGiverPosY), null, questLight.questGiverName);
								agent3.fakeStreaming = true;
								agent3.streamingChunkObjectID = questLight.questGiverID;
							}

							agent = agent3;
						}

						if (questLight.questEnderID != 0)
						{
							Agent agent5 = null;
						
							for (int k = 0; k < GC.agentList.Count; k++)
							{
								Agent agent6 = GC.agentList[k];
							
								if (agent6.streamingChunkObjectID == questLight.questEnderID)
									agent5 = agent6;
							}

							if (agent5 == null)
							{
								agent5 = GC.spawnerMain.SpawnAgent(new Vector2(questLight.questEnderPosX, questLight.questEnderPosY), null, questLight.questEnderName);
								agent5.fakeStreaming = true;
								agent5.streamingChunkObjectID = questLight.questEnderID;
							}
						}

						int playerID = questLight.playerID;
						
						for (int l = 0; l < 3; l++)
						{
							int num = 0;
							string questTarget = "";
							string text = "";
							bool flag = false;
							bool dead = false;
							bool zombified = false;
							Vector2 vector = Vector2.zero;
						
							if (l == 0)
							{
								num = questLight.questTarget1ID;
								questTarget = questLight.questTarget1Type;
								text = questLight.questTarget1NameReal;
								vector = GC.streamingWorldController.GetWorldActualPos(new Vector2(questLight.questTarget1PosX, questLight.questTarget1PosY));
								flag = questLight.questTargetDestroyed[0];
								dead = questLight.questTargetDead[0];
								zombified = questLight.questTargetZombified[0];
							}
							else if (l == 1)
							{
								num = questLight.questTarget2ID;
								questTarget = questLight.questTarget2Type;
								text = questLight.questTarget2NameReal;
								vector = GC.streamingWorldController.GetWorldActualPos(new Vector2(questLight.questTarget2PosX, questLight.questTarget2PosY));
								flag = questLight.questTargetDestroyed[1];
								dead = questLight.questTargetDead[1];
								zombified = questLight.questTargetZombified[1];
							}
							else if (l == 2)
							{
								num = questLight.questTarget3ID;
								questTarget = questLight.questTarget3Type;
								text = questLight.questTarget3NameReal;
								vector = GC.streamingWorldController.GetWorldActualPos(new Vector2(questLight.questTarget3PosX, questLight.questTarget3PosY));
								flag = questLight.questTargetDestroyed[2];
								dead = questLight.questTargetDead[2];
								zombified = questLight.questTargetZombified[2];
							}

							if (num != 0)
							{
								if (questTarget == "Agent")
								{
									Agent agent7 = null;
									Debug.LogError("ID: " + questLight.questTarget1ID);

									for (int m = 0; m < GC.agentList.Count; m++)
									{
										Agent agent8 = GC.agentList[m];
									
										if (agent8.streamingChunkObjectID == num)
										{
											agent7 = agent8;
											Debug.LogError("FOUND AGENT");
										}
									}
									if (agent7 == null)
									{
										agent7 = GC.spawnerMain.SpawnAgent(vector, null, text);
										agent7.fakeStreaming = true;
										agent7.streamingChunkObjectID = num;
										agent7.dead = dead;
										agent7.zombified = zombified;
									}

									agent2 = agent7;
								}
								else if (questTarget == "Object")
								{
									ObjectReal objectReal4 = null;
									
									for (int n = 0; n < GC.objectRealListWithDestroyed.Count; n++)
									{
										ObjectReal objectReal5 = GC.objectRealListWithDestroyed[n];
									
										if (objectReal5.streamingChunkObjectID == num)
										{
											objectReal4 = objectReal5;
											Debug.LogError("FOUND OBJECT");
										}
									}

									if (objectReal4 == null)
									{
										objectReal4 = GC.spawnerMain.spawnObjectReal(vector, null, text);
										objectReal4.fakeStreaming = true;
										objectReal4.streamingChunkObjectID = num;
										objectReal4.destroying = flag;
										objectReal4.destroyed = flag;
									}
									
									if (l == 0)
										objectReal = objectReal4;
									else if (l == 1)
										objectReal2 = objectReal4;
									else if (l == 2)
										objectReal3 = objectReal4;
								}
								else if (questTarget == "Item")
								{
									Item item = null;
								
									for (int num2 = 0; num2 < GC.itemList.Count; num2++)
									{
										Item item2 = GC.itemList[num2];
									
										if (item2.streamingChunkObjectID == num)
										{
											item = item2;
											Debug.LogError("FOUND ITEM");
										}
									}

									if (item == null)
									{
										Debug.LogError(string.Concat("SPAWN QUEST ITEM: ", vector, " - ", text));
										InvItem invItem = new InvItem();
										invItem.invItemName = text;
										invItem.questItem = true;
										invItem.invItemCount = 1;
										invItem.SetupDetails(false);
										item = GC.spawnerMain.SpawnItem(vector, invItem, false);
										item.streamingChunkObjectID = num;
									}
								}
							}
						}

						__instance.StartCoroutine(Quests_SetupQuestTypeLate(__instance, "Kill", agent, agent, null, null, true, questLight, i));
					}

					return false;
				}
			}

			while (__instance.questTriesMain < __instance.questTriesTotal && __instance.settingUpQuests)
			{
				int attempts = 0;
				bool complete = false;

				while (attempts < 100 && !complete && __instance.settingUpQuests)
				{
					Agent agent9 = null;
					Agent agent2 = null;
					ObjectReal objectReal = null;
					ObjectReal objectReal2 = null;
					ObjectReal objectReal3 = null;
					string text2 = "";
					string text3 = "";
					bool flag3 = false;
					int num4 = 0;
					bool flag4 = false;

					if (__instance.numQuests == __instance.totalQuests - 1 && __instance.oneQuest == "" && GC.levelType != "Tutorial")
					{
						flag4 = true;

						while (!flag3)
						{
							if (num4 >= 100)
								break;
							
							agent9 = GC.agentList[Random.Range(0, GC.agentList.Count)];

							try
							{
								if (!__instance.CheckIfQuested(agent9, null, "QuestGiver") && 
									(agent9.agentName == "Shopkeeper" || agent9.agentName == "DrugDealer" || (agent9.agentName == "Clerk" && agent9.startingChunkRealDescription != "DeportationCenter") || agent9.agentName == "Bartender"))
									flag3 = true;

								if (agent9.startingChunkReal.giveQuest > 0)
									flag3 = false;
							}
							catch { }
							
							num4++;
						}
					}
					else
						flag3 = true;
					
					if (flag3)
					{
						if (GC.streamingWorld)
							Random.InitState(GC.loadLevel.randomSeedNum + attempts);
					
						bool flag5 = true;
						___chosenChunk = null;
						
						if (GC.levelType == "Tutorial")
						{
							text2 = "Destroy";
						
							for (int num5 = 0; num5 < GC.objectRealList.Count; num5++)
							{
								ObjectReal objectReal6 = GC.objectRealList[num5];
							
								if (objectReal6.objectName == "Generator")
								{
									objectReal = objectReal6;
								
									break;
								}
							}
						}
						else if (flag5 && __instance.oneQuest == "")
						{
							if (agent9 != null)
								agent9.startingChunkReal.quested = true;
							
							string text4 = "";
							bool flag6 = true;
							
							if (GC.customLevel && attempts < 10 && !GC.loadLevel.customLevel.randomizeQuests)
								foreach (Chunk chunk in GC.loadLevel.levelChunks)
									if (chunk.specificQuest != "" && chunk.specificQuest != null && chunk.specificQuest != "None" && !chunk.quested && chunk.importantObjects.Count != 0)
									{
										___chosenChunk = chunk;
							
										if (chunk.specificQuest != "Random")
										{
											text4 = chunk.specificQuest;
										
											break;
										}

										break;
									}

							if (GC.customLevel && ___chosenChunk == null && !GC.loadLevel.customLevel.randomizeQuests)
								flag6 = false;
							
							if (___chosenChunk == null && flag6)
								foreach (Chunk chunk2 in GC.loadLevel.levelChunks)
									if (chunk2.giveQuest > 0 && !chunk2.quested && chunk2.importantObjects.Count != 0 && __instance.SpecialCasesCheck(chunk2))
									{
										___chosenChunk = chunk2;

										break;
									}
							
							if (___chosenChunk == null && flag6)
							{
								int num6 = 0;
							
								foreach (Chunk chunk3 in GC.loadLevel.levelChunks)
									if (!chunk3.quested && chunk3.importantObjects.Count != 0 && __instance.SpecialCasesCheck(chunk3) && chunk3.difficultyLevel > num6)
									{
										num6 = chunk3.difficultyLevel;
										___chosenChunk = chunk3;
									}
							}

							if (__instance.numQuests == 0)
							{
								bool flag7 = GC.percentChance(30);
								bool flag8 = GC.percentChance(10);
							
								for (int num7 = 0; num7 < GC.agentList.Count; num7++)
								{
									Agent agent10 = GC.agentList[num7];
								
									if (agent10.gang != 0 && !agent10.isBigQuestObject && 
										((agent10.agentName == "Musician" && flag7) || (agent10.agentName == "Mafia" && flag8) || (agent10.agentName == "Gangbanger" && flag8) || (agent10.agentName == "GangbangerB" && flag8)))
									{
										bool flag9 = true;
										
										for (int num8 = 0; num8 < GC.playerAgentList.Count; num8++)
										{
											Agent agent11 = GC.playerAgentList[num8];
										
											if ((agent10.agentName == "Mafia" || agent10.agentName == "Gangbanger" || agent10.agentName == "GangbangerB") && agent11.agentName == agent10.agentName)
												flag9 = false;
										}

										for (int num9 = 0; num9 < GC.agentList.Count; num9++)
										{
											Agent agent12 = GC.agentList[num9];
										
											if (agent12.gang == agent10.gang && agent12.isBigQuestObject)
												flag9 = false;
										}

										if (flag9)
										{
											agent2 = agent10;
											___chosenChunk = null;
											text2 = "Kill";
										
											break;
										}
									}
								}
							}

							if (___chosenChunk != null)
							{
								___chosenChunk.quested = true;
								List<string> list = new List<string>();
							
								foreach (PlayfieldObject playfieldObject in ___chosenChunk.importantObjects)
								{
									if (playfieldObject.playfieldObjectType == "Agent")
									{
										agent2 = (Agent)playfieldObject;
								
										if (!__instance.isAlignedInChunk(agent2))
										{
											if (agent2.prisoner > 0 && agent2.ownerID == 0)
											{
												bool flag10 = false;
										
												for (int num10 = 0; num10 < GC.agentList.Count; num10++)
													if (GC.agentList[num10].startingChunk == agent2.startingChunk && GC.agentList[num10].prisoner > 0 && GC.agentList[num10].prisoner != agent2.prisoner)
													{
														flag10 = true;
												
														break;
													}
												
												if (flag10)
												{
													if (GC.levelFeeling != "Ooze")
														text2 = GC.Choose<string>("Rescue", "Rescue", "Rescue");
													
													agent2.potentialQuestTypes.Add("PrisonBreak");
													
													if (!list.Contains("PrisonBreak"))
													{
														list.Add("PrisonBreak");
														list.Add("PrisonBreak");
														list.Add("PrisonBreak");
													}
													
													bool flag11 = true;
													
													for (int num11 = 0; num11 < GC.playerAgentList.Count; num11++)
													{
														if (agent2.relationships.GetRel(GC.playerAgentList[num11]) == "Hateful")
															flag11 = false;
														
														if (!agent2.relationships.RelForRescueQuestOkay(GC.playerAgentList[num11]))
															flag11 = false;
													
														if (agent2.agentName == "Gorilla" && GC.playerAgentList[num11].bigQuest == "Gorilla" && flag4)
															flag11 = false;
													}

													if (GC.multiplayerMode && GC.serverPlayer)
														for (int num12 = 0; num12 < GC.networkManagerB.nextCharacter.Count; num12++)
														{
															if (!agent2.relationships.RelForRescueQuestOkayMult(GC.networkManagerB.nextCharacter[num12]))
																flag11 = false;
															if (agent2.agentName == "Gorilla" && GC.networkManagerB.nextCharacter[num12] == "Gorilla" && flag4)
																flag11 = false;
														}
													
													if (agent2.zombified)
														flag11 = false;
													
													if (agent9 != null)
													{
														try
														{
															if (agent9.relationships.GetRel(agent2) == "Hateful" || agent9.relationships.GetRel(agent2) == "Annoyed")
																flag11 = false;
														}
														catch { }
													
														for (int num13 = 0; num13 < GC.agentList.Count; num13++)
														{
															Agent agent13 = GC.agentList[num13];
															
															if (agent13.startingChunk == agent9.startingChunk && agent9.startingChunk != 0 && (agent13.relationships.GetRel(agent2) == "Hateful" || agent13.relationships.GetRel(agent2) == "Annoyed"))
																flag11 = false;
														}
													}

													if (flag11 && GC.levelFeeling != "Ooze")
													{
														agent2.potentialQuestTypes.Add("Rescue");
													
														if (!list.Contains("Rescue"))
															list.Add("Rescue");
													}

													
													bool flag12 = true;
													
													for (int num14 = 0; num14 < GC.playerAgentList.Count; num14++)
														if (agent2.relationships.GetRel(GC.playerAgentList[num14]) == "Aligned" || ((GC.playerAgentList[num14].statusEffects.hasStatusEffect("DontHitOwnKind") || agent2.statusEffects.hasStatusEffect("DontHitOwnKind")) && GC.playerAgentList[num14].agentName == agent2.agentName))
															flag12 = false;
													
													if (agent2.zombified)
														flag12 = false;
													
													if (flag12)
													{
														agent2.potentialQuestTypes.Add("Kill");
													
														if (!list.Contains("Kill"))
															list.Add("Kill");
													}
												}
												else
												{
													if (GC.levelFeeling != "Ooze")
														text2 = GC.Choose<string>("Rescue", "Rescue", "Rescue");
													
													bool flag13 = true;
													
													for (int num15 = 0; num15 < GC.playerAgentList.Count; num15++)
													{
														if (agent2.relationships.GetRel(GC.playerAgentList[num15]) == "Hateful")
															flag13 = false;
														
														if (!agent2.relationships.RelForRescueQuestOkay(GC.playerAgentList[num15]))
															flag13 = false;
													
														if (agent2.agentName == "Gorilla" && GC.playerAgentList[num15].bigQuest == "Gorilla" && flag4)
															flag13 = false;
													}

													if (GC.multiplayerMode && GC.serverPlayer)
														for (int num16 = 0; num16 < GC.networkManagerB.nextCharacter.Count; num16++)
														{
															if (!agent2.relationships.RelForRescueQuestOkayMult(GC.networkManagerB.nextCharacter[num16]))
																flag13 = false;
													
															if (agent2.agentName == "Gorilla" && GC.networkManagerB.nextCharacter[num16] == "Gorilla" && flag4)
																flag13 = false;
														}
													
													if (agent2.zombified)
														flag13 = false;
													
													if (agent9 != null)
													{
														try
														{
															if (agent9.relationships.GetRel(agent2) == "Hateful" || agent9.relationships.GetRel(agent2) == "Annoyed")
																flag13 = false;
														}
														catch { }
													}

													if (flag13 && GC.levelFeeling != "Ooze")
													{
														agent2.potentialQuestTypes.Add("Rescue");
													
														if (!list.Contains("Rescue"))
														{
															list.Add("Rescue");
															list.Add("Rescue");
														}
													}

													bool flag14 = true;
													
													for (int num17 = 0; num17 < GC.playerAgentList.Count; num17++)
														if (agent2.relationships.GetRel(GC.playerAgentList[num17]) == "Aligned" || ((GC.playerAgentList[num17].statusEffects.hasStatusEffect("DontHitOwnKind") || agent2.statusEffects.hasStatusEffect("DontHitOwnKind")) && GC.playerAgentList[num17].agentName == agent2.agentName))
															flag14 = false;
													
													if (agent2.zombified)
														flag14 = false;
													
													if (flag14)
													{
														agent2.potentialQuestTypes.Add("Kill");
													
														if (!list.Contains("Kill"))
															list.Add("Kill");
													}
												}
											}
											else if (agent2.agentName == "Slave" && agent2.slaveOwners.Count != 0)
											{
												text2 = "FreeSlave";
												agent2.potentialQuestTypes.Add("FreeSlave");
												
												if (!list.Contains("FreeSlave"))
												{
													list.Add("FreeSlave");
													list.Add("FreeSlave");
												}
											}
											else
											{
												bool flag15 = false;
												int num18 = 0;
												
												do
												{
													text2 = GC.Choose<string>("Kill", "KillAll", "KillAll", "KillAndRetrieve");
													bool flag16 = true;

													for (int num19 = 0; num19 < GC.playerAgentList.Count; num19++)
														if (agent2.relationships.GetRel(GC.playerAgentList[num19]) == "Aligned" || ((GC.playerAgentList[num19].statusEffects.hasStatusEffect("DontHitOwnKind") || agent2.statusEffects.hasStatusEffect("DontHitOwnKind")) && GC.playerAgentList[num19].agentName == agent2.agentName))
															flag16 = false;

													if (agent2.zombified)
														flag16 = false;

													if (flag16)
													{
														agent2.potentialQuestTypes.Add("Kill");

														if (!list.Contains("Kill"))
															list.Add("Kill");

														agent2.potentialQuestTypes.Add("KillAndRetrieve");

														if (!list.Contains("KillAndRetrieve"))
															list.Add("KillAndRetrieve");

														_ = agent9 != null; // sic
														flag15 = true;
													}

													if (agent2.ownerID != 0)
													{
														for (int num20 = 0; num20 < GC.agentList.Count; num20++)
														{
															Agent agent14 = GC.agentList[num20];

															if (agent2 != agent14 && agent2.startingChunk == agent14.startingChunk && agent2.ownerID == agent14.ownerID && 
																agent2.agentName != "Bouncer" && agent2.agentName != "Guard" && agent2.agentName != "Guard2" && !agent2.ghost && agent14.agentName != "Bouncer" && agent14.agentName != "Guard" && agent14.agentName != "Guard2" && !agent14.ghost)
															{
																bool flag17 = true;

																for (int num21 = 0; num21 < GC.playerAgentList.Count; num21++)
																	if (agent2.relationships.GetRel(GC.playerAgentList[num21]) == "Aligned" || 
																		((GC.playerAgentList[num21].statusEffects.hasStatusEffect("DontHitOwnKind") || agent2.statusEffects.hasStatusEffect("DontHitOwnKind")) && GC.playerAgentList[num21].agentName == agent2.agentName))
																		flag17 = false;
																
																if (agent2.zombified)
																	flag17 = false;
																
																if (flag17)
																{
																	agent2.potentialQuestTypes.Add("KillAll");
																
																	if (!list.Contains("KillAll"))
																	{
																		list.Add("KillAll");
																	
																		if ((!GC.challenges.Contains("QuickGame") && GC.sessionDataBig.curLevelEndless > 9) || (GC.challenges.Contains("QuickGame") && GC.sessionDataBig.curLevelEndless > 6))
																			list.Add("KillAll");
																	
																		if ((!GC.challenges.Contains("QuickGame") && GC.sessionDataBig.curLevelEndless > 12) || (GC.challenges.Contains("QuickGame") && GC.sessionDataBig.curLevelEndless > 8))
																			list.Add("KillAll");
																	}

																	flag15 = true;
																}
															}
														}
													}

													num18++;
													
													if (flag15)
														break;
												}
												while (num18 < 100);
											}
										}
									}
									else if (playfieldObject.playfieldObjectType == "ObjectReal")
									{
										objectReal = (ObjectReal)playfieldObject;
										
										if (!__instance.isAlignedInChunk(objectReal))
										{
											if (objectReal.GetComponent<InvDatabase>() != null)
											{
												if (!objectReal.plantable)
												{
													if (objectReal.destroyableForQuest)
													{
														text2 = "DestroyAndRetrieve";
														objectReal.potentialQuestTypes.Add("DestroyAndRetrieve");
										
														if (!list.Contains("DestroyAndRetrieve"))
															list.Add("DestroyAndRetrieve");
													}
													else if (objectReal.chestReal && objectReal.canSpill)
													{
														text2 = "Retrieve";
														objectReal.potentialQuestTypes.Add("Retrieve");
													
														if (!list.Contains("Retrieve"))
															list.Add("Retrieve");
													}
												}
											}
											else if (objectReal.usable)
											{
												for (int num22 = 0; num22 < GC.objectRealList.Count; num22++)
												{
													ObjectReal objectReal7 = GC.objectRealList[num22];

													if (objectReal != objectReal7 && objectReal.startingChunk == objectReal7.startingChunk && objectReal.objectName == objectReal7.objectName)
													{
														text2 = "UseAll";
														objectReal.potentialQuestTypes.Add("UseAll");
													
														if (!list.Contains("UseAll"))
														{
															list.Add("UseAll");
															list.Add("UseAll");
														}
													}
												}
											}
											else if (!objectReal.OnFloor && (!objectReal.bulletsCanPass || !objectReal.meleeCanPass) && !objectReal.cantBeDestroyed && objectReal.damageAccumulates)
											{
												bool flag18 = true;
												text2 = "Destroy";

												for (int num23 = 0; num23 < GC.playerAgentList.Count; num23++)
												{
													if ((objectReal.objectName == "Generator" || objectReal.objectName == "Generator2") && GC.playerAgentList[num23].bigQuest == "Soldier" && flag4)
														flag18 = false;
												
													if (objectReal.objectName == "PowerBox" && GC.playerAgentList[num23].bigQuest == "RobotPlayer" && flag4)
														flag18 = false;
												}
												
												if (GC.multiplayerMode && GC.serverPlayer)
													for (int num24 = 0; num24 < GC.networkManagerB.nextCharacter.Count; num24++)
													{
														if ((objectReal.objectName == "Generator" || objectReal.objectName == "Generator2") && GC.networkManagerB.nextCharacter[num24] == "Soldier" && flag4)
															flag18 = false;
												
														if (objectReal.objectName == "PowerBox" && GC.networkManagerB.nextCharacter[num24] == "RobotPlayer" && flag4)
															flag18 = false;
													}
												
												if (flag18)
												{
													objectReal.potentialQuestTypes.Add("Destroy");
												
													if (!list.Contains("Destroy"))
														list.Add("Destroy");
													
													for (int num25 = 0; num25 < GC.objectRealList.Count; num25++)
													{
														ObjectReal objectReal8 = GC.objectRealList[num25];
													
														if (objectReal != objectReal8 && objectReal.startingChunk == objectReal8.startingChunk && objectReal.objectName == objectReal8.objectName)
														{
															text2 = "DestroyAll";
															objectReal.potentialQuestTypes.Add("DestroyAll");
														
															if (!list.Contains("DestroyAll"))
															{
																list.Add("DestroyAll");
																list.Add("DestroyAll");
															}
														}
													}
												}
											}
										}
									}
								}

								agent2 = null;
								objectReal = null;
								int num26 = 0;
								
								if (list.Count > 0)
								{
									bool flag19 = true;
								
									if (text4 != "" && list.Contains(text4))
									{
										flag19 = false;
										text2 = text4;
									}
									
									if (flag19)
									{
										do
										{
											text2 = list[Random.Range(0, list.Count)];
											num26++;
										}
										while (num26 < 30 && __instance.levelQuestTypes.Contains(text2));
									}
									
									__instance.levelQuestTypes.Add(text2);
									
									using (List<PlayfieldObject>.Enumerator enumerator2 = ___chosenChunk.importantObjects.GetEnumerator())
									{
										while (enumerator2.MoveNext())
										{
											PlayfieldObject playfieldObject2 = enumerator2.Current;
									
											if (playfieldObject2.potentialQuestTypes.Contains(text2))
											{
												if (playfieldObject2.playfieldObjectType == "Agent")
												{
													agent2 = (Agent)playfieldObject2;
											
													break;
												}

												if (playfieldObject2.playfieldObjectType == "ObjectReal")
												{
													objectReal = (ObjectReal)playfieldObject2;
													
													break;
												}
											}
										}
										goto IL_1E95;
									}
								}

								Debug.Log("Couldn't find potential quest within chunk: " + ___chosenChunk);
							}
						}
					IL_1E95:
						if (__instance.oneQuest != "")
						{
							if (__instance.oneQuest == "MayorQuest")
							{
								text2 = "MayorQuest";

								for (int num27 = 0; num27 < GC.agentList.Count; num27++)
									if (GC.agentList[num27].agentName == "Mayor")
									{
										agent2 = GC.agentList[num27];
										___chosenChunk = agent2.startingChunkReal;
								
										break;
									}
							}
							else if (__instance.oneQuest == "FindBombs")
							{
								text2 = "FindBombs";
								int num28 = 0;
							
								foreach (Chunk chunk4 in GC.loadLevel.levelChunks)
									if (chunk4.giveQuest > 0 && !chunk4.quested)
									{
										foreach (KeyValuePair<int, ObjectReal> keyValuePair in GC.chestDic)
										{
											ObjectReal value = keyValuePair.Value;

											if (value.startingChunkReal == chunk4 && value.canSpill && GC.tileInfo.IsIndoors(value.tr.position) && value.objectName != "WasteBasket")
											{
												if (num28 == 0)
													objectReal = value;
												else if (num28 == 1)
													objectReal2 = value;
												else if (num28 == 2)
													objectReal3 = value;
												
												num28++;
												chunk4.quested = true;
												
												break;
											}
										}

										if (num28 == 3)
											break;
									}

								int num29 = 0;

								while (num28 < 3 && num29 < 5)
								{
									foreach (Chunk chunk5 in GC.loadLevel.levelChunks)
										if (!chunk5.quested)
										{
											foreach (KeyValuePair<int, ObjectReal> keyValuePair2 in GC.chestDic)
											{
												ObjectReal value2 = keyValuePair2.Value;

												if (value2.startingChunkReal == chunk5 && value2.canSpill && GC.tileInfo.IsIndoors(value2.tr.position))
												{
													if (num28 == 0)
														objectReal = value2;
													else if (num28 == 1)
														objectReal2 = value2;
													else if (num28 == 2)
														objectReal3 = value2;
												
													num28++;
													chunk5.quested = true;
													
													break;
												}
											}

											if (num28 == 3)
												break;
										}
									
									num29++;
								}

								num29 = 0;

								while (num28 < 3 && num29 < 5)
								{
									foreach (KeyValuePair<int, ObjectReal> keyValuePair3 in GC.chestDic)
									{
										ObjectReal value3 = keyValuePair3.Value;

										if (value3.canSpill && value3 != objectReal && value3 != objectReal2 && value3 != objectReal3)
										{
											if (num28 == 0)
												objectReal = value3;
											else if (num28 == 1)
												objectReal2 = value3;
											else if (num28 == 2)
												objectReal3 = value3;
										
											num28++;
											
											break;
										}
									}

									if (num28 == 3)
										break;
									
									num29++;
								}
							}
						}

						if (text2 == "")
							text2 = "Kill";
						
						if (agent9 != null)
							text3 = __instance.setQuestInfo(agent9.agentName, text2);
						
						if (GC.streamingWorld)
							text2 = "Kill";

						switch (text2)
						{
							case "DestroyAndRetrieve":
								if (objectReal == null)
									objectReal = __instance.FindObjectType(text3, agent9, "Object");

								if (objectReal != null)
								{
									__instance.SetupQuestType(text2, agent9, objectReal, null, null, flag4);
									__instance.numQuests++;
									complete = true;
								}

								break;

							case "PlantItem":
								if (objectReal == null)
									objectReal = __instance.FindObjectType(text3, agent9, "Object");

								if (objectReal != null)
								{
									__instance.SetupQuestType(text2, agent9, objectReal, null, null, flag4);
									__instance.numQuests++;
									complete = true;
								}

								break;
							case "UseAll":
								if (objectReal == null)
									objectReal = __instance.FindObjectType(text3, agent9, "Object");

								if (objectReal != null)
								{
									__instance.SetupQuestType(text2, agent9, objectReal, null, null, flag4);
									__instance.numQuests++;
									complete = true;
								}

								break;

							case "Kill":
								if (agent2 == null)
									agent2 = __instance.FindAgentType(text3, agent9, "Enemy");

								if (agent2 != null)
								{
									__instance.SetupQuestType(text2, agent9, agent2, null, null, flag4);
									__instance.numQuests++;
									complete = true;
								}

								break;

							case "Rescue":
								if (agent2 == null)
									agent2 = __instance.FindAgentType(text3, agent9, "Prisoner");

								if (agent2 != null)
								{
									__instance.SetupQuestType(text2, agent9, agent2, null, null, flag4);
									__instance.numQuests++;
									complete = true;
								}

								break;

							case "DestroyAll":
								if (objectReal == null)
									objectReal = __instance.FindObjectType(text3, agent9, "Object");

								if (objectReal != null)
								{
									__instance.SetupQuestType(text2, agent9, objectReal, null, null, flag4);
									__instance.numQuests++;
									complete = true;
								}

								break;

							case "Retrieve":
								if (objectReal == null)
									objectReal = __instance.FindRetrieveLocation(null, agent9, "Object");
								
								if (objectReal != null)
								{
									__instance.SetupQuestType(text2, agent9, objectReal, null, null, flag4);
									__instance.numQuests++;
									complete = true;
								}

								break;

							case "Deliver":
								if (agent2 == null)
									agent2 = __instance.FindAgentType(text3, agent9, "QuestEnder");
								
								if (agent2 != null)
								{
									__instance.SetupQuestType(text2, agent9, agent2, null, null, flag4);
									__instance.numQuests++;
									complete = true;
								}

								break;

							case "RoughUp":
								if (agent2 == null)
									agent2 = __instance.FindAgentType(text3, agent9, "Enemy");
								
								if (agent2 != null)
								{
									__instance.SetupQuestType(text2, agent9, agent2, null, null, flag4);
									__instance.numQuests++;
									complete = true;
								}

								break;
							case "RetrieveDontKill":
								if (agent2 == null)
									agent2 = __instance.FindAgentType(text3, agent9, "Enemy");
								
								if (agent2 != null)
								{
									objectReal = __instance.FindRetrieveDontKillLocation(null, agent9, "Object", agent2);

									if (objectReal != null)
									{
										__instance.SetupQuestType(text2, agent9, objectReal, agent2, null, flag4);
										__instance.numQuests++;
										complete = true;
									}
								}

								break;

							case "MayorQuest":
								if (agent2 != null)
								{
									__instance.SetupQuestType(text2, agent9, agent2, null, null, flag4);
									__instance.numQuests++;
									complete = true;
								}

								break;

							case "Destroy":
								if (objectReal == null)
									objectReal = __instance.FindObjectType(text3, agent9, "Object");
								
								if (objectReal != null)
								{
									__instance.SetupQuestType(text2, agent9, objectReal, null, null, flag4);
									__instance.numQuests++;
									complete = true;
								}

								break;

							case "GiveStatusEffect":
								if (agent2 == null)
									agent2 = __instance.FindAgentType(text3, agent9, "Enemy");
								
								if (agent2 != null)
								{
									__instance.SetupQuestType(text2, agent9, agent2, null, null, flag4);
									__instance.numQuests++;
									complete = true;
								}

								break;

							case "KillAll":
								if (agent2 == null)
									agent2 = __instance.FindAgentType(text3, agent9, "Enemy");
								
								if (agent2 != null)
								{
									__instance.SetupQuestType(text2, agent9, agent2, null, null, flag4);
									__instance.numQuests++;
									complete = true;
								}

								break;

							case "FindBombs":
								if (objectReal == null)
									objectReal = __instance.FindObjectType(text3, agent9, "Object");
								
								if (objectReal != null)
								{
									__instance.SetupQuestType(text2, agent9, objectReal, objectReal2, objectReal3, flag4);
									__instance.numQuests++;
									complete = true;
								}

								break;

							case "Escort":
								if (agent2 == null)
									agent2 = __instance.FindAgentType("Shopkeeper", agent9, text3);
								
								if (agent2 != null)
								{
									__instance.SetupQuestType(text2, agent9, agent2, null, null, flag4);
									__instance.numQuests++;
									complete = true;
								}

								break;

							case "FreeSlave":
								if (agent2 == null)
									agent2 = __instance.FindAgentType(text3, agent9, "Slave");
								
								if (agent2 != null)
								{
									__instance.SetupQuestType(text2, agent9, agent2, null, null, flag4);
									__instance.numQuests++;
									complete = true;
								}

								break;

							case "PrisonBreak":
								if (agent2 == null)
									agent2 = __instance.FindAgentType(text3, agent9, "Prisoner");
								
								if (agent2 != null)
								{
									__instance.SetupQuestType(text2, agent9, agent2, null, null, flag4);
									__instance.numQuests++;
									complete = true;
								}

								break;

							case "UseObject":
								if (objectReal == null)
									objectReal = __instance.FindObjectType(text3, agent9, "Object");
								
								if (objectReal != null)
								{
									__instance.SetupQuestType(text2, agent9, objectReal, null, null, flag4);
									__instance.numQuests++;
									complete = true;
								}

								break;

							case "KillAndRetrieve":
								if (agent2 == null)
									agent2 = __instance.FindAgentType(text3, agent9, "Enemy");
								
								if (agent2 != null)
								{
									__instance.SetupQuestType(text2, agent9, agent2, null, null, flag4);
									__instance.numQuests++;
									complete = true;
								}

								break;
						};

						attempts++;
					}
					else
					{
						Debug.Log("Couldn't find quest agent");
						__instance.settingUpQuests = false;
					}
				}

				__instance.questTriesMain++;

				if (__instance.numQuests >= __instance.totalQuests)
				{
					__instance.settingUpQuests = false;
					Debug.Log("Set up all quests");
				}
			}

			if (__instance.questTriesMain == __instance.questTriesTotal && __instance.settingUpQuests)
			{
				__instance.settingUpQuests = false;
				Debug.Log("Couldn't set up all quests");
			}

			return false;
		}
		public static IEnumerator Quests_SetupQuestTypeLate(Quests __instance, string randQuestType, Agent questAgent, PlayfieldObject targetObject, PlayfieldObject targetObject2, PlayfieldObject targetObject3, bool agentQuest, QuestLight myQuestLight, int questNum) // Non-Patch
		{
			yield return null;

			__instance.SetupQuestType(randQuestType, questAgent, targetObject, targetObject2, targetObject3, agentQuest, myQuestLight.questTargetName1, myQuestLight.questTargetName2, myQuestLight.questTargetName3);
			__instance.ConvertToQuest(myQuestLight, __instance.mainQuestList[questNum]);

			yield break;
		}
		#endregion
		#region SkillPoints
		public void SkillPoints_00()
        {
            Prefix(typeof(SkillPoints), "AddPointsLate", GetType(), "SkillPoints_AddPointsLate", new Type[2] { typeof(string), typeof(int) });
        }
        public static bool SkillPoints_AddPointsLate(string pointsType, int extraNum, ref IEnumerator __result, SkillPoints __instance, ref Agent ___agent) // Prefix
        {
            BMLog("SkillPoints_AddPointsLate");

            __result = SkillPoints_AddPointsLate_IEnumerator(pointsType, extraNum, __result, __instance, ___agent);

            return false;
        }
        private static IEnumerator SkillPoints_AddPointsLate_IEnumerator(string pointsType, int extraNum, IEnumerator __result, SkillPoints __instance, Agent ___agent) // Non-Patch
        {
            BMLog("SkillPoints_AddPointsLate_IEnumerator:");
            BMLog("\tpointsType = " + pointsType);
            BMLog("\textraNum = " + extraNum);

            if (pointsType == "DestructionPoints" || pointsType == "DestructionPoints2" || pointsType == "FireExtinguishPoints")
                yield return null;
            else
                yield return new WaitForSeconds(0.3f);

            if (Time.timeScale == 0f && !GC.multiplayerMode)
            {
                while (Time.timeScale == 0f)
                    yield return null;

                yield return new WaitForSeconds(0.2f);
            }

            int xpReward = 0;
            bool suppressAnimation = false;
            string text = pointsType;

            switch (text)
            {
                case "ArrestedPoints":
                    xpReward = 100;
                    break;
                case "ArrestedPointsInnocent":
                    if (___agent.statusEffects.hasTrait(vTrait.TheLaw) && !___agent.oma.superSpecialAbility)
                    {
                        if (___agent.statusEffects.hasTrait(vTrait.Crooked))
                            xpReward = 0;
                        else if (___agent.statusEffects.hasTrait(vTrait.Crooked2))
                            xpReward = -40;
                        else
                            xpReward = -80;
                    }
                    else
                        xpReward = 10;
                    break;
                case "BigQuestBonusDowntown":
                    xpReward = 500;
                    break;
                case "BigQuestBonusFloor":
                    xpReward = 300;
                    break;
                case "BigQuestBonusGame":
                    xpReward = 1000;
                    break;
                case "BigQuestBonusIndustrial":
                    xpReward = 500;
                    break;
                case "BigQuestBonusPark":
                    xpReward = 500;
                    break;
                case "BigQuestBonusSlums":
                    xpReward = 500;
                    break;
                case "BigQuestBonusUptown":
                    xpReward = 500;
                    break;
                case "CompleteMission":
                    xpReward = 300;

                    if (GC.challenges.Contains(cChallenge.UnpaidInternship))
                        xpReward *= 2;

                    break;
                case "CompleteMissionFindBombs":
                    xpReward = 700;
                    break;
                case "CompleteMissionReduced":
                    xpReward = 150;

                    if (GC.challenges.Contains(cChallenge.UnpaidInternship))
                        xpReward *= 2;

                    break;
                case "Destruction":
                    xpReward = 200;
                    break;
                case "DestructionPoints":
                    xpReward = 1;
                    suppressAnimation = true;
                    break;
                case "DestructionPoints2":
                    xpReward = 2;
                    suppressAnimation = true;
                    break;
                case "DisarmDetonatorPoints":
                    xpReward = 20;
                    break;
                case "ElectabilityBonus":
                    xpReward = 100;
                    break;
                case "Enslaved":
                    xpReward = 30;
                    break;
                case "FindTreasure":
                    xpReward = 100;
                    GC.stats.AddToStat(___agent, "TreasuresFound", 1);
                    break;
                case "FireExtinguishPoints":
                    xpReward = 5;
                    suppressAnimation = true;
                    break;
                case "FreedPrisoner":
                    xpReward = 20 * extraNum;

                    if (extraNum > 1)
                        pointsType = "FreedPrisoners";

                    break;
                case "FreedSlave":
                    xpReward = 50 * extraNum;

                    if (extraNum > 1)
                        pointsType = "FreedSlaves";

                    break;
                case "HackPoints":
                    xpReward = 20;
                    break;
                case "IndirectlyKill":
                    xpReward = 30;
                    break;
                case "IndirectlyKillInnocent":
                    if (___agent.statusEffects.hasTrait(vTrait.TheLaw) && !___agent.oma.superSpecialAbility)
                    {
                        if (___agent.statusEffects.hasTrait(vTrait.Crooked))
                            xpReward = 0;
                        else if (___agent.statusEffects.hasTrait(vTrait.Crooked2))
                            xpReward = -15;
                        else
                            xpReward = -30;
                    }
                    else
                        xpReward = 10;
                    break;
                case "IndirectlyKillRival":
                    xpReward = 90;
                    break;
                case "Joke":
                    xpReward = 30 * extraNum;
                    break;
                case "KilledRobot":
                    xpReward = 1000;
                    break;
                case "KillPoints":
                    xpReward = 50;
                    break;
                case "KillPointsInnocent":
                    if (___agent.statusEffects.hasTrait(vTrait.TheLaw) && !___agent.oma.superSpecialAbility)
                    {
                        if (___agent.statusEffects.hasTrait(vTrait.Crooked2))
                            xpReward = 0;
                        else if (___agent.statusEffects.hasTrait(vTrait.Crooked))
                            xpReward = -20;
                        else if (!___agent.oma.superSpecialAbility)
                            xpReward = -40;
                    }
                    else
                        xpReward = 10;

                    break;
                case "KillPointsRival":
                    xpReward = 150;
                    break;
                case "KnockOutPoints":
                    xpReward = 75;
                    break;
                case "KnockOutPointsInnocent":
                    if (___agent.statusEffects.hasTrait(vTrait.TheLaw) && !___agent.oma.superSpecialAbility)
                    {
                        if (___agent.statusEffects.hasTrait(vTrait.Crooked))
                            xpReward = 0;
                        else if (___agent.statusEffects.hasTrait(vTrait.Crooked2))
                            xpReward = -20;
                        else
                            xpReward = -40;
                    }
                    else
                        xpReward = 10;

                    break;
                case "KnockOutPointsRival":
                    xpReward = 150;
                    break;
                case "LockpickPoints":
                    xpReward = 20;
                    break;
                case "ManySleeping":
                    xpReward = 100;
                    break;
                case "Massacre":
                    xpReward = 100;
                    break;
                case "NoAngerLevel":
                    xpReward = 100;
                    break;
                case "NoDamageTaken":
                    xpReward = 100;
                    break;
                case "NoDestruction":
                    xpReward = 200;
                    break;
                case "NoGuns":
                    xpReward = 200;
                    break;
                case "NoKillBonus":
                    xpReward = 100;
                    break;
                case "NoKillLevel":
                    xpReward = 100;
                    break;
                case "NotAlertedBonus":
                    xpReward = 100;
                    break;
                case "OnlyFists":
                    xpReward = 200;
                    break;
                case "PickpocketPoints":
                    if (___agent.statusEffects.hasTrait(vTrait.TheLaw) && !___agent.oma.superSpecialAbility && !___agent.statusEffects.hasTrait(vTrait.PromiseIllReturnIt))
                    {
                        if (___agent.statusEffects.hasTrait(vTrait.Crooked))
                            xpReward = 0;
                        else if (___agent.statusEffects.hasTrait(vTrait.Crooked2))
                            xpReward = -10;
                        else
                            xpReward = -15;
                    }
                    else
                        xpReward = 15;

                    break;
                case "PoisonAirPoints":
                    xpReward = 20;
                    break;
                case "RemoveSlaveHelmetPoints":
                    xpReward = 20;
                    break;
                case "RemoveWindowPoints":
                    xpReward = 20;
                    break;
                case "ShakedownFailPoints":
                    xpReward = -100;
                    break;
                case "ShakedownPoints":
                    xpReward = 100;
                    break;
                case "StealPoints":
                    xpReward = extraNum * 10;
                    break;
                case "StealPointsNegative":
                    xpReward = extraNum * 10;

                    if (___agent.statusEffects.hasTrait(vTrait.TheLaw) && !___agent.oma.superSpecialAbility && !___agent.statusEffects.hasTrait(vTrait.PromiseIllReturnIt))
                    {
                        xpReward *= -1;

                        if (___agent.statusEffects.hasTrait(vTrait.Crooked))
                            xpReward /= 2;
                        else if (___agent.statusEffects.hasTrait(vTrait.Crooked2))
                            xpReward = 0;
                    }
                    break;
                case "StoleLots":
                    xpReward = 200;
                    break;
                case "TamperGeneratorPoints":
                    xpReward = 20;
                    break;
                case "TamperLaserEmitterPoints":
                    xpReward = 20;
                    break;
                case "TamperPoliceBoxPoints":
                    xpReward = 20;
                    break;
                case "TamperSatelliteDishPoints":
                    xpReward = 20;
                    break;
                case "TimeBonus":
                    xpReward = 100;
                    break;
                case "TwoPlayerWinner1":
                    xpReward = 200;
                    break;
                case "TwoPlayerWinner2":
                    xpReward = 200;
                    break;
                case "UnlockSafePoints":
                    xpReward = 20;
                    break;
                case "WonElectionPoints":
                    xpReward = 100;
                    break;
            }

            if (xpReward != 0)
            {
                float xpModifier = 1.00f;

                if (___agent.statusEffects.hasTrait(cTrait.SmoothBrained))
                    xpModifier = 0.00f;
                else if (xpReward > 0)
                {
                    if (___agent.statusEffects.hasTrait(vTrait.Studious))
                        xpModifier = 1.30f;
                    else if (___agent.statusEffects.hasTrait(vTrait.SuperStudious))
                        xpModifier = 1.50f;
                    else if (___agent.statusEffects.hasTrait(cTrait.DimBulb))
                        xpModifier = 0.75f;
                    else if (___agent.statusEffects.hasTrait(cTrait.MoronTheMerrier))
                        xpModifier = 0.50f;
                }
                else if (xpReward < 0)
                    if (___agent.statusEffects.hasTrait(cTrait.VeryHardOnYourself))
                        xpModifier = 2.00f;

                xpReward = (int)((float)xpReward * xpModifier);

                float floorXpAcceleration = 0.075f;
                int cityFloor = Mathf.Clamp(GC.sessionDataBig.curLevelEndless, 1, 16);

                if (GC.sessionDataBig.challenges.Contains("QuickGame"))
                {
                    floorXpAcceleration = 0.1125f;
                    cityFloor = Mathf.Clamp(GC.sessionDataBig.curLevelEndless, 1, 11);
                }

                xpReward = (int)((float)xpReward * (1f + (float)(cityFloor - 1) * floorXpAcceleration));
                GC.sessionData.skillPoints[___agent.isPlayer] += xpReward;
                Color32 myColor;

                if (xpReward < 0)
                {
                    myColor = new Color32(byte.MaxValue, 0, 0, byte.MaxValue);

                    if (GC.sessionData.skillPoints[___agent.isPlayer] < __instance.findLevelThreshold(GC.sessionData.skillLevel[___agent.isPlayer] - 1))
                        GC.sessionData.skillPoints[___agent.isPlayer] = __instance.findLevelThreshold(GC.sessionData.skillLevel[___agent.isPlayer] - 1);
                }
                else
                    myColor = new Color32(byte.MaxValue, 216, 0, byte.MaxValue);

                if (GC.sessionData.skillPoints[___agent.isPlayer] >= __instance.findLevelThreshold(GC.sessionData.skillLevel[___agent.isPlayer]))
                {
                    GC.audioHandler.Play(___agent, "LevelUp");

                    if (___agent.isPlayer == 1)
                        GC.alienFX.GainLevel();

                    GC.sessionData.skillLevel[___agent.isPlayer]++;
                    __instance.levelsGained++;
                    GC.sessionData.levelsGained[___agent.isPlayer]++;
                    __instance.justGainedLevel = true;

                    //__instance.StartCoroutine(__instance.CancelJustGainedLevel()); // Original Private Method Inaccessible

                    MethodInfo CancelJustGainedLevel = AccessTools.DeclaredMethod(typeof(SkillPoints), "CancelJustGainedLevel", new Type[0] { });
                    CancelJustGainedLevel.GetMethodWithoutOverrides<Action>(__instance).Invoke();

                    if (GC.unlocks.CanDoUnlocks())
                    {
                        if (___agent.statusEffects.hasTrait(vTrait.SuperStudious))
                        {
                            GC.unlocks.AddNuggets(2);
                            GC.spawnerMain.SpawnStatusText(___agent, "ItemPickupSlower", "Nuggets", "Item", "Add2Nuggets", "");
                        }
                        else
                        {
                            GC.unlocks.AddNuggets(3);
                            GC.spawnerMain.SpawnStatusText(___agent, "ItemPickupSlower", "Nuggets", "Item", "Add3Nuggets", "");
                        }
                    }

                    ___agent.objectMult.SendChatAnnouncement("LevelUp", GC.sessionData.skillLevel[___agent.isPlayer].ToString(), "");

                    if (___agent.localPlayer)
                        ___agent.skillBar.dTr.localScale = new Vector3(0f, ___agent.skillBar.dTr.localScale.y, ___agent.skillBar.dTr.localScale.z);

                    if (!___agent.finishedLevel)
                        GC.spawnerMain.SpawnStatusText(___agent, "LevelUp", "LevelUp", "Interface");

                    if (___agent.statusEffects.hasTrait(vTrait.PotentialtoNotSuck))
                    {
                        Agent agent = ___agent;

                        if (___agent.possessing || ___agent.transforming)
                        {
                            if (!GC.multiplayerMode)
                            {
                                if (___agent.isPlayer == 1)
                                    agent = GC.backupAgent1;

                                if (___agent.isPlayer == 2)
                                    agent = GC.backupAgent2;

                                if (___agent.isPlayer == 3)
                                    agent = GC.backupAgent3;

                                if (___agent.isPlayer == 4)
                                    agent = GC.backupAgent4;
                            }
                            else
                            {
                                if (___agent.playerColor == 1)
                                    agent = GC.backupAgent1;

                                if (___agent.playerColor == 2)
                                    agent = GC.backupAgent2;

                                if (___agent.playerColor == 3)
                                    agent = GC.backupAgent3;

                                if (___agent.playerColor == 4)
                                    agent = GC.backupAgent4;
                            }
                        }

                        if (GC.sessionData.skillLevel[___agent.isPlayer] % 2 == 0 && (agent.strengthStatMod != 3 || agent.enduranceStatMod != 3 || agent.accuracyStatMod != 3 || agent.speedStatMod != 3))
                        {
                            string randStatMod;
                            bool bonusStat;

                            do
                            {
                                randStatMod = GC.Choose<string>("Strength", "Endurance", "Accuracy", "Speed");
                                bonusStat = true;

                                if (randStatMod == "Strength" && agent.strengthStatMod == 3)
                                    bonusStat = false;
                                else if (randStatMod == "Endurance" && agent.enduranceStatMod == 3)
                                    bonusStat = false;
                                else if (randStatMod == "Accuracy" && agent.accuracyStatMod == 3)
                                    bonusStat = false;
                                else if (randStatMod == "Speed" && agent.speedStatMod == 3)
                                    bonusStat = false;
                            }
                            while (!bonusStat);

                            switch (randStatMod)
                            {
                                case "Accuracy":
                                    agent.SetAccuracy(agent.accuracyStatMod + 1, true);
                                    GC.spawnerMain.SpawnStatusText(___agent, "BuffSpecial", "Accuracy", "BuffSpecial", "");

                                    break;
                                case "Endurance":
                                    agent.SetEndurance(agent.enduranceStatMod + 1, true);
                                    GC.spawnerMain.SpawnStatusText(___agent, "BuffSpecial", "Endurance", "BuffSpecial", "");

                                    break;
                                case "Speed":
                                    agent.SetSpeed(agent.speedStatMod + 1, true);
                                    GC.spawnerMain.SpawnStatusText(___agent, "BuffSpecial", "Speed", "BuffSpecial", "");

                                    break;
                                case "Strength":
                                    agent.SetStrength(agent.strengthStatMod + 1, true);
                                    GC.spawnerMain.SpawnStatusText(___agent, "BuffSpecial", "Strength", "BuffSpecial", "");

                                    break;
                            }
                        }
                    }

                    if (___agent.health > 0f && !GC.menuGUI.demoOver)
                    {
                        if (___agent.possessing)
                        {
                            if (!GC.multiplayerMode)
                            {
                                if (___agent.isPlayer == 1)
                                    GC.backupAgent1.health = GC.backupAgent1.healthMax;

                                if (___agent.isPlayer == 2)
                                    GC.backupAgent2.health = GC.backupAgent2.healthMax;

                                if (___agent.isPlayer == 3)
                                    GC.backupAgent3.health = GC.backupAgent3.healthMax;

                                if (___agent.isPlayer == 4)
                                    GC.backupAgent4.health = GC.backupAgent4.healthMax;
                            }
                            else
                            {
                                if (___agent.playerColor == 1)
                                    GC.backupAgent1.health = GC.backupAgent1.healthMax;

                                if (___agent.playerColor == 2)
                                    GC.backupAgent2.health = GC.backupAgent2.healthMax;

                                if (___agent.playerColor == 3)
                                    GC.backupAgent3.health = GC.backupAgent3.healthMax;

                                if (___agent.playerColor == 4)
                                    GC.backupAgent4.health = GC.backupAgent4.healthMax;
                            }

                            GC.spawnerMain.SpawnStatusText(___agent, "HealthUpSlower", "FullHealth", "StatusEffect");
                        }
                        else
                            ___agent.statusEffects.ChangeHealth(___agent.healthMax);
                    }
                    else
                        ___agent.fullHealthAfterResurrect = true;

                    if (___agent.localPlayer && !suppressAnimation)
                    {
                        if (GC.fourPlayerMode)
                            __instance.SpawnSkillPointsStatusText(pointsType, xpReward);
                        else
                        {
                            ___agent.skillBar.StartChange(true);
                            ___agent.skillBar.StartAnim(xpReward, pointsType, myColor);
                        }
                    }

                    if (___agent.completingBigQuestLevel)
                        GC.quests.SpawnBigQuestCompletedText2(___agent, true);
                    else if (___agent.failingBigQuestLevel)
                        GC.quests.SpawnBigQuestFailedText2(___agent, true);
                }
                else if (___agent.localPlayer && !suppressAnimation)
                {
                    if (GC.fourPlayerMode)
                    {
                        __instance.SpawnSkillPointsStatusText(pointsType, xpReward);

                        if (___agent.completingBigQuestLevel)
                            GC.quests.SpawnBigQuestCompletedText2(___agent, false);
                        else if (___agent.failingBigQuestLevel)
                            GC.quests.SpawnBigQuestFailedText2(___agent, false);
                    }
                    else
                    {
                        ___agent.skillBar.StartChange(false);
                        ___agent.skillBar.StartAnim(xpReward, pointsType, myColor);
                    }
                }

                if (___agent.localPlayer)
                    ___agent.skillBar.UpdateSkillText();
            }

            BMLog("\txpReward = " + xpReward);

            yield break;
        }
        #endregion
    }
}
