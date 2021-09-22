using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RogueLibsCore;
using System.Collections.Generic;
using BepInEx.Logging;
using BunnyMod.Content.Logging;
using UnityEngine;
using Random = UnityEngine.Random;
using BunnyMod.Content.Traits;

namespace BunnyMod.Content.Custom
{
	class C_Relationships
	{
		private static readonly ManualLogSource logger = BMLogger.GetLogger();
		private static GameController GC => GameController.gameController;

		// TODO I'd like to move this entire region somewhere else, but haven't made my mind up yet where it should go
		// TODO unused.
		public static void AnnoyWitnesses(Agent perp, Agent victim)
		{
			foreach (Agent bystander in GC.agentList)
			{
				if (Vector2.Distance(bystander.tr.position, perp.tr.position) < bystander.LOSRange / perp.hardToSeeFromDistance &&
						bystander != perp && bystander != victim && !bystander.zombified && !bystander.ghost && !bystander.oma.hidden &&
						(!perp.aboveTheLaw || !bystander.enforcer || victim.enforcer) &&
						perp.prisoner == bystander.prisoner && !perp.invisible && !victim.noEnforcerAlert)
				{
					string perpRel = bystander.relationships.GetRel(perp);
					string victimRel = bystander.relationships.GetRel(victim);

					if (victimRel != nameof(relStatus.Hostile) && victimRel != nameof(relStatus.Annoyed))
					{
						if (perpRel == nameof(relStatus.Neutral) || perpRel == nameof(relStatus.Friendly))
						{
							if (bystander.relationships.GetRelationship(perp).hasLOS)
							{
								relStatus victimRel2 = bystander.relationships.GetRelCode(victim);
								relStatus perpRel2 = bystander.relationships.GetRelCode(perp);

								if ((victimRel2 == relStatus.Aligned && perpRel2 != relStatus.Aligned) ||
										(victimRel2 == relStatus.Loyal && perpRel2 != relStatus.Aligned && perpRel2 != relStatus.Loyal))
								{
									bystander.sawBiteList.Add(victim);
									bystander.relationships.SetRelHate(perp, 5);
								}
								else
								{
									bystander.sawBiteList.Add(victim);
									bystander.relationships.SetStrikes(perp, 2);
								}
							}
						}
						else if (perpRel == nameof(relStatus.Annoyed) && bystander.relationships.GetRelationship(perp).hasLOS)
						{
							bystander.sawBiteList.Add(victim);
							bystander.relationships.SetRelHate(perp, 5);
						}
					}
				}
			}
		}

		public static void AnnoyWitnessesVictimless(Agent perp)
		{
			foreach (Agent bystander in GC.agentList)
			{
				if (Vector2.Distance(bystander.tr.position, perp.tr.position) < bystander.LOSRange / perp.hardToSeeFromDistance &&
						bystander != perp && !bystander.zombified && !bystander.ghost && !bystander.oma.hidden &&
						(!perp.aboveTheLaw || !bystander.enforcer) &&
						perp.prisoner == bystander.prisoner && !perp.invisible)
				{
					string perpRel = bystander.relationships.GetRel(perp);

					if (perpRel == nameof(relStatus.Neutral) || perpRel == nameof(relStatus.Friendly))
					{
						if (bystander.relationships.GetRelationship(perp).hasLOS)
						{
							relStatus perpRel2 = bystander.relationships.GetRelCode(perp);

							// TODO something isn't right here, condition always evaluates to true
							if (perpRel2 != relStatus.Aligned || perpRel2 != relStatus.Loyal)
								bystander.relationships.SetStrikes(perp, 2);
						}
					}
					else if (perpRel == nameof(relStatus.Annoyed) && bystander.relationships.GetRelationship(perp).hasLOS)
					{
						bystander.relationships.SetRelHate(perp, 5);
					}
				}
			}
		}
	}
}
