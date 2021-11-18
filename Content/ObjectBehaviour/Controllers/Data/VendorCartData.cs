namespace BunnyMod.ObjectBehaviour.Controllers.Data
{
	public class VendorCartData : IObjectData
	{
		public bool wasStolenFrom;

		public void RevertAllVars()
		{
			wasStolenFrom = default;
		}
	}
}