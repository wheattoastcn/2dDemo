using UnityEngine;

public class AnimationComponent : MonoBehaviour
{
   private MovementComponent movement;

    private Animator animator;

    private void Awake()
    {
        movement = GetComponent<MovementComponent>();
        animator = transform.Find("Visual").GetComponent<Animator>();
    }

    private void Update()
    {
        animator.SetFloat("Speed", Mathf.Abs(movement.CurrentSpeedX));
    }
}
