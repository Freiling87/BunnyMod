using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using BepInEx;
using HarmonyLib;
using UnityEngine;
using RogueLibsCore;
using System.Reflection;

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

        private static object __instance;


        public void Awake()
		{
            this.PatchPrefix(typeof(Bathtub), "Interact", GetType(), "Interact_Patch");
        }

        public static bool Interact_Patch(Agent agent)
        {

            MethodInfo baseMethod = AccessTools.DeclaredMethod(__instance.GetType().BaseType, "Interact");
            baseMethod.GetMethodWithoutOverrides<Action<Agent>>(__instance).Invoke(agent);

            this.interactingAgent.statusEffects.BecomeHidden(this);

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
