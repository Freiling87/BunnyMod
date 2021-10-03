using Google2u;

namespace BunnyMod.Content.ObjectBehaviour
{
	public static class FireHydrantController
	{
		private const int RefillWaterCannon_ButtonPrice = 10;

		public static bool FireHydrant_PressedButton_Prefix(FireHydrant fireHydrant, string buttonText, int buttonPrice)
		{
			GameController gc = GameController.gameController;
			if (gc.challenges.Contains(cChallenge.AnCapistan) && buttonText == nameof(InterfaceNameDB.rowIds.RefillWaterCannon) && buttonPrice > 0)
			{
				if (!fireHydrant.moneySuccess(buttonPrice))
				{
					fireHydrant.StopInteraction();
					return false;
				}
			}
			return true;
		}

		public static void FireHydrant_DetermineButtons_Postfix(FireHydrant fireHydrant)
		{
			GameController gc = GameController.gameController;
			if (gc.challenges.Contains(cChallenge.AnCapistan))
			{
				int buttonIndex = fireHydrant.buttons.IndexOf(nameof(InterfaceNameDB.rowIds.RefillWaterCannon));
				if (buttonIndex >= 0)
				{
					fireHydrant.buttonPrices[buttonIndex] = RefillWaterCannon_ButtonPrice;
				}
			}
		}
	}
}