using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using RogueLibsCore;

namespace BunnyMod.Content
{
	public class BMDisasters
	{
		public static GameController gc => GameController.gameController;

		public void Awake()
		{
			InitializeDisasters();

			// Patches here
		}

		public static void InitializeDisasters()
		{
			
		}
	}
}
