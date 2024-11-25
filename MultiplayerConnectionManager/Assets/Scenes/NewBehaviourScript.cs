using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MultiplayerConnectionManagerPackage;

public class NewBehaviourScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        new MultiplayerConnectionManager();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
