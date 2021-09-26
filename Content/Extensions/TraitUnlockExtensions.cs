using System.Collections.Generic;
using RogueLibsCore;

namespace BunnyMod.Content.Extensions
{
	public static class TraitUnlockExtensions
	{
		/// <summary>
		/// Available determines whether the unlock will be available in the menu and in the game
		/// if Available is set to false, you won't find anything about that unlock in the game; if set to true you can find it in the Rewards/Traits/Mutators menu
		///
		/// For upgrade traits, set Available to false.
		/// </summary>
		public static T SetAvailable<T>(this T wrapper, bool isAvailable) where T : UnlockWrapper
		{
			wrapper.IsAvailable = isAvailable;
			return wrapper;
		}

		public static T SetAvailableInCharacterCreation<T>(this T wrapper, bool isAvailableInCC) where T : TraitUnlock
		{
			wrapper.IsAvailableInCC = isAvailableInCC;
			return wrapper;
		}

		public static T SetCharacterCreationCost<T>(this T wrapper, int cost) where T : UnlockWrapper
		{
			wrapper.CharacterCreationCost = cost;
			return wrapper;
		}

		/// <summary>
		/// IsEnabled is the state of the unlock in the Rewards/Traits/Mutators menu
		/// if IsActive is set to true, you'll be able to find that unlock while playing; if set to false you won't
		/// </summary>
		public static T SetEnabled<T>(this T wrapper, bool enabled) where T : UnlockWrapper
		{
			wrapper.IsEnabled = enabled;
			return wrapper;
		}

		public static T SetCantLose<T>(this T wrapper, bool cantLose) where T : UnlockWrapper
		{
			wrapper.Unlock.cantLose = cantLose;
			return wrapper;
		}

		public static T SetCantSwap<T>(this T wrapper, bool cantSwap) where T : UnlockWrapper
		{
			wrapper.Unlock.cantSwap = cantSwap;
			return wrapper;
		}

		public static T SetCancellations<T>(this T wrapper, IEnumerable<string> cancellations) where T : UnlockWrapper
		{
			if (wrapper.Unlock.cancellations == null)
			{
				wrapper.Unlock.cancellations = new List<string>();
			}
			wrapper.Unlock.cancellations.Clear();
			wrapper.Unlock.cancellations.AddRange(cancellations);
			return wrapper;
		}

		public static T SetUpgrade<T>(this T wrapper, bool isUpgrade, string nextUpgrade) where T : UnlockWrapper
		{
			wrapper.Unlock.isUpgrade = isUpgrade;
			wrapper.Unlock.upgrade = nextUpgrade;
			return wrapper;
		}

		public static T SetRecommendations<T>(this T wrapper, IEnumerable<string> recommendations) where T : UnlockWrapper
		{
			if (wrapper.Unlock.recommendations == null)
			{
				wrapper.Unlock.recommendations = new List<string>();
			}
			wrapper.Unlock.recommendations.Clear();
			wrapper.Unlock.recommendations.AddRange(recommendations);
			return wrapper;
		}
	}
}