using UnityEngine;

public class Mouse : MonoBehaviour
{
    [SerializeField] Transform spawnPt;
    [SerializeField] float timeInterval;
    [SerializeField] float speed;
    
    float timeActive = 0;
    bool isActive = false;

    float curTime = 0;

    int enemyEncounters = 0;
    int playerEncounters = 0;

    void Update()
    {
        if (isActive)
        {
           if (curTime >= timeInterval) 
           {
                FindNewAngle();
                Debug.Log("running");
                curTime = 0;
           }
           else curTime++;

           timeActive++;
        }
        else if (Random.value > 0.5) isActive = true;
    }

    void FindNewAngle() //return Euler, maybe?
    {
        
    }

    float ChangeTimeInterval()
    {
        return 0;
    }

    float ChangeSpeed()
    {
        return 0;
    }

    bool CheckRanges()
    {
        return false;
    }
}
