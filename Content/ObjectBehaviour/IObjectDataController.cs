namespace BunnyMod.ObjectBehaviour
{
	public interface IObjectDataController
	{
		void ClearData();
		void RevertAllVars(PlayfieldObject objectInstance);
	}
}