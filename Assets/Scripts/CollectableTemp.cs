using UnityEngine;

public class CollectableTemp : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player")) GameManager.Manager.ReturnToMenu();
    }
}
