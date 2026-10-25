using UnityEngine;

public class Blocker : MonoBehaviour
{
    Rigidbody rb;
    public bool seesPlayer;
    bool foundExit;
    bool patrolledOnce;
    GameObject exit;
    Vector3 exitPos;
    Vector3 pPoint1;
    Vector3 pPoint2;
    int utility;
    float decision;
    float timer;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void OnEnable()
    {
        utility = 0;
        timer = 0.0f;
        rb = GetComponent<Rigidbody>();
        foundExit = false;
        exit = GameObject.FindWithTag("Finish");
        exitPos = new Vector3(exit.transform.position.x + 1, exit.transform.position.y, exit.transform.position.z);
        pPoint1 = new Vector3(exit.transform.position.x + 1, exit.transform.position.y, exit.transform.position.z - 4);
        pPoint2 = new Vector3(exit.transform.position.x + 1, exit.transform.position.y, exit.transform.position.z + 4);
    }

    // Update is called once per frame
    void Update()
    {
        //ups utility value when spotting player
        if (seesPlayer == true){
            utility = 10;
        }
        //utility system should have levels for no interaction, interaction with mouse, then player
        decision = Random.Range(0f, 3f) + utility;
        if (decision > 5){
            Block();
        }
        else
        {
            Patrol();
        }
    }
    void Patrol()
    {
        if (transform.position == exitPos){
            foundExit = true;
        }
        else if (!foundExit){
            transform.LookAt(exitPos, Vector3.down);
            transform.position = Vector3.MoveTowards(transform.position, exitPos, .01f);
        }
        if (foundExit) {
            //consider add way to slowly rotate to point before moving
                if (!patrolledOnce){
                    transform.LookAt(pPoint1);
                    transform.position = Vector3.MoveTowards(transform.position, pPoint1, 0.01f);
                    if (transform.position == pPoint1){
                        timer += Time.deltaTime;
                        if (timer > 1f)
                        {
                            patrolledOnce = true;
                            timer = timer - 1f; // reset timer
                        }
                    }
                }
                else {
                    transform.LookAt(pPoint2);
                    transform.position = Vector3.MoveTowards(transform.position, pPoint2, 0.01f);
                    if (transform.position == pPoint2){
                        timer += Time.deltaTime;
                        if (timer > 1f)
                        {
                            patrolledOnce = false;
                            timer = timer - 1f; // reset timer
                        }
                    }  
            }
        }
    }
    void Block(){
        foundExit = false;
        transform.LookAt(GameObject.FindWithTag("Player").transform);
        transform.position = Vector3.MoveTowards(transform.position, exitPos, .01f);

        if (seesPlayer != true){
            timer += Time.deltaTime;
            if (timer > 0.5f)
            {
                utility = 0;
                timer = timer - 0.5f; // reset timer
            }
        }
    }
}
