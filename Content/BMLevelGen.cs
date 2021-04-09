using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RogueLibsCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using System.Collections;
using HarmonyLib;
using System.Reflection;
using Random = UnityEngine.Random;

namespace BunnyMod.Content
{
	public class BMLevelGen
	{
		public static GameController GC => GameController.gameController;
		public static bool Prefix(Type type, string methodName, Type patchType, string patchMethodName, Type[] types) => BMHeader.MainInstance.PatchPrefix(type, methodName, patchType, patchMethodName, types);
		public static bool Postfix(Type type, string methodName, Type patchType, string patchMethodName, Type[] types) => BMHeader.MainInstance.PatchPostfix(type, methodName, patchType, patchMethodName, types);

		public void Awake()
		{

		}

		#region LoadLevel
		// There is a patch in BMAbilities for this Class, but it uses a variable in that class. TODO: Move it over here.
		#endregion
	}
}
