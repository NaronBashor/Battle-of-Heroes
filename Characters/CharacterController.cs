using UnityEngine;
using UnityEngine.UI;
using static CharacterData;

public class CharacterController : MonoBehaviour
{
    public CharacterData characterData; // Assigned at runtime
    public Vector3 attackSpawnPoint; // Point where projectiles spawn (if ranged)
    private IAttackBehavior attackBehavior;
    public float moveSpeedModifier;
    public float characterDetectDistance;
    public LayerMask enemyLayerMask;
    private Animator animator;
    private int currentHealth;
    private BoxCollider2D boxCollider;
    private SpriteRenderer spriteRenderer;
    private Rigidbody2D rb;
    private float attackCooldownTimer = 0;

    private bool shouldMove = true; // Controls whether the character is moving

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        boxCollider = GetComponent<BoxCollider2D>();
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();

        if (characterData.isPlayerCharacter) {
            gameObject.layer = LayerMask.NameToLayer("Player"); // Change layer to "Player"
            ToggleLayerMask("Player");
            //Debug.Log("Layer is now: " + gameObject.layer); // Should log 6 for "Player"
            transform.localScale = new Vector3(-1, 1, 1);
        } else {
            gameObject.layer = LayerMask.NameToLayer("Enemy"); // Change layer to "Enemy"
            ToggleLayerMask("Enemy");
            //Debug.Log("Layer is now: " + gameObject.layer); // Should log 7 for "Enemy"
        }

        if (characterData != null) {
            InitializeCharacter();
        }

        // Initialize attack behavior based on CharacterData
        if (characterData.attackType == AttackType.Melee) {
            GetComponentInChildren<MeleeDamage>().targetLayers = enemyLayerMask;
            attackBehavior = new MeleeAttackBehavior(animator, characterData.damage);
        } else if (characterData.attackType == AttackType.Ranged) {
            attackBehavior = new RangedAttackBehavior(
                characterData.characterName,
                animator,
                characterData.projectilePrefab,
                transform,
                characterData.projectileSpawnOffset,
                characterData.damage,
                this.gameObject,
                enemyLayerMask
            );
        }

        AdjustColliderToSprite();
    }

    private void Update()
    {
        attackCooldownTimer -= Time.deltaTime;

        // Example: Detect enemy and attack
        if (IsEnemyInRange() && attackCooldownTimer <= 0) {
            //Vector2 targetPosition = GetEnemyPosition(); // Replace with actual target detection
            attackBehavior.Attack(attackSpawnPoint);
            attackCooldownTimer = characterData.attackCooldown; // Reset cooldown
        }

        if (characterData.isPlayerCharacter) {
            shouldMove = !DetectEnemyCharacter(); // Stop moving if an enemy is detected
        } else {
            shouldMove = !DetectPlayerCharacter(); // Stop moving if a player is detected
        }
    }

    private bool IsEnemyInRange()
    {
        if (characterData.isPlayerCharacter) {
            return DetectEnemyCharacter();
        } else {
            return DetectPlayerCharacter();
        }
    }

    private void FixedUpdate()
    {
        if (shouldMove) {
            // Move character based on its type
            if (characterData.isPlayerCharacter) {
                rb.linearVelocity = Vector2.right * characterData.speed * moveSpeedModifier;
            } else {
                rb.linearVelocity = Vector2.left * characterData.speed * moveSpeedModifier;
            }

            // Update walking animation
            animator.SetBool("isWalking", rb.linearVelocity.magnitude > 0.1f);
        } else {
            // Stop the character when shouldMove is false
            rb.linearVelocity = Vector2.zero;
            animator.SetBool("isWalking", false);
        }
    }

    private Rect GetVisiblePixelBounds(Sprite sprite)
    {
        Texture2D texture = sprite.texture;

        // Get the full texture data
        Color[] pixels = texture.GetPixels(
            (int)sprite.textureRect.x,
            (int)sprite.textureRect.y,
            (int)sprite.textureRect.width,
            (int)sprite.textureRect.height
        );

        int width = (int)sprite.textureRect.width;
        int height = (int)sprite.textureRect.height;

        int minX = width, minY = height, maxX = 0, maxY = 0;
        bool foundVisiblePixel = false;

        // Loop through the pixels to find non-transparent ones
        for (int y = 0; y < height; y++) {
            for (int x = 0; x < width; x++) {
                int index = y * width + x;

                // Check if the pixel is non-transparent
                if (pixels[index].a > 0.01f) // Alpha threshold to detect visibility
                {
                    foundVisiblePixel = true;
                    if (x < minX) minX = x;
                    if (x > maxX) maxX = x;
                    if (y < minY) minY = y;
                    if (y > maxY) maxY = y;
                }
            }
        }

        if (!foundVisiblePixel) {
            // If no visible pixels are found, return an empty rect
            return Rect.zero;
        }

        // Create a rect representing the bounds of the visible area
        return new Rect(minX, minY, maxX - minX + 1, maxY - minY + 1);
    }

    private void InitializeCharacter()
    {
        currentHealth = characterData.health;

        // Calculate attack cooldown based on attack speed
        characterData.attackCooldown = 1f / characterData.attackSpeed;

        attackCooldownTimer = characterData.attackCooldown;

        // Assign the AnimatorOverrideController
        if (characterData.animatorOverrideController != null) {
            animator.runtimeAnimatorController = characterData.animatorOverrideController;
        }
    }

    private void AdjustColliderToSprite()
    {
        if (spriteRenderer != null && boxCollider != null) {
            Sprite sprite = spriteRenderer.sprite;

            // Get the bounds of the visible pixels
            Rect visibleBounds = GetVisiblePixelBounds(sprite);

            if (visibleBounds == Rect.zero) {
                Debug.LogWarning("No visible pixels found in sprite!");
                return;
            }

            // Convert pixel dimensions to world space
            float width = visibleBounds.width / sprite.pixelsPerUnit;
            float height = (visibleBounds.height * 2f) / sprite.pixelsPerUnit;

            // Calculate offset from the pivot
            float offsetX = (visibleBounds.x + visibleBounds.width / 2f) / sprite.pixelsPerUnit - (sprite.pivot.x / sprite.pixelsPerUnit);
            float offsetY = (visibleBounds.y + visibleBounds.height / 2f) / sprite.pixelsPerUnit - (sprite.pivot.y / sprite.pixelsPerUnit);

            // Update the BoxCollider2D
            boxCollider.size = new Vector2(width, height);
            boxCollider.offset = new Vector2(offsetX, offsetY);

            //Debug.Log($"Collider adjusted: Size = {boxCollider.size}, Offset = {boxCollider.offset}");
        }
    }

    private bool DetectEnemyCharacter()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.right, characterData.attackRange, enemyLayerMask);
        if (hit) {
            attackSpawnPoint = hit.collider.GetComponent<Transform>().position;
            Debug.DrawRay(transform.position, Vector2.right * characterData.attackRange, Color.green);
            return true;
        } else {
            attackSpawnPoint = Vector3.zero;
            Debug.DrawRay(transform.position, Vector2.right * characterData.attackRange, Color.red);
            return false;
        }
    }

    private bool DetectPlayerCharacter()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.left, characterData.attackRange, enemyLayerMask);
        if (hit) {
            attackSpawnPoint = hit.collider.GetComponent<Transform>().position;
            Debug.DrawRay(transform.position, Vector2.left * characterData.attackRange, Color.green);
            return true;
        } else {
            attackSpawnPoint = Vector3.zero;
            Debug.DrawRay(transform.position, Vector2.left * characterData.attackRange, Color.red);
            return false;
        }
    }

    private void ToggleLayerMask(string layerType)
    {
        if (layerType == "Player") {
            // Set the enemyLayerMask to exclude Player-related layers
            enemyLayerMask = LayerMask.GetMask("Enemy");
        } else if (layerType == "Enemy") {
            // Set the enemyLayerMask to exclude Enemy-related layers
            enemyLayerMask = LayerMask.GetMask("Player");
        } else {
            Debug.LogError("Invalid layer type provided to ToggleLayerMask");
        }

        // Debug the new mask
        //Debug.Log("New enemyLayerMask value: " + enemyLayerMask.value);
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        animator.SetTrigger("Hurt");
        if (currentHealth <= 0) {
            Die();
        }
    }

    private void Die()
    {
        animator.SetTrigger("Die");
        Destroy(gameObject, 2f); // Delay destruction to allow death animation to play
    }

    public void Attack()
    {
        animator.SetTrigger("Attack");
    }
}
