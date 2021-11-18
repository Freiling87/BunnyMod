using Debug = System.Diagnostics.Debug;

namespace BunnyMod.ObjectBehaviour
{
	/// <summary>
	/// This class acts as an abstraction layer for ObjectControllers
	/// 
	/// The goal is to allow grouping ObjectControllers as IObjectController&lt;ObjectReal&gt;
	/// while still providing the derived Type to the underlying Controller.
	/// </summary>
	/// <typeparam name="TargetType">ObjectType of the underlying controller</typeparam>
	public class ObjectControllerAccessor<TargetType>
			: IObjectController<PlayfieldObject>
			where TargetType : PlayfieldObject
	{
		private readonly IObjectController<TargetType> controllerImplementation;

		public ObjectControllerAccessor(IObjectController<TargetType> controllerImplementation)
		{
			this.controllerImplementation = controllerImplementation;
		}

		private TargetType GetAsObjectType(PlayfieldObject playfieldObject)
		{
			TargetType objectInstance = playfieldObject as TargetType;
			Debug.Assert(objectInstance != null, $"{GetType().Name} called with ObjectReal that wasn't of Type {typeof(TargetType).Name}!");
			return objectInstance;
		}

		public void HandleRevertAllVars(PlayfieldObject objectInstance)
		{
			
			controllerImplementation.HandleRevertAllVars(GetAsObjectType(objectInstance));
		}

		public void HandleObjectUpdate(PlayfieldObject objectInstance)
		{
			controllerImplementation.HandleObjectUpdate(GetAsObjectType(objectInstance));
		}

		public void HandlePlayerHasUsableItem(PlayfieldObject objectInstance, InvItem itemToTest, ref bool result)
		{
			controllerImplementation.HandlePlayerHasUsableItem(GetAsObjectType(objectInstance), itemToTest, ref result);
		}

		public void HandlePressedButton(PlayfieldObject objectInstance, string buttonText, int buttonPrice)
		{
			controllerImplementation.HandlePressedButton(GetAsObjectType(objectInstance), buttonText, buttonPrice);
		}

		public void HandleDetermineButtons(PlayfieldObject objectInstance)
		{
			controllerImplementation.HandleDetermineButtons(GetAsObjectType(objectInstance));
		}

		public void HandleFinishedOperating(PlayfieldObject objectInstance)
		{
			controllerImplementation.HandleFinishedOperating(GetAsObjectType(objectInstance));
		}

		public void HandleInteract(PlayfieldObject objectInstance, Agent agent)
		{
			controllerImplementation.HandleInteract(GetAsObjectType(objectInstance), agent);
		}

		public void HandleObjectAction(PlayfieldObject objectInstance, string action, ref bool noMoreObjectActions, string extraString, float extraFloat, Agent causerAgent, PlayfieldObject extraObject)
		{
			controllerImplementation.HandleObjectAction(GetAsObjectType(objectInstance), action, ref noMoreObjectActions, extraString, extraFloat, causerAgent, extraObject);
		}

		public void HandleDamagedObject(PlayfieldObject objectInstance, PlayfieldObject damagerObject, float damageAmount)
		{
			controllerImplementation.HandleDamagedObject(GetAsObjectType(objectInstance), damagerObject, damageAmount);
		}

		public void HandleMakeNonFunctional(PlayfieldObject objectInstance, PlayfieldObject damagerObject)
		{
			controllerImplementation.HandleMakeNonFunctional(GetAsObjectType(objectInstance), damagerObject);
		}

		public void HandleDestroyMe(PlayfieldObject objectInstance, PlayfieldObject damagerObject)
		{
			controllerImplementation.HandleDestroyMe(GetAsObjectType(objectInstance), damagerObject);
		}

		public void HandleDestroyMe3(PlayfieldObject objectInstance)
		{
			controllerImplementation.HandleDestroyMe3(GetAsObjectType(objectInstance));
		}
	}
}