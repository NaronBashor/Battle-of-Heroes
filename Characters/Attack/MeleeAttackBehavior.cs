using UnityEngine;

public class MeleeAttackBehavior : IAttackBehavior
{
    private Animator animator;
    private int damage;

    public MeleeAttackBehavior(Animator animator, int damage)
    {
        this.animator = animator;
        this.damage = damage;
    }

    public void Attack(Vector2 targetPosition)
    {
        // Trigger attack animation
        animator.SetTrigger("Attack");
    }
}
