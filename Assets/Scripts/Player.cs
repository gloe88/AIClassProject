using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    [SerializeField] float spd;
    [SerializeField] float weighDown; //so linear damping can be added w/o ruining gravity
    Rigidbody rb;
    Transform model;

    InputAction move;

    void Start()
    {
        InputActionAsset input = GetComponent<PlayerInput>().actions;
        move = input.FindAction("Move");

        rb = GetComponent<Rigidbody>();

        model = transform.GetChild(0);
    }

    void Update()
    {
        Vector3 distance = RoomControl.curRoom.position - transform.position;
        if (distance.x > 11)
        {
            RoomControl.NextRoom.Invoke(1);
        }
        else if (distance.x < -11)
        {
            RoomControl.NextRoom.Invoke(-1);
        }
    }

    void FixedUpdate()
    {
        Move();
    }


    void Move()
    {
        Vector2 moveVal = move.ReadValue<Vector2>();
        Vector3 movePos = (moveVal.x * transform.right) + (moveVal.y * transform.forward);
        movePos.y = 0;

        rb.AddForce(movePos * spd);

        if (rb.linearVelocity.y <= 1) rb.AddForce(Physics.gravity * weighDown, ForceMode.Acceleration);

        Vector3 facingPos = Vector3.zero;

        if (moveVal != Vector2.zero) facingPos = movePos;

        model.LookAt(transform.position + facingPos * 3f);
    }
}
