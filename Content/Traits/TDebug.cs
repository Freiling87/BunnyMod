using BunnyMod.Content.Extensions;
using JetBrains.Annotations;
using RogueLibsCore;

namespace BunnyMod.Content.Traits
{
	public class TDebug : CustomTrait
	{
		private const string name = nameof(TDebug);

		[RLSetup]
		[UsedImplicitly]
		public static void Setup()
		{
			TraitBuilder traitBuilder = RogueLibs.CreateCustomTrait<TDebug>()
					.WithName(new CustomNameInfo("00_Debug"))
					.WithDescription(new CustomNameInfo("Debug trait. If you see this, I hope you're me."))
					.WithUnlock(new TraitUnlock(name, true)
							.SetAvailable(true)
							.SetAvailableInCharacterCreation(true)
							.SetCantSwap(true)
							.SetCantLose(true)
							.SetCharacterCreationCost(0)
							.SetEnabled(true)
					);

			BMTraitsManager.RegisterTrait<TDebug>(new BMTraitInfo(name, traitBuilder));
		}

		public override void OnAdded() { }

		public override void OnRemoved() { }
	}
}