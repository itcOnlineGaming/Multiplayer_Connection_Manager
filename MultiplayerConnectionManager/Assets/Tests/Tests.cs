using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using UnityEngine.UI;

public class Tests
{
    [UnityTest]
    public IEnumerator TestHostingAServerWithAppropriateAmountOfUsers()
    {
        SceneManager.LoadScene("SampleScene");
        yield return new WaitForSeconds(1);
        var hostButton = GameObject.Find("HostButton").GetComponent<Button>();
        hostButton.onClick.Invoke();
        var inputField = GameObject.Find("UsersAmountInput")?.GetComponent<TMP_InputField>();
        inputField.text = "2";
        var userAmountButton = GameObject.Find("EnterUserAmount").GetComponent<Button>();
        userAmountButton.onClick.Invoke();
        // Wait for the action to propagate
        yield return new WaitForSeconds(0.5f);

        // Get the OnlineManager GameObject and retrieve the Joining script
        var onlineManager = GameObject.Find("OnlineManager");
        var joiningScript = onlineManager.GetComponent<OnlineSceneManager>();

        // Assert that the user amount matches the input field text
        Assert.AreEqual(2, joiningScript.usersPossibleAmount, "The user amount in the Joining script should match the input field text.");
        yield return new WaitForSeconds(0.5f);
    }
    [UnityTest]
    public IEnumerator TestingFindPlayerCorrectlyShowingIndicator()
    {
        SceneManager.LoadScene("SampleScene");
        yield return new WaitForSeconds(1);
        var hostButton = GameObject.Find("HostButton").GetComponent<Button>();
        hostButton.onClick.Invoke();
        var inputField = GameObject.Find("UsersAmountInput")?.GetComponent<TMP_InputField>();
        inputField.text = "2";
        var userAmountButton = GameObject.Find("EnterUserAmount").GetComponent<Button>();
        userAmountButton.onClick.Invoke();
        // Wait for the action to propagate
        yield return new WaitForSeconds(2f);

        var indicator = GameObject.Find("PlayerLocationIndicator(Clone)").GetComponent<SpriteRenderer>();
        Assert.AreEqual(indicator.color, Color.green);
        
        yield return new WaitForSeconds(0.5f);
    }


}
