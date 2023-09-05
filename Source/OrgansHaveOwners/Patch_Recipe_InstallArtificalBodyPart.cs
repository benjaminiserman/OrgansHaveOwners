namespace OrgansHaveOwners
{
	using System.Collections.Generic;
	using System.Linq;
	using System.Reflection;
	using System.Reflection.Emit;
	using HarmonyLib;
	using RimWorld;
	using Verse;

	[HarmonyPatch(typeof(Recipe_InstallArtificialBodyPart))]
	[HarmonyPatch(nameof(Recipe_InstallArtificialBodyPart.ApplyOnPawn))]
	internal class Patch_Recipe_InstallArtificalBodyPart
	{
		static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
		{
			var code = instructions.ToList();
			var addForeignOrgan = typeof(ForeignOrganUtil)
				.GetMethod(nameof(ForeignOrganUtil.AddForeignOrgan));
			var insertInstructions = new List<CodeInstruction>()
			{
				new CodeInstruction(OpCodes.Ldarg, 1), // pawn
				new CodeInstruction(OpCodes.Ldarg, 2), // part
				new CodeInstruction(OpCodes.Ldarg, 4), // ingredients
				new CodeInstruction(OpCodes.Call, addForeignOrgan)
			};

			for (var i = 0; i < code.Count; i++)
			{
				var operandAsMethod = code[i].operand as MethodInfo;
				if (code[i].opcode == OpCodes.Callvirt 
					&& operandAsMethod?.Name == nameof(Pawn_HealthTracker.AddHediff))
				{
					code.InsertRange(i + 1, insertInstructions);

					Log.Message($"Found at position {i}");
					break;
				}
			}

			Log.Message($"completed transpile for {nameof(Patch_Recipe_InstallArtificalBodyPart)}");

			return code;
		}
	}
}
