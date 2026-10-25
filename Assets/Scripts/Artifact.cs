using UnityEngine;

public class Artifact : MonoBehaviour
{
    string name;

    void Awake()
    {
        name = gameObject.name;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player")) 
        {
            GameManager.Manager.AddArtifact(this);
            gameObject.SetActive(false);
            // Destroy(gameObject);
        }
    }
}
