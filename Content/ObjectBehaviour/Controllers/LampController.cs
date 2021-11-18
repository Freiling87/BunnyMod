using UnityEngine;

namespace BunnyMod.ObjectBehaviour.Controllers
{
	public static class LampController
	{
		public static LightTemp HandleSpawnLightTemp(SpawnerMain spawnerInstance, Vector3 lightPos, PlayfieldObject playfieldObject, string lightType)
		{
			LightTemp lightTemp = spawnerInstance.SpawnLightTemp(lightPos, playfieldObject, lightType);
			ApplyLightTemp(ref lightTemp);
			return lightTemp;
		}

		private static void ApplyLightTemp(ref LightTemp lightTemp)
		{
			GameController gc = GameController.gameController;
			if (gc.challenges.Contains(cChallenge.DiscoCityDanceoff) || BMHeader.debugMode)
			{
				int random = Random.Range(0, 100);
				if (random <= 33)
				{
					lightTemp.fancyLight.Color = new Color(0f, 0f, 0.75f, 0.75f);
				}
				else if (random <= 66)
				{
					lightTemp.fancyLight.Color = new Color(0f, 0.75f, 0f, 0.75f);
				}
				else
				{
					lightTemp.fancyLight.Color = new Color(0.75f, 0f, 0f, 0.75f);
				}
			}
		}
	}
}