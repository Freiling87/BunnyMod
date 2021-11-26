namespace BunnyMod.ObjectBehaviour
{
	public interface IObjectController<in TargetType> where TargetType : PlayfieldObject
	{
		void HandleRevertAllVars(TargetType objectInstance);
		
		void HandleObjectUpdate(TargetType objectInstance);

		void HandlePlayerHasUsableItem(TargetType objectInstance, InvItem itemToTest, ref bool result);
		
		void HandlePressedButton(TargetType objectInstance, string buttonText, int buttonPrice);
		
		void HandleDetermineButtons(TargetType objectInstance);

		void HandleFinishedOperating(TargetType objectInstance);
		
		void HandleInteract(TargetType objectInstance, Agent agent);
		
		void HandleObjectAction(TargetType objectInstance, string action, ref bool noMoreObjectActions, string extraString, float extraFloat, Agent causerAgent, PlayfieldObject extraObject);

		void HandleDamagedObject(TargetType objectInstance, PlayfieldObject damagerObject, float damageAmount);
		
		void HandleMakeNonFunctional(TargetType objectInstance, PlayfieldObject damagerObject);
		
		void HandleDestroyMe(TargetType objectInstance, PlayfieldObject damagerObject);
		
		void HandleDestroyMe3(TargetType objectInstance);
	}
}