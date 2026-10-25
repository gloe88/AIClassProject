using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class GameUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI artifactCountText;
    private int artifactCount = 0;

    void Start()
    {
        updateArtifactCount();
    }

    public void artifactCollected()
    {
        artifactCount++;
        updateArtifactCount();
    }

    private void updateArtifactCount()
    {
        if(artifactCountText != null)
        {
            artifactCountText.text = "Artifacts Found: " + artifactCount.ToString();
        } else
        {
            Debug.LogWarning("artifact count text is unassigned in editor");
        }

    }
}
