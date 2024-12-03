using UnityEngine;
using System.Collections.Generic;

public class MeleeDamage : MonoBehaviour
{
    public LayerMask targetLayers; // Layers to check
    public int damage;

    // Keep track of objects that have already been damaged
    private HashSet<GameObject> damagedTargets = new HashSet<GameObject>();

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Get the root object of the collided target
        GameObject rootTarget = collision.transform.root.gameObject;

        // Check if the target is on the correct LayerMask and hasn't been damaged yet
        if (((1 << rootTarget.layer) & targetLayers) != 0 && !damagedTargets.Contains(rootTarget)) {
            // Add to the list of damaged targets
            damagedTargets.Add(rootTarget);

            // Apply damage based on the target type
            if (rootTarget.layer == LayerMask.NameToLayer("Barracks")) {
                Debug.Log("Hitting Barracks.");
                rootTarget.GetComponent<BarracksController>()?.TakeDamage(damage);
            } else {
                rootTarget.GetComponent<CharacterController>()?.TakeDamage(damage);
            }

            // Debug log (optional)
#if UNITY_EDITOR
            Debug.Log($"Dealt {damage} damage to {rootTarget.name} on layer {rootTarget.layer}");
#endif
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        // Remove the root object from the damaged targets list when it exits the trigger
        GameObject rootTarget = collision.transform.root.gameObject;
        if (damagedTargets.Contains(rootTarget)) {
            damagedTargets.Remove(rootTarget);
        }
    }
}
