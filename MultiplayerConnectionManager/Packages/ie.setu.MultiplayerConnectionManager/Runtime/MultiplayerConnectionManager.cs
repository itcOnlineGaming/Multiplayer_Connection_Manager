using Unity.Netcode.Transports.UTP;
using Unity.Netcode;
using Unity.Networking.Transport.Relay;
using Unity.Services.Relay.Models;
using Unity.Services.Relay;
using UnityEngine;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Collections;
using TMPro;
using System;
using UnityEngine.UI;

namespace MultiplayerConnectionManagerPackage
{
    public class MultiplayerConnectionManager : MonoBehaviour
    {
        public TMP_InputField inputCode;
        public string joinCode;

        public NetworkVariable<string> pinkReadyString = new NetworkVariable<string>();
        public NetworkVariable<string> blueReadyString = new NetworkVariable<string>();
        public NetworkVariable<string> redReadyString = new NetworkVariable<string>();
        public NetworkVariable<string> greenReadyString = new NetworkVariable<string>();


        public async void Start()
        {
            await UnityServices.InitializeAsync();

            AuthenticationService.Instance.SignedIn += () =>
            {
                Debug.Log(AuthenticationService.Instance.PlayerId + " Has Signed In");
            };
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
        }
        private void UpdatePlayerReadyUI(int playerId, TextMeshProUGUI readyText, Image readyImage)
        {
            string readyState = GetPlayerReadyState(playerId);

            // Update the readiness text
            readyText.text = readyState;

            if (readyState == "Ready")
            {
                readyImage.color = Color.green;
            }
            else if (readyState == "Not Ready")
            {
                readyImage.color = new Color(0.5019608f, 0.1921569f, 0.8156863f);
            }
        }
        public string GetPlayerReadyState(int playerId)
        {
            switch (playerId)
            {
                case 0:
                    return pinkReadyString.Value.ToString();
                default:
                    return "Not Ready"; 
            }
        }
        public void TogglePinkReady()
        {
            pinkReadyString.Value = pinkReadyString.Value.Equals("Not Ready") ? "Ready" : "Not Ready";
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
    }
}
