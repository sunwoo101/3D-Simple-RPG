using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Entity.Player
{
    public class AttackHandler : MonoBehaviour
    {
        [SerializeField] protected GameObject m_ProjectilePrefab;
        [Range(0.0f, 1.0f)]
        [SerializeField] internal float m_MinimumChargeMultiplier;
        internal float m_Charge;

        internal bool Attack(bool attackInput, float attackDamage, TeamID.ID teamID, Transform cameraTransform, Transform damageSource, float knockbackSpeed)
        {
            attackDamage = (int)attackDamage;
            if (attackInput)
            {
                GameObject instance = Instantiate(m_ProjectilePrefab, cameraTransform.position, cameraTransform.rotation); // Instantiate the arrow
                Projectile projectile = instance.GetComponent<Projectile>(); // Get a reference to the arrow's projectile script
                projectile.m_AttackDamage = attackDamage; // Set the damage
                projectile.m_TeamID = teamID; // Set the team ID
                projectile.transform.forward = cameraTransform.forward; // Make the arrow face the same direction as the camera
                float offset = cameraTransform.GetComponent<CameraController>().ZDistanceOffset() + 0.0001f; // Forward offset from the camera to the player
                projectile.transform.position += cameraTransform.forward * offset; // Add the offset to the arrow
                projectile.m_DamageSource = damageSource; // Set the damage source for the arrow
                projectile.m_KnockbackSpeed = knockbackSpeed; // Set the arrow knockback speed

                return true;
            }

            return false;
        }
    }
}