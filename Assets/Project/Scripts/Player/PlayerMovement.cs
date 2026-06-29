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
    private float lastFacingX = 1f;
    private float interactCooldown = 0f;

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
        if (moveAction != null && moveAction.action != null) moveAction.action.Enable();
        if (interactAction != null && interactAction.action != null) interactAction.action.Enable();
    }

    private void Update()
    {
        if (interactCooldown > 0f)
        {
            interactCooldown -= Time.deltaTime;
        }

        if (canMove)
        {
            movement = moveAction.action.ReadValue<Vector2>();
            CheckInteractable();

            bool isInteractPressed = false;
            
            if (interactAction != null && interactAction.action != null && interactAction.action.WasPressedThisFrame())
            {
                isInteractPressed = true;
            }
            
            if (Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame)
            {
                isInteractPressed = true;
            }

            if (isInteractPressed && interactCooldown <= 0f)
            {
                Interact();
            }
        }
        else
        {
            movement = Vector2.zero;
            if (interactUI != null && interactUI.activeSelf)
            {
                interactUI.SetActive(false);
            }
        }
    }

    private void LateUpdate()
    {
        Vector3 currentPos = transform.position;
        Vector3 delta = currentPos - lastPosition;

        bool isMoving;
        float dirX;

        if (movement.sqrMagnitude > 0.01f)
        {
            isMoving = true;
            dirX = movement.x;
        }
        else
        {
            isMoving = delta.sqrMagnitude > 0.000001f;
            dirX = delta.x;
        }

        if (isMoving)
        {
            if (Mathf.Abs(dirX) > 0.001f)
            {
                lastFacingX = dirX;
            }
            if (spriteRenderer != null)
            {
                spriteRenderer.flipX = lastFacingX < 0f;
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

    public void SetMovementState(bool state)
    {
        canMove = state;
        if (!canMove)
        {
            movement = Vector2.zero;
            if (animator != null) animator.SetBool("IsRunning", false);
        }
        else
        {
            interactCooldown = 0.5f;
        }
    }
}