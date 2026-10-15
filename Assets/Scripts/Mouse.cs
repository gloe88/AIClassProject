using UnityEngine;

public class Mouse : MonoBehaviour
{
    [SerializeField] Transform spawnPt;
    [SerializeField] float timeInterval;
    [SerializeField] float speed;

    Rigidbody rb;
    
    float timeActive = 0; //waiiittt only make this go up if in camera view!!!
    bool isActive = true; //temp

    float curTime = 0;

    int enemyEncounters = 0;
    int playerEncounters = 0;

    Vector3 moveTo;

    void OnEnable()
    {
        rb = GetComponent<Rigidbody>();
        transform.position = spawnPt.position;
        // isActive = false;
    }

    void Update()
    {
        if (moveTo == spawnPt.position)
        {
            Vector3.MoveTowards(transform.position, spawnPt.position, speed * 10f * Time.deltaTime); //10f is temp
        }
        else if (isActive) //how to deactivate once it goes back to the mouse hole?
        {
           if (curTime >= timeInterval) 
           {
                rb.linearVelocity = Vector3.zero;
                moveTo = FindNewAngle();
                ChangeSpeed();
                ChangeTimeInterval();
                curTime = 0;
           }
           else curTime++;

           if (InCamera()) timeActive++;
        }
        else if (Random.value > 0.5) isActive = true;
    }

    void FixedUpdate()
    {
        if (isActive && moveTo != spawnPt.position) rb.AddForce(moveTo * speed); //need to make sure it stays in bounds
    } //also needs to rotate

    Vector3 FindNewAngle() //return Euler, maybe?
    {
        float calc = 0;
        Vector3 storedAngle = Vector3.zero;

        for (int i = 0; i < 3; i++)
        {
            int utility = 0; //how to get a random angle?
            Vector3 angle = (transform.forward * Random.Range(-10, 10)) + (transform.right * Random.Range(-10, 10));
            angle.y = transform.position.y;//transform.forward;//Random.Range(-10f, 10f);
            RaycastHit hit;
            if (Physics.Raycast(transform.position, angle, out hit)) //filter by mask?
            {
                //should it be RaycastAll(), or would that make it run horribly slow?
                if (hit.collider.gameObject.CompareTag("Player"))
                {
                    utility = 5;
                }
                else if (hit.collider.gameObject.CompareTag("Enemy"))
                {
                    utility = 3;
                }
                else //if it finds something that could be considered "cover." But does it go behind it?
                {
                    utility = 10;
                }
            }
            else //if the raycast hits open air
            {
                utility = 7;
            }

            float newCalc = Mathf.Abs(utility * (Vector3.Distance(transform.position, spawnPt.position) - (timeInterval + speed))); //mb rethink this calculation?

                Debug.Log(newCalc);

            if (newCalc > calc) 
            {
                calc = newCalc; //need to get index of raycstHit too
                storedAngle = angle;
            }
        }

        if (storedAngle == Vector3.zero) //if nothing is chosen, go back to spawn point
        {
            return spawnPt.position;
        }
        else
        {
            Debug.Log(storedAngle);
            return storedAngle;
        }        
    }

    float ChangeTimeInterval()
    {
        if ((enemyEncounters + playerEncounters) / timeActive > 0.5f)
        {
            timeInterval--;
        }
        return 0;
    }

    float ChangeSpeed()
    {
        if ((enemyEncounters + playerEncounters) / timeActive > 0.5f)
        {
            speed++;
        }
        return 0;
    }

    bool InCamera()
    {
        return true;
    }

    void PingEnemies()
    {

    }

    void DistractEnemy()
    {

    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player")) 
        {
            PingEnemies();
            playerEncounters++;
        }
        else if (other.gameObject.CompareTag("Enemy")) 
        {
            DistractEnemy();
            enemyEncounters++;
        }
    }
}
