using Google2u;

namespace BunnyMod.Extensions
{
	public static class InvDatabaseExtensions
	{
		public static bool HasItem(this InvDatabase inventory, ItemNameDB.rowIds itemName) =>
			inventory.HasItem(itemName.GetName());

		public static InvItem FindItem(this InvDatabase inventory, ItemNameDB.rowIds itemName) =>
			inventory.FindItem(itemName.GetName());
	}
}