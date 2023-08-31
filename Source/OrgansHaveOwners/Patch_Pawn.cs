namespace OrgansHaveOwners
{
	using System.Collections.Generic;
	using System.Reflection;
	using HarmonyLib;
	using RimWorld;
	using Verse;

	[HarmonyPatch]
	internal class Patch_Pawn
	{
		static MethodBase TargetMethod() => typeof(Pawn).GetMethod("ButcherProducts", BindingFlags.Public | BindingFlags.Instance);

		[HarmonyPostfix]
		static void Postfix(ref IEnumerable<Thing> __result, ref Pawn butcher)
		{
			foreach (var thing in __result)
			{
				if (thing is ThingWithComps thingWithComps)
				{
					CompSourcePawn.AddCompSourcePawn(thingWithComps, butcher);
				}
			}
		}
	}
}
