using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Entity.Enemy
{
    public class HealthBar : MonoBehaviour
    {
        #region Variables

        // References
        [SerializeField] private Transform m_HealthBarAnchor;
        [SerializeField] private Image m_HealthBar;
        private Transform m_CameraTransform;

        #endregion

        private void Start()
        {
            m_CameraTransform = Camera.main.transform;
        }

        private void Update()
        {
            m_HealthBarAnchor.LookAt(m_CameraTransform);
        }

        internal void UpdateHealthBar(float maxHealth, float currentHealth)
        {
            m_HealthBar.fillAmount = currentHealth / maxHealth;
        }
    }
}