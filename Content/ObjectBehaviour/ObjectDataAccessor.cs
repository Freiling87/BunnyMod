using System.Collections.Generic;
using BunnyMod.ObjectBehaviour.Controllers.Data;

namespace BunnyMod.ObjectBehaviour
{
	public class ObjectDataAccessor<DataType> : IObjectDataController
			where DataType : IObjectData, new()
	{
		private readonly Dictionary<PlayfieldObject, DataType> objectDataDictionary = new Dictionary<PlayfieldObject, DataType>();

		public void ClearData()
		{
			objectDataDictionary.Clear();
		}

		public void RevertAllVars(PlayfieldObject objectInstance)
		{
			if (objectDataDictionary.ContainsKey(objectInstance))
			{
				objectDataDictionary[objectInstance].RevertAllVars();
			}
		}

		public DataType GetObjectData(PlayfieldObject objectInstance)
		{
			if (!objectDataDictionary.ContainsKey(objectInstance))
			{
				return objectDataDictionary[objectInstance] = new DataType();
			}
			return objectDataDictionary[objectInstance];
		}
	}
}