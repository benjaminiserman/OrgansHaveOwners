namespace OrgansHaveOwners
{
	using System.IO;
	using System.Reflection;
	using HarmonyLib;
	using Verse;

	internal class OrgansHaveOwners : Mod
	{
		private static readonly string Identifier = "OrgansHaveOwners";
		internal static string VersionDir => Path.Combine(ModLister.GetActiveModWithIdentifier($"winggar.{Identifier}").RootDir.FullName, "Version.txt");
		public static string CurrentVersion { get; private set; }

		public static string ForeignOrganHediffName = "ForeignOrgan";

		public OrgansHaveOwners(ModContentPack content) : base(content)
		{
			var version = Assembly.GetExecutingAssembly().GetName().Version;
			CurrentVersion = $"{version.Major}.{version.Minor}.{version.Build}";

			if (Prefs.DevMode)
			{
				File.WriteAllText(VersionDir, CurrentVersion);
			}

			new Harmony($"winggar.{Identifier}").PatchAll();
		}
	}
}