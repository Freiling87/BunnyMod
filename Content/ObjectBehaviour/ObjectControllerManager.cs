using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using RogueLibsCore;

namespace BunnyMod.Content.ObjectBehaviour
{
	public static class ObjectControllerManager
	{
		public delegate void OnChangeLevel();
		public static event OnChangeLevel ChangeLevelEvent;

		public static void TriggerChangeLevelEvent()
		{
			ChangeLevelEvent?.Invoke();
		}
		
		private static readonly Dictionary<Type, ObjectControllerInterface<PlayfieldObject>> controllers =
				new Dictionary<Type, ObjectControllerInterface<PlayfieldObject>>();

		[RLSetup, UsedImplicitly]
		private static void Setup_Controllers()
		{
			RegisterController(new AlarmButtonController());
			RegisterController(new ElevatorController());
			RegisterController(new FlamingBarrelController());
			RegisterController(new FountainController());
			RegisterController(new ManholeController());
			RegisterController(new SecurityCamController());
			RegisterController(new StoveController());
			RegisterController(new TrashCanController());
			RegisterController(new VendorCartController());
		}

		private static void RegisterController<T>(ObjectControllerInterface<T> controller) where T : PlayfieldObject
		{
			controllers[typeof(T)] = new ObjectControllerAccessor<T>(controller);
		}

		[CanBeNull]
		public static ObjectControllerInterface<PlayfieldObject> GetController(PlayfieldObject objectInstance)
		{
			if (objectInstance is null)
			{
				return null;
			}

			Type objectType = objectInstance.GetType();
			return !controllers.ContainsKey(objectType)
					? null
					: controllers[objectType];
		}
	}
}