namespace OrgansHaveOwners
{
	using System.Collections.Generic;
	using System.Reflection;
	using System.Linq;
	using HarmonyLib;
	using RimWorld;
	using Verse;

	[HarmonyPatch]
	internal class Patch_MedicalRecipesUtility_SpawnNaturalPartIfClean
	{
		static MethodBase TargetMethod() => typeof(MedicalRecipesUtility).GetMethod("SpawnNaturalPartIfClean", BindingFlags.Public | BindingFlags.Static);

		[HarmonyPostfix]
		static void Postfix(ref Thing __result, ref Pawn pawn)
		{
			CompSourcePawn.AddCompSourcePawn(__result as ThingWithComps, pawn);
		}
	}

	[HarmonyPatch]
	internal class Patch_MedicalRecipesUtility_IsClean
	{
		static MethodBase TargetMethod() => typeof(MedicalRecipesUtility).GetMethod("IsClean", BindingFlags.Public | BindingFlags.Static);

		[HarmonyPostfix]
		static void Postfix(ref bool __result, ref Pawn pawn, ref BodyPartRecord part)
		{
			var unboxedPartRef = part;

			if (!pawn.Dead && !__result)
			{
				__result = !pawn.health.hediffSet.hediffs
					.Where((Hediff hediff) => hediff.Part == unboxedPartRef && hediff.sourceHediffDef.defName != "ForeignPart").Any();
			}
		}
	}
}
