using Google2u;

namespace BunnyMod.Extensions
{
	public static class InvItem_Extensions
	{
		public static bool IsItem(this InvItem item, ItemNameDB.rowIds itemNameType)
		{
			return item.invItemName == itemNameType.GetName();
		}
	}
}