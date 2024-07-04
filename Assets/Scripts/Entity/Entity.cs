using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Entity
{
    public class Entity : MonoBehaviour, IDamagable
    {
        #region Variables

        // Team ID
        [SerializeField] private TeamID.ID m_TeamID;

        // Attributes
        [SerializeField] protected EntityBaseAttributesScriptableObject m_EntityBaseAttributesSO; // Scriptable object for entity attributes

        #endregion

        protected virtual void Start()
        {

        }

        protected virtual void Update()
        {

        }

        public TeamID.ID TeamID()
        {
            return m_TeamID;
        }

        public virtual void TakeDamage(float damage, Transform damageSource)
        {
            // Give xp to player
            Player.Player player = damageSource.GetComponent<Player.Player>();
            if (player != null)
            {
                player.GiveXP(10);
            }
        }

        public virtual void Death()
        {
            Debug.Log("dead");
        }
    }
}