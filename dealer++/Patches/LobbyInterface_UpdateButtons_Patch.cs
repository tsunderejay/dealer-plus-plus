using Il2CppScheduleOne.UI.Multiplayer;
using HarmonyLib;
using System.Reflection.Emit;

namespace dealer__.Patches
{
    [HarmonyPatch(typeof(LobbyInterface), "UpdateButtons")]
    class LobbyInterface_UpdateButtons_Patch
    {
        [HarmonyTranspiler]
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            foreach (var instruction in instructions)
            {
                if (instruction.opcode == OpCodes.Ldc_I4_4)
                {
                    yield return new CodeInstruction(OpCodes.Ldsfld, Config.LobbySize.Value);
                    yield return new CodeInstruction(OpCodes.Conv_I4, null);
                    continue;
                }
                yield return instruction;
            }
        }
    }
}
