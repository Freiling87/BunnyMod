using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using UnityEngine;
using RogueLibsCore;

namespace BunnyMod
{
	public class BunnyHeader : BaseUnityPlugin
	{
		public const string pluginGuid = "freiling87.streetsofrogue.bunnymod";
		public const string pluginName = "Bunny Mod";
		public const string pluginVersion = "1.1.2";

		public static ManualLogSource ConsoleMessage;
		public static BaseUnityPlugin MainInstance;

		public void Awake()
		{
			MainInstance = this;

			ConsoleMessage = Logger;
			RoguePatcher patcher = new RoguePatcher(this, GetType());

			//new BunnyItems().Awake();
			new BunnyObjects().Awake();
			//new BunnyTraits().Awake();
		}
	}

	public static class BunnyHeaderTools
	{
		public static T GetMethodWithoutOverrides<T>(this MethodInfo method, object callFrom)
			where T : Delegate
		{
			IntPtr ptr = method.MethodHandle.GetFunctionPointer();
			return (T)Activator.CreateInstance(typeof(T), callFrom, ptr);
		}
	}
}
