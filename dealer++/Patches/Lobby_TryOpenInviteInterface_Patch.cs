using Il2CppScheduleOne.Networking;
using HarmonyLib;
using System.Reflection.Emit;

namespace dealer__.Patches
{
    [HarmonyPatch(typeof(Lobby), "TryOpenInviteInterface")]
    class Lobby_TryOpenInviteInterface_Patch
    {
        [HarmonyTranspiler]
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            foreach (var instruction in instructions)
            {
                if (instruction.opcode == OpCodes.Ldc_I4_4)
                {
                    yield return new CodeInstruction(OpCodes.Ldc_I4, Config.LobbySize.Value);
                    continue;
                }
                yield return instruction;
            }
        }
    }
}
