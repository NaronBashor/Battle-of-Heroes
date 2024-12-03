using UnityEngine;

public class MeleeDamage : MonoBehaviour
{
    // Define a LayerMask for the layers you want to check
    public LayerMask targetLayers;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Check if the collided object is on the target LayerMask
        if (((1 << collision.gameObject.layer) & targetLayers) != 0) {
            collision.GetComponent<CharacterController>().TakeDamage(10);
            Debug.Log($"{gameObject.name} dealt {10} damage to {collision.gameObject.name} on layer {targetLayers.value}");
        }
    }
}
