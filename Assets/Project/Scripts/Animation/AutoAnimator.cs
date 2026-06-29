using UnityEngine;

public class AutoAnimator : MonoBehaviour
{
    public Animator animator;
    public SpriteRenderer spriteRenderer;

    private Vector3 lastPosition;

    private void Start()
    {
        lastPosition = transform.position;
    }

    private void Update()
    {
        Vector3 currentPos = transform.position;
        Vector3 delta = currentPos - lastPosition;
        
        bool isMoving = delta.magnitude > 0.001f;

        if (animator != null)
        {
            animator.SetBool("IsRunning", isMoving);
        }

        if (isMoving && spriteRenderer != null)
        {
            if (delta.x > 0.01f) spriteRenderer.flipX = false;
            else if (delta.x < -0.01f) spriteRenderer.flipX = true;
        }

        lastPosition = currentPos;
    }
}