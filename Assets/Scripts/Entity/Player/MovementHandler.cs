using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Entity.Player
{
    // Require components
    [RequireComponent(typeof(CharacterController))]

    public class MovementHandler : MonoBehaviour
    {
        #region Variables

        // References
        internal CharacterController m_CharacterController;

        // Movement vector
        private Vector3 m_Velocity;
        private bool m_LockedMovement;

        // Player
        private bool m_IsGrounded;

        #endregion

        private void Start()
        {
            // References
            m_CharacterController = GetComponent<CharacterController>();
        }

        internal bool IsGrounded()
        {
            return m_IsGrounded;
        }

        internal void GroundCheck()
        {
            m_IsGrounded = m_CharacterController.isGrounded;

            if (m_IsGrounded) // Check if the player is on the ground
            {
                m_Velocity.y = -100f; // Increase gravity to make sure player sticks to the ground while walking down slopes
            }
        }

        internal void Gravity()
        {
            m_Velocity.y += Physics.gravity.y * Time.deltaTime; // Add gravity to velocity
            m_CharacterController.Move(m_Velocity * Time.deltaTime); // Apply gravity
        }

        internal bool Move(Vector2 movementInput, Transform cameraTransform, float speed, bool lockMovement, CameraController cameraController)
        {
            Vector3 movement = (cameraTransform.forward * movementInput.y) + (cameraTransform.right * movementInput.x);

            bool skipRotation = false;

            // Lets the player continue moving the same direction while looking around
            if (lockMovement)
            {
                if (!m_LockedMovement)
                {
                    m_LockedMovement = true;
                    cameraController.CacheRotation();
                }

                movement = (transform.forward * movementInput.y) + (transform.right * movementInput.x);
            }
            else if (m_LockedMovement)
            {
                m_LockedMovement = false;
                skipRotation = true;
                cameraController.ReturnCamera();
            }

            movement.y = 0; // Makes sure player doesn't move slower when looking down
            movement.Normalize(); // Prevents player from moving faster on the diagonal axis

            m_CharacterController.Move(movement * speed * Time.deltaTime); // Move the player

            if (movement != Vector3.zero && !lockMovement && !skipRotation)
            {
                transform.forward = movement; // Set player rotation to the direction the player is moving

                return true;
            }
            else if (movement != Vector3.zero)
            {
                return true;
            }

            return false;
        }

        internal bool Jump(bool jumpInput, float jumpHeight)
        {
            if (jumpInput && m_IsGrounded)
            {
                m_Velocity.y = Mathf.Sqrt(jumpHeight * -3.0f * Physics.gravity.y); // Jump
                return true;
            }

            return false;
        }
    }
}