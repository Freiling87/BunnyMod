using System;

namespace BunnyMod
{
	public class BMLevelFeelings
	{
		public static GameController GC => GameController.gameController;

		public static bool Prefix(Type type, string methodName, Type patchType, string patchMethodName, Type[] types) =>
			BMHeader.MainInstance.PatchPrefix(type, methodName, patchType, patchMethodName, types);

		public static bool Postfix(Type type, string methodName, Type patchType, string patchMethodName, Type[] types) =>
			BMHeader.MainInstance.PatchPostfix(type, methodName, patchType, patchMethodName, types);

		public static void BMLog(string logMessage) => BMHeader.Log(logMessage);

		public void Awake()
		{
			InitializeLevelFeelings();
		}

		public static void InitializeLevelFeelings()
		{
			// Pending addition to RogueLibs
		}
	}
}