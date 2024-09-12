using System;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private float targetSpeed;
    [SerializeField] private float LerpSpeedT; // keep the player lerp speed and the camera lerp speed in sync
    private Vector3 moveVector;
    [SerializeField] private RotateCamera rotateCamera;
    private TurnController turnController;
    private FaceDirection faceDirection;

    private void Awake() 
    {  
        if(!TryGetComponent<TurnController>(out turnController))    
        {
            Debug.LogError("CameraRaycastCheck is NULL");
        }  
    }

    private void Start()
    {
        // speed is set to 0 at start and then in update it lerps to the target speed
        speed = 0f;
        faceDirection = rotateCamera.GetFaceDirection();
    }

    #region Subscribing and Unsubscribing
    private void OnEnable() 
    {
        turnController.OnCameraTurned += TurnController_OnCameraTurned;
    }

    private void OnDisable()
    {
        turnController.OnCameraTurned -= TurnController_OnCameraTurned;
    }
    #endregion
    void Update()
    {
        if(GameManager.Instance.isGameOver) return;

        LerpSpeed(targetSpeed);
        Move();
        ChangeRotation();
    }

    private void Move()
    {
        float rotationAngle = transform.rotation.eulerAngles.y;
        int roundedAngle = Mathf.RoundToInt(rotationAngle / 90f) * 90; // round to the nearest multiple of 90

        moveVector = new Vector3(0, moveVector.y, 0);

        if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A))
        {
            moveVector = CalculateMovementRelatedToRotation(roundedAngle, isRight: false) * speed * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D))
        {
            moveVector = CalculateMovementRelatedToRotation(roundedAngle, isRight: true) * speed * Time.deltaTime;
        }

        transform.position += moveVector;
    }

    private Vector3 CalculateMovementRelatedToRotation(int rotationAngle, bool isRight)
    {
        // Normalize the rotation angle to be within 0 to 360 degrees
        int normalizedAngle = rotationAngle % 360;

        switch (normalizedAngle)
        {
            case 0:
            case 360:
                return isRight ? new Vector3(1, 0, 0) : new Vector3(-1, 0, 0); // x-axis

            case 90:
                return isRight ? new Vector3(0, 0, -1) : new Vector3(0, 0, 1); // z-axis

            case 180:
                return isRight ? new Vector3(-1, 0, 0) : new Vector3(1, 0, 0); // x-axis

            case 270:
                return isRight ? new Vector3(0, 0, 1) : new Vector3(0, 0, -1); // z-axis

            default:
                throw new ArgumentException("Rotation angle is not valid.");
        }
    }

    // lerp the speed from 0 to targetSpeed at the start of the game so the character doens't move instantly
    public void LerpSpeed(float targetSpeed)
    {
        speed = Mathf.Lerp(speed, targetSpeed, Time.deltaTime * LerpSpeedT);
    }

    public float GetSpeed() => speed;

    public void ChangeRotation()
    {
        // move function should adapt the movement based on the camera rotation
        
        switch (faceDirection)
        {
            case FaceDirection.faceRight:
                transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0, 90, 0), Time.deltaTime * LerpSpeedT);
                break;
            case FaceDirection.faceLeft:
                transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0, -90, 0), Time.deltaTime * LerpSpeedT);
                break;
            case FaceDirection.faceForward:
                transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0, 0, 0), Time.deltaTime * LerpSpeedT);
                break;
            case FaceDirection.faceBackward:
                transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0, 180, 0), Time.deltaTime * LerpSpeedT);
                break;
        }
        
    }

    public FaceDirection GetFaceDirection() => faceDirection;
    
    private void TurnController_OnCameraTurned()
    {
        faceDirection = rotateCamera.GetFaceDirection();
    }
}
