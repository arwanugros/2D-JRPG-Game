using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float interactRadius = 1.5f;
    public bool canMove = true;
    public LayerMask interactableLayer;
    
    [Header("Interaction UI")]
    public GameObject interactUI;
    public TextMeshProUGUI interactText;
    public float interactUIOffsetY = 0.5f;

    public InputActionReference moveAction;
    public InputActionReference interactAction;

    public Animator animator;
    public SpriteRenderer spriteRenderer;

    private Rigidbody2D rb;
    private Vector2 movement;
    private Collider2D currentInteractable;
    private Vector3 lastPosition;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        lastPosition = transform.position;
        if (interactText != null)
        {
            interactText.text = "Space";
        }
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
        if (canMove)
        {
            movement = moveAction.action.ReadValue<Vector2>();
            CheckInteractable();
        }
        else
        {
            movement = Vector2.zero;
        }
    }

    private void LateUpdate()
    {
        bool isMoving = false;
        Vector3 currentPos = transform.position;
        Vector3 delta = currentPos - lastPosition;

        if (canMove && movement.sqrMagnitude > 0.01f)
        {
            isMoving = true;
            
            if (movement.x > 0.01f && spriteRenderer != null) spriteRenderer.flipX = false;
            else if (movement.x < -0.01f && spriteRenderer != null) spriteRenderer.flipX = true;
        }
        else if (!canMove)
        {
            if (delta.sqrMagnitude > 0.000001f)
            {
                isMoving = true;
                
                if (delta.x > 0.001f && spriteRenderer != null) spriteRenderer.flipX = false;
                else if (delta.x < -0.001f && spriteRenderer != null) spriteRenderer.flipX = true;
            }
        }

        if (animator != null)
        {
            animator.SetBool("IsRunning", isMoving);
        }

        lastPosition = currentPos;
    }

    private void FixedUpdate()
    {
        if (canMove && movement != Vector2.zero)
        {
            rb.MovePosition(rb.position + movement * moveSpeed * Time.fixedDeltaTime);
        }
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
            if (interactUI != null)
            {
                interactUI.SetActive(true);
                
                SpriteRenderer npcSprite = currentInteractable.GetComponentInChildren<SpriteRenderer>();
                if (npcSprite != null)
                {
                    float spriteHeight = npcSprite.bounds.size.y;
                    Vector3 targetPos = npcSprite.transform.position + new Vector3(0f, spriteHeight / 2f + interactUIOffsetY, 0f);
                    interactUI.transform.position = targetPos;
                }
                else
                {
                    interactUI.transform.position = currentInteractable.transform.position + new Vector3(0f, 1.5f + interactUIOffsetY, 0f);
                }
            }
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
            if (interactUI != null) interactUI.SetActive(false);
            currentInteractable.SendMessage("OnInteract", SendMessageOptions.DontRequireReceiver);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, interactRadius);
    }
}