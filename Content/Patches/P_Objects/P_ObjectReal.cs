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
	}
}