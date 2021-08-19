using System.Reflection;
using BepInEx.Logging;

namespace BunnyMod.Content.Traits
{
	/// <summary>
	/// Provides methods for interfacing with BM Custom Traits
	/// </summary>
	public static class BMTraitController
	{
		private static readonly string loggerName = $"BunnyMod_{nameof(BMTraitController)}";
		private static ManualLogSource Logger => _logger ?? (_logger = BepInEx.Logging.Logger.CreateLogSource(loggerName));
		private static ManualLogSource _logger;
		
		public static int ApplyToolCostModifiers(Agent agent, int baseCost)
		{
			Logger.LogDebug($"{MethodBase.GetCurrentMethod().Name} ( agent = '{agent.agentName}', baseCost = '{baseCost}'");
			double costFactor = TamperTantrum.GetToolCostFactor(agent)
					* TamperTantrum2.GetToolCostFactor(agent);
			return (int) (baseCost * costFactor);
		}
	}
}