using System;
using System.Diagnostics;
using BepInEx.Logging;

namespace BunnyMod.Logging
{
	public static class BMLogger
	{
		private static string GetLoggerName(Type containingClass)
		{
			return $"BunnyMod_{containingClass.Name}";
		}
		
		public static ManualLogSource GetLogger()
		{
			Type containingClass = new StackFrame(1, false).GetMethod().ReflectedType;
			return Logger.CreateLogSource(GetLoggerName(containingClass));
		}
	}
}