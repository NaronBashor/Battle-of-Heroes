using UnityEngine;

public class ProjectileController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float speed; // Speed of the projectile
    [SerializeField] private bool canMove = false;
    [SerializeField] private Vector2 targetPosition;

    [Header("Damage Settings")]
    [SerializeField] private int damage;

    [Header("Appearance Settings")]
    [SerializeField] private bool flipX;

    [Header("Target Settings")]
    [SerializeField] private LayerMask layerMask;

    [Header("Ownership Settings")]
    [SerializeField] private GameObject projectileOwner;

    public void Initialize(Vector2 targetPosition, int damage, bool flip, LayerMask layer, GameObject projectileOwner)
    {
        this.targetPosition = targetPosition;
        this.damage = damage;
        this.flipX = flip;
        this.layerMask = layer;
        this.projectileOwner = projectileOwner;
        if (!canMove) { return; }
        RotateTowardsTarget();
    }

    private void Update()
    {
        if (!canMove) { return; }
        // Move the projectile toward the target position
        Vector2 position = Vector2.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);
        transform.position = position;

        // Destroy if it reaches the target
        if (Vector2.Distance(transform.position, targetPosition) < 0.01f) {
            ApplyDamage();
            Destroy(gameObject);
        }
    }

    private void RotateTowardsTarget()
    {
        if (flipX) {
            transform.localScale = new Vector3(-.75f, .75f);
        }

        // Rotate the projectile to face the target (optional)
        Vector2 direction = (targetPosition - (Vector2)transform.position).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    private void ApplyDamage()
    {
        GameObject targetObject = projectileOwner.GetComponent<CharacterController>().GetTargetGameObject();
        // Check if the target is on the correct LayerMask and hasn't been damaged yet
        // Apply damage based on the target type
        if (targetObject.layer == LayerMask.NameToLayer("Barracks")) {
            targetObject.GetComponent<BarracksController>()?.TakeDamage(damage);
        } else {
            targetObject.GetComponent<CharacterController>()?.TakeDamage(damage);
        }

        // Debug log (optional)
#if UNITY_EDITOR
        //Debug.Log($"Dealt {damage} damage to {targetObject.name} on layer {targetObject.layer}");
#endif
    }
}
