using UnityEngine;

public class Mouse : MonoBehaviour
{
    [SerializeField] Transform spawnPt;
    [SerializeField] float timeInterval;
    [SerializeField] float speed;
    [SerializeField] float timeDecr;
    [SerializeField] float speedIncr;

    Rigidbody rb;
    
    float timeActive = 0;
    bool isActive = true; //temp value

    float curTime = 0;

    int enemyEncounters = 0;
    int playerEncounters = 0;

    Vector3 moveTo;

    void OnEnable()
    {
        rb = GetComponent<Rigidbody>();
        transform.position = spawnPt.position;
    }

    void Update()
    {
        if (moveTo == spawnPt.position)
        {
            Vector3.MoveTowards(transform.position, spawnPt.position, speed * 10f * Time.deltaTime); //10f is temp value
        }
        else if (isActive)
        {
           if (curTime >= timeInterval) 
           {
                ResetMove();
           }
           else curTime++;

           if (InCamera()) timeActive++;
        }
        else if (Random.value > 0.5) isActive = true;
    }

    void FixedUpdate()
    {
        if (isActive && moveTo != spawnPt.position) 
        {
            rb.AddForce(Vector3.Normalize(moveTo) * speed);
        }
    }


    void ResetMove()
    {
        rb.linearVelocity = Vector3.zero;
        moveTo = FindNewAngle();
        ChangeSpeed();
        ChangeTimeInterval();
        curTime = 0;
    }

    Vector3 FindNewAngle()
    {        
        float calc = 0;
        Vector3 storedAngle = Vector3.zero;

        for (int i = 0; i < 3; i++)
        {
            int utility = 0;
            Vector3 angle = (transform.forward * Random.Range(-10, 10)) + (transform.right * Random.Range(-10, 10));
            angle.y = transform.position.y;
            RaycastHit hit;
            if (Physics.Raycast(transform.position, angle, out hit))
            {
                if (hit.collider.gameObject.CompareTag("Player"))
                {
                    utility = 5;
                }
                else if (hit.collider.gameObject.CompareTag("Enemy"))
                {
                    utility = 3;
                }
                else if (hit.collider.gameObject.CompareTag("Wall"))
                {
                    utility = 1;
                }
                else
                {
                    utility = 10;
                }
            }
            else //if the raycast hits open air
            {
                utility = 7;
            }

            float newCalc = Mathf.Abs(utility * (Vector3.Distance(transform.position, spawnPt.position) - ((timeInterval * 0.01f) + speed)));

            if (newCalc > calc) 
            {
                calc = newCalc;
                storedAngle = angle;
            }
        }

        if (storedAngle == Vector3.zero) //if nothing is chosen, go back to spawn point
        {
            Debug.Log("going home");
            isActive = false;
            return spawnPt.position;
        }
        else return storedAngle;   
    }

    float ChangeTimeInterval()
    {
        if ((enemyEncounters + playerEncounters) / timeActive > 0.5f)
        {
            timeInterval -= timeDecr;
        }
        Debug.Log("decreasing time");
        return 0;
    }

    float ChangeSpeed()
    {
        if ((enemyEncounters + playerEncounters) / timeActive > 0.5f)
        {
            speed += speedIncr;
        }
        Debug.Log("increasing speed");
        return 0;
    }

    bool InCamera()
    {
        RaycastHit hit;
        if (Physics.Linecast(Camera.main.transform.position, transform.position, out hit))
        {
            if (hit.collider.gameObject == gameObject) return true;
        }
        return false;
    }

    //For multi-agent implementation
    void PingEnemies(GameObject player)
    {
        Vector3 playerPos = player.transform.position;
    }

    //For multi-agent implementation
    void DistractEnemy(GameObject enemy)
    {

    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player")) 
        {
            PingEnemies(other.gameObject);
            playerEncounters++;
            ResetMove();
        }
        else if (other.gameObject.CompareTag("Enemy")) 
        {
            DistractEnemy(other.gameObject);
            enemyEncounters++;
            ResetMove();
        }
    }

    //should be floating a little off the ground
    void OnCollisionStay(Collision col)
    {
        ResetMove();
    }
}
