using System.Reflection;
using BepInEx.Logging;
using BunnyMod.Content.Extensions;
using BunnyMod.Content.ObjectBehaviour;
using HarmonyLib;
using BunnyMod.Content.Logging;
using UnityEngine;
using RogueLibsCore;
using Random = UnityEngine.Random;

namespace BunnyMod.Content.Patches
{
	[HarmonyPatch(declaringType: typeof(ObjectReal))]
	public static class P_ObjectReal
	{
		private static readonly ManualLogSource logger = BMLogger.GetLogger();
		public static GameController GC => GameController.gameController;

		[HarmonyPrefix, HarmonyPatch(methodName: nameof(ObjectReal.DestroyMe), argumentTypes: new[] { typeof(PlayfieldObject) })]
		private static void DestroyMe_Prefix(PlayfieldObject damagerObject, ObjectReal __instance)
		{
			logger.LogDebug($"{__instance.name}: {MethodBase.GetCurrentMethod().Name}");
			if (__instance is Stove stove)
			{
				StoveController.RegisterDamagedBy(stove, damagerObject);
			}
		}

		[HarmonyPrefix, HarmonyPatch(methodName: nameof(ObjectReal.DestroyMe3))]
		private static void DestroyMe3_Prefix(ObjectReal __instance)
		{
			if (__instance is Stove stove)
			{
				StoveController.TriggerExplosion(stove);
			}
		}

		[HarmonyPostfix, HarmonyPatch(methodName: nameof(ObjectReal.DetermineButtons))]
		private static void DetermineButtons_Postfix(ObjectReal __instance)
		{
			switch (__instance)
			{
				case FlamingBarrel barrel:
					FlamingBarrelController.AddButtons(barrel);
					break;
				case Stove stove:
					StoveController.AddButtons(stove);
					break;
				case TrashCan trashCan:
					// TODO if I ended up creating a TrashCanController class, move this there instead
					trashCan.AddButton(text: cButtonText.HideInContainer);
					trashCan.AddButton(text: cButtonText.OpenContainer);
					break;
				case VendorCart vendorCart:
					// TODO if I ended up creating a VendorCartController class, move this there instead
					vendorCart.AddButton(text: cButtonText.StealItem);
					break;
			}
		}

		[HarmonyPrefix, HarmonyPatch(methodName: nameof(ObjectReal.ObjectRealOnCamera))]
		public static bool ObjectReal_ObjectRealOnCamera(ObjectReal __instance, ref bool __result) 
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
				float cameraWidth = 13f / BMInterface.GetZoomLevel();
				float cameraHeight = 8f / BMInterface.GetZoomLevel();
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

	}
}