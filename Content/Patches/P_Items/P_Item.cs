using BepInEx.Logging;
using BunnyMod.Content.Logging;
using BunnyMod.Content.Traits;
using HarmonyLib;
using UnityEngine;
using RogueLibsCore;
using Random = UnityEngine.Random;

namespace BunnyMod.Content.Patches
{
	[HarmonyPatch(declaringType: typeof(Item))]
	public static class P_Item
	{
		private static readonly ManualLogSource logger = BMLogger.GetLogger();
		public static GameController GC => GameController.gameController;

		[HarmonyPrefix, HarmonyPatch(methodName:nameof(Item.SpecialDamage), argumentTypes:new[] { typeof(PlayfieldObject) })]
		public static bool Item_SpecialDamage(PlayfieldObject damagerObject, Item __instance)
		{
			string itemName = __instance.invItem.invItemName;

			if (CustomListDump.customThrowables.Contains(itemName) && damagerObject.CompareTag("Agent"))
			{
				Agent agent = (Agent)damagerObject;

				if (itemName == cItem.BeerCan)
					GC.audioHandler.Play(agent, vAudioClip.BulletHitObject);
				else if (itemName == cItem.ManholeCover)
					GC.audioHandler.Play(agent, vAudioClip.MeleeHitAgentLarge);
				else if (itemName == cItem.Sawblade)
					GC.audioHandler.Play(agent, vAudioClip.SawBladeHit);
				else if (itemName == cItem.ThrowingKnife)
					GC.audioHandler.Play(agent, vAudioClip.MeleeHitAgentCutSmall2);
				else if (itemName == cItem.WhiskeyBottle)
					GC.audioHandler.Play(agent, vAudioClip.WindowDamage);
				else
					GC.audioHandler.Play(agent, vAudioClip.MeleeHitAgentLarge);

				GC.spawnerMain.SpawnParticleEffect("ObjectDestroyed", __instance.tr.position, __instance.tr.eulerAngles.z);

				if (agent.inhuman || agent.mechFilled || agent.mechEmpty)
				{
					GC.spawnerMain.SpawnParticleEffect("BloodHitYellow", agent.tr.position, __instance.tr.eulerAngles.z);
					GC.playerAgent.objectMultPlayfield.SpawnParticleEffect("BloodHitYellow", agent.tr.position, __instance.tr.eulerAngles.z, false, agent);
				}
				else
				{
					GC.spawnerMain.SpawnParticleEffect("BloodHit", agent.tr.position, __instance.tr.eulerAngles.z);
					GC.playerAgent.objectMultPlayfield.SpawnParticleEffect("BloodHit", agent.tr.position, __instance.tr.eulerAngles.z, false, agent);
				}

				__instance.DestroyMeFromClient();

				return false;
			}

			return true;
		}
	}
}
