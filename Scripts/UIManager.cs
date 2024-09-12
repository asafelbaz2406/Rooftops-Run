using UnityEngine;

public class UIManager : MonoBehaviour
{ 
    [SerializeField] private GameObject startButton;
    [SerializeField] private GameObject stopButton;
    [SerializeField] private GameObject gameOver;
    [SerializeField] private GameObject restart;
    
    #region Subscribing and Unsubscribing
    private void Start() 
    {
        GameManager.Instance.OnGameOver += GameOver;
        //GameManager.Instance.OnGameRestart += RestartGame;
    }

    private void OnDisable() 
    {
        GameManager.Instance.OnGameOver -= GameOver;
        //GameManager.Instance.OnGameRestart -= RestartGame;
    }
    #endregion

    private void GameOver()
    {
        print("OnGameOver");
        gameOver.SetActive(true);
        restart.SetActive(true);
    }

    //[Button("Stop Time")]
    public void StopTimer() 
    {
        Time.timeScale = 0f;
        startButton.SetActive(true);
        stopButton.SetActive(false);
    }

    //[Button("Start Time")]
    public void StartTimer() 
    {
        Time.timeScale = 1f;
        startButton.SetActive(false);
        stopButton.SetActive(true);
    } 

    public void RestartGame()
    {
        //Time.timeScale = 1f;
        //gameOver.SetActive(false);
        //restart.SetActive(false);
        GameManager.Instance.RestartGame();
    }

}
