using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hero_Genji : Hero
{
    private float Jump_Timer;
    private bool Post_Climb;
    private bool Double_Jumping;    //has genji touched the ground since double jumping
    public float Climb_Speed;
    private bool Wall_Climbing;
    private bool Small_Stepping;
    private bool Can_Climb; //has genji touched the ground since wall climbing
    public float Terrain_Climb_Distance;
    private bool By_A_Wall;
    public bool Swift_Striking;
    public Hero_Genji_Swift_Strike_Target Swift_Strike_Target;
    private Hero_Genji_Swift_Strike_Target Strike_Target;
    private Vector3 Swift_Strike_End;
    public float Swift_Strike_Speed;
    private float Swift_Strike_Time;
    public float Swift_Strike_Duration = 0.4f;
    private Vector3 Last_Position;

    //Debug
    //float timer;

    void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        CC = GetComponent<CharacterController>();
        Ca = GetComponentInChildren<Camera>();
    }

    void Update()
    {
        Move_Direction.x = Input.GetAxis("Horizontal") * Time.deltaTime * Move_Speed;
        Move_Direction.z = Input.GetAxis("Vertical") * Time.deltaTime * Move_Speed;
        mouse_movement.x = Input.GetAxis("Mouse X") * Time.deltaTime;
        mouse_movement.y = Input.GetAxis("Mouse Y") * Time.deltaTime;   //get look and horizontal move input

        Check_For_Wall();   //check if looking at a wall

        if (CC.isGrounded)  //once genji touches the ground he can jump and double jump again; and begin wall climbing again; and is no longer falling
        {
            Jumping = false;
            Jump_Timer = 0;
            Double_Jumping = false;
            Falling = false;
            Can_Climb = true;

            Gravity_Applied = 0;

            if (!Wall_Climbing && !Jumping && Input.GetButtonDown("Jump"))   //if genji is not already wall climbing, not jumping and jump button is pressed
            {
                Jumping = true;
                Post_Climb = false;
                Jump_Timer = 0;
            }
        }
        else
        {
            Jump_Timer += Time.deltaTime;

            if (Input.GetButtonDown("Jump") && !Double_Jumping)     //while in the air check for double jump input
            {
                Double_Jumping = true;
                Jumping = true;
                Gravity_Applied = 0;
                Post_Climb = false;
            }
            if (!Jumping && !Wall_Climbing)   //if not grounded, not jumping and not wall climbing must be falling
            {
                Falling = true;
            }
        }
        if (!Swift_Striking && Input.GetButtonDown("Left Shift"))
            Swift_Strike_Destination();

        if ((CC.collisionFlags & CollisionFlags.Above) != 0)    //if head is hitting something
        {
            Wall_Climbing = false;
            Jumping = false;
        }

        if (Input.GetKeyDown(KeyCode.Escape))   //show cursor in editor
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    void FixedUpdate()
    {
        grounded = CC.isGrounded;
        if (!Swift_Striking)
        {
            Move_Direction.y = 0;
            Small_Stepping = false;
            Jump();
            Wall_Climb();
            Small_Step();
            Apply_Gravity();
            if (Wall_Climbing)  //while wall climbing, cannot move on horizontal axes
                Move_Direction = new Vector3(0, Move_Direction.y, 0);
            CC.Move(transform.TransformDirection(Move_Direction));
            transform.Rotate(Vector3.up * mouse_movement.x * Sensitivity);
            Ca.transform.Rotate(Vector3.left * mouse_movement.y * Sensitivity);
        }
        Swift_Strike();
    }

    #region Wall Climb

    void Check_For_Wall()
    {
        RaycastHit hit;
        Vector3[] dir = new Vector3[2];
        dir[0] = Ca.transform.localPosition;    //check from genji's head (camera)
        dir[1] = new Vector3(0, (CC.height / 2 * -0.7f), 0);      //check from genji's knees (assuming they're 30% up his body ish)
        int layermask = 1 << 9;
        for (int i = 0; i < dir.Length; i++)
        {
            if (Physics.Raycast(transform.TransformPoint(dir[i]), transform.forward, out hit, Terrain_Climb_Distance, layermask))
            {
                By_A_Wall = true;
                break;  //no need to check further, genji is by a surface he can climb
            }
            else
                By_A_Wall = false;
        }

        if (By_A_Wall && !Wall_Climbing && Can_Climb)
        {
            if (Input.GetButton("Jump"))
            {
                if (Jump_Timer >= Gravity)  //TODO change gravity to a non magic number - a small amount of jump time (probs less than half)
                {   //if player still holding jump after small delay begin wall climbing - allows for jumping over things at head height
                    Can_Climb = false;
                    Wall_Climbing = true;
                    Jumping = false;
                    Falling = false;
                }
            }
        }
        else if (Wall_Climbing)
        {
            if (Input.GetButtonUp("Jump"))  //once jump button is released, no more climbing
            {
                Jumping = true;
                Wall_Climbing = false;
            }
        }
    }

    void Wall_Climb()
    {
        if (!By_A_Wall) //if genji is no longer looking at the wall stop wall climbing
        {
            if (Wall_Climbing)  //if genji WAS climbing jump off the wall
            {
                Jumping = true;
                Post_Climb = true;
            }
            Wall_Climbing = false;

            int layermask = 1 << 9;
            if (Input.GetButton("Jump") && Physics.Raycast(transform.TransformPoint(0, -CC.height / 1.9f, 0), transform.forward, Terrain_Climb_Distance, layermask))   //if there is still a climbable surface by his feet
            {   //this check confirms that there is a step sized piece of climbable geometry and genji is looking to climb it
                Small_Stepping = true;
            }
        }
        else if (Wall_Climbing)
        {
            if (Move_Direction.z > 0)
            {
                Move_Direction.y += Climb_Speed * Time.deltaTime;
                Gravity_Applied = 0;
            }
            else
                Wall_Climbing = false;
        }
    }

    void Small_Step()
    {
        if (Small_Stepping)
        {
            Move_Direction.y += Climb_Speed * Time.deltaTime;
        }
    }
    #endregion

    #region Swift Strike

    void Swift_Strike_Destination()
    {
        Swift_Striking = true;
        Swift_Strike_Time = Time.time;
        Vector3 dir = Ca.transform.forward;
        dir.Normalize();
        RaycastHit hit; //check whether anything in the way of 14m
        int layermask = 1 << 9; //only check terrain layer

        if (!Physics.Raycast(Ca.transform.position, dir, out hit, 14, layermask))   //send raycast to see if terrain blocks any of the swift strike
        {
            Swift_Strike_End = transform.position + dir * 14;
        }
        else
        {
            Swift_Strike_End = hit.point + hit.normal;
        }
        Strike_Target = Instantiate(Swift_Strike_Target, Swift_Strike_End, Quaternion.identity);
        Strike_Target.Genji = gameObject;
    }

    void Swift_Strike()
    {
        if (Swift_Striking)
        {
            if (Time.time - Swift_Strike_Duration >= Swift_Strike_Time)
            {
                Swift_Striking = false;
                Destroy(Strike_Target.gameObject);
            }
            else
            {
                Vector3 heading = Swift_Strike_End - transform.position;    //face the target
                //heading.y = 0;
                //transform.rotation = Quaternion.LookRotation(heading);
                //Ca.transform.LookAt(Swift_Strike_End);
                //Quaternion rot = Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0);
                //transform.rotation = rot;
                CC.Move(/*Ca.transform.forward*/ heading.normalized * Swift_Strike_Speed);
                if (Last_Position == transform.position)
                    Swift_Striking = false;
                Last_Position = transform.position;
            }
        }
    }

    #endregion

    protected override void Jump()
    {
        if (Jumping)
        {
            if (!Post_Climb) //regular jump
            {
                if (Double_Jumping)
                {
                    Move_Direction.y += Jump_Force * 1.5f * Time.deltaTime; //TODO make double jump multiplier
                }
                else
                {
                    Move_Direction.y += Jump_Force * Time.deltaTime;
                }
            }
            else    //after climbing apply jump force if moving away from surface
            {
                if (Move_Direction.z < 0)  //moving backwards
                {
                    Move_Direction.y += Jump_Force * Time.deltaTime;
                }
                else if (Move_Direction.z == 0) //if no forward or backwards input is detected after climbing, move genji forwards until grounded
                {
                    Move_Direction.z = Time.deltaTime * Move_Speed;
                }
            }
        }
    }

    protected override void Apply_Gravity()
    {
        if (!Wall_Climbing)
        {
            Gravity_Applied += Gravity * Time.deltaTime;
            Move_Direction.y -= Gravity_Applied;
        }
    }

    void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.layer == 9)  //is it terrain
        {
            for (int i = 0; i < col.contacts.Length; i++)
            {
                if (col.contacts[i].point.y == -CC.height / 2)
                {
                    Post_Climb = false;
                    break;
                }
            }
        }
    }
}
