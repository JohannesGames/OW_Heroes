using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public abstract class Hero : Entity
{
    public override Entity_Type EType
    {
        get
        {
            return Entity_Type.Hero;
        }
    }

    protected CharacterController CC;
    protected Camera Ca;

    public bool grounded;

    protected Vector2 mouse_movement;
    protected Vector3 Move_Direction;
    public float Sensitivity;
    public float Move_Speed;
    public int Health_Max;
    [HideInInspector]
    public int Health_Current;
    //-------------------- Jumping variables
    protected bool Jumping;   //is the hero currently jumping
    protected bool Falling;
    public float Jump_Force;
    //--------------------------
    public float Gravity;
    protected float Gravity_Applied;

    #region Add to all
    //void Start()
    //{
    //    Cursor.visible = false;
    //    Cursor.lockState = CursorLockMode.Locked;
    //    CC = GetComponent<CharacterController>();
    //    Ca = GetComponentInChildren<Camera>();
    //}

    //void Update()
    //{
    //    if (!Jumping)
    //    {
    //        Move_Direction.x = Input.GetAxis("Horizontal") * Time.deltaTime * Move_Speed;
    //        Move_Direction.z = Input.GetAxis("Vertical") * Time.deltaTime * Move_Speed;
    //    }
    //    else    //if jumping, halve the horizontal movement speed
    //    {
    //        Move_Direction.x = (Input.GetAxis("Horizontal") * Time.deltaTime * Move_Speed) / 2;
    //        Move_Direction.z = (Input.GetAxis("Vertical") * Time.deltaTime * Move_Speed) / 2;
    //    }

    //    mouse_movement.x = Input.GetAxis("Mouse X") * Time.deltaTime;
    //    mouse_movement.y = Input.GetAxis("Mouse Y") * Time.deltaTime;

    //    if (CC.isGrounded)
    //    {
    //        Falling = false;
    //    }

    //    if (Input.GetKeyDown(KeyCode.Escape))
    //    {
    //        Cursor.lockState = CursorLockMode.None;
    //        Cursor.visible = true;
    //    }
    //}

    //void FixedUpdate()
    //{
    //    Move_Direction.y = 0;
    //    if (!Jumping && CC.isGrounded && Input.GetButtonDown("Jump"))
    //    {
    //        Jump_Timer = Time.time + Jump_Length;
    //        Jumping = true;
    //        Gravity_Applied = 0;
    //    }
    //    Jump();
    //    if (!Jumping)
    //        Apply_Gravity();
    //    CC.Move(transform.TransformDirection(Move_Direction));
    //    transform.Rotate(Vector3.up * mouse_movement.x * Sensitivity);
    //    Ca.transform.Rotate(Vector3.left * mouse_movement.y * Sensitivity);
    //}
    #endregion

    protected virtual void Jump()
    {
        if (Jumping)
        {
            Move_Direction.y += Jump_Force * Time.deltaTime;
        }
    }

    protected virtual void Apply_Gravity()
    {
        Gravity_Applied += Gravity * Time.deltaTime;
        Move_Direction.y -= Gravity_Applied;
    }
}
