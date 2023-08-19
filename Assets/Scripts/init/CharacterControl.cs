using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CharacterControl : MonoBehaviour
{

    public CharacterController m_CharacterController;

    public bool m_Flying = true;

    Vector3 m_MoveVelocity;  // for have Inertia when Move Stops
    Vector3 m_GravityVelocity;

    float m_RotX = 0;

    public float m_MouseSensitivity = 400;

    public Transform m_CameraTransform;

    public Vector3 m_Gravity = new(0, -9.81f, 0);

    public float m_JumpHeight = 2;

    public InputAction m_ActionMove;
    public InputAction m_ActionLook;


    void Start()
    {
        m_CharacterController = GetComponent<CharacterController>();
    }

    void OnEnable()
    {
        m_ActionMove.Enable();
        m_ActionLook.Enable();
    }

    public bool IsGrounded()
    {
        return m_CharacterController.isGrounded;
    }

    public bool IsFlying()
    {
        return m_Flying;
    }

    //public Vector3 Move(float sprint, float front, float back, float left, float right, float jmp)
    //{
    //    Vector3 mov =
    //        front * Vector3.forward +
    //        back * Vector3.back +
    //        left * Vector3.left +
    //        right * Vector3.right + Vector3.up * jmp;
    //    mov *= 3.0f + sprint * 2.0f;

    //    return mov;
    //}


    void Update()
    {
        if (!Input.GetKey(KeyCode.LeftAlt))
        {
            Vector2 I_Look = m_ActionLook.ReadValue<Vector2>();

            //I_Look.x = Input.GetAxis("Mouse X");
            //I_Look.y = Input.GetAxis("Mouse Y");

            I_Look *= m_MouseSensitivity * Time.deltaTime;

            transform.Rotate(0, I_Look.x, 0);

            m_RotX += -I_Look.y;
            m_RotX = Mathf.Clamp(m_RotX, -90, 90);
            m_CameraTransform.localRotation = Quaternion.Euler(m_RotX, 0, 0);
        }


        Vector3 velMove = new();

        //velMove.x = Input.GetAxis("Horizontal");
        //velMove.z = Input.GetAxis("Vertical");

        Vector2 I_Move = m_ActionMove.ReadValue<Vector2>();
        velMove.x = I_Move.x;
        velMove.z = I_Move.y;

        if (Input.GetKey(KeyCode.Space) && IsGrounded())
        {
            velMove += (m_JumpHeight * -2f * m_Gravity);  // sqrt here?
        }

        if (IsFlying())
        {
            if (Input.GetKey(KeyCode.Space)) velMove.y += 1;
            if (Input.GetKey(KeyCode.LeftShift)) velMove.y += -1;
        }

        if (Input.GetKey(KeyCode.LeftControl))
        {
            velMove *= 3.5f;
        }

        velMove = transform.TransformDirection(velMove);  // apply move orientation.
        m_MoveVelocity += velMove;


        // Gravity acceleration.
        if (IsFlying() || IsGrounded())
        {
            m_GravityVelocity = Vector3.zero;
        } 
        else
        {
            m_GravityVelocity += m_Gravity * Time.deltaTime;
        }


        m_CharacterController.Move((m_MoveVelocity + m_GravityVelocity) * Time.deltaTime);

        // Damping
        m_MoveVelocity *= 0.9f; 
    }
}
