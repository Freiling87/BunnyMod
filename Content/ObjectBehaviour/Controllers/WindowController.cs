using BunnyMod.Traits.T_Stealth;
using RogueLibsCore;

namespace BunnyMod.ObjectBehaviour.Controllers
{
	public static class WindowController
	{
		public static bool SlipThroughWindow_ShouldDamageAgent(Agent agent)
		{
			return !agent.HasTrait<StealthBastardDeluxe>();
		}

		public static void SetVars_Postfix(Window window)
		{
			window.breakForAthleteQuest = true;
		}
	}
}