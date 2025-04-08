using MelonLoader;
using Il2CppSteamworks;
using Il2CppScheduleOne.Networking;
using Il2CppScheduleOne.UI.Multiplayer;
using Il2Cpp;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Il2CppScheduleOne.Persistence;
using Il2CppScheduleOne.UI;
using Il2CppScheduleOne.PlayerScripts;
using Il2CppScheduleOne.DevUtilities;

[assembly: MelonInfo(typeof(dealer__.Core), "dealer++", "1.0.0", "jamie", null)]
[assembly: MelonGame("TVGS", "Schedule I")]

namespace dealer__;

public class Core : MelonMod
{
    private HarmonyLib.Harmony harmony;

    public static MelonLogger.Instance Logger;

    public override void OnInitializeMelon()
    {
        Logger = LoggerInstance;
        Logger.Msg("getting ready...");

        harmony = new HarmonyLib.Harmony("dealer++");
        harmony.PatchAll();

        Config.Category = MelonPreferences.CreateCategory("dealer++", "dealer++");
        Config.LobbyPassword = Config.Category.CreateEntry("LobbyPassword", "p455w0rd");
        Config.LobbySize = Config.Category.CreateEntry("LobbySize", 20);

        Core.Logger.Msg(System.ConsoleColor.Cyan, "dealer++ loaded");
        Core.Logger.Msg(System.ConsoleColor.Cyan, "lobby size: " + Config.LobbySize.Value);
        Core.Logger.Msg(System.ConsoleColor.Cyan, "lobby password: " + Config.LobbyPassword.Value);
    }

    public override void OnSceneWasLoaded(int buildIndex, string sceneName)
    {
        // "Main" for in game
        if (sceneName == "Menu")
        {
            MelonCoroutines.Start(Utils.NewLobby.Initialize());

            if (!SteamManager.Initialized)
            {
                Logger.Error("steam not initialized");
                return;
            }
        }
    }
}