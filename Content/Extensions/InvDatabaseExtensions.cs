using Google2u;

namespace BunnyMod.Extensions
{
	public static class InvDatabaseExtensions
	{
		public static bool HasItem(this InvDatabase inventory, ItemNameDB.rowIds itemName)
		{
			return inventory.HasItem(itemName.GetName());
		}

		public static InvItem FindItem(this InvDatabase inventory, ItemNameDB.rowIds itemName)
		{
			return inventory.FindItem(itemName.GetName());
		}
	}
}