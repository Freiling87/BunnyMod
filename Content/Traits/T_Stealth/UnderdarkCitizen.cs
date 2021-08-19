using BunnyMod.Content.Extensions;
using Google2u;
using JetBrains.Annotations;
using RogueLibsCore;

namespace BunnyMod.Content.Traits
{
	public class UnderdarkCitizen : CustomTrait
	{
		private const string name = nameof(UnderdarkCitizen);

		[RLSetup]
		[UsedImplicitly]
		public static void Setup()
		{
			TraitBuilder traitBuilder = RogueLibs.CreateCustomTrait<UnderdarkCitizen>()
					.Localize<UnderdarkCitizen>()
					.WithUnlock(new TraitUnlock(name, true)
							.SetAvailable(true)
							.SetAvailableInCharacterCreation(true)
							.SetCantLose(true)
							.SetCantSwap(true)
							.SetCharacterCreationCost(2)
							.SetEnabled(true)
					);

			// TODO conflict: vTrait.TheLaw
			BMTraitsManager.RegisterTrait<UnderdarkCitizen>(new BMTraitInfo(name, traitBuilder));
		}

		public override void OnAdded() { }

		public override void OnRemoved() { }

		public static void Handle_StatusEffects_BecomeHidden(StatusEffects instance, ObjectReal hiddenInObject)
		{
			if (BMTraits_Old.IsPlayerTraitActive<UnderdarkCitizen>())
			{
				if (instance.agent.isPlayer == 0 && hiddenInObject.objectName == nameof(ObjectNameDB.rowIds.Manhole))
				{
					instance.BecomeNotHidden();
				}
			}
		}
	}
}