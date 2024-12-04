using UnityEngine;

[CreateAssetMenu(fileName = "NewCharacter", menuName = "Game/Character")]
public class CharacterData : ScriptableObject
{
    public enum AttackType { Melee, Ranged }

    public string characterName;
    [TextArea(3, 5)] public string characterDescription;
    [TextArea(3, 5)] public string characterLore;

    public Sprite characterButtonSprite;

    public bool isPlayerCharacter;

    public Sprite characterSprite;
    public AnimatorOverrideController animatorOverrideController;

    public int level = 1;
    public int health;
    public int damage;
    public float speed;
    public int armor;
    public int cost;
    public float yOffset;

    [Header("Attack Stats")]
    public float attackRange;
    public float attackCooldown; // Time in seconds between attacks
    public float attackSpeed; // New stat: Number of attacks per second

    // Attack behavior
    public AttackType attackType; // Melee or Ranged
    public GameObject projectilePrefab; // For ranged attacks
    public Vector2 projectileSpawnOffset; // Offset for projectile spawn location

    [Header("Level-Up Stats")]
    public int healthIncreasePerLevel;
    public int damageIncreasePerLevel;
    public int attackSpeedIncreasePerLevel;
}
