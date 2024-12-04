using UnityEngine;

public class CoinManager : MonoBehaviour
{
    public int coinsPerInterval = 5;
    public float incomeInterval = 10f;
    private float timer;

    private void Start()
    {
        LevelDifficulty levelDifficulty = GameManager.Instance.GetCurrentLevelDifficulty();
        coinsPerInterval = levelDifficulty.passiveCoinAmount;
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
        Debug.Log($"Passive coins earned: {coinsPerInterval}");
    }

    public void SetPassiveIncome(int coinsPerInterval, float incomeInterval)
    {
        this.coinsPerInterval = coinsPerInterval;
        this.incomeInterval = incomeInterval;
    }
}
