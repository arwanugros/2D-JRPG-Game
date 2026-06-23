using UnityEngine;
using Fungus;

[RequireComponent(typeof(BoxCollider2D))]
public class CutsceneTrigger : MonoBehaviour
{
    public string fungusMessage = "TriggerCutscene";
    private bool hasTriggered = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!hasTriggered && collision.CompareTag("Player"))
        {
            hasTriggered = true;
            
            PlayerMovement player = collision.GetComponent<PlayerMovement>();
            if (player != null)
            {
                player.canMove = false;
            }
            
            Flowchart.BroadcastFungusMessage(fungusMessage);
        }
    }
}