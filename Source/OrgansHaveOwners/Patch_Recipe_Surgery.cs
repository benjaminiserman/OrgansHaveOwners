namespace OrgansHaveOwners
{
	using System.Collections.Generic;
	using System.Linq;
	using System.Reflection;
	using HarmonyLib;
	using RimWorld;
	using Verse;

	[HarmonyPatch]
	internal class Patch_Recipe_Surgery
	{
		static MethodBase TargetMethod() => typeof(Recipe_Surgery).GetMethod("OnSurgerySuccess", BindingFlags.NonPublic | BindingFlags.Instance);

		[HarmonyPostfix]
		static void Postfix(ref Pawn pawn, ref List<Thing> ingredients)
		{
			var ingredientWithSource = ingredients.FirstOrDefault(ingredient => ingredient is ThingWithComps ingredientWithComps
				&& ingredientWithComps.TryGetComp<CompSourcePawn>() != null);

			if (ingredientWithSource != null)
			{
				var hediff = pawn.health.AddHediff(HediffDef.Named("ForeignOrgan")) as HediffWithComps;

				var hediffComp = new HediffComp_SourcePawn();
				var sourcePawn = ingredientWithSource.TryGetComp<CompSourcePawn>().SourcePawn;

				hediffComp.AssignExtractedPawn(sourcePawn);
				hediff.comps.Add(hediffComp);
			}
		}
	}
}
