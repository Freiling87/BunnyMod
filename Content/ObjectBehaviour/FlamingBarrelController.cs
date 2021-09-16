using BunnyMod.Content.Extensions;
using BunnyMod.Content.Traits;
using Google2u;

namespace BunnyMod.Content.ObjectBehaviour
{
	public static class FlamingBarrelController
	{
		public static void AddButtons(FlamingBarrel barrel)
		{
			Agent agent = barrel.interactingAgent;
			if (barrel.ora.hasParticleEffect)
			{
				if (agent.inventory.HasItem(ItemNameDB.rowIds.Fud))
				{
					barrel.AddButton(
							text: "GrillFud",
							extraText: $" (Burn hands for {BMTraitController.HealthCost(agent, 10, DamageType.burnedFingers)} damage)"
					);
				}
				else
				{
					BMHeaderTools.SayDialogue(agent, "CantGrillFud", vNameType.Dialogue);
				}
			}
			else
			{
				if (agent.inventory.HasItem(ItemNameDB.rowIds.CigaretteLighter))
				{
					barrel.AddButton(text: "LightBarbecue");
				}
				else
				{
					BMHeaderTools.SayDialogue(agent, "CantOperateBarbecue", vNameType.Dialogue);
				}
			}
		}
	}
}