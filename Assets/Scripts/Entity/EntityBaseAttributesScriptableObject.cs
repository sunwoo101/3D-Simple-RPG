using UnityEngine;

namespace Entity
{
    [CreateAssetMenu(fileName = "EntityBaseAttributesScriptableObject", menuName = "ScriptableObjects/Entity/EntityBaseAttributes")]
    public class EntityBaseAttributesScriptableObject : ScriptableObject
    {
        #region Variables

        [SerializeField] internal float m_MovementSpeed;
        [SerializeField] internal float m_SprintSpeedMultiplier;
        [SerializeField] internal float m_JumpHeight;
        [SerializeField] internal float m_BaseHealth;
        [SerializeField] internal float m_BaseAttackDamage;
        [SerializeField] internal float m_BowMaxChargeTime;
        [SerializeField] internal float m_BowKnockback;

        #endregion
    }
}