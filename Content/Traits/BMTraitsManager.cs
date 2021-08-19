using System;
using System.Collections.Generic;
using System.Linq;
using BunnyMod.Content.Extensions;
using RogueLibsCore;

namespace BunnyMod.Content.Traits
{
	public static class BMTraitsManager
	{
		private static readonly Dictionary<Type, BMTraitInfo> registeredTraits = new Dictionary<Type, BMTraitInfo>();

		/// <summary>
		/// mapping of a TraitType 'a' to the TraitTypes 'b[]' that use 'a' as their upgrade
		/// </summary>
		private static readonly Dictionary<Type, List<Type>> upgradeDowngradeDict = new Dictionary<Type, List<Type>>();

		/// <summary>
		/// mapping of ConflictGroups to the TraitTypes in that ConflictGroup
		/// </summary>
		private static readonly Dictionary<ETraitConflictGroup, List<Type>> conflictGroupDict = new Dictionary<ETraitConflictGroup, List<Type>>();

		public static void RegisterTrait<TraitType>(BMTraitInfo info)
		{
			info.FinalizeInfo();
			registeredTraits.Add(typeof(TraitType), info);
			RegisterTraitUpgrades<TraitType>(info);
			RegisterTraitConflictGroup<TraitType>(info);
		}

		private static void RegisterTraitUpgrades<TraitType>(BMTraitInfo info)
		{
			if (info.Upgrade != null)
			{
				if (!upgradeDowngradeDict.ContainsKey(info.Upgrade))
				{
					upgradeDowngradeDict[info.Upgrade] = new List<Type>();
				}

				upgradeDowngradeDict[info.Upgrade].Add(typeof(TraitType));
			}
		}

		private static void RegisterTraitConflictGroup<TraitType>(BMTraitInfo info)
		{
			foreach (ETraitConflictGroup conflictGroup in info.ConflictGroups)
			{
				if (!conflictGroupDict.ContainsKey(conflictGroup))
				{
					conflictGroupDict[conflictGroup] = new List<Type>();
				}

				conflictGroupDict[conflictGroup].Add(typeof(TraitType));
			}
		}

		public static BMTraitInfo GetTraitInfo<TraitType>()
		{
			return GetTraitInfo(typeof(TraitType));
		}

		public static BMTraitInfo GetTraitInfo(Type traitType)
		{
			return registeredTraits.ContainsKey(traitType)
					? registeredTraits[traitType]
					: null;
		}

		/// <summary>
		/// Should be called *after* all of the custom Traits have been registered.
		/// </summary>
		public static void FinalizeTraits()
		{
			foreach (KeyValuePair<Type, BMTraitInfo> traitEntry in registeredTraits)
			{
				RegisterUpgrades(traitEntry.Key, traitEntry.Value);
				RegisterCancellations(traitEntry.Key, traitEntry.Value);
				RegisterRecommendations(traitEntry.Key, traitEntry.Value);
			}
		}

		/// <summary>
		/// Sets the `isUpgrade` and `upgrade` fields for the given trait.
		/// </summary>
		private static void RegisterUpgrades(Type traitType, BMTraitInfo traitInfo)
		{
			TraitUnlock unlock = traitInfo.TraitBuilder.Unlock;
			BMTraitInfo upgradeTraitInfo = GetTraitInfo(traitInfo.Upgrade);
			unlock.SetUpgrade(
					upgradeDowngradeDict.ContainsKey(traitType),
					upgradeTraitInfo?.Name
			);
		}

		/// <summary>
		/// Sets the cancellations for the given trait.
		/// </summary>
		private static void RegisterCancellations(Type traitType, BMTraitInfo traitInfo)
		{
			TraitUnlock unlock = traitInfo.TraitBuilder.Unlock;
			HashSet<string> cancellations = new HashSet<string>();

			// cancel all traits in this conflictGroup 
			if (traitInfo.ConflictGroups.Count > 0)
			{
				foreach (string cancelTrait in traitInfo.ConflictGroups
						.SelectMany(group => conflictGroupDict[group])
						.Where(type => type != traitType) // prevent trait from cancelling itself
						.Select(GetTraitInfo)
						.Where(info => info != null)
						.Select(info => info.Name))
				{
					cancellations.Add(cancelTrait);
				}
			}

			// make sure this trait cancels any downgrade-traits
			if (upgradeDowngradeDict.ContainsKey(traitType))
			{
				foreach (string cancelTrait in upgradeDowngradeDict[traitType]
						.Select(type => GetTraitInfo(type)?.Name)
						.Where(name => name != null))
				{
					cancellations.Add(cancelTrait);
				}
			}

			// make sure this trait cancels the upgrade trait
			if (traitInfo.Upgrade != null && registeredTraits.ContainsKey(traitInfo.Upgrade))
			{
				cancellations.Add(registeredTraits[traitInfo.Upgrade].Name);
			}

			// TODO conflicts with vanilla traits

			unlock.SetCancellations(cancellations);
		}

		/// <summary>
		/// Sets the recommendations for the given trait 
		/// </summary>
		private static void RegisterRecommendations(Type traitType, BMTraitInfo traitInfo)
		{
			TraitUnlock unlock = traitInfo.TraitBuilder.Unlock;
			HashSet<string> recommendations = new HashSet<string>();

			if (traitInfo.Recommendations.Count > 0)
			{
				foreach (string recommendTrait in traitInfo.Recommendations
						.Select(type => GetTraitInfo(type)?.Name)
						.Where(name => name != null))
				{
					recommendations.Add(recommendTrait);
				}
			}

			// TODO recommend vanilla traits
			
			unlock.SetRecommendations(recommendations);
		}
	}
}