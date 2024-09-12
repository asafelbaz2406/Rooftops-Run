using TMPro;
using UnityEngine;

public class ScoreDisplay : MonoBehaviour
{
    [SerializeField] private Score score; 
    private TextMeshProUGUI scoreText;

    private void Awake()
    {
        score = FindObjectOfType<Score>();
        scoreText = GetComponent<TextMeshProUGUI>();
    }
   
    void Update()
    {
        scoreText.text = score.GetScore().ToString();
    }
}
