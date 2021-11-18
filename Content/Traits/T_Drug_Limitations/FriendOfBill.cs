using BunnyMod.Extensions;
using JetBrains.Annotations;
using RogueLibsCore;

namespace BunnyMod.Traits.T_Drug_Limitations
{
	public class FriendOfBill : CustomTrait
	{
		private const string name = nameof(FriendOfBill);

		[RLSetup]
		[UsedImplicitly]
		private static void Setup()
		{
			TraitBuilder traitBuilder = RogueLibs.CreateCustomTrait<FriendOfBill>()
					.Localize<FriendOfBill>()
					.WithUnlock(new TraitUnlock(name, true)
							.SetAvailable(true)
							.SetAvailableInCharacterCreation(true)
							.SetCantLose(false)
							.SetCantSwap(true)
							.SetCharacterCreationCost(-1)
							.SetEnabled(true)
					);

			// TODO conflict with vTrait.Addict
			BMTraitsManager.RegisterTrait<FriendOfBill>(new BMTraitInfo(name, traitBuilder)
					.WithConflictGroup(ETraitConflictGroup.LimitDrugs)
			);
		}

		public override void OnAdded() { }

		public override void OnRemoved() { }
	}
}