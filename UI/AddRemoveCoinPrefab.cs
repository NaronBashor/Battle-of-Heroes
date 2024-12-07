using TMPro;
using UnityEngine;

public class AddRemoveCoinPrefab : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI coinAmountText;

    [SerializeField] public int coinAmount;
    [SerializeField] public float moveSpeed;

    private void Start()
    {
        coinAmountText.text = "+" + coinAmount.ToString();
    }

    private void Update()
    {
        transform.position += Vector3.up * Time.deltaTime * moveSpeed;
        Destroy(gameObject, 1f);
    }
}
