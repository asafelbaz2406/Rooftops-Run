using System.Collections.Generic;
using UnityEngine;

public class MovingObjects : MonoBehaviour, IPooledObject
{
    [SerializeField] private float speed = 10f;
    private float originalSpeed;
    private float maxSpeed = 10f;
    [SerializeField] private Vector3 direction = Vector3.zero;
    [SerializeField] private RotateCamera rotateCamera;
    public bool shouldMove;
    [SerializeField] private List<Coin> coins;
    
    private void Awake() 
    {
        rotateCamera = GameObject.FindWithTag("MainCamera").GetComponent<RotateCamera>();
        if(!rotateCamera)
        {
            Debug.LogError("Main Camera not found", transform);
        }

        originalSpeed = speed;
    }

    #region Subscribing and Unsubscribing
    private void OnEnable() 
    {
        rotateCamera.OnCameraRotation += CalculateDirectionBasedOnCameraDirection;
        
        ShuffleCoins();
    }

    private void OnDisable() 
    {
        rotateCamera.OnCameraRotation -= CalculateDirectionBasedOnCameraDirection;
    }
    #endregion
    
    private void Update()
    {   
        if(GameManager.Instance.isGameOver) return;
        if (!shouldMove) return;
     
        transform.position += speed * Time.deltaTime * direction;
    }
    

    public void OnObjectSpawn()
    {
        CalculateDirectionBasedOnCameraDirection();
        shouldMove = true; 
    }

    // this is getting called onEnable
    public void ShuffleCoins()
    {
        foreach(Coin coin in coins)
        {
            coin.ChangeCoinPosition();
        }
    }

    public void SetSpeed(float newSpeed) => speed = newSpeed;

    public void SetAccumulatedSpeed(float newSpeed = 0f) 
    {
        // cant go above maxSpeed = 10f
        originalSpeed = Mathf.Min(maxSpeed, originalSpeed + newSpeed);
        speed = originalSpeed;
        //Debug.Log(originalSpeed);
    }

    public void CalculateDirectionBasedOnCameraDirection()
    {
        switch(rotateCamera.GetFaceDirection())
        {
            case FaceDirection.faceForward:
                direction = -Vector3.forward;
                break;
            case FaceDirection.faceRight:
                direction = -Vector3.right;
                break;
            case FaceDirection.faceBackward:
                direction = -Vector3.back;
                break;
            case FaceDirection.faceLeft:
                direction = -Vector3.left;
                break;
        }
    }
}
    


