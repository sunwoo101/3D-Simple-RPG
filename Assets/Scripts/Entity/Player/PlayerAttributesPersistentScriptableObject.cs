using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Entity.Player
{
    [CreateAssetMenu(fileName = "PlayerAttributesPersistentScriptableObject", menuName = "ScriptableObjects/Entity/Player/PlayerAttributesPersistent")]
    public class PlayerAttributesPersistentScriptableObject : ScriptableObject
    {
        internal float m_Health;
        internal float m_AttackDamage;
        internal int m_Level;
        internal float m_Experience;
        // https://blog.jakelee.co.uk/converting-levels-into-xp-vice-versa/
        internal float m_ExperienceRequiredForLevel;
        [Header("Lower value requires more XP")]
        [SerializeField] private float m_ExperienceAmountConstant; // x
        [Header("Higher value increaes the requirement by a larger amount")]
        [SerializeField] private float m_ExperienceRequirementConstant; // y

        internal void UpdateExperienceRequired()
        {
            m_ExperienceRequiredForLevel = Mathf.Pow(m_Level / m_ExperienceAmountConstant, m_ExperienceRequirementConstant);
            m_ExperienceRequiredForLevel = (int)m_ExperienceRequiredForLevel;
            Debug.Log(m_ExperienceRequiredForLevel);
        }
    }
}