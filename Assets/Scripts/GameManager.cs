using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public static GameManager Manager;
    [SerializeField] private GameUI gameUI;

    List<Artifact> artifacts = new List<Artifact>();

    void Awake()
    {
        if (Manager != null) Destroy(this);
        Manager = this;
    }

    public void ReturnToMenu()
    {
        SceneManager.LoadScene(0);
    }

    public void StartGame()
    {
        SceneManager.LoadScene(1);
    }

    public void AddArtifact(Artifact newArt)
    {
        artifacts.Add(newArt);
        gameUI.artifactCollected();
        Debug.Log("Added new artifact");
    }
}