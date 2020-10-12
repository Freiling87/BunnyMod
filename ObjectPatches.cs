using BepInEx;
using HarmonyLib;
using RogueLibsCore;
using System;
using System.Reflection;
using UnityEngine;

using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace BunnyMod
{
	[BepInPlugin(pluginGuid, pluginName, pluginVersion)]
    [BepInProcess("StreetsOfRogue.exe")]
    [BepInDependency(RogueLibs.pluginGuid, "1.0")]

    public class ObjectPatches : BaseUnityPlugin
    {
        public const string pluginGuid = "freiling87.streetsofrogue.bunnymod";
        public const string pluginName = "Bunny Mod";
        public const string pluginVersion = "1.0.0.0";

        public void Awake()
		{
            Logger.LogInfo("BunnyMod loaded");
            this.PatchPrefix(typeof(ObjectReal), "Interact", GetType(), "Interact_Patch");
        }

        public static bool Interact_Patch(Agent agent, Bathtub __instance)
        {
            if (!(__instance is Bathtub b)) return true;

            MethodInfo baseMethod = AccessTools.DeclaredMethod(__instance.GetType().BaseType, "Interact");
            baseMethod.GetMethodWithoutOverrides<Action<Agent>>(__instance).Invoke(agent);

            agent.statusEffects.BecomeHidden(__instance);

            baseMethod = AccessTools.DeclaredMethod(__instance.GetType().BaseType, "StopInteraction");
            baseMethod.GetMethodWithoutOverrides<Action>(__instance).Invoke();

            return false;
        }
    }

    public static class SSS
    {
        public static T GetMethodWithoutOverrides<T>(this MethodInfo method, object callFrom)
            where T : Delegate
        {
            IntPtr ptr = method.MethodHandle.GetFunctionPointer();
            return (T)Activator.CreateInstance(typeof(T), callFrom, ptr);
        }
    }
}
