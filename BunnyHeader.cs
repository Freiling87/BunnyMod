using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using UnityEngine;
using RogueLibsCore;
using System.Threading.Tasks;

namespace BunnyMod
{
	[BepInPlugin(pluginGuid, pluginName, pluginVersion)]
	[BepInProcess("StreetsOfRogue.exe")]
	[BepInDependency(RogueLibs.pluginGuid, "2.0")]

	public class BunnyHeader : BaseUnityPlugin
	{
		public const string pluginGuid = "freiling87.streetsofrogue.bunnymod";
		public const string pluginName = "Bunny Mod";
		public const string pluginVersion = "1.2.0";

		public static ManualLogSource ConsoleMessage;
		public static GameController gc = GameController.gameController;
		public static BaseUnityPlugin MainInstance;

		public void Awake()
		{
			MainInstance = this;

			ConsoleMessage = Logger;

			//new BunnyBehaviors().Awake();
			//new BunnyItems().Awake();
			new BunnyObjects().Awake();
			new BunnyTraits().Awake();
		}
		#region ----Category Lists
		public static List<string> alcohol = new List<string>() { "Beer", "Cocktail", "Whiskey" };
		public static List<string> drugs = new List<string>() { "Antidote", "Cigarettes", "Cocaine", "CritterUpper", "CyanidePill", "ElectroPill", "Giantizer", "KillerThrower", "RagePoison", "Shrinker", "Steroids", "Syringe" };
		public static List<string> nonVegetarian = new List<string>() { "BaconCheeseburger", "HamSandwich" };
		public static List<string> vegetarian = new List<string>() { "Banana", "Fud", "HotFud" };

		public static List<string> blunt = new List<string>() { };
		public static List<string> explosive = new List<string>() { };
		public static List<string> heavy = new List<string>() { "Axe", "BaseballBat", "BearTrap", "BombTrigger", "BulletproofVest", "FireExtinguisher", "FireproofSuit", "Flamethrower", "GhostBlaster", "LandMine", "MachineGun", "Revolver", "RocketLauncher", "Shotgun", "Sledgehammer", "Wrench" };
		public static List<string> loud = new List<string>() { "BoomBox", "DizzyGrenade", "DoorDetonator", "EMPGrenade", "ExplosiveStimulator", "FireExtinguisher", "Fireworks", "GhostBlaster", "Grenade", "HearingBlocker", "Leafblower", "LandMine", "MachineGun", "MolotovCocktail", "Pistol", "RemoteBomb", "Revolver", "RocketLauncher", "Shotgun", "TimeBomb", "WarpGrenade" };
		public static List<string> piercing = new List<string>() { "Axe", "BearTrap", "Grenade", "Knife", "LandMine", "MachineGun", "Pistol", "Revolver", "RocketLauncher", "Shotgun", "Shuriken", "Sword" };
		public static List<string> tools = new List<string> { "Crowbar", "Wrench" };
		#endregion
		public static void Log (string logMessage) =>
			ConsoleMessage.LogMessage(logMessage);
	}

	public static class BunnyHeaderTools
	{
		public static T GetMethodWithoutOverrides<T>(this MethodInfo method, object callFrom)
			where T : Delegate
		{
			IntPtr ptr = method.MethodHandle.GetFunctionPointer();
			return (T)Activator.CreateInstance(typeof(T), callFrom, ptr);
		}
		public static void InvokeRepeating(object instance, string method, float delay, float interval)
		{
			MethodInfo method2 = AccessTools.Method(instance.GetType(), method);
			Task task = InvokeRepeating2(instance, method2, (int)Mathf.Floor(delay * 1000), (int)Mathf.Floor(interval * 1000));
		}
		private static async Task InvokeRepeating2(object instance, MethodInfo method, int delay, int interval)
		{
			await Task.Delay(delay);
			while (true)
			{
				method.Invoke(instance, new object[0]);
				await Task.Delay(interval);
			}
		}

		public static void Set(this object obj, params Func<string, object>[] hash)
		{
			foreach (Func<string, object> member in hash)
			{
				var propertyName = member.Method.GetParameters()[0].Name;
				var propertyValue = member(string.Empty);
				obj.GetType()
					.GetProperty(propertyName)
						.SetValue(obj, propertyValue, null);
			};
		}
	}
}
