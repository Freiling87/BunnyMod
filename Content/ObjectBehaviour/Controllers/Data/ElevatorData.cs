namespace BunnyMod.ObjectBehaviour.Controllers.Data
{
	public class ElevatorData : IObjectData
	{
		public bool isTicketPurchased;

		public void RevertAllVars()
		{
			isTicketPurchased = default;
		}
	}
}