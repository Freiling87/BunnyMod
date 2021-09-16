using BunnyMod.Content.Extensions;
using JetBrains.Annotations;
using RogueLibsCore;

namespace BunnyMod.Content.Traits
{
	public class Reinforcements : CustomTrait
	{
		private const string name = nameof(Reinforcements);

		[RLSetup]
		[UsedImplicitly]
		private static void Setup()
		{
			TraitBuilder traitBuilder = RogueLibs.CreateCustomTrait<Reinforcements>()
					.Localize<Reinforcements>()
					.WithUnlock(new TraitUnlock(name, true)
							.SetAvailable(true)
							.SetAvailableInCharacterCreation(true)
							.SetCantLose(true)
							.SetCantSwap(false)
							.SetCharacterCreationCost(4)
							.SetEnabled(true)
					);

			BMTraitsManager.RegisterTrait<Reinforcements>(new BMTraitInfo(name, traitBuilder)
					.WithUpgrade(typeof(Reinforcements2))
			);
		}

		public override void OnAdded() { }

		public override void OnRemoved() { }

		public static void SetupAgentStats(Agent agent)
		{
			if (!agent.HasTrait<Reinforcements>())
			{
				return;
			}

			switch (agent.agentName)
			{
				case vAgent.ResistanceLeader:
					agent.SetStrength(3);
					agent.SetEndurance(2);
					agent.inventory.startingHeadPiece = vArmorHead.HardHat;
					break;
				case cAgent.ResistanceSoldier:
					agent.inventory.startingHeadPiece = vArmorHead.HardHat;
					agent.inventory.AddItemPlayerStart(vItem.Pistol, 0);
					agent.inventory.AddItemPlayerStart(vItem.Knife, 100);
					break;
			}
		}

		public static void EquipReinforcement(Agent agent)
		{
			InvItem item = new InvItem
					{ invItemName = gc.Choose(vItem.Pistol, vItem.Knife) };
			item.ItemSetup(false);
			item.invItemCount = item.rewardCount;

			agent.inventory.AddItemAtEmptySlot(item, true, false);
			agent.inventory.equippedWeapon = item;

			agent.inventory.startingHeadPiece = vArmorHead.HardHat;
		}
	}
}