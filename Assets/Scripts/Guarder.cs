using System.Collections.Generic;
using UnityEngine;

public class Guarder : MonoBehaviour
{
    enum State { Patrol, Guard, Chase }

    [SerializeField] Transform artifact;
    [SerializeField] float patrolRadius = 8f;   //how far from the artifact it can roam
    [SerializeField] float speed = 3f;
    [SerializeField] float chaseSpeed = 4.5f;
    [SerializeField] float turnSpeed = 90f;     //degrees per second while scanning
    [SerializeField] float viewRange = 10f;
    [SerializeField] float viewAngle = 90f;     //full cone angle
    [SerializeField] int numCandidates = 3;     //how many random points it checks
    [SerializeField] float distWeight = 1f;     //penalty for being far from the artifact
    [SerializeField] float guardTime = 6f;
    [SerializeField] float loseSightTime = 3f;

    public bool seesPlayer;

    State state = State.Patrol;
    GameObject player;
    Vector3 artifactHome;
    Vector3 target;
    Vector3 lastKnownPlayerPos;
    float timer;
    float lostTimer;

    //remembers where it has been recently so it prefers new areas
    List<Vector3> recentPoints = new List<Vector3>();

    void Start()
    {
        player = GameObject.FindWithTag("Player");
        artifactHome = artifact.position;
        PickPatrolPoint();
    }

    void Update()
    {
        seesPlayer = CanSee(player.transform);
        CheckArtifact();

        switch (state)
        {
            case State.Patrol: Patrol(); break;
            case State.Guard:  Guard();  break;
            case State.Chase:  Chase();  break;
        }
    }

    void Patrol()
    {
        if (seesPlayer)
        {
            state = State.Guard;
            timer = guardTime;
            return;
        }

        MoveToward(target, speed);

        if (Vector3.Distance(transform.position, target) < 0.2f)
        {
            //scan on the spot for a moment then pick a new point
            transform.Rotate(0, turnSpeed * Time.deltaTime, 0);
            timer += Time.deltaTime;
            if (timer > 2f)
            {
                timer = 0;
                PickPatrolPoint();
            }
        }
    }

    //utility = how new the spot is - how far it is from the artifact
    void PickPatrolPoint()
    {
        float bestUtility = float.MinValue;
        Vector3 best = artifactHome;

        for (int i = 0; i < numCandidates; i++)
        {
            Vector2 r = Random.insideUnitCircle * patrolRadius;
            Vector3 point = new Vector3(artifactHome.x + r.x, transform.position.y, artifactHome.z + r.y);

            float novelty = 10f; //if no history, everything is new
            foreach (Vector3 old in recentPoints)
                novelty = Mathf.Min(novelty, Vector3.Distance(point, old));

            float utility = novelty - distWeight * Vector3.Distance(point, artifactHome);

            if (utility > bestUtility)
            {
                bestUtility = utility;
                best = point;
            }
        }

        target = best;
        recentPoints.Add(best);
        if (recentPoints.Count > 5) recentPoints.RemoveAt(0); //only remember the last few
    }

    void Guard()
    {
        if (seesPlayer)
        {
            state = State.Chase;
            lostTimer = 0;
            return;
        }

        if (Vector3.Distance(transform.position, artifactHome) > 1.5f)
        {
            MoveToward(artifactHome, speed);
        }
        else
        {
            transform.Rotate(0, turnSpeed * Time.deltaTime, 0); //scan around the artifact
            timer -= Time.deltaTime;
            if (timer <= 0)
            {
                state = State.Patrol;
                timer = 0;
                PickPatrolPoint();
            }
        }
    }

    void Chase()
    {
        if (seesPlayer)
        {
            lastKnownPlayerPos = player.transform.position;
            lostTimer = 0;
        }
        else
        {
            lostTimer += Time.deltaTime;
            if (lostTimer > loseSightTime)
            {
                state = State.Guard;
                timer = guardTime;
                return;
            }
        }
        MoveToward(lastKnownPlayerPos, chaseSpeed);
    }

    //only finds out the artifact is gone if it actually looks at its spot
    void CheckArtifact()
    {
        if (!InCone(artifactHome)) return;

        bool stolen = artifact == null
                   || !artifact.gameObject.activeInHierarchy
                   || Vector3.Distance(artifact.position, artifactHome) > 1f;

        if (stolen) BecomeOtherGuard();
    }

    void BecomeOtherGuard()
    {
        if (Random.value < 0.5f)
        {
            gameObject.AddComponent<Blocker>(); //needs an object tagged Finish in the scene
        }
        else
        {
            Debug.Log("Guarder -> Prowler"); //add prowler script here when it exists
        }
        enabled = false;
    }

    void MoveToward(Vector3 dest, float s)
    {
        dest.y = transform.position.y;
        transform.LookAt(dest);
        transform.position = Vector3.MoveTowards(transform.position, dest, s * Time.deltaTime);
    }

    bool InCone(Vector3 pos)
    {
        Vector3 to = pos - transform.position;
        to.y = 0;
        return to.magnitude <= viewRange && Vector3.Angle(transform.forward, to) <= viewAngle / 2f;
    }

    bool CanSee(Transform t)
    {
        if (!InCone(t.position)) return false;
        RaycastHit hit;
        Vector3 dir = t.position - transform.position;
        if (Physics.Raycast(transform.position, dir, out hit, viewRange))
            return hit.collider.gameObject.CompareTag("Player");
        return false;
    }
}