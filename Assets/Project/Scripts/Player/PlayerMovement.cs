using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float interactRadius = 1.5f;
    public bool canMove = true;
    public LayerMask interactableLayer;
    public GameObject interactUI;

    public InputActionReference moveAction;
    public InputActionReference interactAction;

    private Rigidbody2D rb;
    private Vector2 movement;
    private Collider2D currentInteractable;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void OnEnable()
    {
        interactAction.action.performed += OnInteractPerformed;
    }

    private void OnDisable()
    {
        interactAction.action.performed -= OnInteractPerformed;
    }

    private void Update()
    {
        if (!canMove)
        {
            movement = Vector2.zero;
            return;
        }

        movement = moveAction.action.ReadValue<Vector2>();
        CheckInteractable();
    }

    private void FixedUpdate()
    {
        rb.MovePosition(rb.position + movement * moveSpeed * Time.fixedDeltaTime);
    }

    private void OnInteractPerformed(InputAction.CallbackContext context)
    {
        if (!canMove) return;
        Interact();
    }

    private void CheckInteractable()
    {
        currentInteractable = Physics2D.OverlapCircle(rb.position, interactRadius, interactableLayer);
        
        if (currentInteractable != null)
        {
            if (interactUI != null) interactUI.SetActive(true);
        }
        else
        {
            if (interactUI != null) interactUI.SetActive(false);
        }
    }

    private void Interact()
    {
        if (currentInteractable != null)
        {
            currentInteractable.SendMessage("OnInteract", SendMessageOptions.DontRequireReceiver);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, interactRadius);
    }
}