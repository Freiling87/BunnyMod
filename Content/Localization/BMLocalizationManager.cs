using YamlDotNet.Serialization;

namespace BunnyMod.Localization
{
	public class BMLocalizationManager
	{
		public static BMLocalizationManager Instance => _instance ?? (_instance = new BMLocalizationManager());
		private static BMLocalizationManager _instance;

		public AbilityLocalization AbilityLocalization { get; }
		public TraitsLocalization TraitsLocalization { get; }

		private BMLocalizationManager()
		{
			IDeserializer deserializer = new DeserializerBuilder().Build();
			// TODO figure out path for localizationFile.
			AbilityLocalization = deserializer.Deserialize<AbilityLocalization>("");
			TraitsLocalization = deserializer.Deserialize<TraitsLocalization>("");
		}
	}
}