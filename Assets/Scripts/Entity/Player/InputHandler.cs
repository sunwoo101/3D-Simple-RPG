using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Entity.Player
{
    public class InputHandler : MonoBehaviour
    {
        #region enums

        public enum InputMode
        {
            KBMouse,
        }

        #endregion

        #region Variables

        // Input
        public static InputMode s_InputMode;
        private bool m_InputEnabled;
        internal Vector2 m_Camera;
        internal bool m_LockMovement;
        internal Vector2 m_Movement;
        internal bool m_Sprint;
        internal bool m_Jump;
        internal bool m_PrimaryAttack;
        internal bool m_SecondaryAttack;
        internal bool m_PrimarySpecialAbility;
        internal bool m_SecondarySpecialAbility;

        // Buffers
        [SerializeField] private float m_JumpBufferDuration;
        private float m_JumpBufferTimer;

        #endregion

        internal void GetInput()
        {
            switch (s_InputMode)
            {
                case InputMode.KBMouse:
                    KBMouseInput();
                    break;
                default:
                    KBMouseInput();
                    break;
            }
        }

        private void KBMouseInput()
        {
            if (m_InputEnabled)
            {
                m_Camera.x = Input.GetAxisRaw("Mouse X");
                m_Camera.y = Input.GetAxisRaw("Mouse Y");
                m_LockMovement = Input.GetKey(KeyCode.LeftAlt);
                m_Movement.x = Input.GetAxisRaw("Horizontal");
                m_Movement.y = Input.GetAxisRaw("Vertical");
                m_Sprint = Input.GetKey(KeyCode.LeftShift);
                if (Input.GetKeyDown(KeyCode.Space)) m_JumpBufferTimer = m_JumpBufferDuration; // Buffer jump
                m_Jump = m_JumpBufferTimer > 0;
                m_PrimaryAttack = Input.GetMouseButtonDown(0);
                m_SecondaryAttack = Input.GetMouseButton(1);
                m_PrimarySpecialAbility = Input.GetKeyDown(KeyCode.E);
                m_SecondarySpecialAbility = Input.GetKeyDown(KeyCode.Q);
            }
            else
            {
                m_Camera = Vector2.zero;
                m_LockMovement = false;
                m_Movement = Vector2.zero;
                m_Sprint = false;
                m_Jump = false;
                m_PrimaryAttack = false;
                m_SecondaryAttack = false;
                m_PrimarySpecialAbility = false;
                m_SecondarySpecialAbility = false;
            }

            m_JumpBufferTimer -= Time.deltaTime;
        }

        internal bool Enabled()
        {
            return m_InputEnabled;
        }

        internal void Enable()
        {
            m_InputEnabled = true;
        }

        internal void Disable()
        {
            m_InputEnabled = false;
        }
    }
}