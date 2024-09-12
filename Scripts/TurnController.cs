using System;
using UnityEngine;
using System.Collections.Generic;

public class TurnController : MonoBehaviour
{
    public event Action OnCameraTurned;
    private RotateCamera rotateCamera;
    [SerializeField] private float rayDistance = 50f;
    [SerializeField] private LayerMask floorLayer;
    private const float distanceFromPlayer = 5.5f;
    private Camera mainCamera;
    private readonly Dictionary<int, Vector3> DistanceToPlayerOffsetDictionary = new Dictionary<int, Vector3>()
    {
        { 0, new Vector3(0, 0, -distanceFromPlayer) },
        { 90, new Vector3(-distanceFromPlayer, 0, 0) },
        { 180, new Vector3(0, 0, distanceFromPlayer) },
        { 270, new Vector3(distanceFromPlayer, 0, 0) }
    };

    // at the start of the game this is the right distance

    private void Awake()
    {
        mainCamera = Camera.main;
       
        rotateCamera = mainCamera.GetComponent<RotateCamera>(); 
    }

    private void Update() 
    {
        if(GameManager.Instance.gameover) return;

        if(Input.GetKeyDown(KeyCode.UpArrow) && CanTurn())  
        {
            RayCastFromCamera();
        }  
    }

    private bool CanTurn()
    {
        bool result = false;
        Vector3 directionToPlayer = Vector3.down.normalized;
        Vector3 rayPos = transform.position; 

        float floorYRotation = 0;
        float thisRotattion = 0;

        if (Physics.Raycast(rayPos, directionToPlayer ,out RaycastHit hit, rayDistance, floorLayer))
        {
            floorYRotation = hit.collider.transform.rotation.eulerAngles.y;
            int roundedFloorYRotation = (int)Mathf.Round(floorYRotation / 90) * 90;
            thisRotattion = (int)transform.rotation.eulerAngles.y;
            int roundedThisRotation = (int)Mathf.Round(thisRotattion / 90) * 90;

            result = roundedFloorYRotation != roundedThisRotation;
        }

        if(!result)
            print("CANT TURN");
        return result;
    }

    //[Button("RayCastFromCamera")]
    private void RayCastFromCamera()
    {
        Vector3 directionToPlayer = Vector3.down.normalized;
        Vector3 upwardOffset = new Vector3(0, 1f, 0);
        Vector3 rayPos = transform.position + upwardOffset; 

        if (Physics.Raycast(rayPos, directionToPlayer ,out RaycastHit hit, rayDistance, floorLayer))
        {
            int floorYRotation = (int)hit.collider.transform.rotation.eulerAngles.y;
            Vector3 floorPosition = hit.transform.position;

            rotateCamera.CalculateFaceDirection(floorYRotation);
            Vector3 offset = DistanceToPlayerOffsetDictionary[floorYRotation];
            Vector3 cameraPosition = GetOffsetRelatedToFaceDirection(offset, floorPosition, transform.position);
            
            floorYRotation = (floorYRotation + 360) % 360;
            Quaternion newRotation = Quaternion.Euler(mainCamera.transform.rotation.eulerAngles.x, floorYRotation, mainCamera.transform.rotation.eulerAngles.z);

            // lerp the current camera position to the cameraPosition
            rotateCamera.SetCameraPosition(cameraPosition, newRotation);
            OnCameraTurned?.Invoke();
        }

        Debug.DrawRay(rayPos, directionToPlayer * rayDistance, Color.red);
    }
 
    private Vector3 GetOffsetRelatedToFaceDirection(Vector3 offset, Vector3 floorPosition, Vector3 playerPosition)
    {
        if(offset.x != 0)
        {
            return new Vector3(offset.x + playerPosition.x, mainCamera.transform.position.y, floorPosition.z);
        }
        else if(offset.z != 0)
        {
            return new Vector3(floorPosition.x, mainCamera.transform.position.y, offset.z + playerPosition.z);
        }

        return Vector3.zero;
    }
}
