using UnityEngine;

public class ProwlerScript : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private float speed = 3f;
    [SerializeField] private float detectionRange = 5f;
    [SerializeField] private Transform[] waypoint;
    [SerializeField] private float patrolInterval = 15f;
    [SerializeField] private float rayDistance = 2f;
    [SerializeField] private float avoidanceStrength = 2f;
    [SerializeField] private LayerMask obstacleLayers;

    private bool intruderAlert = false;
    private int waypointIndex = 0;
    private float patrolTimer = 0f;


    void Update()
    {
        if (target == null)
        {
            return;

        }

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
        MoveWithAvoidance(target.position);

    }


    void Patrol()
    {
        if (waypoint == null || waypoint.Length == 0)
        {
            return;
        }

        MoveWithAvoidance(waypoint[waypointIndex].position);

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


    void MoveWithAvoidance(Vector3 destination)
    {
        Vector3 direction = destination - transform.position;
        direction.y = 0f;

        if (direction.sqrMagnitude < 0.001f)
        {
            return;
        }

        direction.Normalize();
        Vector3 rayOrigin = transform.position + Vector3.up * 0.5f;
        Debug.DrawRay(rayOrigin, direction * rayDistance, Color.red);

        if (Physics.Raycast(rayOrigin, direction, rayDistance, obstacleLayers))
        {

            Vector3 left = Vector3.Cross(Vector3.up, direction).normalized;
            Vector3 right = -left;
            Vector3 leftDirection = (direction + left * avoidanceStrength).normalized;
            Vector3 rightDirection = (direction + right * avoidanceStrength).normalized;

            bool leftBlocked = Physics.Raycast(rayOrigin, leftDirection, rayDistance, obstacleLayers);
            bool rightBlocked = Physics.Raycast(rayOrigin, rightDirection, rayDistance, obstacleLayers);

            if (!leftBlocked)
            {
                direction = leftDirection;
            }
            else if (!rightBlocked)
            {
                direction = rightDirection;
            }
            else
            {
                return;
            }

        }

        Quaternion targetRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 10f * Time.deltaTime);
        transform.position += direction * speed * Time.deltaTime;

    }


}
