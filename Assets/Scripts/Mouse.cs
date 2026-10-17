using UnityEngine;

public class Mouse : MonoBehaviour
{
    [SerializeField] Transform spawnPt;
    [SerializeField] float timeInterval;
    [SerializeField] float speed;
    [SerializeField] float timeDecr;
    [SerializeField] float speedIncr;
    [SerializeField] float speedCap;
    [SerializeField] float timeActiveInc;
    [SerializeField] int numSensors;
    [SerializeField] float limit;
    [SerializeField] float encounterEffectNum;
    [SerializeField] float spawnChance;
    [SerializeField] float retreatSpeed;

    [Header("Weights")]
    [SerializeField] int coverWeight;
    [SerializeField] int enemyWeight;
    [SerializeField] int playerWeight;
    [SerializeField] int openWeight;
    [SerializeField] int wallWeight;

    Rigidbody rb;
    
    float timeActive = 0;
    bool isActive = false;

    float curTime;
    float initTimeInterval;
    float initSpeed;

    int enemyEncounters = 0;
    int playerEncounters = 0;

    Vector3 moveTo;

    void OnEnable()
    {
        rb = GetComponent<Rigidbody>();
        transform.position = spawnPt.position;
        GetComponent<MeshRenderer>().enabled = false;
        curTime = timeInterval;
        initTimeInterval = timeInterval;
        initSpeed = speed;

        StartRetreat(); //to make sure all values reset after leaving the room
    }

    void Update()
    {
        if (isActive)
        {
            if (curTime >= timeInterval) 
            {
                ResetMove();
            }
           else curTime++;

           if (InCamera()) timeActive += timeActiveInc;
        }
        else if (Vector3.Distance(transform.position, spawnPt.position) > 0.01f)//moveTo == spawnPt.position)
        {
            transform.position = Vector3.MoveTowards(transform.position, spawnPt.position, retreatSpeed); //10f is temp value
        }
        else if (Random.value > spawnChance)
        {
            GetComponent<MeshRenderer>().enabled = false;
            Debug.Log("retreated");
        }
        else
        {
            GetComponent<MeshRenderer>().enabled = true;
            Debug.Log("out again");
            isActive = true;
        }
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
        float calc = limit;
        Vector3 storedAngle = Vector3.zero;

        for (int i = 0; i < numSensors; i++)
        {
            int utility = 0;
            Vector3 angle = (transform.forward * Random.Range(-10, 10)) + (transform.right * Random.Range(-10, 10));
            angle.y = transform.position.y;
            RaycastHit hit;
            if (Physics.Raycast(transform.position, angle, out hit))
            {
                if (hit.collider.gameObject.CompareTag("Player"))
                {
                    utility = playerWeight;
                }
                else if (hit.collider.gameObject.CompareTag("Enemy"))
                {
                    utility = enemyWeight;
                }
                else if (hit.collider.gameObject.CompareTag("Wall"))
                {
                    utility = wallWeight;
                }
                else
                {
                    utility = coverWeight;
                }
            }
            else //if the raycast hits open air
            {
                utility = openWeight;
            }
            
            float newCalc = utility - (enemyEncounters + playerEncounters + timeActive);

            if (newCalc > calc)
            {
                calc = newCalc;
                storedAngle = angle;
            }
        }

        if (calc == limit)
        {
            StartRetreat();
            return Vector3.zero;
        }
        else return storedAngle;   
    }

    void StartRetreat()
    {
        rb.linearVelocity = Vector3.zero;
        timeActive = 0;
        timeInterval = initTimeInterval;
        speed = initSpeed;
        isActive = false;
    }

    void ChangeTimeInterval()
    {
        if ((enemyEncounters + playerEncounters) > encounterEffectNum && timeInterval > 0)
        {
            timeInterval -= timeDecr;
        }
    }

    void ChangeSpeed()
    {
        if ((enemyEncounters + playerEncounters) > encounterEffectNum && speed < speedCap)
        {
            speed += speedIncr;
        }
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
        }
        else if (other.gameObject.CompareTag("Enemy")) 
        {
            DistractEnemy(other.gameObject);
            enemyEncounters++;
        }
    }

    //should be floating a little off the ground
    void OnCollisionStay(Collision col)
    {
        ResetMove();
    }
}
