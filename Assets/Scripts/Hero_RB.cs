using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hero_RB : MonoBehaviour
{
    Rigidbody RB;
    protected Camera Ca;
    private GameObject Grounded_Terrain;

    protected Vector2 mouse_movement;
    public float Sensitivity;
    protected Vector3 Move_Direction;
    public float Move_Speed;
    public bool Grounded;
    private bool Jumping;
    public float Jump_Force;

    void Start()
    {
        RB = GetComponent<Rigidbody>();
        Ca = GetComponentInChildren<Camera>();
    }

    void FixedUpdate()
    {
        Jump();
        Move();
    }

    void Update()
    {
        Move_Direction.x = Input.GetAxis("Horizontal"); // * Move_Speed;
        Move_Direction.z = Input.GetAxis("Vertical");// * Move_Speed;

        mouse_movement.x = Input.GetAxis("Mouse X") * Time.deltaTime;
        mouse_movement.y = Input.GetAxis("Mouse Y") * Time.deltaTime;
        transform.Rotate(Vector3.up * mouse_movement.x * Sensitivity);

        if (Grounded && Input.GetButtonDown("Jump"))
        {
            Jumping = true;
        }
    }

    void LateUpdate()
    {
        Ca.transform.Rotate(Vector3.left * mouse_movement.y * Sensitivity);
    }

    void Jump()
    {
        if (Jumping)
        {
            Jumping = false;
            RB.AddForce(transform.up * Jump_Force, ForceMode.VelocityChange);
        }
    }

    void Move()
    {
        if (Move_Direction.x != 0 || Move_Direction.z != 0)
        {
            Vector3 direction = new Vector3(Move_Direction.x, 0, Move_Direction.z);
            direction.Normalize();
            RB.AddForce(transform.TransformDirection(direction) * Move_Speed, ForceMode.Impulse);
            //RB.velocity = transform.TransformDirection(direction);
        }
        else
        {
            RB.AddForce(-RB.velocity * Move_Speed);
        }
    }

    void OnCollisionStay(Collision col)
    {
        if (col.gameObject.layer == 9)      //is it a piece of terrain
        {
            for (int i = 0; i < col.contacts.Length; i++)
            {
                if (transform.InverseTransformPoint(col.contacts[i].point).y <= -0.9 &&
                    transform.InverseTransformPoint(col.contacts[i].point).y >= -1.1)   //is the contact point at the very bottom of the collider
                                                                                        //(within a band due to floating point inexactness)
                {
                    Grounded = true;
                    Grounded_Terrain = col.gameObject;  //log the piece of terrain the hero is grounded to to compare in OnCollisionExit
                    break;
                }
                else
                    Grounded = false;
            }
        }
    }

    void OnCollisionExit(Collision col)
    {
        if (col.gameObject == Grounded_Terrain)
        {
            Grounded = false;
        }
    }
}
