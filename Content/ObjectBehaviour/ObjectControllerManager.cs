using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace BunnyMod.ObjectBehaviour
{
	public static class ObjectControllerManager
	{
		private static readonly Dictionary<Type, IObjectController<PlayfieldObject>> objectControllers = new Dictionary<Type, IObjectController<PlayfieldObject>>();
		private static readonly Dictionary<Type, IObjectDataController> dataControllers = new Dictionary<Type, IObjectDataController>();

		public static void RegisterObjectController<T>(IObjectController<T> controller) where T : PlayfieldObject
		{
			objectControllers[typeof(T)] = new ObjectControllerAccessor<T>(controller);
		}

		/// <summary>
		/// Register a DataController to be automatically managed
		/// </summary>
		/// <param name="dataController">dataController to register</param>
		/// <typeparam name="TargetType">Type of the object to store data for</typeparam>
		public static void RegisterDataController<TargetType>(IObjectDataController dataController)
				where TargetType : PlayfieldObject
		{
			dataControllers[typeof(TargetType)] = dataController;
		}

		[CanBeNull]
		public static IObjectController<PlayfieldObject> GetObjectController(PlayfieldObject objectInstance)
		{
			if (objectInstance is null)
			{
				return null;
			}

			Type objectType = objectInstance.GetType();
			return objectControllers.ContainsKey(objectType) ? objectControllers[objectType] : null;
		}

		[CanBeNull]
		private static IObjectDataController GetDataController(PlayfieldObject objectInstance)
		{
			if (objectInstance is null)
			{
				return null;
			}

			Type objectType = objectInstance.GetType();
			return dataControllers.ContainsKey(objectType) ? dataControllers[objectType] : null;
		}

		public static void RevertAllVars<T>(T objectInstance) where T : PlayfieldObject
		{
			GetDataController(objectInstance)?.RevertAllVars(objectInstance);
			GetObjectController(objectInstance)?.HandleRevertAllVars(objectInstance);
		}

		public static void OnLevelChange()
		{
			foreach (IObjectDataController dataController in dataControllers.Values)
			{
				dataController.ClearData();
			}
		}
	}
}