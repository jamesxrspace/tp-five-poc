using UnityEngine;

namespace I2.Loc
{
	public static class ScriptLocalization
	{

		public static string ApplicationName 		{ get{ return LocalizationManager.GetTranslation ("ApplicationName"); } }
	}

    public static class ScriptTerms
	{

		public const string ApplicationName = "ApplicationName";
	}
}