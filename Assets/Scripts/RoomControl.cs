using UnityEngine;
using Unity.Cinemachine;
using System.Linq;
using UnityEngine.Events;

public class RoomControl : MonoBehaviour
{
    public static UnityEvent<int>NextRoom = new UnityEvent<int>();

    [SerializeField] CinemachineCamera[] cam;
    [SerializeField] int curCam;

    public static Transform curRoom;

    void Awake()
    {
        NextRoom.AddListener(SwitchCam);
    }

    void Start()
    {
        //https://stackoverflow.com/questions/67427748/unity-is-storing-the-gameobjects-in-my-arrays-out-of-order
        cam = FindObjectsByType<CinemachineCamera>(FindObjectsInactive.Include).OrderBy(cm => cm.gameObject.transform.parent.name).ToArray();
        SwitchCam(0);
    }

    void SwitchCam(int num)
    {
        Debug.Log("running " + num);
        if ((num < 0 && curCam > 0) || (num > 0 && curCam < cam.Length) || num == 0)
        {
            curCam += num;
            for (int i = 0; i < cam.Length; i++)
            {
                if (i != curCam) cam[i].enabled = false; 
                else 
                {
                    cam[i].enabled = true;
                    curRoom = cam[curCam].gameObject.transform.parent;
                }
            }
        }
        
    }
}
