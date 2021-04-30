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
			Item_00();
			ObjectReal_00();
			PlayerControl_00();
		}
		#endregion

		#region Custom
		public static float GetZoomLevel(Agent playerAgent)
		{
			if (playerAgent.statusEffects.hasTrait(cTrait.EagleEyes))
				return 0.70f;
			else if (playerAgent.statusEffects.hasTrait(cTrait.EagleEyes_2))
				return 0.40f;
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
		}
		// AgentOnCamera seems to only work on PlayerAgents and their own cameras. 
		// NPCs might be handled via ObjectReal?
		#endregion
		#region Item
		public void Item_00()
		{
			Type t = typeof(Item);
			Type g = GetType();

			Prefix(t, "ItemOnCamera", g, "Item_ItemOnCamera", new Type[0] { });
		}
		public static bool Item_ItemOnCamera(Item __instance, ref bool __result) // Replacement
		{
			// Eagle Eye activation range

			if ((!GC.coopMode && !GC.fourPlayerMode) || GC.splitScreen)
			{
				float cameraWidth = 13f / GetZoomLevel();
				float cameraHeight = 8f / GetZoomLevel();
				Vector2 spot = GC.playerAgent.agentCamera.curPosition;

				if (spot.x > __instance.curPosition.x - cameraWidth && 
					spot.x < __instance.curPosition.x + cameraWidth && 
					spot.y > __instance.curPosition.y - cameraHeight && 
					spot.y < __instance.curPosition.y + cameraHeight)
				{
					__instance.onCamera = true;

					__result = true;
					return false;
				}

				if (GC.coopMode || GC.fourPlayerMode)
				{
					spot = GC.playerAgent2.agentCamera.curPosition;

					if (spot.x > __instance.curPosition.x - cameraWidth &&
						spot.x < __instance.curPosition.x + cameraWidth && 
						spot.y > __instance.curPosition.y - cameraHeight && 
						spot.y < __instance.curPosition.y + cameraHeight)
					{
						__instance.onCamera = true;

						__result = true;
						return false;
					}

					if (GC.fourPlayerMode)
					{
						spot = GC.playerAgent3.agentCamera.curPosition;

						if (spot.x > __instance.curPosition.x - cameraWidth && 
							spot.x < __instance.curPosition.x + cameraWidth && 
							spot.y > __instance.curPosition.y - cameraHeight &&
							spot.y < __instance.curPosition.y + cameraHeight)
						{
							__instance.onCamera = true;

							__result = true;
							return false;
						}

						if (!GC.sessionDataBig.threePlayer)
						{
							spot = GC.playerAgent4.agentCamera.curPosition;

							if (spot.x > __instance.curPosition.x - cameraWidth && 
								spot.x < __instance.curPosition.x + cameraWidth && 
								spot.y > __instance.curPosition.y - cameraHeight &&
								spot.y < __instance.curPosition.y + cameraHeight)
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
	}
}
