using System.Collections.Generic;
using System.Linq;
using BunnyMod.Content.Localization;
using RogueLibsCore;

namespace BunnyMod.Content.Extensions
{
	public static class TraitBuilderExtensions
	{
		public static TraitBuilder Localize<TraitType>(this TraitBuilder builder) where TraitType : CustomTrait
		{
			TraitsLocalization traitsLocalization = BMLocalizationManager.Instance.TraitsLocalization;
			Dictionary<LanguageCode, TraitsLocalization.LocalizedTrait> localizedTraits = traitsLocalization.GetLocalization<TraitType>();
			builder.WithName(new CustomNameInfo(localizedTraits.ToDictionary(entry => entry.Key, entry => entry.Value.Name)));
			builder.WithDescription(new CustomNameInfo(localizedTraits.ToDictionary(entry => entry.Key, entry => entry.Value.Desc)));
			return builder;
		}
	}
}