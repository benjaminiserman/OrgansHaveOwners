namespace OrgansHaveOwners
{
	using System;
	using System.Collections.Generic;
	using RimWorld;
	using Verse;

	internal class CompSourcePawn : ThingComp
	{
		protected bool wasExtractedFromNamedPawn;

		protected Pawn sourcePawn;

		protected string sourcePawnLabel;

		public bool WasExtractedFromNamedPawn => wasExtractedFromNamedPawn;

		public Pawn SourcePawn => sourcePawn;

		public string SourcePawnLabel => sourcePawnLabel;

		public static void AddCompSourcePawn(ThingWithComps thingWithComps, Pawn sourcePawn)
		{
			var compSourceComp = Activator.CreateInstance<CompSourcePawn>();
			compSourceComp.parent = thingWithComps;
			compSourceComp.AssignExtractedPawn(sourcePawn);
			thingWithComps.AllComps.Add(compSourceComp);
			compSourceComp.Initialize(new CompProperties());
		}

		public virtual void AssignExtractedPawn(Pawn p)
		{
			wasExtractedFromNamedPawn = true;
			sourcePawn = p;
			sourcePawnLabel = p.Name.ToStringFull;
		}

		public virtual void UnassignExtractedPawn()
		{
			wasExtractedFromNamedPawn = false;
			sourcePawn = null;
			sourcePawnLabel = null;
		}

		public override string CompInspectStringExtra()
		{
			if (!wasExtractedFromNamedPawn)
			{
				return string.Empty;
			}

			return "Winggar.OrgansHaveOwners.ExtractedFrom".Translate(sourcePawnLabel.ApplyTag(TagType.Name)).Resolve();
		}

		public override IEnumerable<StatDrawEntry> SpecialDisplayStats()
		{
			if (wasExtractedFromNamedPawn)
			{
				yield return new StatDrawEntry(
					StatCategoryDefOf.BasicsNonPawn, 
					"Winggar.Stat_Thing_ExtractedFrom_Name".Translate(), 
					sourcePawnLabel, 
					"Winggar.Stat_Thing_ExtractedFrom_Desc".Translate(),
					1105
				);
			}
		}

		public override void PostExposeData()
		{
			base.PostExposeData();
			Scribe_Values.Look(ref wasExtractedFromNamedPawn, "wasExtractedFromNamedPawn", defaultValue: false);
			Scribe_Values.Look(ref sourcePawnLabel, "sourcePawnLabel");
			Scribe_References.Look(ref sourcePawn, "sourcePawn", saveDestroyedThings: true);
		}
	}
}