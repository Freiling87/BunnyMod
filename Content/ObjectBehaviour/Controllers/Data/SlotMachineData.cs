namespace BunnyMod.ObjectBehaviour.Controllers.Data
{
	public class SlotMachineData : IObjectData
	{
		public bool isPlayingSound;
		public bool isHacked;

		public void RevertAllVars()
		{
			isPlayingSound = default;
			isHacked = default;
		}
	}
}