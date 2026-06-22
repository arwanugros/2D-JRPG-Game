using UnityEngine;
using Fungus;

[RequireComponent(typeof(BoxCollider2D))]
public class NPCInteractable : MonoBehaviour
{
    public string fungusMessage;

    public void OnInteract()
    {
        if (!string.IsNullOrEmpty(fungusMessage))
        {
            Flowchart.BroadcastFungusMessage(fungusMessage);
        }
    }
}