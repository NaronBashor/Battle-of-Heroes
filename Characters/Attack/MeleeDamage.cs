using UnityEngine;
using System.Collections.Generic;

public class MeleeDamage : MonoBehaviour
{
    [Header("Targeting Settings")]
    [SerializeField] public LayerMask targetLayers; // Layers to check

    [Header("Damage Settings")]
    [SerializeField] public int damage;

    public void ApplyDamage()
    {
        GameObject target = GetComponentInParent<CharacterController>().GetTargetGameObject();
        if (target.layer == LayerMask.NameToLayer("Barracks")) {
            target.GetComponent<BarracksController>()?.TakeDamage(damage);
        } else {
            target.GetComponent<CharacterController>()?.TakeDamage(damage);
        }
    }
}
