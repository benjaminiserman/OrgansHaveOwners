namespace OrgansHaveOwners
{
	using RimWorld;
	using Verse;

	internal class HediffComp_SourcePawn : HediffComp
	{
		protected bool wasExtractedFromNamedPawn;

		protected Pawn sourcePawn;

		protected string sourcePawnLabel;

		public bool WasExtractedFromNamedPawn => wasExtractedFromNamedPawn;

		public Pawn SourcePawn => sourcePawn;

		public string SourcePawnLabel => sourcePawnLabel;

		public static bool WasExtractedFromSpecificPawn(Hediff hediff, Pawn pawn)
		{
			var compSourcePawn = hediff.TryGetComp<HediffComp_SourcePawn>();

			if (compSourcePawn != null && compSourcePawn.WasExtractedFromNamedPawn)
			{
				return compSourcePawn.SourcePawn == pawn;
			}

			return false;
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

		public override string CompTipStringExtra => "Winggar.ForeignPart_CompTip".Translate(sourcePawnLabel);
	}
}