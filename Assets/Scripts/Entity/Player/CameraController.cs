using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Entity.Player
{
    public class CameraController : MonoBehaviour
    {
        #region Variables

        [SerializeField] private Vector3 m_DistanceOffset;
        [SerializeField] private Vector2 m_VerticalClamp;

        private float m_XRotation;
        private float m_YRotation;

        [SerializeField] private float m_AimInSpeed;
        [SerializeField] private float m_AimInFov;
        [SerializeField] private float m_DefaultFov;

        private float m_XRotationCache;
        private float m_YRotationCache;

        private Transform m_PlayerTransform;

        #endregion

        internal bool ZoomIn()
        {
            if (Camera.main.fieldOfView - m_AimInFov > m_AimInSpeed * Time.deltaTime)
            {
                Camera.main.fieldOfView -= m_AimInSpeed * Time.deltaTime;
            }
            else
            {
                Camera.main.fieldOfView = m_AimInFov;
                return true;
            }

            return false;
        }

        internal bool ZoomOut()
        {
            if (m_DefaultFov - Camera.main.fieldOfView > m_AimInSpeed * Time.deltaTime)
            {
                Camera.main.fieldOfView += m_AimInSpeed * Time.deltaTime;
            }
            else
            {
                Camera.main.fieldOfView = m_DefaultFov;
                return true;
            }

            return false;
        }

        internal void CacheRotation()
        {
            m_XRotationCache = m_XRotation;
            m_YRotationCache = m_YRotation;
        }

        internal void ReturnCamera()
        {
            m_XRotation = m_XRotationCache;
            m_YRotation = m_YRotationCache;
        }

        internal void RotateCamera(Vector2 axisInput, float sensitivity)
        {
            axisInput *= sensitivity;

            // Left/Right
            m_YRotation += axisInput.x;

            // Up/Down
            m_XRotation -= axisInput.y;
            m_XRotation = Mathf.Clamp(m_XRotation, m_VerticalClamp.x, m_VerticalClamp.y);

            // Apply rotation
            transform.rotation = Quaternion.Euler(m_XRotation, m_YRotation, 0f);
        }

        internal void OffsetCamera(Transform player)
        {
            m_PlayerTransform = player;

            // Get the player position
            Vector3 position = player.transform.position;

            // Offset the camera from the player position
            position += transform.right * m_DistanceOffset.x;
            position += transform.up * m_DistanceOffset.y;
            position -= transform.forward * m_DistanceOffset.z;
            transform.position = position;

            // Adjust camera if behind a wall
            RaycastHit raycastHit;
            Vector3 direction = transform.position - player.position;
            if (Physics.Raycast(player.position, direction, out raycastHit, Vector3.Distance(player.position, transform.position)))
            {
                if (!raycastHit.collider.CompareTag("MainCamera") && !raycastHit.collider.CompareTag("Player") && !raycastHit.collider.CompareTag("IgnoreProjectile"))
                {
                    transform.position = raycastHit.point;
                }
            }
        }

        public float ZDistanceOffset()
        {
            Vector3 direction = transform.position - m_PlayerTransform.position;
            Vector3 localDirection = transform.InverseTransformDirection(direction);
            float zDistance = Math.Abs(localDirection.z);

            return zDistance;
        }
    }
}