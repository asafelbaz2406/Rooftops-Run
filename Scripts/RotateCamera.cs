using System;
using UnityEngine;

public class RotateCamera : MonoBehaviour
{
    [SerializeField] private FaceDirection cameraFaceDirection;
    public event Action OnCameraRotation;

    [Header("Camera Transition Settings")]
    [SerializeField] private float rotateSpeed = 5f;
    private bool isTransitioning = false;
    private Vector3 startPositionBeforeTransition;
    private Quaternion startRotationBeforeTransition;
    private Vector3 targetPositionAfterTransition;
    private Quaternion targetRotationAfterTransition;
    private float transitionProgress = 0f; 
    private TileManager tileManager;
    private bool gameOver = false;

    private void Start()
    {
        cameraFaceDirection = FaceDirection.faceForward;
        tileManager = TileManager.instance;

        targetPositionAfterTransition = transform.position;
    }

    void Update()
    {
        if(gameOver) return;

        if (isTransitioning)
        {
            // Calculate the progress and interpolate position and rotation
            transitionProgress += Time.deltaTime * rotateSpeed;
            
            float t = Mathf.Clamp01(transitionProgress);

            transform.position = Vector3.Lerp(startPositionBeforeTransition, targetPositionAfterTransition, t);
            transform.rotation = Quaternion.Lerp(startRotationBeforeTransition, targetRotationAfterTransition, t);

            tileManager.StopAllMovingObjects();            

            // Check if the transition is complete
            if (t >= 1f)
            {
                isTransitioning = false;
             
                OnCameraRotation?.Invoke();
            }
        }
        
    }

    public void SetCameraPosition(Vector3 cameraPosition, Quaternion cameraRotation)
    {
        if (isTransitioning)
        {
            return; // Skip setting new target if already transitioning
        }

        startPositionBeforeTransition = transform.position;
        startRotationBeforeTransition = transform.rotation;
        targetPositionAfterTransition = cameraPosition;
        targetRotationAfterTransition = cameraRotation;

        transitionProgress = 0f; // Reset transition progress

        isTransitioning = true;
    }

    public FaceDirection GetFaceDirection() => cameraFaceDirection;
    public Vector3 GetTargetPositionAfterTransition() => targetPositionAfterTransition; 

    public void CalculateFaceDirection(int yRotation)
    {
        // Normalize the angle to the range 0-360
        int normalizedRotationY = Mathf.RoundToInt(yRotation) % 360;
        if (normalizedRotationY < 0)
        {
            normalizedRotationY += 360;
        }
        switch(normalizedRotationY)
        {
            case 0:
                cameraFaceDirection = FaceDirection.faceForward;
                break;
            case 90:
                cameraFaceDirection = FaceDirection.faceRight;
                break;
            case 180:
                cameraFaceDirection = FaceDirection.faceBackward;
                break;
            case 270:
                cameraFaceDirection = FaceDirection.faceLeft;
                break;
            default:
                Debug.LogError("Invalid camera rotation: " + normalizedRotationY);
                break;
        }
    } 
}