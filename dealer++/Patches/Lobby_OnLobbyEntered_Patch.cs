using Il2CppScheduleOne.Networking;

namespace dealer__.Patches
{
    [HarmonyLib.HarmonyPatch(typeof(Lobby), "OnLobbyEntered")]
    class Lobby_OnLobbyEntered_Patch
    {
        [HarmonyLib.HarmonyPrefix]
        static bool Prefix(Lobby __instance)
        {
            return false;
        }
    }
}
