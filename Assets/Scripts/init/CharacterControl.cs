using Ethertia;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Ethertia
{

    public class CharacterControl : MonoBehaviour
    {

        public CharacterController m_CharacterController;

        public Gamemode m_Gamemode = Gamemode.Survival;

        public bool m_Flying = true;

        // Seprator
        public bool m_HasCollision = true;

        Vector3 m_MoveVelocity;  // for have Inertia when Move Stops
        Vector3 m_GravityVelocity;

        float m_EulerPitch = 0;

        public float m_MouseSensitivity = 0.5f;

        // attach Camera.transform to here
        public Transform m_CameraTransform;

        public Vector3 m_Gravity = new(0, -9.81f, 0);

        public float m_JumpHeight = 2;

        public InputAction m_ActionMove;
        public InputAction m_ActionLook;

        public bool g_IsManipulatingGame = true;


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

            if (Input.GetKeyDown(KeyCode.Comma))
            {
                g_IsManipulatingGame = !g_IsManipulatingGame;
            }

            m_CharacterController.detectCollisions = m_HasCollision;

            if (g_IsManipulatingGame)
            {
                // Camera View Rotate.
                Vector2 I_Look = m_ActionLook.ReadValue<Vector2>() * m_MouseSensitivity;

                // apply Yaw to the player entity.
                transform.Rotate(0, I_Look.x, 0);

                // apply Pitch to the camera
                m_EulerPitch += -I_Look.y;
                m_EulerPitch = Mathf.Clamp(m_EulerPitch, -90, 90);
                m_CameraTransform.localRotation = Quaternion.Euler(m_EulerPitch, 0, 0);


                // Movement. Disp
                Vector2 I_Move = m_ActionMove.ReadValue<Vector2>();

                Vector3 disp = new();

                disp.x = I_Move.x;
                disp.z = I_Move.y;

                if (!IsGrounded() && !IsFlying())
                {
                    disp *= 0.12f;  // air no move
                }

                if (IsFlying())
                {
                    if (Input.GetKey(KeyCode.Space)) disp.y += 1;
                    if (Input.GetKey(KeyCode.LeftShift)) disp.y += -1;
                }
                else if (Input.GetKey(KeyCode.Space) && IsGrounded())
                {
                    // Jump
                    disp.y += (m_JumpHeight * -2f * m_Gravity.y);  // sqrt here?
                }

                // Sprint
                if (Input.GetKey(KeyCode.LeftControl))
                {
                    disp *= 3.5f;
                }

                disp = transform.TransformDirection(disp);  // apply movement orientation.
                m_MoveVelocity += disp;
            }


            // Gravity acceleration.
            if (IsFlying() || IsGrounded())
            {
                //m_MoveVelocity.y = 0;
                m_GravityVelocity = Vector3.zero;
            } 
            else
            {
                m_MoveVelocity.y += m_Gravity.y * Time.deltaTime;
                m_GravityVelocity += m_Gravity * Time.deltaTime;
            }

            m_CharacterController.Move((m_MoveVelocity) * Time.deltaTime);

            // Damping
            m_MoveVelocity *= Mathf.Pow(0.03f, Time.deltaTime);
        }
    }

}
