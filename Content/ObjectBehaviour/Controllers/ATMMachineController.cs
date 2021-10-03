using BunnyMod.Content.Extensions;
using BunnyMod.Content.Traits;
using JetBrains.Annotations;
using RogueLibsCore;

namespace BunnyMod.Content.ObjectBehaviour
{
	public static class ATMMachineController
	{
		private const string PayCops_ButtonText = "PayCops"; // Vanilla string

		/// <returns>true if Buttons were added</returns>
		[UsedImplicitly]
		public static bool AddButtonsIfPossible(ATMMachine atmMachine)
		{
			Agent agent = atmMachine.interactingAgent;
			bool didAddButton = false;
			if (!agent.interactionHelper.interactingFar)
			{
				bool alreadyHasPayCopsButton = atmMachine.buttons.Contains(PayCops_ButtonText);
				if (!alreadyHasPayCopsButton && agent.HasTrait<Priors>())
				{
					atmMachine.AddButton(
							text: PayCops_ButtonText,
							price: atmMachine.determineMoneyCost("PayCops2")
					);
					didAddButton = true;
				}
			}
			return didAddButton;
		}
		
		public static void HandlePayCops(ATMMachine atmMachine)
		{
			Agent agent = atmMachine.interactingAgent;
			if (agent.HasTrait<Priors>())
			{
				// Argh, why doesn't vanilla PayCops use the stupid button-price like every other PressedButton implementation ?!
				if (!atmMachine.moneySuccess(atmMachine.determineMoneyCost("PayCops2")))
				{
					atmMachine.StopInteraction();
					return;
				}
				atmMachine.DidPayCops(agent);
			}
		}
	}
}