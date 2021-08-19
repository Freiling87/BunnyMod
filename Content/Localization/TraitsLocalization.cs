using System.Collections.Generic;
using JetBrains.Annotations;
using RogueLibsCore;
using UnityEngine;

namespace BunnyMod.Content.Localization
{
	public class TraitsLocalization
	{
		[UsedImplicitly] public Dictionary<string, Dictionary<LanguageCode, LocalizedTrait>> traits;

		public Dictionary<LanguageCode, LocalizedTrait> GetLocalization<TraitType>()
		{
			string id = typeof(TraitType).Name;
			if (!traits.ContainsKey(id))
			{
				Debug.LogWarning("TraitsLocalization did not find Localization for ID: '" + id + "'");
				return null;
			}
			return traits[id];
		}

		[UsedImplicitly]
		public class LocalizedTrait
		{
			public string Name { get; set; }
			public string Desc { get; set; }
		}
	}
}