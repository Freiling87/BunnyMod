using System;

namespace BunnyMod.Extensions
{
	public static class EnumExtensions
	{
		public static string GetName<EnumType>(this EnumType enumValue) where EnumType : Enum => Enum.GetName(enumValue.GetType(), enumValue);
	}
}