using System;

namespace BunnyMod.Content.Extensions
{
	public static class EnumExtensions
	{
		public static string GetName<EnumType>(this EnumType enumValue) where EnumType : Enum
		{
			return Enum.GetName(enumValue.GetType(), enumValue);
		}
	}
}