using UnityEngine;

public class ProwlerScript : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private float speed = 4f;
    [SerializeField] private float detectionRange = 7f;
    [SerializeField] private Transform[] waypoint;
    
    private bool intruderAlert = false;
    private int waypointIndex = 0;
    private float patrolTimer;
    [SerializeField] private float patrolInterval = 10f;

    void Update()
    {
        if (target != null)
        {
            float distance = Vector3.Distance(transform.position, target.position);

            Intruder_Detection(distance);
      
            if (intruderAlert)
            {
                Player_Persue();
            }
            else
            {
                Patrol();
            }
        }
    }

    void Intruder_Detection(float distance)
    {

        if (distance < detectionRange)
        {
            intruderAlert = true;
        }
        else
        {
            intruderAlert = false;
        }

    }

    void Player_Persue()
    {
        transform.position = Vector3.MoveTowards(transform.position, target.position, speed * Time.deltaTime);
    }

    void Patrol()
    {
        if (waypoint == null || waypoint.Length == 0)
        {
            return;
        }

        transform.position = Vector3.MoveTowards(
            transform.position,
            waypoint[waypointIndex].position,
            speed * Time.deltaTime
        );

        patrolTimer += Time.deltaTime;

        if (patrolTimer >= patrolInterval)
        {
            patrolTimer = 0f;

            waypointIndex++;

            if (waypointIndex >= waypoint.Length)
            {
                waypointIndex = 0;
            }
        }
    }



}
