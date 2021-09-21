using BunnyMod.Content.Traits;
using RogueLibsCore;
using System;

namespace BunnyMod.Content
{
	public class BMInterface
	{
		// TODO unused
		public static float GetZoomLevel(Agent playerAgent)
		{
			if (playerAgent.HasTrait<EagleEyes>())
				return 0.75f;
			if (playerAgent.HasTrait<EagleEyes2>())
				return 0.50f;
			if (playerAgent.HasTrait<Myopic>())
				return 1.50f;
			if (playerAgent.HasTrait<Myopic2>())
				return 2.00f;
			return 1f;
		}

		public static float GetZoomLevel()
		{
			float result = 1.0f;

			if (GC.splitScreen)
				result = 0.8f;
			if (GC.fourPlayerMode)
				result = 0.6f;

			if (BMTraitController.IsPlayerTraitActive<EagleEyes>())
				result *= 0.70f;
			else if (BMTraitController.IsPlayerTraitActive<EagleEyes2>())
				result *= 0.40f;
			else if (BMTraitController.IsPlayerTraitActive<Myopic>())
				result *= 1.50f;
			else if (BMTraitController.IsPlayerTraitActive<Myopic2>())
				result *= 2.00f;

			return result;
		}
	}
}