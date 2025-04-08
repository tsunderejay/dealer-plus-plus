using Il2CppScheduleOne.Networking;
using Il2CppSteamworks;

namespace dealer__.Patches
{
    [HarmonyLib.HarmonyPatch(typeof(Lobby), "Awake")]
    class Lobby_Awake_Patch
    {
        [HarmonyLib.HarmonyPostfix]
        static void Postfix(Lobby __instance)
        {
            __instance.Players = new CSteamID[Config.LobbySize.Value];
        }
    }
}
