using UnityEngine;
using Unity.Netcode;
using System;

public class PlayerController : NetworkBehaviour
{
    // Reference to the Player's GameObject
    private int playerIndex = -1;
    private int xOffsetStart = -6;
    private int yOffsetStart = -2;
    void Start()
    {
        // Check if this script is running on the server
        if (IsServer)
        {
            // Only the server needs to handle spawning and moving players.
            AssignPlayerPosition();
        }
    }

    void AssignPlayerPosition()
    {
        // Get the current player’s index
        playerIndex = NetworkManager.Singleton.ConnectedClientsList.Count -1;

        if (playerIndex >= 0)
        {
            // Move the player on the X axis based on the player index
            MovePlayerOnXAxis(playerIndex);
        }
    }

    void MovePlayerOnXAxis(int index)
    {
        float xPosition = xOffsetStart + (index * 2f);
        float yPosition = 0;
        //When Adding new joining players move their starting Postion
        if(xPosition > 6)
        {
            yPosition = -yOffsetStart;
            xPosition -= xOffsetStart;
        }
        transform.position = new Vector3(xPosition, yPosition, transform.position.z);

    }
}
