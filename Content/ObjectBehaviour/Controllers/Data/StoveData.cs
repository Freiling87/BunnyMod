namespace BunnyMod.ObjectBehaviour.Controllers.Data
{
	public class StoveData : IObjectData
	{
		public PlayfieldObject countdownCauser;
		public bool noOwnCheckCountdown;
		public PlayfieldObject savedDamagerObject;

		public void RevertAllVars()
		{
			countdownCauser = default;
			noOwnCheckCountdown = default;
			savedDamagerObject = default;
		}
	}
}