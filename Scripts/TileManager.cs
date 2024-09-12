using System.Collections.Generic;
using UnityEngine;

public class TileManager : MonoBehaviour
{
    static public TileManager instance;
    public int numberOfTiles = 5;
    private string tileTag = "MapChunk";  // Tag for the tiles in the object pool
    [SerializeField] private List<GameObject> activeTiles;
    private ObjectPooler objectPooler;
    private const float horizontalOffset = 20f;
    private readonly Dictionary<int, Vector3> HorizontalOffsetDictionary = new Dictionary<int, Vector3>
    {
        { 0, new Vector3(0, 0, horizontalOffset) },
        { 90, new Vector3(horizontalOffset, 0, 0) },
        { 180, new Vector3(0, 0, -horizontalOffset) },
        { 270, new Vector3(-horizontalOffset, 0, 0) }
    };

    private const float verticalOffset = 15f;
    private readonly Dictionary<int, Vector3> VerticalOffsetDictionary = new Dictionary<int, Vector3>
    {
        { 0, new Vector3(0, 0, verticalOffset) },
        { 90, new Vector3(verticalOffset, 0, 0) },
        { 180, new Vector3(0, 0, -verticalOffset) },
        { 270, new Vector3(-verticalOffset, 0, 0) }
    };

    [SerializeField] private int forceXTilesToBeStraight = 3;
    [SerializeField] private int currentStraightTiles = 3;
    [SerializeField] private RotateCamera rotateCamera;

    private void Awake() 
    {
        instance = this;
        rotateCamera = GameObject.FindWithTag("MainCamera").GetComponent<RotateCamera>();
    }

    void Start()
    {
        objectPooler = ObjectPooler.Instance;
        activeTiles = new List<GameObject>();
        for(int i = 0; i < numberOfTiles; i++)
        {
            SpawnFirstTiles();
        }
    }
    #region Subscribing and Unsubscribing
    private void OnEnable() 
    {
        rotateCamera.OnCameraRotation += MoveAllMovingObjects;
    }

    private void OnDisable() 
    {
        rotateCamera.OnCameraRotation -= MoveAllMovingObjects;
    }
    #endregion

    private void SpawnFirstTiles()
    {
        if(activeTiles.Count == 0) // force first tile to be straight
        {
            SpawnFirstTileAndAddToList();
            return;
        }

        GameObject lastTile = activeTiles[activeTiles.Count - 1];
        GameObject currentTile = objectPooler.SpawnFromPool(tileTag, Vector3.zero, Quaternion.identity); 
        
        int lastTileYRotation = (int)lastTile.transform.rotation.eulerAngles.y;
        int currentTileYRotation = (int)currentTile.transform.rotation.eulerAngles.y;

        if(currentTileYRotation == lastTileYRotation) 
        {
            GetNewPositionFromHorizontalDic(lastTile, currentTile, lastTileYRotation);
        }

        activeTiles.Add(currentTile);
    }

    private void GetNewPositionFromHorizontalDic(GameObject lastTile, GameObject currentTile, int lastTileYRotation)
    {
        if (!HorizontalOffsetDictionary.ContainsKey(lastTileYRotation))
        {
            Debug.LogError("Horizontal offset not found for rotation: " + lastTileYRotation);
            return;
        }

        currentTile.transform.position = lastTile.transform.position + HorizontalOffsetDictionary[lastTileYRotation];
        lastTileYRotation = (lastTileYRotation + 360) % 360; // keeping the values between 0 and 360, avoiding -

        currentTile.transform.rotation = Quaternion.Euler(0, lastTileYRotation, 0);
    }

    private void GetNewPositionFromVerticalDic(GameObject lastTile, GameObject currentTile, int lastTileYRotation)
    {
        if (!VerticalOffsetDictionary.ContainsKey(lastTileYRotation))
        {
            Debug.LogError("Vertical offset not found for rotation: " + lastTileYRotation);
            return;
        }

        currentTile.transform.position = lastTile.transform.position + VerticalOffsetDictionary[lastTileYRotation];
        int[] randomRotations = { 90, 270}; // turn right(90) or left(270)
        lastTileYRotation = (lastTileYRotation + randomRotations[Random.Range(0, randomRotations.Length)]) % 360; // keeping the values between 0 and 360, avoiding -

        currentTile.transform.rotation = Quaternion.Euler(0, lastTileYRotation, 0);
    }

    public void SpawnFirstTileAndAddToList()
    {
        GameObject currentTile = objectPooler.SpawnFromPool(tileTag, new Vector3(0,0,10f), Quaternion.identity);
        activeTiles.Add(currentTile);
    }

    public void RecycleTile()
    {
        GameObject firstTile = activeTiles[0]; // this is the recycled tile
        activeTiles.RemoveAt(0);
        GameObject lastTile = activeTiles[activeTiles.Count - 1];

        int lastTileYRotation = (int)lastTile.transform.rotation.eulerAngles.y;

        if(currentStraightTiles == forceXTilesToBeStraight) // force a turn every 3 straight tiles
        {
            GetNewPositionFromVerticalDic(lastTile, firstTile, lastTileYRotation);
            currentStraightTiles = 1;
        }
        else
        {
            GetNewPositionFromHorizontalDic(lastTile, firstTile, lastTileYRotation);
            currentStraightTiles++;
        }

        firstTile.SetActive(true);
        
        activeTiles.Add(firstTile);
    }

    public void StopAllMovingObjects()
    {
        foreach(GameObject tile in activeTiles)
        {
            if(tile.TryGetComponent<MovingObjects>(out MovingObjects movingObject))
            {
                movingObject.SetSpeed(0f);
            }
        }
    }

    public void MoveAllMovingObjects()
    {
        foreach(GameObject tile in activeTiles)
        {
            if(tile.TryGetComponent<MovingObjects>(out MovingObjects movingObject))
            {
                movingObject.CalculateDirectionBasedOnCameraDirection();

                // set acumulated speed
                float speedBonus = 0.5f;
                movingObject.SetAccumulatedSpeed(speedBonus);
            } 
        }
    }
}
