namespace OrgansHaveOwners
{
	using System.Collections.Generic;
	using System.Linq;
	using Verse;

	internal class ForeignOrganUtil
	{
		public static void AddForeignOrgan(Pawn pawn, BodyPartRecord part, List<Thing> ingredients)
		{
			Log.Message("installing something");

			var ingredientWithSource = ingredients
				.FirstOrDefault(ingredient => ingredient is ThingWithComps ingredientWithComps
					&& ingredientWithComps.TryGetComp<CompSourcePawn>() != null);

			if (ingredientWithSource != null)
			{
				Log.Message($"installing {ingredientWithSource.def.defName}");
				var hediff = pawn.health.AddHediff(HediffDef.Named(OrgansHaveOwners.ForeignOrganHediffName), part) as HediffWithComps;

				var hediffComp = new HediffComp_SourcePawn();
				var sourcePawn = ingredientWithSource.TryGetComp<CompSourcePawn>().SourcePawn;

				hediffComp.AssignExtractedPawn(sourcePawn);
				hediff.comps.Add(hediffComp);
			}
		}
	}
}
