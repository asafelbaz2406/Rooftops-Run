using UnityEngine;

public class Score : MonoBehaviour
{
    [SerializeField] private int baseScore = 0; // Score from time
    [SerializeField] private int additionalScore  = 0; // Score from events
    [SerializeField] private int score = 0;
    [SerializeField] private float pointsPerSecond = 10f; 
    private float elapsedTime = 0f;
    private int coinReward = 50;

    #region Subscribing and Unsubscribing
    private void OnEnable()
    {
        Coin.OnCoinCollected += Coin_OnCoinCollected;
    }

    private void OnDisable() 
    {
        Coin.OnCoinCollected -= Coin_OnCoinCollected;
    }
    #endregion

    void Update()
    {
        if(GameManager.Instance.isGameOver) return;

        if (Time.timeScale > 0)
        {
            // Increment elapsed time
            elapsedTime += Time.deltaTime;

            // Calculate the score based on elapsed time
            baseScore = Mathf.FloorToInt(elapsedTime * pointsPerSecond);

            score = baseScore + additionalScore;
        }
    }

    private void Coin_OnCoinCollected() => IncreateScore();
    private void IncreateScore() => additionalScore += coinReward;
    public int GetScore() => score;

}
