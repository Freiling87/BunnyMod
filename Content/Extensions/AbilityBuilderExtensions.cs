using System.Collections.Generic;
using System.Linq;
using BunnyMod.Localization;
using RogueLibsCore;

namespace BunnyMod.Extensions
{
	public static class AbilityBuilderExtensions
	{
		public static AbilityBuilder Localize<AbilityType>(this AbilityBuilder builder) where AbilityType : CustomAbility
		{
			AbilityLocalization abilityLocalization = BMLocalizationManager.Instance.AbilityLocalization;
			Dictionary<LanguageCode, AbilityLocalization.LocalizedAbility> localizedAbilities = abilityLocalization.GetLocalization<AbilityType>();
			builder.WithName(new CustomNameInfo(localizedAbilities.ToDictionary(entry => entry.Key, entry => entry.Value.Name)));
			builder.WithDescription(new CustomNameInfo(localizedAbilities.ToDictionary(entry => entry.Key, entry => entry.Value.Desc)));
			return builder;
		}
	}
}
