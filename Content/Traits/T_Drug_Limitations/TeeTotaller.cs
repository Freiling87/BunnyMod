using BunnyMod.Extensions;
using JetBrains.Annotations;
using RogueLibsCore;

namespace BunnyMod.Traits.T_Drug_Limitations
{
	public class TeeTotaller : CustomTrait
	{
		private const string name = nameof(TeeTotaller);

		[RLSetup]
		[UsedImplicitly]
		private static void Setup()
		{
			TraitBuilder traitBuilder = RogueLibs.CreateCustomTrait<TeeTotaller>()
					.Localize<TeeTotaller>()
					.WithUnlock(new TraitUnlock(name, true)
							.SetAvailable(true)
							.SetAvailableInCharacterCreation(true)
							.SetCantLose(false)
							.SetCantSwap(true)
							.SetCharacterCreationCost(-4)
							.SetEnabled(true)
					);

			// TODO conflict with vTrait.Addict
			BMTraitsManager.RegisterTrait<TeeTotaller>(new BMTraitInfo(name, traitBuilder)
					.WithConflictGroup(ETraitConflictGroup.LimitDrugs)
			);
		}

		public override void OnAdded() { }

		public override void OnRemoved() { }
	}
}