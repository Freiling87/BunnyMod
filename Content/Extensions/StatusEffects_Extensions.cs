using Google2u;

namespace BunnyMod.Extensions
{
	public static class StatusEffects_Extensions
	{
		public static void AddStatusEffect(this StatusEffects statusEffects, StatusEffectNameDB.rowIds statusEffect)
		{
			statusEffects.AddStatusEffect(statusEffect.GetName());
		}

		public static void AddStatusEffect(this StatusEffects statusEffects, StatusEffectNameDB.rowIds statusEffect, bool showText, bool dontPrevent)
		{
			statusEffects.AddStatusEffect(statusEffect.GetName(), showText, dontPrevent);
		}

		public static bool HasStatusEffect(this StatusEffects statusEffects, StatusEffectNameDB.rowIds statusEffect) =>
			statusEffects.hasStatusEffect(statusEffect.GetName());
	}
}