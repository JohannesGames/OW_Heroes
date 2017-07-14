using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hero_Pharah : Hero
{
    private bool Jump_Jetting;
    private bool Hovering;
    public float Hover_Power = 5;
    private float Hover_Power_Current;
    public float Jump_Jet_Force;
    public float Jump_Jet_Length;   //seconds
    private float Jump_Jet_Timer;


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
        mouse_movement.y = Input.GetAxis("Mouse Y") * Time.deltaTime;

        if (CC.isGrounded)
        {
            Falling = false;
            Hover_Power_Current = 0;
            Gravity_Applied = 0;

            if (!Jumping && Input.GetButtonDown("Jump"))    //jump
            {
                Jumping = true;
                Gravity_Applied = 0;
            }
            if (Input.GetMouseButton(1))
            {
                Hovering = true;
                Gravity_Applied = 0;
                Hover_Power_Current = Hover_Power / 1.9f;
            }
        }
        if (!Hovering && Input.GetButton("Jump")) //&& Time.time >= Jump_Timer - Jump_Length * 0.9f)   //hover
        {
            Hovering = true;
            if (!CC.isGrounded)
                Hover_Power_Current = Hover_Power / 3;
            if (Gravity_Applied >= Gravity / 2) //TODO explain this
                Hover_Power_Current = 0;
            Gravity_Applied = 0;
        }
        if (Input.GetMouseButton(1))
        {
            Hovering = true;
            if (Gravity_Applied >= Gravity / 2)
                Hover_Power_Current = 0;
            Gravity_Applied = 0;
        }
        if (Hovering)   //hover
        {
            if (Input.GetButtonUp("Jump") || Input.GetMouseButtonUp(1))
            {
                Hovering = false;
                Hover_Power_Current = Hover_Power / 3;
                Gravity_Applied = 0;
            }
        }
        if (Input.GetButtonDown("Left Shift"))  //jump jet
        {
            Jump_Jetting = true;
            Jump_Jet_Timer = Time.time + Jump_Jet_Length;
            Gravity_Applied = 0;
        }
        #region Cursor
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        #endregion
    }

    void FixedUpdate()
    {
        Move_Direction.y = 0;
        Jump();
        Hover();
        Jump_Jet();
        if (Falling && !CC.isGrounded)    //!Jumping && !Jump_Jetting && Hover_Power_Current <= Hover_Power / 2)
            Apply_Gravity();
        CC.Move(transform.TransformDirection(Move_Direction));
        transform.Rotate(Vector3.up * mouse_movement.x * Sensitivity);
        Ca.transform.Rotate(Vector3.left * mouse_movement.y * Sensitivity);
    }

    void Hover()
    {
        if (Hovering)
        {
            if (Hover_Power_Current < Hover_Power)
            {
                Hover_Power_Current += Time.deltaTime * 5;
                Move_Direction.y += Hover_Power_Current;
            }
            else
                Move_Direction.y += Time.deltaTime * Hover_Power;

            if (Hover_Power_Current <= Hover_Power / 2)
                Falling = true;
        }
    }

    void Jump_Jet()
    {
        if (Jump_Jetting && Time.time < Jump_Jet_Timer)
        {
            Vector3 point = new Vector3(0, 1, 0);   //a metre above
            transform.position = Vector3.Lerp(transform.position, transform.TransformPoint(point), Time.deltaTime * Jump_Jet_Force);
        }
        else
        {
            Jump_Jetting = false;
            Falling = true;
        }
    }
}
