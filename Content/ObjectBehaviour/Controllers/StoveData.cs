namespace BunnyMod.Content.ObjectBehaviour
{
	/// <summary>
	/// Stores extra data on stoves
	/// </summary>
	public class StoveData
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