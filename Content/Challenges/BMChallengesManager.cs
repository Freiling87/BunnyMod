using System;
using System.Collections.Generic;
using System.Linq;
using BunnyMod.Content.Extensions;
using RogueLibsCore;
using BunnyMod.Content.Challenges;

namespace BunnyMod.Content.Challenges
{
	public static class BMChallengesManager
	{
		private static readonly Dictionary<Type, BMChallengeInfo> registeredChallenges = new Dictionary<Type, BMChallengeInfo>();

		/// <summary>
		/// mapping of ConflictGroups to the ChallengeTypes in that ConflictGroup
		/// </summary>
		private static readonly Dictionary<EChallengeConflictGroup, List<Type>> conflictGroupDict = new Dictionary<EChallengeConflictGroup, List<Type>>();

		public static void RegisterChallenge<ChallengeType>(BMChallengeInfo info)
		{
			info.FinalizeInfo();
			registeredChallenges.Add(typeof(ChallengeType), info);
			RegisterChallengeConflictGroup<ChallengeType>(info);
		}

		private static void RegisterChallengeConflictGroup<ChallengeType>(BMChallengeInfo info)
		{
			foreach (EChallengeConflictGroup conflictGroup in info.ConflictGroups)
			{
				if (!conflictGroupDict.ContainsKey(conflictGroup))
				{
					conflictGroupDict[conflictGroup] = new List<Type>();
				}

				conflictGroupDict[conflictGroup].Add(typeof(ChallengeType));
			}
		}

		public static BMChallengeInfo GetChallengeInfo<ChallengeType>()
		{
			return GetChallengeInfo(typeof(ChallengeType));
		}

		public static BMChallengeInfo GetChallengeInfo(Type ChallengeType)
		{
			return registeredChallenges.ContainsKey(ChallengeType)
					? registeredChallenges[ChallengeType]
					: null;
		}

		/// <summary>
		/// Should be called *after* all of the custom Challenges have been registered.
		/// </summary>
		public static void FinalizeChallenges()
		{
			foreach (KeyValuePair<Type, BMChallengeInfo> ChallengeEntry in registeredChallenges)
			{
				RegisterCancellations(ChallengeEntry.Key, ChallengeEntry.Value);
			}
		}

		/// <summary>
		/// Sets the cancellations for the given Challenge.
		/// </summary>
		private static void RegisterCancellations(Type ChallengeType, BMChallengeInfo ChallengeInfo)
		{
			MutatorUnlock unlock = ChallengeInfo.UnlockBuilder.Unlock;
			HashSet<string> cancellations = new HashSet<string>();

			// cancel all Challenges in this conflictGroup 
			if (ChallengeInfo.ConflictGroups.Count > 0)
			{
				foreach (string cancelChallenge in ChallengeInfo.ConflictGroups
						.SelectMany(group => conflictGroupDict[group])
						.Where(type => type != ChallengeType) // prevent Challenge from cancelling itself
						.Select(GetChallengeInfo)
						.Where(info => info != null)
						.Select(info => info.Name))
				{
					cancellations.Add(cancelChallenge);
				}
			}

			// TODO conflicts with vanilla Challenges

			unlock.SetCancellations(cancellations);
		}
	}
}