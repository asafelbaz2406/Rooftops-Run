using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using Sirenix.OdinInspector;

public class GameManager : MonoBehaviour
{
    private static GameManager instance;
    public static GameManager Instance => instance;
    public event Action OnGameOver;
    public bool isGameOver { get; private set; } = false;
    [ReadOnly] public bool gameover = false;
    
    private void Awake() 
    {
        if(instance != null)
        {
            Debug.LogError("There's more than one GameManager in the scene. Something went wrong.");
            Destroy(gameObject);
            return;
        }    

        instance = this;
        isGameOver = false;
    }

    private void Update() 
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }

        gameover = isGameOver;
    }

    public void GameOver()
    {
        isGameOver = true;
        OnGameOver?.Invoke();
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}