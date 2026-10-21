using UnityEngine;

public class Sight : MonoBehaviour
{
    public Blocker blocker;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //blocker = this.transform.parent.GameObject.GetComponent<Blocker>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void OnTriggerEnter(Collider col){
        if (col.gameObject.tag == "Player"){
            blocker.seesPlayer = true;
        }
    }
    void OnTriggerExit(Collider col){
        if (col.gameObject.tag == "Player"){
            blocker.seesPlayer = false;
        }
    }
}
