namespace BunnyMod.Content.ObjectBehaviour
{
	/// <summary>
	/// Stores extra data on stoves
	/// </summary>
	public class StoveData
	{
		public int animateSpriteID;
		public int animateSpriteID2;
		public float animationCountdown;
		public int animationFrame;
		public PlayfieldObject countdownCauser;
		public bool noOwnCheckCountdown;
		public PlayfieldObject savedDamagerObject;		
	}
}