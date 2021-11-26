using System;
using System.Collections.Generic;
using RogueLibsCore;

namespace BunnyMod.Content.Challenges
{
	public class BMChallengeInfo
	{
		public string Name { get; }
		public UnlockBuilder UnlockBuilder { get; }
		public List<EChallengeConflictGroup> ConflictGroups { get; } = new List<EChallengeConflictGroup>();

		private bool finalized;

		public BMChallengeInfo(string name, UnlockBuilder builder)
		{
			Name = name;
			UnlockBuilder = builder;
		}

		private void AssertNotFinalized()
		{
			if (finalized)
				throw new NotSupportedException("cannot modify finalized BMChallengeInfo!");
		}

		public BMChallengeInfo WithConflictGroup(params EChallengeConflictGroup[] conflictGroup)
		{
			AssertNotFinalized();
			ConflictGroups.AddRange(conflictGroup);
			return this;
		}

		public void FinalizeInfo()
		{
			finalized = true;
		}
	}
}