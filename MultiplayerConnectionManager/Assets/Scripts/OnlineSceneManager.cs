using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MultiplayerConnectionManagerPackage;
using TMPro;
using System.Transactions;
using UnityEngine.UI;
using Unity.Netcode;
public class OnlineSceneManager : MonoBehaviour
{
    MultiplayerConnectionManager packageManager = new MultiplayerConnectionManager();
    public GameObject playerIndicator;
    public Button hostButton;
    public Button joinButton;
    public Button useJoinCodeButton;
    public TMP_InputField input;
    public TMP_Text joinCode;
    public TMP_InputField userAmountInput;
    public TMP_Text userAmountText;
    public Button enterAmountButton;
    bool hostWasClicked = false;
    public int usersPossibleAmount = 0;

    void Start()
    {
        packageManager.Start();
        input.gameObject.SetActive(false);
        joinCode.gameObject.SetActive(false);
        useJoinCodeButton.gameObject.SetActive(false);
        userAmountInput.gameObject.SetActive(false);
        userAmountText.gameObject.SetActive(false);
        enterAmountButton.gameObject.SetActive(false);
    }

    void Update()
    {
        if (hostWasClicked)
        {
            joinCode.text = "Join Code:" + packageManager.joinCode;
            List<PlayerInfo> info = packageManager.FindPlayers("PlayerTag");
            // Instantiate the indicator at the player's position
            foreach (var playerInfo in info)
            {
                // Instantiate the indicator at the player's position
                GameObject indicator = Instantiate(playerIndicator, playerInfo.Position - new Vector3(0,-5,0), Quaternion.identity);
                // Spawn the indicator across the network
                NetworkObject networkObject = indicator.GetComponent<NetworkObject>();
                if (networkObject != null)
                {
                    networkObject.Spawn();
                }
                // Access the Indicator component
                Indicator indicatorScript = indicator.GetComponent<Indicator>();

                if (indicatorScript != null)
                {
                    // Change the indicator color based on whether the player joined or left
                    if (playerInfo.Joined)
                    {
                        Debug.Log($"Player {playerInfo.NetworkObjectId} joined at position {playerInfo.Position}");
                        indicatorScript.ChangeColor(Color.green);  // Change color to green for joining
                    }
                    else
                    {
                        Debug.Log($"Player {playerInfo.NetworkObjectId} left at position {playerInfo.Position}");
                        indicatorScript.ChangeColor(Color.red);  // Change color to red for leaving
                    }

                    
                }
            }
        }
    }
    public void Host()
    {
        // Turn Off Current UI
        joinButton.gameObject.SetActive(false);
        hostButton.gameObject.SetActive(false);
        // Allow User to Set Relay size through UI
        userAmountInput.gameObject.SetActive(true);
        userAmountText.gameObject.SetActive(true);
        enterAmountButton.gameObject.SetActive(true);
    }
    public void JoinButtonClick()
    {
        // Turn off current UI
        joinButton.gameObject.SetActive(false);
        hostButton.gameObject.SetActive(false);
        // Allow User to Input the JoinCode
        useJoinCodeButton.gameObject.SetActive(true);
        input.gameObject.SetActive(true);
    }
    public void JoinRelay()
    {
        // Join the relay with the joincode
        packageManager.ProcessJoinCode(input);
        // Turn off current UI
        useJoinCodeButton.gameObject.SetActive(false);
        input.gameObject.SetActive(false);
    }
    public void SetUsersAmount()
    {
        // Get the Int Value of the InputBox
        if(int.TryParse(userAmountInput.text, out int result))
        {
            usersPossibleAmount = result;
        }
        else
        {
            usersPossibleAmount = 3;
        }
        Debug.Log(usersPossibleAmount.ToString());
        // Turn Off Current UI
        userAmountInput.gameObject.SetActive(false);
        userAmountText.gameObject.SetActive(false);
        enterAmountButton.gameObject.SetActive(false);
        // Create Relay using the amount set through the Input Box
        packageManager.CreateRelay(usersPossibleAmount);
        joinCode.gameObject.SetActive(true);
        hostWasClicked = true;
    }
}
