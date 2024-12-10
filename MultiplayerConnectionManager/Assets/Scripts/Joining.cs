using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MultiplayerConnectionManagerPackage;
using TMPro;
using System.Transactions;
using UnityEngine.UI;
public class Joining : MonoBehaviour
{
    MultiplayerConnectionManager packageManager = new MultiplayerConnectionManager();
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
        if(hostWasClicked)
        {
            joinCode.text = "Join Code:" + packageManager.joinCode;
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
