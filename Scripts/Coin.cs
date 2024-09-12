using System;
using UnityEngine;

public class Coin : MonoBehaviour
{
    [SerializeField] private int[] xPositions = {-3, 0 , 3};
    public static event Action OnCoinCollected;
    
    public void ChangeCoinPosition()
    {
        int xPos =  xPositions[UnityEngine.Random.Range(0, xPositions.Length)];
        transform.localPosition = new Vector3(xPos, transform.localPosition.y, transform.localPosition.z);

        gameObject.SetActive(true); 
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            gameObject.SetActive(false);
            // later add an event to increase score + change ui + sound effect
            OnCoinCollected?.Invoke();
        }    
    }
}
