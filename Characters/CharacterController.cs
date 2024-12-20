using System;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using static CharacterData;

public class CharacterController : MonoBehaviour
{
    [Header("Character Stats")]
    [SerializeField] public CharacterData characterData; // Assigned at runtime
    [SerializeField] private float moveSpeedModifier;
    [SerializeField] private int currentHealth;
    [SerializeField] private bool isAlive = true; // Indicates if the character is alive

    [Header("Attack Settings")]
    [SerializeField] private MeleeDamage meleeDamage;
    [SerializeField] private RangedAttackBehavior rangedAttack;
    [SerializeField] private Vector3 attackSpawnPoint; // Point where projectiles spawn (if ranged)
    [SerializeField] private LayerMask enemyLayerMask;
    [SerializeField] private float attackCooldownTimer = 0;
    [SerializeField] private GameObject targetGameObject;

    [Header("Detection Settings")]
    [SerializeField] private float characterDetectDistance;
    [SerializeField] private LayerMask barracksLayerMask;

    [Header("Component References")]
    [SerializeField] private Animator animator;
    [SerializeField] private BoxCollider2D boxCollider;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Rigidbody2D rb;

    [Header("Movement Settings")]
    [SerializeField] private bool shouldMove = true; // Controls whether the character is moving

    [Header("Events")]
    [SerializeField] private string addingThisForHeaderToWork;
    [SerializeField] private delegate void DeathEventHandler();
    [SerializeField] private event DeathEventHandler onDeath;

    private void Awake()
    {
        onDeath += FindAnyObjectByType<PartyGameplayController>().OnCharacterDeath;

        StartCoroutine(SpriteRendererDelay());
    }

    void Start()
    {
        isAlive = true;

        spriteRenderer = GetComponent<SpriteRenderer>();
        boxCollider = GetComponent<BoxCollider2D>();
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        rangedAttack = GetComponentInChildren<RangedAttackBehavior>();

        if (characterData.isPlayerCharacter) {
            gameObject.layer = LayerMask.NameToLayer("Player");
            ToggleLayerMask("Player");
            transform.localScale = new Vector3(-1, 1, 1);
        } else {
            gameObject.layer = LayerMask.NameToLayer("Enemy");
            ToggleLayerMask("Enemy");
        }

        if (characterData != null) {
            InitializeCharacter();
        }

        // Initialize attack behavior
        meleeDamage = GetComponentInChildren<MeleeDamage>();
        if (characterData.attackType == AttackType.Melee && meleeDamage != null) {
            meleeDamage.targetLayers = enemyLayerMask | barracksLayerMask; // Include barracks
            meleeDamage.damage = characterData.damage;
        } else if (characterData.attackType == AttackType.Ranged) {
            rangedAttack.RangedAttackInfo(
                characterData.characterName,
                animator,
                characterData.projectilePrefab,
                transform,
                characterData.projectileSpawnOffset,
                characterData.damage,
                this.gameObject,
                enemyLayerMask | barracksLayerMask // Include barracks
            );
        }

        AdjustColliderToSprite();
    }

    private void Update()
    {
        attackCooldownTimer -= Time.deltaTime;

        if (attackCooldownTimer <= 0 && IsEnemyInRange() && isAlive) {
            //attackBehavior.Attack(attackSpawnPoint, characterData.damage);
            if (characterData.attackType == AttackType.Ranged && isAlive) {
                rangedAttack.Attack(attackSpawnPoint, characterData.damage);
            } else {
                animator.SetTrigger("Attack");
                GetComponentInChildren<MeleeDamage>().ApplyDamage();
            }
            attackCooldownTimer = characterData.attackCooldown; // Reset cooldown
        }

        if (characterData.isPlayerCharacter && isAlive) {
            shouldMove = !DetectEnemyCharacter() && !DetectBarracks();
        } else {
            shouldMove = !DetectPlayerCharacter() && !DetectBarracks();
        }
    }

    private IEnumerator SpriteRendererDelay()
    {
        yield return new WaitForSeconds(0.125f);

        this.spriteRenderer.enabled = true;
    }

    private bool IsEnemyInRange()
    {
        if (characterData.isPlayerCharacter) {
            return DetectEnemyCharacter() || DetectBarracks(); // Check for barracks as well
        } else {
            return DetectPlayerCharacter() || DetectBarracks(); // Check for barracks as well
        }
    }

    private void FixedUpdate()
    {
        if (shouldMove && isAlive) {
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
            targetGameObject = hit.collider.gameObject;
            attackSpawnPoint = hit.collider.GetComponent<Transform>().position;
            Debug.DrawRay(transform.position, Vector2.right * characterData.attackRange, Color.green);
            return true;
        } else {
            attackSpawnPoint = Vector3.zero;
            Debug.DrawRay(transform.position, Vector2.right * characterData.attackRange, Color.red);
            return false;
        }
    }

    public GameObject GetTargetGameObject()
    {
        if (targetGameObject == null) { Debug.LogWarning("No target game object assigned."); return null; }
        return targetGameObject;
    }

    private bool DetectPlayerCharacter()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.left, characterData.attackRange, enemyLayerMask);
        if (hit) {
            targetGameObject = hit.collider.gameObject;
            attackSpawnPoint = hit.collider.GetComponent<Transform>().position;
            Debug.DrawRay(transform.position, Vector2.left * characterData.attackRange, Color.green);
            return true;
        } else {
            attackSpawnPoint = Vector3.zero;
            Debug.DrawRay(transform.position, Vector2.left * characterData.attackRange, Color.red);
            return false;
        }
    }

    private bool DetectBarracks()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, characterData.isPlayerCharacter ? Vector2.right : Vector2.left, characterData.attackRange, barracksLayerMask);
        if (hit) {
            targetGameObject = hit.collider.gameObject;
            attackSpawnPoint = hit.collider.GetComponent<Transform>().position;
            Debug.DrawRay(transform.position, (characterData.isPlayerCharacter ? Vector2.right : Vector2.left) * characterData.attackRange, Color.blue);
            return true;
        }
        return false;
    }

    private void ToggleLayerMask(string layerType)
    {
        if (layerType == "Player") {
            enemyLayerMask = LayerMask.GetMask("Enemy");
            barracksLayerMask = LayerMask.GetMask("Barracks");
        } else if (layerType == "Enemy") {
            enemyLayerMask = LayerMask.GetMask("Player");
            barracksLayerMask = LayerMask.GetMask("Barracks");
        } else {
            Debug.LogError("Invalid layer type provided to ToggleLayerMask");
        }
    }

    public void TakeDamage(int damage)
    {
        Hurt();

        if (gameObject.layer == LayerMask.NameToLayer("Enemy")) {
            SaveManager.Instance.gameData.coinTotal += 1;
            FindAnyObjectByType<LevelUI>().SpawnCoinPrefabUI(1);
            //Debug.Log($"Added one coin for attacking {gameObject.layer}.");
        }

        currentHealth -= damage;
        animator.SetTrigger("Hurt");
        if (currentHealth <= 0) {
            Die();
        }
    }

    private void Die()
    {
        isAlive = false;

        if (gameObject.layer == LayerMask.NameToLayer("Player")) {
            onDeath?.Invoke();
        }

        FindAnyObjectByType<PartyGameplayController>().UpdatePartySizeText();

        foreach(Transform child in transform) {
            child.gameObject.SetActive(false);
        }

        boxCollider.enabled = false;

        animator.SetTrigger("Die");
        Destroy(gameObject, 2f); // Delay destruction to allow death animation to play
    }

    public void SwordSwingNoise()
    {
        AudioManager.Instance.PlaySFX("Sword Swing");
    }

    public void LoudGroundSmashNoise()
    {
        AudioManager.Instance.PlaySFX("Ground Smash");
    }

    public void ShootArrowNoise()
    {
        AudioManager.Instance.PlaySFX("Arrow Release");
    }

    public void BoltThrowNoise()
    {
        AudioManager.Instance.PlaySFX("Bolt Throw");
    }

    public void PoisonAttackNoise()
    {
        AudioManager.Instance.PlaySFX("Poison Attack");
    }

    public void ShamanNoise()
    {
        AudioManager.Instance.PlaySFX("Shaman");
    }

    public void BomberNoise()
    {
        AudioManager.Instance.PlaySFX("Bomber");
    }

    public void CannonballNoise()
    {
        AudioManager.Instance.PlaySFX("Cannonball");
    }

    public void Hurt()
    {
        AudioManager.Instance.PlaySFX("Hurt");
    }
}
