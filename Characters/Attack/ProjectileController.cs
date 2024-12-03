using UnityEngine;

public class ProjectileController : MonoBehaviour
{
    public float speed; // Speed of the projectile
    public bool canMove = false;
    private Vector2 targetPosition;
    private int damage;
    private bool flipX;
    private LayerMask layerMask;

    public void Initialize(Vector2 targetPosition, int damage, bool flip, LayerMask layer)
    {
        this.targetPosition = targetPosition;
        this.damage = damage;
        this.flipX = flip;
        this.layerMask = layer;
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

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Check if the collided object is on the target LayerMask
        if (((1 << collision.gameObject.layer) & layerMask) != 0) {
            collision.GetComponent<CharacterController>().TakeDamage(damage);
            Debug.Log($"{gameObject.name} dealt {damage} damage to {collision.gameObject.name} on layer {layerMask.value}");
        }
    }
}
