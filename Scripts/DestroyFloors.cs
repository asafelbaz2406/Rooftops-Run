using UnityEngine;

public class DestroyFloors : MonoBehaviour
{
    private void OnTriggerEnter(Collider other) {
        if(other.gameObject.CompareTag("Floor"))
        {
            other.gameObject.SetActive(false);
            TileManager.instance.RecycleTile();
        }
    }
}
