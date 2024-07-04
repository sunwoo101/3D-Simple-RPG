using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Entity.Player
{
    public class PlayerHUD : MonoBehaviour
    {
        #region Variables

        // UI Component References
        [SerializeField] private Image m_HealthBar;
        [SerializeField] private Image m_ExperienceBar;
        [SerializeField] private TMP_Text m_LevelText;
        [SerializeField] private Image m_ChargeIndicator;

        #endregion

        internal void SetHealthBar(float maxHealth, float currentHealth)
        {
            m_HealthBar.fillAmount = currentHealth / maxHealth;
        }

        internal void SetExperienceBar(float requiredXP, float currentXP)
        {
            m_ExperienceBar.fillAmount = currentXP / requiredXP;
        }

        internal void SetLevelText(int level)
        {
            m_LevelText.text = "Level " + level;
        }

        internal void SetChargeIndicator(float maxCharge, float currentCharge)
        {
            m_ChargeIndicator.fillAmount = currentCharge / maxCharge;
        }
    }
}