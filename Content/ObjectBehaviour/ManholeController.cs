using BunnyMod.Content.Extensions;
using BunnyMod.Content.Traits;
using Google2u;
using RogueLibsCore;

namespace BunnyMod.Content.ObjectBehaviour
{
	public static class ManholeController
	{
		private const int crowbarTamperCost = 15;

		public static void AddButtons(Manhole manhole)
		{
			Agent agent = manhole.interactingAgent;
			if (!manhole.opened)
			{
				if (agent.inventory.HasItem(ItemNameDB.rowIds.Crowbar))
				{
					int crowbarCount = agent.inventory.FindItem(ItemNameDB.rowIds.Crowbar).invItemCount;
					int toolCost = BMTraitController.ApplyToolCostModifiers(agent, crowbarTamperCost);
					manhole.AddButton(
							text: "UseCrowbar",
							extraText: $" ({crowbarCount}) -{toolCost}"
					);	
				}
			}
			else
			{
				if (agent.HasTrait<UnderdarkCitizen>())
				{
					manhole.AddButton(text: "FlushYourself");
				}
			}
		}
	}
}