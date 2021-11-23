using Google2u;
using RogueLibsCore;

namespace BunnyMod.Content.Extensions
{
	public static class AgentExtensions
	{
		public static bool IsEnforcer(this Agent agent) =>
			agent.enforcer || 
			agent.HasTrait(nameof(StatusEffectNameDB.rowIds.TheLaw)) || 
			vAgent.LawEnforcement.Contains(agent.agentName);

		public static bool IsCriminal(this Agent agent) =>
			agent.objectMultAgent.mustBeGuilty || vAgent.Criminal.Contains(agent.agentName);

		public static bool IsAgent(this Agent agent, AgentNameDB.rowIds agentNameType) =>
			agent.agentName == agentNameType.GetName();

		public static bool HasTrait(this Agent agent, StatusEffectNameDB.rowIds vanillaTraitType) =>
			agent.HasTrait(vanillaTraitType.GetName());
	}
}