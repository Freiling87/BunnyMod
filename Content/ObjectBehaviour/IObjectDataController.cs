namespace BunnyMod.ObjectBehaviour
{
	public interface IObjectDataController
	{
		/// <summary>
		/// Reset all data stored by this controller
		/// </summary>
		void ClearData();
		
		/// <summary>
		/// Resets the stored data for a given object instance
		/// </summary>
		/// <param name="objectInstance">The object instance to reset data for</param>
		void RevertAllVars(PlayfieldObject objectInstance);
	}
}