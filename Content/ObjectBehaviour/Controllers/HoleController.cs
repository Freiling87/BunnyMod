using BunnyMod.Extensions;
using BunnyMod.Traits.T_Stealth;
using Google2u;
using RogueLibsCore;
using UnityEngine;

namespace BunnyMod.ObjectBehaviour.Controllers
{
	public static class HoleController
	{
		public static bool Hole_EnterRange_Prefix(Hole hole, GameObject myObject)
		{
			GameController gc = GameController.gameController;
			if (gc.loadComplete && myObject.CompareTag(nameof(Agent)))
			{
				Agent agent = myObject.GetComponent<Agent>();
				if (hole.objectMultHole.objectHoleType == nameof(Manhole)
						&& agent.HasTrait<UnderdarkCitizen>() && !agent.statusEffects.HasStatusEffect(StatusEffectNameDB.rowIds.Giant)
						&& hole.playfieldObject is Manhole manhole)
				{
					ManholeController.HandleFlushYourself(manhole, agent);
					if (gc.challenges.Contains(vChallenge.LowHealth))
					{
						agent.statusEffects.ChangeHealth(-7);
					}
					else
					{
						agent.statusEffects.ChangeHealth(-15);
					}
					return false;
				}
			}
			return true;
		}
	}
}