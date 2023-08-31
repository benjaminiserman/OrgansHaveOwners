namespace OrgansHaveOwners
{
	using System;
	using System.Linq;
	using System.Reflection;
	using HarmonyLib;
	using RimWorld;
	using Verse;

	[HarmonyPatch]
	internal class Patch_EquipmentUtility
	{
		static MethodBase TargetMethod()
		{
			var methods = typeof(EquipmentUtility).GetMethods(
				BindingFlags.Public | BindingFlags.Static | BindingFlags.ExactBinding
			).Where(method => method.Name == "CanEquip")
				.Where(method => method.GetParameters().Length == 4)
				.ToList();
			// no idea why the regular GetMethod was ambiguous here

			Log.Message(string.Join(", ", methods.Select(t => t.FullDescription())));

			return methods.First();
		}

		[HarmonyPostfix]
		static void Postfix(ref bool __result, ref Thing thing, ref Pawn pawn, ref string cantReason)
		{
			if (cantReason != "BiocodedCodedForSomeoneElse".Translate())
			{
				return;
			}

			if (thing.TryGetComp<CompBiocodable>() is CompBiocodable compBiocodable)
			{
				var biocodedPawn = compBiocodable.CodedPawn;

				foreach (var hediff in pawn.health.hediffSet.hediffs
					.Where(hediff => hediff.Part.def == BodyPartDefOf.Arm
						|| hediff.Part.def == BodyPartDefOf.Hand))
				{
					if (!HediffComp_SourcePawn.WasExtractedFromSpecificPawn(hediff, biocodedPawn))
					{
						__result = true;
						cantReason = null;
					}
				}
			}
		}
	}
}
