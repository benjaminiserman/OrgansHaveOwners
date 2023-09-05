namespace OrgansHaveOwners
{
	using System.Collections.Generic;
	using System.Reflection;
	using System.Linq;
	using HarmonyLib;
	using RimWorld;
	using Verse;
	using System.Reflection.Emit;

	/*[HarmonyPatch(typeof(MedicalRecipesUtility))]
	[HarmonyPatch(nameof(MedicalRecipesUtility.SpawnThingsFromHediffs))]
	internal class Patch_MedicalRecipesUtility_SpawnThingsFromHediffs
	{
		public static Thing SpawnWithComp(ThingDef def, IntVec3 loc, Map map, WipeMode wipeMode, Pawn sourcePawn)
		{
			Log.Message(sourcePawn.ToString());
			var thingSpawned = GenSpawn.Spawn(def, loc, map, wipeMode);
			if (thingSpawned is ThingWithComps thingWithCompsSpawned)
			{
				CompSourcePawn.AddCompSourcePawn(thingWithCompsSpawned, sourcePawn);
			}

			return thingSpawned;
		}

		static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
		{
			var code = instructions.ToList();
			var addCompSourcePawn = typeof(Patch_MedicalRecipesUtility_SpawnThingsFromHediffs)
				.GetMethod(nameof(Patch_MedicalRecipesUtility_SpawnThingsFromHediffs.SpawnWithComp));
			var insertInstructions = new List<CodeInstruction>()
			{
				new CodeInstruction(OpCodes.Ldarg, 1), // pawn
				new CodeInstruction(OpCodes.Call, addCompSourcePawn),
			};

			for (var i = 0; i < code.Count; i++)
			{
				var operandAsMethod = code[i].operand as MethodInfo;
				if (code[i].opcode == OpCodes.Call
					&& operandAsMethod?.Name == nameof(GenSpawn.Spawn))
				{
					code.RemoveAt(i);
					code.InsertRange(i, insertInstructions);

					Log.Message($"Found at position {i}");
					break;
				}
			}

			Log.Message($"completed transpile for {nameof(Patch_MedicalRecipesUtility_SpawnThingsFromHediffs)}");

			Log.Message(string.Join("\n", code.Select(x => $"{x.opcode} {x.operand}")));

			return code;
		}
	}*/

	[HarmonyPatch]
	internal class Patch_MedicalRecipesUtility_SpawnThingsFromHediffs
	{
		static MethodBase TargetMethod() => typeof(MedicalRecipesUtility)
			.GetMethod(nameof(MedicalRecipesUtility.SpawnThingsFromHediffs), BindingFlags.Public | BindingFlags.Static);

		[HarmonyPrefix]
		static bool Prefix(ref Pawn pawn, BodyPartRecord part, ref IntVec3 pos, ref Map map)
		{
			if (!pawn.health.hediffSet.GetNotMissingParts().Contains(part))
			{
				return false;
			}

			foreach (var item in pawn.health.hediffSet.hediffs
				.Where(x => x.Part == part))
			{
				if (item.def.spawnThingOnRemoved == null)
				{
					continue;
				}

				var thingSpawned = GenSpawn.Spawn(item.def.spawnThingOnRemoved, pos, map);
				if (thingSpawned is ThingWithComps thingWithCompsSpawned)
				{
					Log.Message($"1: {thingWithCompsSpawned}");
					var sourcePawn = pawn;
					if (pawn.health.hediffSet.hediffs
						.FirstOrDefault(x => x?.Part == part 
							&& x?.sourceHediffDef?.defName == OrgansHaveOwners.ForeignOrganHediffName) 
						is HediffWithComps existingForeignOrganHediffWithComps)
					{
						Log.Message($"2: {existingForeignOrganHediffWithComps}");
						if (existingForeignOrganHediffWithComps.comps
							.FirstOrDefault(x => x is HediffComp_SourcePawn) 
							is HediffComp_SourcePawn sourcePawnHediffComp)
						{
							Log.Message($"3: {sourcePawnHediffComp}");
							sourcePawn = sourcePawnHediffComp.SourcePawn;
							Log.Message($"4: {sourcePawn}");
						}
					}

					Log.Message($"5: {sourcePawn}");
					CompSourcePawn.AddCompSourcePawn(thingWithCompsSpawned, sourcePawn);
				}
			}

			for (var i = 0; i < part.parts.Count; i++)
			{
				Prefix(ref pawn, part.parts[i], ref pos, ref map);
			}

			return false;
		}
	}

	[HarmonyPatch]
	internal class Patch_MedicalRecipesUtility_SpawnNaturalPartIfClean
	{
		static MethodBase TargetMethod() => typeof(MedicalRecipesUtility)
			.GetMethod(nameof(MedicalRecipesUtility.SpawnNaturalPartIfClean), BindingFlags.Public | BindingFlags.Static);

		[HarmonyPostfix]
		static void Postfix(ref Thing __result, ref Pawn pawn)
		{
			if (__result is ThingWithComps thingWithComps)
			{
				CompSourcePawn.AddCompSourcePawn(thingWithComps, pawn);
			}
		}
	}

	[HarmonyPatch]
	internal class Patch_MedicalRecipesUtility_IsClean
	{
		static MethodBase TargetMethod() => typeof(MedicalRecipesUtility)
			.GetMethod(nameof(MedicalRecipesUtility.IsClean), BindingFlags.Public | BindingFlags.Static);

		[HarmonyPostfix]
		static void Postfix(ref bool __result, ref Pawn pawn, ref BodyPartRecord part)
		{
			var unboxedPartRef = part;

			if (pawn.Dead)
			{
				__result = false;
				return;
			}

			if (!__result)
			{
				var foundHediffThatIsntForeignPart = pawn.health.hediffSet.hediffs
					.Where((Hediff hediff) => hediff.Part == unboxedPartRef
						&& hediff.sourceHediffDef?.defName != OrgansHaveOwners.ForeignOrganHediffName)
					.Any();
				__result = !foundHediffThatIsntForeignPart;
			}
		}
	}
}
