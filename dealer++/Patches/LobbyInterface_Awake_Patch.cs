using HarmonyLib;
using Il2CppScheduleOne.UI.Multiplayer;

namespace dealer__.Patches
{
    [HarmonyPatch(typeof(LobbyInterface), "Awake")]
    class LobbyInterface_Awake_Patch
    {
        [HarmonyPostfix]
        static void Postfix(LobbyInterface __instance)
        {
            if (__instance.Lobby == null) return;

            Action lobbyChange = delegate ()
            {
                __instance.UpdateButtons();
                __instance.UpdatePlayers();

                __instance.LobbyTitle.text =
                    $"Lobby ({__instance.Lobby.PlayerCount}/{Config.LobbySize.Value})";
            };

            __instance.Lobby.onLobbyChange = lobbyChange;
        }
    }
}
