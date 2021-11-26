namespace BunnyMod.ObjectBehaviour.Controllers.Data
{
	public class FountainData : IObjectData
	{
		public bool wasStolenFrom;

		public void RevertAllVars()
		{
			wasStolenFrom = default;
		}
	}
}