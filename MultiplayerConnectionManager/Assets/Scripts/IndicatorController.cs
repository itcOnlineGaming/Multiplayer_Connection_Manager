using Unity.Netcode;
using UnityEngine;

public class Indicator : NetworkBehaviour
{
    public SpriteRenderer spriteRenderer;

    // Network variable to synchronize the color across clients
    public NetworkVariable<Color> indicatorColor = new NetworkVariable<Color>(Color.green);

    // Called when the color is updated on the network
    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        // Set the color based on the NetworkVariable
        indicatorColor.OnValueChanged += OnColorChanged;
        spriteRenderer.color = indicatorColor.Value;
    }

    private void OnColorChanged(Color oldColor, Color newColor)
    {
        // Update the color when the NetworkVariable changes
        spriteRenderer.color = newColor;
    }

    // Method to change the color and update it on all clients
    public void ChangeColor(Color newColor)
    {
        if (IsServer)
        {
            indicatorColor.Value = newColor;  // Update the color on the server
        }
        else
        {
            // Call an RPC to change the color if you're not on the server
            ChangeColorServerRpc(newColor);
        }
    }

    // ServerRpc to update color on the server
    [ServerRpc]
    private void ChangeColorServerRpc(Color newColor)
    {
        indicatorColor.Value = newColor;
    }
}
