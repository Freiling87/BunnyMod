using RogueLibsCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BunnyMod.Content
{
	class BMMiscellaneous
	{
		public static GameController GC => GameController.gameController;
		public static bool Prefix(Type type, string methodName, Type patchType, string patchMethodName, Type[] types) => BMHeader.MainInstance.PatchPrefix(type, methodName, patchType, patchMethodName, types);
		public static bool Postfix(Type type, string methodName, Type patchType, string patchMethodName, Type[] types) => BMHeader.MainInstance.PatchPostfix(type, methodName, patchType, patchMethodName, types);
		public static void BMLog(string logMessage) => BMHeader.Log(logMessage);

		public void Awake()
		{
			// RandomOther
			Postfix(typeof(RandomOther), "fillOther", GetType(), "RandomOther_fillOther", new Type[0] { });

			// RandomSelection
			Prefix(typeof(RandomSelection), "RandomSelect", GetType(), "RandomSelection_RandomSelect", new Type[2] { typeof(string), typeof(string) });
		}

		#region RandomOther
		public static void RandomOther_fillOther(ref RandomSelection ___component, ref RandomList ___rList) // Postfix
		{
			BMLog("RandomOther_fillOther");
			// Pay special attention to this. If this is only called at Game Start, you need to find another place post-mutator to mod this.

			if (GC.challenges.Contains(cChallenge.ShantyTown) || GC.challenges.Contains(cChallenge.GreenLiving))
			{
				___rList = ___component.CreateRandomList("FireSpewerSpawnChance1", "Others", "Other");
				___component.CreateRandomElement(___rList, "No", 5);

				___rList = ___component.CreateRandomList("FireSpewerSpawnChance2", "Others", "Other");
				___component.CreateRandomElement(___rList, "No", 5);

				___rList = ___component.CreateRandomList("FireSpewerSpawnChance3", "Others", "Other");
				___component.CreateRandomElement(___rList, "No", 5);

				___rList = ___component.CreateRandomList("FireSpewerSpawnChance4", "Others", "Other");
				___component.CreateRandomElement(___rList, "No", 5);

				___rList = ___component.CreateRandomList("FireSpewerSpawnChance5", "Others", "Other");
				___component.CreateRandomElement(___rList, "No", 5);
			}
		}
		#endregion
		#region RandomSelection
		public static bool RandomSelection_RandomSelect(string rName, string rCategory, ref string __result) // Prefix
		{
			if (rName.StartsWith("FireSpewerSpawnChance") && GC.challenges.Contains(cChallenge.GreenLiving))
			{
				__result = "No";
				return false;
			}

			return true;
		}
		#endregion
	}
}
