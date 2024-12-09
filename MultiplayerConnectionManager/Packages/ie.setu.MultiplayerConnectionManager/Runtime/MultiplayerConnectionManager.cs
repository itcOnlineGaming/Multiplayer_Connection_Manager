using Unity.Netcode.Transports.UTP;
using Unity.Netcode;
using Unity.Networking.Transport.Relay;
using Unity.Services.Relay.Models;
using Unity.Services.Relay;
using UnityEngine;
using Unity.Services.Authentication;
using Unity.Services.Core;
using TMPro;

namespace MultiplayerConnectionManagerPackage
{
    public class MultiplayerConnectionManager : MonoBehaviour
    {
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

        public async void CreateRelay()
        {
            try
            {
                Allocation alloc = await RelayService.Instance.CreateAllocationAsync(3);
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
        public int CheckNumberOfPlayers()
        {
            int playersAmount = NetworkManager.Singleton.ConnectedClientsList.Count; // Including the host
            Debug.Log("Current number of players in the game: " + playersAmount);
            return playersAmount; // Including the host
        }
    }
}
