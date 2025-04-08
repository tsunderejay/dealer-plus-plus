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
using Il2CppScheduleOne.DevUtilities;
using System.Linq;

namespace dealer__.Utils
{
    public class NewLobby
    {
        private static Lobby Lobby;
        private static LobbyInterface LobbyInterface;

        private static Callback<LobbyCreated_t> LobbyCreatedCallback;
        private static Callback<LobbyEnter_t> LobbyEnteredCallback;

        private static bool IsCreatingLobby = false;
        private static CSteamID HostID;

        private static void CreatePlayerIcons(int size)
        {
            size = Mathf.Clamp(size, 2, 32);

            try
            {
                var lobbyInterface = LobbyInterface.transform;
                var lobbyInterfaceContainer = lobbyInterface.FindChild("Container");
                var avatarListTransform = lobbyInterfaceContainer.FindChild("Entries");
                var avatarList = avatarListTransform.gameObject;
                var avatarSize = avatarListTransform.childCount - 2;
                var avatarGameObject = avatarListTransform.FindChild("Entry").gameObject;

                for (int i = avatarSize + 1; i < size; i++)
                {
                    GameObject newEntry = UnityEngine.Object.Instantiate(avatarGameObject, avatarListTransform);
                    newEntry.name = $"{avatarGameObject.name} ({i - 1})";
                }

                var hostEntry = avatarListTransform.FindChild($"Entry ({avatarSize})");
                hostEntry.name = $"Entry ({size - 1})";
                hostEntry.SetAsLastSibling();

                Lobby.onLobbyChange?.Invoke();
            }
            catch (System.Exception e)
            {
                Core.Logger.Error(e);
            }
        }

        public static System.Collections.IEnumerator Initialize()
        {
            NewLobby.LobbyInterface = Singleton<LobbyInterface>.Instance;
            NewLobby.Lobby = Singleton<Lobby>.Instance;

            NewLobby.LobbyCreatedCallback = Callback<LobbyCreated_t>.Create(new Action<LobbyCreated_t>(OnLobbyCreated));
            NewLobby.LobbyEnteredCallback = Callback<LobbyEnter_t>.Create(new Action<LobbyEnter_t>(OnLobbyEntered));

            try
            {
                CreatePlayerIcons(Config.LobbySize.Value);

                var lobbyInterface = LobbyInterface.transform;
                var lobbyInterfaceContainer = lobbyInterface.FindChild("Container");
                var avatarListTransform = lobbyInterfaceContainer.FindChild("Entries");
                var inviteButton = avatarListTransform.FindChild("Invite").GetComponent<Button>();
                inviteButton.onClick.RemoveAllListeners();
                inviteButton.onClick.AddListener(new Action(OnInviteClicked));
            }
            catch (System.Exception e)
            {
                Core.Logger.Error(e);
            }

            yield break;
        }

        private static void OnInviteClicked()
        {
            if (!SteamManager.Initialized)
            {
                return;
            }

            if (Lobby.LobbyID == 0ul)
            {
                IsCreatingLobby = true;
                Lobby.CreateLobby();
                SteamFriends.ActivateGameOverlayInviteDialog(Lobby.LobbySteamID);
            }
            else 
            {
                if (SteamMatchmaking.GetNumLobbyMembers(Lobby.LobbySteamID) <= Config.LobbySize.Value)
                {
                    SteamFriends.ActivateGameOverlayInviteDialog(Lobby.LobbySteamID);
                }
            }
        }

        private static void OnLobbyCreated(LobbyCreated_t result)
        {
            CSteamID cSteamID = new(result.m_ulSteamIDLobby);
            Lobby._LobbyID_k__BackingField = cSteamID.m_SteamID;

            SteamMatchmaking.SetLobbyData(cSteamID, "owner", SteamUser.GetSteamID().ToString());
            SteamMatchmaking.SetLobbyData(cSteamID, "password", Config.LobbyPassword.Value);
            SteamMatchmaking.SetLobbyData(cSteamID, "host_loading", "false");
            SteamMatchmaking.SetLobbyData(cSteamID, "ready", "false");
            SteamMatchmaking.SetLobbyData(cSteamID, "size", Config.LobbySize.Value.ToString());

            Lobby.onLobbyChange?.Invoke();
        }

        private static void OnLobbyEntered(LobbyEnter_t result)
        {
            MelonCoroutines.Start(OnLobbyEnterCoroutine(result));
        }

        private static System.Collections.IEnumerator OnLobbyEnterCoroutine(LobbyEnter_t result)
        {
            Func<bool> isMenuLoaded = () => SceneManager.GetSceneByName("Menu").isLoaded;
            yield return new WaitUntil(isMenuLoaded);

            CSteamID lobbyID = new(result.m_ulSteamIDLobby);
            CSteamID ownerID = SteamMatchmaking.GetLobbyOwner(lobbyID);

            if (IsCreatingLobby)
            {
                IsCreatingLobby = false;
                HostID = ownerID;

                Lobby._LobbyID_k__BackingField = lobbyID.m_SteamID;
                Lobby.UpdateLobbyMembers();

                Lobby.onLobbyChange?.Invoke();
            }
            else
            {
                HostID = ownerID;

                string lobbyPassword = SteamMatchmaking.GetLobbyData(lobbyID, "password");

                if (lobbyPassword != Config.LobbyPassword.Value)
                {
                    Core.Logger.Msg("lobby password does not match");
                    yield break;
                }

                Lobby._LobbyID_k__BackingField = lobbyID.m_SteamID;

                Core.Logger.Msg("lobby password matches");

                var lobbySizeStr = SteamMatchmaking.GetLobbyData(lobbyID, "size");
                var lobbySize = 0;
                if (string.IsNullOrEmpty(lobbySizeStr))
                    lobbySize = 20; // multiplayer+fullgame compat ( fuck that mod )
                else
                    lobbySize = int.Parse(lobbySizeStr);

                CreatePlayerIcons(lobbySize);
                Config.LobbySize.Value = lobbySize;

                Lobby.UpdateLobbyMembers();
                Lobby.onLobbyChange?.Invoke();

                if (SteamMatchmaking.GetLobbyData(lobbyID, "ready") == "true")
                {
                    Lobby.JoinAsClient(ownerID.m_SteamID.ToString());
                }

                if (SteamMatchmaking.GetLobbyData(lobbyID, "host_loading") == "true")
                {
                    Singleton<LoadManager>.Instance.SetWaitingForHostLoad();
                    Singleton<LoadingScreen>.Instance.Open(false);
                }
            }

            yield break;
        }
    }
}
