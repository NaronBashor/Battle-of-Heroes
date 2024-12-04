using UnityEngine;

public class CoinManager : MonoBehaviour
{
    [Header("Income Settings")]
    [SerializeField] private int coinsPerInterval;
    [SerializeField] private float incomeInterval;

    [Header("Timer")]
    [SerializeField] private float timer;

    private void Start()
    {
        LevelDifficulty levelDifficulty = GameManager.Instance.GetCurrentLevelDifficulty();
        coinsPerInterval = levelDifficulty.passiveCoinAmount;
        incomeInterval = 10f;
    }

    private void Update()
    {
        timer += Time.deltaTime;

        if (timer >= incomeInterval) {
            timer = 0f;
            GeneratePassiveCoins();
        }
    }

    private void GeneratePassiveCoins()
    {
        SaveManager.Instance.gameData.coinTotal += coinsPerInterval;
        //Debug.Log($"Passive coins earned: {coinsPerInterval}");
    }

    public void SetPassiveIncome(int coinsPerInterval, float incomeInterval)
    {
        this.coinsPerInterval = coinsPerInterval;
        this.incomeInterval = incomeInterval;
    }
}
