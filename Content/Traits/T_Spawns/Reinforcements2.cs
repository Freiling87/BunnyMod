using BunnyMod.Content.Extensions;
using JetBrains.Annotations;
using RogueLibsCore;

namespace BunnyMod.Content.Traits
{
	public class Reinforcements2 : CustomTrait
	{
		private const string name = nameof(Reinforcements2);

		[RLSetup]
		[UsedImplicitly]
		public static void Setup()
		{
			TraitBuilder traitBuilder = RogueLibs.CreateCustomTrait<Reinforcements2>()
					.Localize<Reinforcements2>()
					.WithUnlock(new TraitUnlock(name, true)
							.SetAvailable(true)
							.SetAvailableInCharacterCreation(true)
							.SetCantLose(true)
							.SetCantSwap(false)
							.SetCharacterCreationCost(8)
							.SetEnabled(true)
					);

			BMTraitsManager.RegisterTrait<Reinforcements2>(new BMTraitInfo(name, traitBuilder));
		}

		public override void OnAdded() { }

		public override void OnRemoved() { }

		public static void SetupAgentStats(Agent agent)
		{
			if (!agent.HasTrait<Reinforcements2>())
			{
				return;
			}

			switch (agent.agentName)
			{
				case vAgent.ResistanceLeader:
					agent.SetStrength(3);
					agent.SetEndurance(3);
					agent.inventory.startingHeadPiece = vArmorHead.SoldierHelmet;
					break;
				case cAgent.ResistanceSoldier:
					agent.inventory.startingHeadPiece = vArmorHead.SoldierHelmet;
					agent.inventory.AddItemPlayerStart(vItem.MachineGun, 0);
					agent.inventory.AddItemPlayerStart(vItem.Grenade, 5);
					agent.inventory.AddItemPlayerStart(vItem.Knife, 100);
					break;
			}
		}

		public static void EquipReinforcement(Agent agent)
		{
			InvItem item = new InvItem
					{ invItemName = gc.Choose(vItem.Revolver, vItem.MachineGun) };
			item.ItemSetup(false);
			item.invItemCount = item.rewardCount;

			agent.inventory.AddItemAtEmptySlot(item, true, false);
			agent.inventory.equippedWeapon = item;

			agent.inventory.startingHeadPiece = vArmorHead.SoldierHelmet;
		}
	}
}