using Il2CppScheduleOne.Networking;

namespace dealer__.Patches
{
    [HarmonyLib.HarmonyPatch(typeof(Lobby), "OnLobbyCreated")]
    class Lobby_OnLobbyCreated_Patch
    {
        [HarmonyLib.HarmonyPrefix]
        static bool Prefix(Lobby __instance)
        {
            return false;
        }
    }
}
