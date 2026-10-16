using UnityEngine;

//NOTE: main things left:
//  rethink calculations
//  put in player/enemy interactions properly
//  mb try and make some of the behavior more complex? Like hiding behind objects specifically idk
//  polish (make sure it doesn't get stuck so often)
//  make sure it returns to the mouse hole (related to calculations issue)


//add states? like an enum? State-based??
public class Mouse : MonoBehaviour
{
    [SerializeField] Transform spawnPt;
    [SerializeField] float timeInterval;
    [SerializeField] float speed;
    [SerializeField] float timeDecr;
    [SerializeField] float speedIncr;

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
        // curSpeed = speed;
        // isActive = false;
    }

    void Update()
    {
        // Debug.Log("will it change: " + (enemyEncounters + playerEncounters) / timeActive);
        if (moveTo == spawnPt.position)
        {
            Vector3.MoveTowards(transform.position, spawnPt.position, speed * 10f * Time.deltaTime); //10f is temp
        }
        else if (isActive) //how to deactivate once it goes back to the mouse hole?
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
            rb.AddForce(Vector3.Normalize(moveTo) * speed); //need to make sure it stays in bounds
            //and that the speed isn't changing between angles???
            //it's getting stuck now??
            //probably rotate a model instead
        }
    } //also needs to rotate to face where it's moving


    void ResetMove()
    {
        rb.linearVelocity = Vector3.zero;
        moveTo = FindNewAngle();
        ChangeSpeed();
        ChangeTimeInterval();
        curTime = 0;
    }

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
                else if (hit.collider.gameObject.CompareTag("Wall"))
                {
                    utility = 1;
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

            float newCalc = Mathf.Abs(utility * (Vector3.Distance(transform.position, spawnPt.position) - ((timeInterval * 0.01f) + speed))); //mb rethink this calculation?

            // Debug.Log(newCalc);

            if (newCalc > calc) 
            {
                calc = newCalc; //need to get index of raycstHit too
                storedAngle = angle;
            }
        }

        if (storedAngle == Vector3.zero) //if nothing is chosen, go back to spawn point
        {
            Debug.Log("going home");
            isActive = false;
            return spawnPt.position; //does it do this yet?
        }
        else
        {
            // Debug.Log(storedAngle);
            // transform.LookAt(storedAngle);
            return storedAngle;
        }        
    }

    float ChangeTimeInterval()
    {//need to rethink these calculations
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
        //temporarily decrease speed when hidden?
        RaycastHit hit;
        if (Physics.Linecast(Camera.main.transform.position, transform.position, out hit))
        {
            if (hit.collider.gameObject == gameObject) return true;
        }
        // Debug.Log("hidden");
        return false;
    }

    void PingEnemies(GameObject player)
    {
        //not here yet
        Vector3 playerPos = player.transform.position; //only current position, or are they aware of the changing location for a period of time? Or is it only updated when the player is still within sight of the enemies?

        //broadcast this current position to enemies
        //freeze the player for a sec??
    }

    void DistractEnemy(GameObject enemy)
    {
        //not here yet
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
    void OnCollisionStay(Collision col) //probably want to make it wait under cover for a few sec later; will this override it?
    {
        ResetMove();
    }
}
