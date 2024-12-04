using System.Collections;
using UnityEngine;

public class RangedAttackBehavior : MonoBehaviour
{
    [Header("Animation Settings")]
    [SerializeField] private Animator animator;

    [Header("Projectile Settings")]
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Vector2 spawnOffset;

    [Header("Character Transform")]
    [SerializeField] private Transform characterTransform;

    [Header("Character Details")]
    [SerializeField] private string characterName;
    [SerializeField] private GameObject characterParent;

    [Header("Combat Settings")]
    [SerializeField] private int damage;
    [SerializeField] private LayerMask targetLayerMask;

    public void RangedAttackInfo(string characterName, Animator animator, GameObject projectilePrefab, Transform characterTransform, Vector2 spawnOffset, int damage, GameObject characterParent,
        LayerMask targetLayerMask)
    {
        this.characterName = characterName;
        this.animator = animator;
        this.projectilePrefab = projectilePrefab;
        this.characterTransform = characterTransform;
        this.spawnOffset = spawnOffset;
        this.damage = damage;
        this.characterParent = characterParent;
        this.targetLayerMask = targetLayerMask;
    }

    public void Attack(Vector2 targetPosition, int damage)
    {
        // Trigger attack animation
        animator.SetTrigger("Attack");

        switch (characterName) {
            case "Zeus the Stormcaller":
                GameObject zeusProjectile = GameObject.Instantiate(projectilePrefab, new Vector3(targetPosition.x, -0.38f), Quaternion.identity);
                zeusProjectile.GetComponent<ProjectileController>().Initialize(targetPosition, 25, false, targetLayerMask, this.transform.root.gameObject);
                break;
            case "Shadow Mage":
                GameObject mageProjectile = GameObject.Instantiate(projectilePrefab, new Vector3(targetPosition.x, -2f), Quaternion.identity);
                mageProjectile.GetComponent<ProjectileController>().Initialize(targetPosition, 25, false, targetLayerMask, this.transform.root.gameObject);
                break;
            case "Spectral Shaman":
                Transform childTransform = characterParent.transform.Find("Spectral Shaman Projectile Pos");
                GameObject shamanProjectile = GameObject.Instantiate(projectilePrefab, childTransform.position, Quaternion.identity);
                shamanProjectile.GetComponent<ProjectileController>().Initialize(targetPosition, 25, false, targetLayerMask, this.transform.root.gameObject);
                break;
            case "Goblin Bomber":
                Transform child = characterParent.transform.Find("Goblin Bomber Projectile Pos");
                GameObject projectile = GameObject.Instantiate(projectilePrefab, child.position, Quaternion.identity);
                projectile.GetComponent<ProjectileController>().Initialize(targetPosition, 25, true, targetLayerMask, this.transform.root.gameObject);
                break;
            case "Goblin Archer":
                Transform archerChild = characterParent.transform.Find("Goblin Archer Projectile Pos");
                GameObject arrowProjectile = GameObject.Instantiate(projectilePrefab, archerChild.position, Quaternion.identity);
                arrowProjectile.GetComponent<ProjectileController>().Initialize(targetPosition, 25, false, targetLayerMask, this.transform.root.gameObject);
                break;
            case "Cannonball Trooper":
                Transform cannonballChild = characterParent.transform.Find("Cannonball Trooper Projectile Pos");
                GameObject cannonballProjectile = GameObject.Instantiate(projectilePrefab, cannonballChild.position, Quaternion.identity);
                cannonballProjectile.GetComponent<ProjectileController>().Initialize(targetPosition, 25, false, targetLayerMask, this.transform.root.gameObject);
                break;
        }
    }
}
