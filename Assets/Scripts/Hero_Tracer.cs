using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Recall_Info
{
    public Vector3 Position;
    public Quaternion Rotation;
    public int Health;
}

public class Hero_Tracer : Hero
{
    bool Blinking;
    float Blink_Distance = 10;

    bool Recalling;
    int Recall_Index;
    public float Recall_Length = 2; //how long recall takes - seconds
    private float Recall_Start_Time;
    private List<Recall_Info> Recall_List = new List<Recall_Info>();
    private float Recall_List_Interval;
    private Vector3 Recall_From_Pos;
    private Quaternion Recall_From_Rot;

    void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        CC = GetComponent<CharacterController>();
        Ca = GetComponentInChildren<Camera>();
    }

    void Update()
    {

        if (!Jumping)
        {
            Move_Direction.x = Input.GetAxis("Horizontal") * Time.deltaTime * Move_Speed;
            Move_Direction.z = Input.GetAxis("Vertical") * Time.deltaTime * Move_Speed;
        }
        else    //if jumping, halve the horizontal movement speed
        {
            Move_Direction.x = Input.GetAxis("Horizontal");
            Move_Direction.z = Input.GetAxis("Vertical");
        }

        mouse_movement.x = Input.GetAxis("Mouse X") * Time.deltaTime;
        mouse_movement.y = Input.GetAxis("Mouse Y") * Time.deltaTime;

        if (Input.GetButtonDown("Fire2") || Input.GetButtonDown("Left Shift"))  //blink?
        {
            Blinking = true;
            Move_Direction.y = 0;
            if (Move_Direction.x == 0 && Move_Direction.z == 0) //if no direction has been chosen blink forward
            {
                Blink(transform.forward);
            }
            else
            {
                Blink(transform.TransformDirection(Move_Direction));
            }
        }

        if (Input.GetButtonDown("Ability 2"))   //recall?
        {
            Recalling = true;
            Recall_Index = 29;
            Recall_Start_Time = Time.time;
            Recall_From_Pos = transform.position;
            Recall_From_Rot = transform.rotation;
        }
        Update_Recall_List();
        Recall();

        Move_Direction.x = (Move_Direction.x * Time.deltaTime * Move_Speed) / 2;
        Move_Direction.z = (Move_Direction.z * Time.deltaTime * Move_Speed) / 2;

        if (CC.isGrounded)
        {
            Falling = false;
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    void FixedUpdate()
    {
        Move_Direction.y = 0;
        if (!Jumping && CC.isGrounded && Input.GetButtonDown("Jump"))
        {
            Jumping = true;
            Gravity_Applied = 0;
        }
        Jump();
        if (!Jumping && !Blinking)
            Apply_Gravity();
        CC.Move(transform.TransformDirection(Move_Direction));
        transform.Rotate(Vector3.up * mouse_movement.x * Sensitivity);
        Ca.transform.Rotate(Vector3.left * mouse_movement.y * Sensitivity);
    }
    
    void Blink(Vector3 dir)
    {
        dir.Normalize();
        transform.position = transform.position + dir * Blink_Distance;
        Invoke("Reset_Blink", 0.05f);
    }

    void Reset_Blink()
    {
        Blinking = false;
    }

    void Update_Recall_List()
    {
        if (!Recalling)
        {
            if (Recall_List_Interval <= Time.time)
            {
                Recall_List_Interval = Time.time + 0.05f;    //save ten points per second
                if (Recall_List.Count == 60)
                {
                    Recall_List.RemoveAt(0);
                }
                else if (Recall_List.Count > 60)    //in the unlikely event of too many items being added, delete the relevant amount
                {
                    int toDelete = Recall_List.Count - 60;
                    for (int i = toDelete; i > 0; i--)
                    {
                        Recall_List.RemoveAt(i);
                    }
                }
                Recall_Info temp = new Recall_Info();
                temp.Health = Health_Current;
                temp.Position = transform.position;
                temp.Rotation = transform.rotation;
                Recall_List.Add(temp);
            }
        }
    }

    void Recall()
    {
        if (Recalling)
        {
            float time_allowed = Recall_Length / 30;
            float progress = (Time.time - Recall_Start_Time) / time_allowed;
            transform.position = Vector3.Lerp(Recall_From_Pos, Recall_List[Recall_Index].Position, progress);
            transform.rotation = Quaternion.Lerp(Recall_From_Rot, Recall_List[Recall_Index].Rotation, progress);

            if (transform.position == Recall_List[Recall_Index].Position)
            {
                Recall_Index--;

                if (Recall_Index < 0)   //once all the points have been passed through
                {
                    for (int i = Recall_List.Count - 1; i > 0; i--)
                    {
                        Recall_List.RemoveAt(i);
                        Health_Current = Recall_List[0].Health;
                        Recalling = false;
                    }
                }
                else
                {
                    Recall_From_Pos = Recall_List[Recall_Index + 1].Position;
                    Recall_From_Rot = Recall_List[Recall_Index + 1].Rotation;
                    Recall_Start_Time = Time.time;
                }
            }

        }
    }
}
