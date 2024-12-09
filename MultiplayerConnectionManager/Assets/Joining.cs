using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MultiplayerConnectionManagerPackage;
using TMPro;
public class Joining : MonoBehaviour
{
    MultiplayerConnectionManager packageManager = new MultiplayerConnectionManager();
    public TMP_InputField input;
    public TMP_Text joinCode;
    // Start is called before the first frame update
    void Start()
    {
        packageManager.Start();
        
    }

    // Update is called once per frame
    void Update()
    {
        joinCode.text = packageManager.joinCode;
    }
    public void CreateRelay()
    {
        packageManager.CreateRelay(3);
    }
    public void JoinRelay()
    {
        packageManager.ProcessJoinCode(input);
    }
}
