using UnityEngine;

namespace BunnyMod.Content.Extensions
{
	public static class PlayfieldObjectExtensions
	{
		/// <summary>
		/// This method ensures that there is an equal amount of buttons, buttonsExtra and buttonsPrices on the object
		/// </summary>
		/// <param name="playfieldObject">instance of the playfieldObject to normalize buttons on</param>
		public static void NormalizeButtons(this PlayfieldObject playfieldObject)
		{
			int buttonCount = playfieldObject.buttons.Count;
			int buttonExtraCount = playfieldObject.buttonsExtra.Count;
			int buttonPriceCount = playfieldObject.buttonPrices.Count;
			int buttonTotal = Mathf.Max(buttonCount, buttonExtraCount, buttonPriceCount);

			for (int i = buttonCount; i < buttonTotal; i++)
			{
				playfieldObject.buttons.Add(null);
			}

			for (int i = buttonExtraCount; i < buttonTotal; i++)
			{
				playfieldObject.buttonsExtra.Add(null);
			}

			for (int i = buttonPriceCount; i < buttonTotal; i++)
			{
				playfieldObject.buttonPrices.Add(0);
			}
		}

		/// <summary>
		/// This method ensures that the text, extraText and price button lists will always match up, even if some buttons don't have extraText / prices
		/// </summary>
		/// <param name="playfieldObject">instance of the playFieldObject to add buttons to</param>
		/// <param name="text">base text for the button</param>
		/// <param name="extraText">extra text for the button</param>
		/// <param name="price">price value for the button</param>
		public static void AddButton(this PlayfieldObject playfieldObject, string text, string extraText = null, int price = 0)
		{
			NormalizeButtons(playfieldObject);
			playfieldObject.buttons.Add(text);
			playfieldObject.buttonsExtra.Add(extraText);
			playfieldObject.buttonPrices.Add(price);
		}

		/// <summary>
		/// This method removes all buttons from the button-lists with the specified buttonText
		/// </summary>
		/// <param name="playfieldObject">instance of the playfieldObject to remove buttons from</param>
		/// <param name="text">base text of the buttons to remove</param>
		/// <returns>amount of</returns>
		public static int RemoveButton(this PlayfieldObject playfieldObject, string text)
		{
			NormalizeButtons(playfieldObject);
			int removals = 0;
			for (int i = playfieldObject.buttons.Count - 1; i >= 0; i--)
			{
				if (playfieldObject.buttons[i] == text)
				{
					playfieldObject.buttons.RemoveAt(i);
					playfieldObject.buttonsExtra.RemoveAt(i);
					playfieldObject.buttonPrices.RemoveAt(i);
					removals++;
				}
			}
			return removals;
		}
	}
}