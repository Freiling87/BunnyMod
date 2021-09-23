using System;
using System.Collections.Generic;
using RogueLibsCore;

namespace BunnyMod.Content.Traits
{
	public class BMTraitInfo
	{
		public string Name { get; }
		public TraitBuilder TraitBuilder { get; }
		public Type Upgrade { get; private set; }
		public List<ETraitConflictGroup> ConflictGroups { get; } = new List<ETraitConflictGroup>();
		public List<Type> Recommendations { get; } = new List<Type>();

		private bool finalized;

		private void AssertNotFinalized()
		{
			if (finalized)
				throw new NotSupportedException("cannot modify finalized BMTraitInfo!");
		}

		public BMTraitInfo(string name, TraitBuilder builder)
		{
			Name = name;
			TraitBuilder = builder;
		}

		public BMTraitInfo WithUpgrade(Type upgradeTrait)
		{
			AssertNotFinalized();
			Upgrade = upgradeTrait;
			return this;
		}

		public BMTraitInfo WithConflictGroup(params ETraitConflictGroup[] conflictGroup)
		{
			AssertNotFinalized();
			ConflictGroups.AddRange(conflictGroup);
			return this;
		}

		public BMTraitInfo WithRecommendation(params Type[] recommendedTrait)
		{
			AssertNotFinalized();
			Recommendations.AddRange(recommendedTrait);
			return this;
		}

		public void FinalizeInfo()
		{
			finalized = true;
		}
	}
}