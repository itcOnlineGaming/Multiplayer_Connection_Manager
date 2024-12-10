using UnityEngine;
using Unity.Services.Authentication;
using Unity.Services.Core;
using static UnityEngine.EventSystems.StandaloneInputModule;
using TMPro;
using Unity.Netcode.Transports.UTP;
using Unity.Netcode;
using Unity.Networking.Transport.Relay;
using Unity.Services.Relay.Models;
using Unity.Services.Relay;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MultiplayerConnectionManagerPackage
{
    public class PlayerInfo
    {
        public ulong NetworkObjectId;
        public GameObject PlayerObject;
        public Vector3 Position;
        public bool Joined;
        public PlayerInfo(ulong networkObjectId, GameObject playerObject, Vector3 position, bool joined)
        {
            NetworkObjectId = networkObjectId;
            PlayerObject = playerObject;
            Position = position;
            Joined = joined;
        }
    }



    public class MultiplayerConnectionManager : MonoBehaviour
    {

        public event Action<ulong> OnPlayerJoined;
        public event Action<ulong> OnPlayerLeft;
        private HashSet<ulong> activePlayerIds = new HashSet<ulong>();
        private List<PlayerInfo> activePlayers = new List<PlayerInfo>(); // To store all currently active players

        public TMP_InputField inputCode;
        public string joinCode;
        public async void Start()
        {
            await UnityServices.InitializeAsync();

            AuthenticationService.Instance.SignedIn += () =>
            {
                Debug.Log(AuthenticationService.Instance.PlayerId + " Has Signed In");
            };
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
        }

        public async void CreateRelay(int numberOfPlayers)
        {
            try
            {
                Allocation alloc = await RelayService.Instance.CreateAllocationAsync(numberOfPlayers);
                joinCode = await RelayService.Instance.GetJoinCodeAsync(alloc.AllocationId);
                Debug.Log(joinCode);

                RelayServerData relayServerData = new RelayServerData(alloc, "dtls");
                NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);
                NetworkManager.Singleton.StartHost();
            }
            catch (RelayServiceException e)
            {
                Debug.Log(e);
            }
        }

        public void ProcessJoinCode(TMP_InputField inputCode)
        {
            JoinRelay(inputCode.text);
        }

        private async void JoinRelay(string joinCode)
        {
            try
            {
                JoinAllocation joinAlloc = await RelayService.Instance.JoinAllocationAsync(joinCode);
                Debug.Log("Joining Relay with code: " + joinCode);

                RelayServerData relayServerData = new RelayServerData(joinAlloc, "dtls");
                NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);
                NetworkManager.Singleton.StartClient();
            }
            catch (RelayServiceException e)
            {
                Debug.Log(e);
            }
        }

        private PlayerInfo NotifyPlayerJoined(ulong clientId, GameObject playerObject)
        {
            // Add the player to the activePlayers list
            PlayerInfo playerInfo = new PlayerInfo(clientId, playerObject, playerObject.transform.position, true);
            activePlayers.Add(playerInfo);

            return playerInfo;
        }

        private PlayerInfo NotifyPlayerLeft(ulong clientId)
        {
            // Find the player in the activePlayers list to get their last position
            PlayerInfo playerInfoToRemove = activePlayers.Find(player => player.NetworkObjectId == clientId);

            if (playerInfoToRemove != null)
            {
                // Return the complete PlayerInfo before removal
                return new PlayerInfo(clientId, playerInfoToRemove.PlayerObject, playerInfoToRemove.Position, false);
            }

            // Return a default PlayerInfo if no match is found
            return new PlayerInfo(clientId, null, Vector3.zero, false);
        }

        public List<PlayerInfo> FindPlayers(string nameTag)
        {
            // Get all GameObjects with the given tag
            GameObject[] playersFound = GameObject.FindGameObjectsWithTag(nameTag);

            HashSet<ulong> currentPlayerIds = new HashSet<ulong>();
            List<PlayerInfo> changes = new List<PlayerInfo>(); // To store only changes (joins or leaves)

            List<PlayerInfo> playersToRemove = new List<PlayerInfo>(); // To store players who should leave

            // Iterate over the found players
            foreach (GameObject player in playersFound)
            {
                if (player.TryGetComponent<NetworkObject>(out NetworkObject networkObject))
                {
                    ulong networkObjectId = networkObject.NetworkObjectId;
                    currentPlayerIds.Add(networkObjectId);

                    // If this player is not in the active list, they are a new player
                    PlayerInfo existingPlayerInfo = activePlayers.Find(p => p.NetworkObjectId == networkObjectId);
                    if (existingPlayerInfo == null)
                    {
                        // Add the new player to the changes list
                        changes.Add(NotifyPlayerJoined(networkObjectId, player));
                    }
                    else
                    {
                        // If the player is already in the active list, update their position
                        existingPlayerInfo.Position = player.transform.position;
                    }
                }
            }

            // Now check for players who have left
            foreach (PlayerInfo previousPlayerInfo in activePlayers)
            {
                if (!currentPlayerIds.Contains(previousPlayerInfo.NetworkObjectId))
                {
                    // If a player is no longer found, add them to the changes list
                    changes.Add(NotifyPlayerLeft(previousPlayerInfo.NetworkObjectId));

                    // Add the player to be removed
                    playersToRemove.Add(previousPlayerInfo);
                }
            }

            // After the loop, remove players who left from the activePlayers list
            foreach (var playerToRemove in playersToRemove)
            {
                activePlayers.Remove(playerToRemove);
            }

            return changes; // Return only the changes (joins or leaves)
        }

    }
}