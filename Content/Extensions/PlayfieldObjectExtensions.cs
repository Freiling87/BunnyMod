namespace BunnyMod.Content.Extensions
{
	public static class PlayfieldObjectExtensions
	{
		/// <summary>
		/// This method ensures that the text, extraText and price button lists will always match up, even if some buttons don't have extraText / prices
		/// </summary>
		/// <param name="playfieldObject">instance of the playFieldObject to add buttons to</param>
		/// <param name="text">base text for the button</param>
		/// <param name="extraText">extra text for the button</param>
		/// <param name="price">price value for the button</param>
		public static void AddButton(this PlayfieldObject playfieldObject, string text, string extraText = null, int price = 0)
		{
			int initialButtonCount = playfieldObject.buttons.Count;
			int initialButtonExtraCount = playfieldObject.buttonsExtra.Count;
			int initialButtonPriceCount = playfieldObject.buttonPrices.Count;
			
			// Always add the main button.
			playfieldObject.buttons.Add(text);

			// if provided extraText, or other buttons already have extraText, add extraText.
			if (extraText != null || initialButtonExtraCount > 0)
			{
				// fill buttonsExtra with null until size equals buttonCount
				for (int i = initialButtonExtraCount; i < initialButtonCount; i++)
				{
					playfieldObject.buttonsExtra.Add(null);
				}
				// then add the new extraText
				playfieldObject.buttonsExtra.Add(extraText);
			}

			// if provided price, or other buttons already have prices, add price
			if (price != 0 || initialButtonPriceCount > 0)
			{
				// fill buttonsPrice with null until size equals buttonCount
				for(int i = initialButtonPriceCount; i < initialButtonCount; i++)
				{
					playfieldObject.buttonPrices.Add(0);
				}
				// then add the new price
				playfieldObject.buttonPrices.Add(price);
			}
		}
	}
}